import { v4 as uuidv4 } from "uuid";
import { WikiEntryModel } from "@/models/WikiEntry.model";
import { getNeo4jDriver } from "@/db/neo4j";
import { IWikiEntry, WikiEntityType } from "@/interfaces/wiki/IWikiEntry";

/**
 * MERGEs a wiki entity node in Neo4j.
 * On CREATE sets all properties; on MATCH updates mutable fields.
 *
 * @throws When the Neo4j session fails — callers must handle and set neo4jSynced = false.
 */
const mergeEntityInNeo4j = async (entry: IWikiEntry): Promise<void> => {
  const driver = getNeo4jDriver();
  const session = driver.session();
  try {
    await session.run(
      `MERGE (e:Entity { entityId: $entityId })
       ON CREATE SET e.projectId = $projectId, e.name = $name, e.entityType = $entityType
       ON MATCH  SET e.name = $name, e.entityType = $entityType`,
      {
        entityId: entry.entityId,
        projectId: entry.projectId,
        name: entry.name,
        entityType: entry.entityType,
      },
    );
  } finally {
    await session.close();
  }
};

/**
 * Deletes an entity node (and all its relationships) from Neo4j by `entityId`.
 *
 * Failures are non-fatal — MongoDB is the source of truth for existence.
 */
const deleteEntityFromNeo4j = async (entityId: string): Promise<void> => {
  const driver = getNeo4jDriver();
  const session = driver.session();
  try {
    await session.run(
      "MATCH (e:Entity { entityId: $entityId }) DETACH DELETE e",
      { entityId },
    );
  } finally {
    await session.close();
  }
};

/**
 * Lists wiki entries for a project, optionally filtered by entity type.
 * Returns a lightweight projection (no description or __v fields).
 */
export const listEntries = async (
  projectId: string,
  entityType?: WikiEntityType,
) => {
  const filter: Record<string, string> = { projectId };
  if (entityType) filter["entityType"] = entityType;
  return WikiEntryModel.find(filter)
    .select("-description -__v")
    .lean();
};

/**
 * Returns a single wiki entry by its `entityId`, or `null` if not found.
 */
export const getEntry = async (entityId: string) => {
  return WikiEntryModel.findOne({ entityId }).lean();
};

/**
 * Creates a new wiki entry in MongoDB and attempts an immediate Neo4j sync.
 *
 * If Neo4j is unavailable, the entry is saved with `neo4jSynced = false`.
 * The background {@link neo4jSyncWorker} will retry it on the next tick.
 */
export const createEntry = async (data: {
  projectId: string;
  name: string;
  entityType: WikiEntityType;
  description?: string;
  tags?: string[];
}) => {
  const entry = await WikiEntryModel.create({
    projectId: data.projectId,
    entityId: uuidv4(),
    name: data.name,
    entityType: data.entityType,
    description: data.description ?? "",
    tags: data.tags ?? [],
    neo4jSynced: false,
  });

  try {
    await mergeEntityInNeo4j(entry);
    entry.neo4jSynced = true;
    await entry.save();
  } catch (err) {
    // Neo4j is unavailable — neo4jSyncWorker will retry on the next tick.
    console.warn(
      `[WikiEntry.service] Neo4j sync failed for entry ${entry.entityId}; will retry.`,
      err,
    );
  }

  return entry;
};

/**
 * Updates mutable fields of an existing wiki entry in MongoDB and re-syncs
 * to Neo4j if the entry exists.
 *
 * Returns the updated entry, or `null` if no entry with `entityId` exists.
 */
export const updateEntry = async (
  entityId: string,
  data: Partial<Pick<IWikiEntry, "name" | "entityType" | "description" | "tags">>,
) => {
  const entry = await WikiEntryModel.findOneAndUpdate(
    { entityId },
    { $set: data },
    { new: true },
  );

  if (entry) {
    try {
      await mergeEntityInNeo4j(entry);
      if (!entry.neo4jSynced) {
        entry.neo4jSynced = true;
        await entry.save();
      }
    } catch (err) {
      // Non-fatal — MongoDB is authoritative; neo4jSyncWorker will retry.
      console.warn(
        `[WikiEntry.service] Neo4j sync failed on update for ${entityId}.`,
        err,
      );
    }
  }

  return entry;
};

/**
 * Deletes a wiki entry from MongoDB and attempts to remove its Neo4j node.
 *
 * Returns `true` if the entry was deleted, `false` if it did not exist.
 * Neo4j deletion failures are logged but do not fail the overall operation.
 */
export const deleteEntry = async (entityId: string): Promise<boolean> => {
  const entry = await WikiEntryModel.findOneAndDelete({ entityId });
  if (!entry) return false;

  try {
    await deleteEntityFromNeo4j(entityId);
  } catch (err) {
    console.error(
      `[WikiEntry.service] Failed to delete Neo4j node for entity ${entityId}.`,
      err,
    );
  }

  return true;
};
