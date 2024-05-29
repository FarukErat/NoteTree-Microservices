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
- **Mediator Pattern**: to separate concerns for request-response communications
- **Argon2id**: to hash passwords
- **Docker**: to containerize apps

### Diagrams
![System_Architecture](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/1db61f52-ad5a-4028-87b3-a29cf71984d2)
![Register_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/14bcd267-c5e5-4bb1-9cff-170cb3aa2d37)
![Login_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/2ec42906-5385-47ef-a171-c8f024a06f96)
![Flow_of_Access_to_Resource](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/23a140c8-7489-4764-be31-74f2fdf73a12)
![Public_Key_Retrieval_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/17b95709-dfad-4df9-b461-4b0e6f6b9082)
