#!/bin/bash

(cd ./MessageBroker && docker compose up --build -d)
(cd ./Log && docker compose up --build -d)

(cd ./Authentication && cat .env.example > .env && docker compose up --build -d)
(cd ./NoteTree && cat .env.example > .env && docker compose up --build -d)
(cd ./BackendForFrontend && cat .env.example > .env && docker compose up --build -d)
