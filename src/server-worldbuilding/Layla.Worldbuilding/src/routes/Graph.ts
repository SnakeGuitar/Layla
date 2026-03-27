import { Router } from "express";
import { MiddlewareAuthenticate } from "@/middlewares/Auth";
import { requireProjectAccess } from "@/middlewares/ProjectGuard";
import { asyncHandler } from "@/utils/asyncHandler";
import * as GraphController from "@/controllers/Graph.controller";

/** Routes for graph query and relationship management, scoped to a project. */
const router: ReturnType<typeof Router> = Router();
router.use(MiddlewareAuthenticate);
router.use(asyncHandler(requireProjectAccess()));

router.get("/:projectId", asyncHandler(GraphController.getGraph));

router.post(
  "/:projectId/relationships",
  asyncHandler(GraphController.createRelationship),
);

router.delete(
  "/:projectId/relationships",
  asyncHandler(GraphController.deleteRelationship),
);

export default router;
