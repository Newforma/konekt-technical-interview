/**
 * Base path of the app. Empty on a normal machine. On the interview VM the app is served by CloudFront
 * under /app, and the API under /api on the same host (set by `npm run start:vm`).
 */
export const BASE_PATH: string = import.meta.env.PUBLIC_BASE_PATH ?? '';

/** Root of the API: a local port normally, the same host as the page on the VM. */
export const API_ROOT: string = BASE_PATH ? window.location.origin : 'http://localhost:5008';
