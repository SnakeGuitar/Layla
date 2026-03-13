import { WikiEntryModel } from "@/models/WikiEntry.model";
import { getNeo4jDriver } from "@/db/neo4j";
import { IWikiEntry } from "@/interfaces/wiki/IWikiEntry";

/** Interval between sync attempts, in milliseconds. */
const SYNC_INTERVAL_MS = 60_000;

/**
 * Attempts to MERGE a single {@link IWikiEntry} into Neo4j and,
 * on success, marks it as synced in MongoDB.
 */
const retrySyncEntry = async (entry: IWikiEntry): Promise<void> => {
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

    await WikiEntryModel.updateOne(
      { entityId: entry.entityId },
      { neo4jSynced: true },
    );

    console.log(`[neo4jSyncWorker] Synced entry ${entry.entityId}`);
  } finally {
    await session.close();
  }
};

/**
 * Polls MongoDB every {@link SYNC_INTERVAL_MS} milliseconds for
 * {@link WikiEntry} documents where `neo4jSynced === false` and retries
 * the Neo4j MERGE for each one.
 *
 * This replaces the previous approach of re-publishing to a RabbitMQ
 * exchange key that had no registered consumer.
 */
export const startNeo4jSyncWorker = (): void => {
  const tick = async (): Promise<void> => {
    try {
      const unsyncedEntries = await WikiEntryModel.find({
        neo4jSynced: false,
      }).lean();

      if (unsyncedEntries.length === 0) return;

      console.log(
        `[neo4jSyncWorker] Retrying ${unsyncedEntries.length} unsynced entries...`,
      );

      await Promise.allSettled(unsyncedEntries.map(retrySyncEntry));
    } catch (err) {
      console.error("[neo4jSyncWorker] Tick error:", err);
    }
  };

  setInterval(() => void tick(), SYNC_INTERVAL_MS);
  console.log(
    `[neo4jSyncWorker] Started (interval: ${SYNC_INTERVAL_MS / 1000}s)`,
  );
};
