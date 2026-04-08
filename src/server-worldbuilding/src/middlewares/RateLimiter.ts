import { rateLimit } from "express-rate-limit";

/**
 * General-purpose API rate limiter.
 *
 * Allows {@link API_MAX} requests per IP inside a rolling
 * {@link API_WINDOW_MS}-ms window.  Returns standard `RateLimit-*` headers
 * (draft-7) so clients can self-throttle.
 */
const API_WINDOW_MS = 60_000; // 1 minute
const API_MAX = 100;

export const apiLimiter = rateLimit({
  windowMs: API_WINDOW_MS,
  max: API_MAX,
  standardHeaders: "draft-7",
  legacyHeaders: false,
  message: { error: "Too many requests, please try again later" },
});
