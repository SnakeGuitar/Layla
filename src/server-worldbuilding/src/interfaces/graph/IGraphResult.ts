export interface GraphNode {
  entityId: string;
  name: string;
  entityType: string;
}

export interface GraphEdge {
  sourceId: string;
  targetId: string;
  type: string;
  label?: string;
}

export interface IGraphResult {
  nodes: GraphNode[];
  edges: GraphEdge[];
}
