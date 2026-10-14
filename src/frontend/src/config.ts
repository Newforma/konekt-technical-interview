/**
 * Base path of the app. Empty on a normal machine. On the interview VM the page is served through the
 * code-server proxy, so the app lives under /absproxy/<port> (set by `npm run start:vm`).
 */
export const BASE_PATH: string = import.meta.env.PUBLIC_BASE_PATH ?? '';

/** Root of the API: a local port normally, the code-server proxy of port 5008 on the VM (/proxy strips the prefix, the API does not know it). */
export const API_ROOT: string = BASE_PATH
  ? `${window.location.origin}/proxy/5008`
  : 'http://localhost:5008';
