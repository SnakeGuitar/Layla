import { Router } from "express";
import { MiddlewareAuthenticate } from "@/middlewares/Auth";
import { requireProjectAccess } from "@/middlewares/ProjectGuard";
import { asyncHandler } from "@/utils/asyncHandler";
import * as WikiController from "@/controllers/Wiki.controller";

/** Routes for wiki entry management, scoped to a project. */
const router: ReturnType<typeof Router> = Router();

router.get(
  "/:projectId/entries",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(WikiController.listEntries),
);

router.get(
  "/:projectId/entries/:entityId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(WikiController.getEntry),
);

router.post(
  "/:projectId/entries",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(WikiController.createEntry),
);

router.put(
  "/:projectId/entries/:entityId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(WikiController.updateEntry),
);

router.delete(
  "/:projectId/entries/:entityId",
  MiddlewareAuthenticate,
  asyncHandler(requireProjectAccess()),
  asyncHandler(WikiController.deleteEntry),
);

export default router;
