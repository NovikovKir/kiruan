# Киноплатформа (MVC + MSSQL)

Проект состоит из трёх контейнеров: база данных Microsoft SQL Server, ASP.NET Core MVC (backend + frontend) и Nginx прокси.

## Запуск

```bash
docker compose up --build
```

После запуска:
- Веб-приложение доступно на `http://localhost`
- MSSQL доступен только внутри Docker-сети (порт 1433 с хоста не проброшен)

## Учетные данные администратора

По умолчанию создаётся администратор:
- Email: `admin@example.com`
- Пароль: `Admin123!`

Параметры можно переопределить через переменные окружения в `docker-compose.yml`:
- `AdminSettings__Email`
- `AdminSettings__Password`

## Хранение файлов фильмов

В таблице `Movies` хранится путь до файла. Файлы размещаются в volume контейнера базы данных по пути:

```
/var/opt/mssql-files
```

Пример пути в БД: `/var/opt/mssql-files/films/movie1.mp4`.

## Структура

- `backend` — ASP.NET Core MVC + EF Core
- `frontend` — Nginx reverse-proxy
- `docker-compose.yml` — запуск всех сервисов

## Таблицы БД

Создаются таблицы: `Users`, `Subscritptions`, `Movies`, `Audio_tracks`, `Subtitles`.

## Резервное копирование БД

Ручной бэкап (контейнеры должны быть запущены):

```bash
chmod +x scripts/backup-db.sh
./scripts/backup-db.sh
```

Файлы сохраняются в папку `backups/` в формате `MoviePlatform_YYYY-MM-DD_HH-MM-SS.bak`.

Автоматический бэкап раз в месяц (cron на хосте, 1-го числа в 03:00):

```cron
0 3 1 * * /home/andr/Desktop/rhe\ cursach/scripts/backup-db.sh >> /home/andr/Desktop/rhe\ cursach/backups/backup.log 2>&1
```

Параметры можно переопределить через переменные окружения:
- `DB_NAME` (по умолчанию `MoviePlatform`)
- `SA_PASSWORD` (по умолчанию из docker-compose)
