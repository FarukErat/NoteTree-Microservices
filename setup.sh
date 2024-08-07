#!/bin/bash

set -e

docker compose -f ./Authentication/docker-compose.yml up --build -d
docker compose -f ./BackendForFrontend/docker-compose.yml up --build -d
docker compose -f ./Log/docker-compose.yml up --build -d
docker compose -f ./MessageBroker/docker-compose.yml up --build -d
docker compose -f ./NoteTree/docker-compose.yml up --build -d
