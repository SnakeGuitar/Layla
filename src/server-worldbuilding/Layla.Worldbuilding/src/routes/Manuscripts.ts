import { Router } from "express";
import { MiddlewareAuthenticate } from "@/middlewares/Auth";
import { requireProjectAccess } from "@/middlewares/ProjectGuard";
import { asyncHandler } from "@/utils/asyncHandler";
import * as ManuscriptsController from "@/controllers/Manuscripts.controller";

/** Routes for manuscript and chapter management, scoped to a project. */
const router: ReturnType<typeof Router> = Router();

router.get(
  "/:projectId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.getManuscript),
);

router.get(
  "/:projectId/chapters/:chapterId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.getChapter),
);

router.post(
  "/:projectId/chapters",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.createChapter),
);

router.put(
  "/:projectId/chapters/:chapterId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.updateChapter),
);

router.delete(
  "/:projectId/chapters/:chapterId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(ManuscriptsController.deleteChapter),
);

export default router;
