# .Net Core Cookie Validation
This sample app shows how to revalidate your ASP.NET Core cookies when your user's information may change in the backend.

### Scenario
You login, you get user information out of your database and you store that info in your cookie.  You go to a route in your API that reads `HttpContext/NancyContext.CurrentUser` and you read that user's name.  You have a `/users/1` route where a user can update their information or a sneaky DBA changes their information in the database.  When the data changes and you read the current user's name from the cookie/ClaimsPrincipal it won't match.  We can tell the user to logoff and login again and all will be well but that's not great is it?  To solve this issue step inside my repo....
