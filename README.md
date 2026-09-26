# Hệ thống Booking Tour – Triển khai & Quản trị

Đề 20 – Website đặt tour du lịch, triển khai bằng Docker Compose.

## Kiến trúc
- **app**: ASP.NET Core 10 MVC (quản lý tour, lịch trình, khách hàng, đặt tour)
- **postgres**: PostgreSQL 16
- **pgadmin**: pgAdmin 4 – quản lý DB
- **nginx**: Reverse proxy, HTTPS, security headers

## Yêu cầu
- Docker Desktop, Git, OpenSSL (có sẵn trong Git Bash)

## Cách chạy
```bash
git clone https://github.com/DTC245210003/booking-tour-devops.git
cd booking-tour-devops

# 1. Tạo file biến môi trường và đổi mật khẩu
cp .env.example .env

# 2. Tạo chứng chỉ HTTPS tự ký
MSYS_NO_PATHCONV=1 openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout nginx/certs/server.key -out nginx/certs/server.crt -subj "/CN=localhost" -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"

# 3. Khởi chạy
docker compose up -d --build
```

## Truy cập
| Dịch vụ | URL |
|---|---|
| Website | https://localhost |
| Quản lý đặt tour | https://localhost/Tours/Bookings |
| pgAdmin | https://localhost/pgadmin/ |
| Grafana | https://localhost/grafana/ (admin / GRAFANA_ADMIN_PASSWORD) |
| Prometheus | http://localhost:9090 (chỉ truy cập từ máy chủ) |

## Giám sát (Prometheus + Grafana)
| Nguồn số liệu | Giám sát | Dashboard Grafana (ID) |
|---|---|---|
| cAdvisor | Container: CPU, RAM, network | 14282 |
| nginx-exporter | Web server Nginx | 12708 |
| postgres-exporter (user `db_exporter`, quyền `pg_monitor`) | PostgreSQL | 9628 |
| app `/metrics` (prometheus-net) | Request vào web Booking Tour | Booking Tour App |

Lần đầu chạy, nếu DB đã tồn tại từ trước, tạo user giám sát DB:
```bash
MSYS_NO_PATHCONV=1 docker compose exec postgres bash /docker-entrypoint-initdb.d/02-monitoring.sh
```

> Lưu ý Docker Desktop: tắt *Settings → General → Use containerd for pulling and storing images* để cAdvisor đọc được tên container.
