import { v4 as uuidv4 } from "uuid";
import { IChapter } from "@/interfaces/manuscript/IManuscript";
import { MongooseManuscriptRepository } from "@/repositories/MongooseManuscriptRepository";

const manuscriptRepo = new MongooseManuscriptRepository();

/**
 * Returns the manuscript index for a project: chapter list without `content`.
 * Returns `null` if no manuscript exists for the given `projectId`.
 */
export const getManuscript = async (projectId: string) => {
  const manuscript = await manuscriptRepo.getManuscript(projectId);
  if (!manuscript) return null;

  const chapters = manuscript.chapters.map(
    ({ chapterId, title, order, updatedAt, createdAt }) => ({
      chapterId,
      title,
      order,
      updatedAt,
      createdAt,
    }),
  );

  return { projectId: manuscript.projectId, chapters };
};

/**
 * Returns a single chapter (including content) from a project's manuscript.
 * Returns `null` if neither the manuscript nor the chapter is found.
 */
export const getChapter = async (projectId: string, chapterId: string) => {
  return manuscriptRepo.getChapter(projectId, chapterId);
};

/**
 * Creates a new chapter and appends it to the project's manuscript.
 * If no manuscript exists, one is created via `upsert`.
 */
export const createChapter = async (
  projectId: string,
  data: { title: string; content?: string; order?: number },
) => {
  const newChapter: Partial<IChapter> = {
    chapterId: uuidv4(),
    title: data.title,
    content: data.content ?? "",
    order: data.order ?? 0,
  };

  return manuscriptRepo.createChapter(projectId, newChapter);
};

/** Data accepted by {@link updateChapter}. */
export interface UpdateChapterData {
  title?: string;
  content?: string;
  order?: number;
  clientTimestamp?: string;
}

/** Result returned by {@link updateChapter}. */
export interface UpdateChapterResult {
  conflict: boolean;
  chapter?: IChapter;
}

/**
 * Updates a chapter's fields using a Last-Write-Wins (LWW) strategy.
 */
export const updateChapter = async (
  projectId: string,
  chapterId: string,
  data: UpdateChapterData,
): Promise<UpdateChapterResult> => {
  // We need to fetch the chapter first to check for conflicts
  const chapter = await manuscriptRepo.getChapter(projectId, chapterId);
  if (!chapter) return { conflict: false };

  if (data.clientTimestamp) {
    const clientDate = new Date(data.clientTimestamp);
    if (clientDate < chapter.updatedAt) {
      return { conflict: true, chapter };
    }
  }

  const updateData: any = {};
  if (data.title !== undefined) updateData.title = data.title;
  if (data.content !== undefined) updateData.content = data.content;
  if (data.order !== undefined) updateData.order = data.order;
  updateData.updatedAt = new Date();

  await manuscriptRepo.updateChapter(projectId, chapterId, updateData);
  
  // Fetch again to return updated version (or we could optimize this)
  const updatedChapter = await manuscriptRepo.getChapter(projectId, chapterId);
  return { conflict: false, chapter: updatedChapter ?? undefined };
};

/**
 * Removes a chapter from a project's manuscript.
 * Returns `true` if the chapter was found and removed, `false` otherwise.
 */
export const deleteChapter = async (
  projectId: string,
  chapterId: string,
): Promise<boolean> => {
  return manuscriptRepo.deleteChapter(projectId, chapterId);
};
