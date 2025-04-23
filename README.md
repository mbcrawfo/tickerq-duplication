Stress test app demonstrating issues with TickerQ distributed locking and possible DbContext leaks under heavy load.

Run `docker compose up` to start, 1000 ticker jobs are queued on startup and processed by 5 workers.

TickerQ dashboard at http://localhost:5000/dashboard.

Endpoint at http://localhost:5000/duplicates will return the first 100 jobs that have had duplicate executions.
