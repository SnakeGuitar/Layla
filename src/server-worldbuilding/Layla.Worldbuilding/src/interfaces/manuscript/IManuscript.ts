/** Represents a single chapter within a manuscript. */
export interface IChapter {
  /** UUID identifying this chapter within its manuscript. */
  chapterId: string;
  /** Display title shown in the chapter navigation panel. */
  title: string;
  /** Full RTF content of the chapter. Omitted in index responses. */
  content: string;
  /** Zero-based position of the chapter within the manuscript. */
  order: number;
  /** Timestamp set by Mongoose on document creation. */
  createdAt: Date;
  /** Timestamp updated by Mongoose on every save; used for Last-Write-Wins conflict detection. */
  updatedAt: Date;
}

/** Represents a manuscript belonging to a project. A project may have multiple manuscripts. */
export interface IManuscript {
  /** UUID identifying this manuscript. Used alongside `projectId` as a compound lookup key. */
  manuscriptId: string;
  /** UUID of the owning project, issued by `server-core`. */
  projectId: string;
  /** Human-readable title of the manuscript (e.g. "Book 1", "Draft 2"). */
  title: string;
  /** Zero-based display order among the project's manuscripts. */
  order: number;
  /** Ordered list of chapters embedded in this manuscript. */
  chapters: IChapter[];
  /** Timestamp set by Mongoose on document creation. */
  createdAt: Date;
  /** Timestamp updated by Mongoose on every save. */
  updatedAt: Date;
}
