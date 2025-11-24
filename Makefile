COMPOSE_FILE=./compose.yaml

.PHONY: up down

up:
	@echo "Starting Docker Compose..."
	docker compose -f $(COMPOSE_FILE) up -d

down:
	@echo "Stopping Docker Compose..."
	docker compose -f $(COMPOSE_FILE) down --volumes --rmi all