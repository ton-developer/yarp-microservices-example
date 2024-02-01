# How to play with it
In order to be able to make it work on local, just run both yarp-microservice and user-service.

### User-Microservice
This service is in charge of registering and login users. To do that, it generates a JWT with some secrey Keys.

Also this service have two different controllers one for 'User roles' an other for 'Admin roles'. Only request with token with proper roles will be allowed.

###### Steps:
1. Register: http://localhost:5200/api/Authentication/Login
1. Login to get the token: http://localhost:5200/api/Authentication/Login
1. Try to get user: http://localhost:5200/api/User/user/{ID} or http://localhost:5000/user/api/AdminUser/user/{ID}


### Yarp-Microservice
This service is a reverse-proxy configured in the appsettings.json
Several things have been configured:
1. Authenticate requests to the internal APIs verifying the JWT (that User-Microservice created)
1. Use roles to Authorize
1. Rate limiting

### Blog-Microservice
Empty proyect to try things and have fun!