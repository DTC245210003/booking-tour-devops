#!/bin/bash
set -e
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
CREATE TABLE tours (
  id SERIAL PRIMARY KEY,
  name VARCHAR(200) NOT NULL,
  destination VARCHAR(100) NOT NULL,
  days INT NOT NULL,
  price NUMERIC(12,0) NOT NULL,
  description TEXT
);
CREATE TABLE schedules (
  id SERIAL PRIMARY KEY,
  tour_id INT NOT NULL REFERENCES tours(id),
  start_date DATE NOT NULL,
  seats INT NOT NULL CHECK (seats >= 0)
);
CREATE TABLE customers (
  id SERIAL PRIMARY KEY,
  full_name VARCHAR(100) NOT NULL,
  phone VARCHAR(20) NOT NULL UNIQUE,
  email VARCHAR(150)
);
CREATE TABLE bookings (
  id SERIAL PRIMARY KEY,
  customer_id INT NOT NULL REFERENCES customers(id),
  schedule_id INT NOT NULL REFERENCES schedules(id),
  num_people INT NOT NULL CHECK (num_people > 0),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

INSERT INTO tours (name, destination, days, price, description) VALUES
 ('Vịnh Hạ Long 2N1Đ', 'Quảng Ninh', 2, 2500000, 'Ngủ đêm trên du thuyền, chèo kayak hang Sửng Sốt.'),
 ('Sa Pa – Fansipan 3N2Đ', 'Lào Cai', 3, 3800000, 'Bản Cát Cát, cáp treo Fansipan, chợ đêm Sa Pa.'),
 ('Đà Nẵng – Hội An 4N3Đ', 'Đà Nẵng', 4, 5900000, 'Bà Nà Hills, phố cổ Hội An, biển Mỹ Khê.');
INSERT INTO schedules (tour_id, start_date, seats) VALUES
 (1, '2026-10-10', 20), (1, '2026-10-24', 20),
 (2, '2026-10-15', 15), (3, '2026-11-01', 25);

CREATE USER tour_app WITH PASSWORD '${APP_DB_PASSWORD}';
GRANT CONNECT ON DATABASE ${POSTGRES_DB} TO tour_app;
GRANT USAGE ON SCHEMA public TO tour_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO tour_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO tour_app;
EOSQL
