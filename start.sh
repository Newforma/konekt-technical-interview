#!/usr/bin/env bash
# Starts MySQL and the API. Run the front end in a second terminal (cd src/frontend && npm install && npm start).
set -euo pipefail
cd "$(dirname "$0")"

docker compose up -d
echo "Waiting for MySQL..."
for _ in $(seq 1 30); do
  if docker exec interview-mysql mysqladmin ping -h localhost -uuser -ppassword --silent >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

cd src/backend
dotnet run
