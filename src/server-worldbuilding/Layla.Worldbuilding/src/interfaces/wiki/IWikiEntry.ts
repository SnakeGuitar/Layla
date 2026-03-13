export type WikiEntityType = "Character" | "Location" | "Event" | "Object";

export interface IWikiEntry {
  projectId: string; // GUID from .NET
  entityId: string; // UUID shared with Neo4j node
  name: string;
  entityType: WikiEntityType;
  description: string;
  tags: string[];
  neo4jSynced: boolean; // false = Neo4j write failed, queued for retry
  createdAt: Date;
  updatedAt: Date;
}
