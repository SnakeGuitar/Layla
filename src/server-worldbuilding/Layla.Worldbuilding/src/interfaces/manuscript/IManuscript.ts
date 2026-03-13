export interface IChapter {
  chapterId: string;
  title: string;
  content: string;
  order: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface IManuscript {
  projectId: string;
  chapters: IChapter[];
  createdAt: Date;
  updatedAt: Date;
}
