import type { IGraphResult } from "@/interfaces/graph/IGraphResult";
import type { IAppearanceRecord } from "@/interfaces/repositories/IGraphRepository";
import { container } from "./container";

/**
 * Returns the entity graph for a project as a list of nodes and directed edges.
 */
export const getGraph = async (
  projectId: string,
  entityType?: string,
  repo = container.graphRepo,
): Promise<IGraphResult> => {
  return repo.getGraph(projectId, entityType);
};

/**
 * Creates a directed relationship between two entity nodes.
 * Returns `false` if either entity does not exist in the project.
 */
export const createRelationship = async (
  data: {
    projectId: string;
    sourceEntityId: string;
    targetEntityId: string;
    type: string;
    label?: string;
  },
  repo = container.graphRepo,
): Promise<boolean> => {
  return repo.createRelationship(data);
};

/**
 * Deletes all directed relationships between two entity nodes within a project.
 */
export const deleteRelationship = async (
  data: {
    projectId: string;
    sourceEntityId: string;
    targetEntityId: string;
  },
  repo = container.graphRepo,
): Promise<void> => {
  return repo.deleteRelationship(data);
};

/**
 * Returns all chapters where a given entity appears via APPEARS_IN edges.
 */
export const getEntityAppearances = async (
  projectId: string,
  entityId: string,
  repo = container.graphRepo,
): Promise<IAppearanceRecord[]> => {
  return repo.getEntityAppearances({ projectId, entityId });
};
