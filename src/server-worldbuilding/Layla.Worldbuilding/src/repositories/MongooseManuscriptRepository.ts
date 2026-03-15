import { ManuscriptModel } from "../models/Manuscript.model";
import { IManuscript, IChapter } from "../interfaces/manuscript/IManuscript";
import { IManuscriptRepository } from "../interfaces/repositories/IManuscriptRepository";

export class MongooseManuscriptRepository implements IManuscriptRepository {
  async getManuscript(projectId: string): Promise<IManuscript | null> {
    return ManuscriptModel.findOne({ projectId }).lean() as unknown as IManuscript | null;
  }

  async getChapter(projectId: string, chapterId: string): Promise<IChapter | null> {
    const manuscript = await ManuscriptModel.findOne({ projectId }).lean();
    if (!manuscript) return null;
    return (manuscript.chapters.find((c: any) => c.chapterId === chapterId) ?? null) as unknown as IChapter | null;
  }

  async createChapter(projectId: string, chapter: Partial<IChapter>): Promise<IChapter | null> {
    const manuscript = await ManuscriptModel.findOneAndUpdate(
      { projectId },
      { $push: { chapters: chapter } },
      { upsert: true, new: true }
    );
    return (manuscript.chapters.find((c: any) => c.chapterId === chapter.chapterId) ?? null) as unknown as IChapter | null;
  }

  async updateChapter(projectId: string, chapterId: string, data: any): Promise<IManuscript | null> {
    // This is a more complex update, potentially involving LWW. 
    // The service handles the LWW logic, so here we just save the state.
    // However, the service currently calls manuscript.save().
    // We can implement a generic update or specifically for chapter.
    // For now, let's stick to returning the full manuscript after update.
    
    // In a real scenario, we might want a more granular update.
    // But since the service does manuscript.save(), we'll implement that pattern.
    const manuscript = await ManuscriptModel.findOne({ projectId });
    if (!manuscript) return null;

    const chapter = manuscript.chapters.find((c: any) => c.chapterId === chapterId);
    if (!chapter) return null;

    Object.assign(chapter, data);
    await manuscript.save();
    return manuscript.toObject() as unknown as IManuscript;
  }

  async deleteChapter(projectId: string, chapterId: string): Promise<boolean> {
    const result = await ManuscriptModel.updateOne(
        { projectId },
        { $pull: { chapters: { chapterId } } }
    );
    return result.modifiedCount > 0;
  }
}
