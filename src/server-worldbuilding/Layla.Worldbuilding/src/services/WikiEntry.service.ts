import { v4 as uuidv4 } from "uuid";
import type { IWikiEntry, WikiEntityType } from "@/interfaces/wiki/IWikiEntry";
import { container } from "./container";

/**
 * Lists wiki entries for a project, optionally filtered by entity type.
 */
export const listEntries = async (
  projectId: string,
  entityType?: WikiEntityType,
  repo = container.wikiRepo,
) => {
  return repo.listEntries(projectId, entityType);
};

/**
 * Returns a single wiki entry by its `entityId`, or `null` if not found.
 */
export const getEntry = async (entityId: string, repo = container.wikiRepo) => {
  return repo.getEntry(entityId);
};

/**
 * Creates a new wiki entry in MongoDB and attempts an immediate Neo4j sync.
 */
export const createEntry = async (
  data: {
    projectId: string;
    name: string;
    entityType: WikiEntityType;
    description?: string;
    tags?: string[];
  },
  repo = container,
) => {
  const entryData: Partial<IWikiEntry> = {
    projectId: data.projectId,
    entityId: uuidv4(),
    name: data.name,
    entityType: data.entityType,
    description: data.description ?? "",
    tags: data.tags ?? [],
    neo4jSynced: false,
  };

  const entry = await repo.wikiRepo.createEntry(entryData);

  try {
    await repo.graphRepo.mergeEntity({
      entityId: entry.entityId,
      projectId: entry.projectId,
      name: entry.name,
      entityType: entry.entityType,
    });

    await repo.wikiRepo.updateEntry(entry.entityId, { neo4jSynced: true });
    entry.neo4jSynced = true;
  } catch (err) {
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
 */
export const updateEntry = async (
  entityId: string,
  data: Partial<
    Pick<IWikiEntry, "name" | "entityType" | "description" | "tags">
  >,
  repo = container,
) => {
  const entry = await repo.wikiRepo.updateEntry(entityId, data);

  if (entry) {
    try {
      await repo.graphRepo.mergeEntity({
        entityId: entry.entityId,
        projectId: entry.projectId,
        name: entry.name,
        entityType: entry.entityType,
      });

      if (!entry.neo4jSynced) {
        await repo.wikiRepo.updateEntry(entityId, { neo4jSynced: true });
        entry.neo4jSynced = true;
      }
    } catch (err) {
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
 */
export const deleteEntry = async (
  entityId: string,
  repo = container,
): Promise<boolean> => {
  const deleted = await repo.wikiRepo.deleteEntry(entityId);
  if (!deleted) return false;

  try {
    await repo.graphRepo.deleteEntity(entityId);
  } catch (err) {
    console.error(
      `[WikiEntry.service] Failed to delete Neo4j node for entity ${entityId}.`,
      err,
    );
  }

  return true;
};
