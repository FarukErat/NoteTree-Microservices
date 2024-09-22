#!/bin/bash

(cd ./MessageBroker && docker compose up --build -d)
(cd ./Log && docker compose up --build -d)

(cd ./Authentication && cp .env.example .env && docker compose up --build -d)
(cd ./NoteTree && cp .env.example .env && docker compose up --build -d)
(cd ./BackendForFrontend && cp .env.example .env && docker compose up --build -d)
