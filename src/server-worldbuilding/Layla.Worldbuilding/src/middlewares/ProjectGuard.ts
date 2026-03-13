import { Response, NextFunction } from "express";
import InterfaceAuthRequest from "@/interfaces/auth/AuthRequest";
import { getNeo4jDriver } from "@/db/neo4j";

/**
 * Middleware factory that enforces per-project access control.
 *
 * Queries the Neo4j `:Project` node for a `projectId` / `ownerId` match
 * against the authenticated user's JWT `id`. Returns **403 Forbidden**
 * if the project does not exist or the user is not the owner.
 *
 * Must be used **after** {@link MiddlewareAuthenticate} so that
 * `req.user` is already populated.
 *
 * @example
 * router.get("/:projectId", MiddlewareAuthenticate, requireProjectAccess(), handler);
 */
export const requireProjectAccess = () => {
  return async (
    req: InterfaceAuthRequest,
    res: Response,
    next: NextFunction,
  ): Promise<void> => {
    const { projectId } = req.params as { projectId?: string };

    if (!projectId) {
      next();
      return;
    }

    if (!req.user) {
      res.status(401).json({ error: "Unauthorized" });
      return;
    }

    const driver = getNeo4jDriver();
    const session = driver.session();

    try {
      const result = await session.run(
        `MATCH (p:Project { projectId: $projectId, ownerId: $ownerId }) RETURN p LIMIT 1`,
        { projectId, ownerId: req.user.id },
      );

      if (result.records.length === 0) {
        res.status(403).json({ error: "Access denied to this project" });
        return;
      }

      next();
    } catch (err) {
      console.error("[ProjectGuard] Neo4j query failed:", err);
      // Fail open only during a Neo4j outage — still log it
      next();
    } finally {
      await session.close();
    }
  };
};
