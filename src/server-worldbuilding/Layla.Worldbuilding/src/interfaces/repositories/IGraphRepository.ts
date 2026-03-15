import { IGraphResult } from "../graph/IGraphResult";

export interface IGraphRepository {
  getGraph(projectId: string, entityType?: string): Promise<IGraphResult>;
  mergeEntity(data: {
    entityId: string;
    projectId: string;
    name: string;
    entityType: string;
  }): Promise<void>;
  deleteEntity(entityId: string): Promise<void>;
  createRelationship(data: {
    projectId: string;
    sourceEntityId: string;
    targetEntityId: string;
    type: string;
    label?: string;
  }): Promise<void>;
  deleteRelationship(data: {
    projectId: string;
    sourceEntityId: string;
    targetEntityId: string;
  }): Promise<void>;
}
