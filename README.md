### Materials and Methods

- **gRPC**: Utilized for server-to-server communications.
- **REST**: Employed for client-to-server communications.
- **Session-Based Authentication**: Implemented between the client and the gateway.
- **Token-Based Authentication**: Applied between the gateway and servers.
- **RS256**: Used for asymmetric signing of JSON Web Tokens (JWT) (JWS).
- **PostgreSQL**: Serves as the user database for the authentication server.
- **MongoDB**: Functions as the note database for the Note Trees server.
- **RedisCache**: Employed for session management within the gateway.
- **Clean Architecture**: Adopted for both the Authentication Server and the Note Trees server.
- **Vertical Slice Architecture**: Implemented in the gateway.
- **Dotnet**: Framework utilized for all servers.

### Diagrams
![System_Architecture](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/9b32097d-71e5-42f6-b2b1-692846711d12)
![Register_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/14bcd267-c5e5-4bb1-9cff-170cb3aa2d37)
![Login_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/05d195e2-2274-44ca-a0e6-3bf812148eda)
![Flow_of_Access_to_Resource](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/5b016723-1a7f-4f09-bb59-44120649a2e4)
![Public_Key_Retrieval_Flow](https://github.com/FarukErat/NoteTree-Microservices/assets/92527106/17b95709-dfad-4df9-b461-4b0e6f6b9082)
