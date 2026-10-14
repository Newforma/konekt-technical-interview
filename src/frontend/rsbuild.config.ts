import { defineConfig } from '@rsbuild/core';
import { pluginReact } from '@rsbuild/plugin-react';
import { pluginSass } from '@rsbuild/plugin-sass';

// On the interview VM the app is served under a proxy path, e.g. PUBLIC_BASE_PATH=/absproxy/3101
const base = process.env.PUBLIC_BASE_PATH ?? '';

// Docs: https://rsbuild.rs/config/
export default defineConfig({
  plugins: [pluginReact(), pluginSass()],
  html: {
    title: 'Interview Technical Test',
  },
  server: {
    port: 3101,
    ...(base ? { base } : {}),
  },
  dev: {
    ...(base ? { assetPrefix: `${base}/` } : {}),
  },
});
