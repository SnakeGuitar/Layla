/**
 * Container for both the short-lived access token and the
 * long-lived refresh token.
 */
export default interface TokenPair {
  accessToken: string;
  refreshToken: string;
}
