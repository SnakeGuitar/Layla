import { Router } from "express";
import { MiddlewareAuthenticate } from "@/middlewares/Auth";
import { requireProjectAccess } from "@/middlewares/ProjectGuard";
import { asyncHandler } from "@/utils/asyncHandler";
import * as ManuscriptsController from "@/controllers/Manuscripts.controller";

/**
 * Express router for manuscript and chapter management.
 *
 * All routes are scoped under `/api/manuscripts` and require:
 * - A valid JWT via {@link MiddlewareAuthenticate}.
 * - Membership in the target project via {@link requireProjectAccess}.
 *
 * Manuscript routes: `/:projectId` and `/:projectId/:manuscriptId`
 * Chapter routes:    `/:projectId/:manuscriptId/chapters/:chapterId`
 */
const router: ReturnType<typeof Router> = Router();

router.get(
  "/:projectId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.getManuscriptsByProject),
);

router.post(
  "/:projectId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.createManuscript),
);

router.get(
  "/:projectId/:manuscriptId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.getManuscript),
);

router.put(
  "/:projectId/:manuscriptId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.updateManuscript),
);

router.delete(
  "/:projectId/:manuscriptId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.deleteManuscript),
);

router.get(
  "/:projectId/:manuscriptId/chapters/:chapterId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.getChapter),
);

router.post(
  "/:projectId/:manuscriptId/chapters",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.createChapter),
);

router.put(
  "/:projectId/:manuscriptId/chapters/:chapterId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.updateChapter),
);

router.delete(
  "/:projectId/:manuscriptId/chapters/:chapterId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.deleteChapter),
);

export default router;
