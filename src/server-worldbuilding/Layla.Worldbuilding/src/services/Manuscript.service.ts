import { v4 as uuidv4 } from "uuid";
import { ManuscriptModel } from "@/models/Manuscript.model";
import { IChapter } from "@/interfaces/manuscript/IManuscript";

/**
 * Returns the manuscript index for a project: chapter list without `content`.
 * Returns `null` if no manuscript exists for the given `projectId`.
 */
export const getManuscript = async (projectId: string) => {
  const manuscript = await ManuscriptModel.findOne({ projectId }).lean();
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
  const manuscript = await ManuscriptModel.findOne({ projectId }).lean();
  if (!manuscript) return null;

  return manuscript.chapters.find((c) => c.chapterId === chapterId) ?? null;
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

  const manuscript = await ManuscriptModel.findOneAndUpdate(
    { projectId },
    { $push: { chapters: newChapter } },
    { upsert: true, new: true },
  );

  return manuscript.chapters.find((c) => c.chapterId === newChapter.chapterId);
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
 *
 * If `clientTimestamp` is provided and is older than the chapter's stored
 * `updatedAt`, returns `{ conflict: true, chapter }` with the current version
 * so the client can reconcile. Returns `{ conflict: false }` if the chapter
 * or manuscript does not exist.
 */
export const updateChapter = async (
  projectId: string,
  chapterId: string,
  data: UpdateChapterData,
): Promise<UpdateChapterResult> => {
  const manuscript = await ManuscriptModel.findOne({ projectId });
  if (!manuscript) return { conflict: false };

  const chapter = manuscript.chapters.find((c) => c.chapterId === chapterId);
  if (!chapter) return { conflict: false };

  if (data.clientTimestamp) {
    const clientDate = new Date(data.clientTimestamp);
    if (clientDate < chapter.updatedAt) {
      return { conflict: true, chapter };
    }
  }

  if (data.title !== undefined) chapter.title = data.title;
  if (data.content !== undefined) chapter.content = data.content;
  if (data.order !== undefined) chapter.order = data.order;
  chapter.updatedAt = new Date();

  await manuscript.save();
  return { conflict: false, chapter };
};

/**
 * Removes a chapter from a project's manuscript.
 * Returns `true` if the chapter was found and removed, `false` otherwise.
 */
export const deleteChapter = async (
  projectId: string,
  chapterId: string,
): Promise<boolean> => {
  const result = await ManuscriptModel.updateOne(
    { projectId },
    { $pull: { chapters: { chapterId } } },
  );
  return result.modifiedCount > 0;
};
