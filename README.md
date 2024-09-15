### Materials and Methods

- **gRPC**: server-to-server communications
- **REST**: client-to-server communications
- **Session-Based Authentication**: between the client and the gateway
- **Token-Based Authentication**: between the gateway and servers
- **RS256**: for asymmetric signing of JSON Web Tokens (JWT) (JWS)
- **PostgreSQL**: user database for the Authentication Server
- **MongoDB**: note database for the Note Trees server
- **RedisCache**: session management within the gateway
- **RabbitMQ**: for messaging between servers
- **Clean Architecture**: Adopted for both the Authentication Server and the Note Trees server
- **Vertical Slice Architecture**: Implemented in the gateway
- **Dotnet**: Framework for all servers
- **Mediator Pattern**: to make direct function calls interceptable
- **Argon2id**: to hash passwords
- **Docker**: to containerize apps

### Diagrams
![System_Architecture](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/1db61f52-ad5a-4028-87b3-a29cf71984d2)
![Register_Flow](https://github.com/user-attachments/assets/175a4002-1337-484e-b707-b53ed52052e9)
![Login_Flow](https://github.com/user-attachments/assets/143d41d2-be99-417f-9467-27f3332b7858)
![Flow_of_Access_to_Resource](https://github.com/user-attachments/assets/68a092a3-3e18-436d-8f41-6f40c998b5d5)
![Public_Key_Retrieval_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/17b95709-dfad-4df9-b461-4b0e6f6b9082)
