COMPOSE_FILE=./compose.yaml

.PHONY: up load down

up:
	@echo "Starting Docker Compose..."
	docker compose -f $(COMPOSE_FILE) up -d

load:
	@echo "Verifying API's health check..."
	@while true; do \
		content=$$(curl -sSf http://localhost:3333/health || true); \
		if [ "$$content" = "Healthy" ]; then \
			echo "API is healthy."; \
			break; \
		else \
			echo "API is not healthy yet. Retrying in 5 seconds..."; \
			sleep 5; \
		fi; \
	done
	@echo "Starting Load Test..."
	docker compose -f $(COMPOSE_FILE) run k6-load-test run //scripts//script.js

down:
	@echo "Stopping Docker Compose..."
	docker compose -f $(COMPOSE_FILE) down --volumes --rmi all