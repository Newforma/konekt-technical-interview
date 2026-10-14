import type { Project } from '../types';

import { API_ROOT } from '../config';

const API_URL = `${API_ROOT}/api/projects`;

export const getProjects = async (): Promise<Project[]> => {
  try {
    const response = await fetch(API_URL);
    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    return await response.json();
  } catch (error) {
    console.error('Failed to fetch projects:', error);
    return [];
  }
};
