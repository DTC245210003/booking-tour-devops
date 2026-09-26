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

## Log tập trung (Loki + Promtail)
Promtail đọc log của mọi container qua Docker socket, gắn nhãn `container`, đẩy về Loki. Truy vấn trong Grafana → Explore → datasource **Loki**.

| # | Mục đích | LogQL |
|---|---|---|
| 1 | Toàn bộ log truy cập web | `{container="nginx"}` |
| 2 | Các lượt đặt tour | `{container="nginx"} \|= "POST /Tours/Book"` |
| 3 | Request lỗi 4xx/5xx | ``{container="nginx"} \| pattern `<ip> - <_> [<_>] "<method> <path> <_>" <status> <_>` \| status >= 400`` |
| 4 | Log nghiệp vụ đặt tour | `{container="app"} \|= "New booking"` |
| 5 | Lỗi database | `{container="postgres"} \|= "ERROR"` |
| 6 | Số dòng log theo container | `sum by (container) (count_over_time({container=~".+"}[5m]))` |

## Hardening
| Biện pháp | Áp dụng |
|---|---|
| Non-root container | app (`USER $APP_UID`), nginx-unprivileged, grafana, prometheus, loki |
| Network isolation | Mạng `backend` là `internal`; chỉ Nginx mở cổng 80/443; Prometheus chỉ bind `127.0.0.1` |
| Mật khẩu mạnh | Lưu trong `.env` (bị `.gitignore`), repo chỉ có `.env.example` |
| Hạn chế quyền DB | `tour_app` chỉ CRUD (không DROP/CREATE); `db_exporter` chỉ có `pg_monitor` |
| Nginx security headers | HSTS, CSP, X-Frame-Options, X-Content-Type-Options, Referrer-Policy, Permissions-Policy; `server_tokens off` |
| HTTPS | TLS 1.2/1.3, HTTP → HTTPS redirect |
| Container read-only | app, nginx: `read_only`, `tmpfs /tmp` |
| Bỏ đặc quyền | app, nginx: `cap_drop: ALL`, `no-new-privileges` |
| Giới hạn tài nguyên | Giới hạn CPU/RAM cho app, nginx, postgres |
| Ẩn endpoint nội bộ | `/metrics` trả 404 từ bên ngoài; `stub_status` chỉ cho mạng nội bộ |
| Grafana | Tắt đăng ký user, đổi mật khẩu admin mặc định |
| Pin phiên bản image | Không dùng tag `latest` |

Ngoại lệ có chủ đích: `cadvisor` cần `privileged` và `promtail` cần Docker socket để thu thập số liệu/log của container.
