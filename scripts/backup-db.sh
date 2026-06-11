#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
BACKUP_DIR="${PROJECT_DIR}/backups"
DB_NAME="${DB_NAME:-MoviePlatform}"
SA_PASSWORD="${SA_PASSWORD:-YourStrong!Passw0rd}"
TIMESTAMP="$(date +%Y-%m-%d_%H-%M-%S)"
BACKUP_FILE="MoviePlatform_${TIMESTAMP}.bak"
CONTAINER_BACKUP_PATH="/var/opt/mssql-files/backups/${BACKUP_FILE}"

cd "$PROJECT_DIR"

if ! docker compose ps --status running --services | grep -qx db; then
    echo "Ошибка: контейнер db не запущен. Сначала выполните: docker compose up -d"
    exit 1
fi

CONTAINER_ID="$(docker compose ps -q db)"
if [ -z "$CONTAINER_ID" ]; then
    echo "Ошибка: не удалось найти контейнер db."
    exit 1
fi

mkdir -p "$BACKUP_DIR"

SQLCMD=""
for candidate in /opt/mssql-tools18/bin/sqlcmd /opt/mssql-tools/bin/sqlcmd; do
    if docker exec "$CONTAINER_ID" test -x "$candidate"; then
        SQLCMD="$candidate"
        break
    fi
done

if [ -z "$SQLCMD" ]; then
    echo "Ошибка: sqlcmd не найден в контейнере db."
    exit 1
fi

docker exec "$CONTAINER_ID" mkdir -p /var/opt/mssql-files/backups

docker exec "$CONTAINER_ID" "$SQLCMD" \
    -S localhost -U sa -P "$SA_PASSWORD" -C \
    -Q "BACKUP DATABASE [${DB_NAME}] TO DISK = N'${CONTAINER_BACKUP_PATH}' WITH INIT, COMPRESSION, CHECKSUM;"

if [ -f "${BACKUP_DIR}/${BACKUP_FILE}" ]; then
    echo "Резервная копия создана: ${BACKUP_DIR}/${BACKUP_FILE}"
else
    echo "Резервная копия создана в контейнере: ${CONTAINER_BACKUP_PATH}"
    echo "Проверьте, что volume ./backups смонтирован в docker-compose.yml."
fi
