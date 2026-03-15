import { IManuscript, IChapter } from "../manuscript/IManuscript";

export interface IManuscriptRepository {
  getManuscript(projectId: string): Promise<IManuscript | null>;
  getChapter(projectId: string, chapterId: string): Promise<IChapter | null>;
  createChapter(projectId: string, chapter: Partial<IChapter>): Promise<IChapter | null>;
  updateChapter(projectId: string, chapterId: string, data: any): Promise<IManuscript | null>;
  deleteChapter(projectId: string, chapterId: string): Promise<boolean>;
}
