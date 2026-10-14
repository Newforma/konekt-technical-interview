import { API_ROOT } from '../config';

export interface AskResponse {
  answer: string;
  sources: string[];
  abstained: boolean;
}

const API_URL = `${API_ROOT}/api/ask`;

export const ask = async (question: string): Promise<AskResponse> => {
  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ question }),
  });
  if (!response.ok) {
    throw new Error(`Request failed (${response.status})`);
  }
  return response.json();
};
