import { Request, Response } from "express";
import * as ManuscriptService from "@/services/Manuscript.service";

/**
 * GET /api/manuscripts/:projectId
 *
 * Returns the manuscript index (chapter list without content) for a project.
 */
export const getManuscript = async (
  req: Request,
  res: Response,
): Promise<void> => {
  const manuscript = await ManuscriptService.getManuscript(
    req.params["projectId"] as string,
  );
  if (!manuscript) {
    res.status(404).json({ error: "Manuscript not found" });
    return;
  }
  res.json(manuscript);
};

/**
 * GET /api/manuscripts/:projectId/chapters/:chapterId
 *
 * Returns the full content of a single chapter.
 */
export const getChapter = async (
  req: Request,
  res: Response,
): Promise<void> => {
  const chapter = await ManuscriptService.getChapter(
    req.params["projectId"] as string,
    req.params["chapterId"] as string,
  );
  if (!chapter) {
    res.status(404).json({ error: "Chapter not found" });
    return;
  }
  res.json(chapter);
};

/**
 * POST /api/manuscripts/:projectId/chapters
 *
 * Creates a new chapter in the project's manuscript.
 * Requires `title` in the request body.
 */
export const createChapter = async (
  req: Request,
  res: Response,
): Promise<void> => {
  const { title, content, order } = req.body as {
    title: string;
    content?: string;
    order?: number;
  };

  if (!title) {
    res.status(400).json({ error: "title is required" });
    return;
  }

  const chapter = await ManuscriptService.createChapter(
    req.params["projectId"] as string,
    { title, content, order },
  );
  res.status(201).json(chapter);
};

/**
 * PUT /api/manuscripts/:projectId/chapters/:chapterId
 *
 * Updates a chapter's title, content, or order.
 * Responds with **409 Conflict** if the write is stale (Last-Write-Wins check).
 */
export const updateChapter = async (
  req: Request,
  res: Response,
): Promise<void> => {
  const result = await ManuscriptService.updateChapter(
    req.params["projectId"] as string,
    req.params["chapterId"] as string,
    req.body as ManuscriptService.UpdateChapterData,
  );

  if (result.conflict) {
    res.status(409).json({
      error: "Version conflict (Last-Write-Wins)",
      currentVersion: result.chapter,
    });
    return;
  }

  if (!result.chapter) {
    res.status(404).json({ error: "Chapter not found" });
    return;
  }

  res.json(result.chapter);
};

/**
 * DELETE /api/manuscripts/:projectId/chapters/:chapterId
 *
 * Deletes a chapter. Returns **204 No Content** on success.
 */
export const deleteChapter = async (
  req: Request,
  res: Response,
): Promise<void> => {
  const deleted = await ManuscriptService.deleteChapter(
    req.params["projectId"] as string,
    req.params["chapterId"] as string,
  );
  if (!deleted) {
    res.status(404).json({ error: "Chapter not found" });
    return;
  }
  res.status(204).send();
};
