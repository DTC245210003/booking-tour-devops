#!/bin/bash
set -e
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
CREATE USER db_exporter WITH PASSWORD '${EXPORTER_DB_PASSWORD}';
GRANT pg_monitor TO db_exporter;
GRANT CONNECT ON DATABASE ${POSTGRES_DB} TO db_exporter;
EOSQL
