Movie API
You are required to create a simple JSON API for a Movie Studio.
The list of endpoints you have to implement is given below:
Rules
● You should solve this problem using .NET Core 3.1, please create a .NET Core Web
API.
● You should provide any required instructions to get the API up and running.
● You should try and spend no more than 1 hour on this challenge.
● Please upload the solution to a public GitHub repo. (They are free!)
● You have been provided with a data file containing all the metadata for some Sony
movies, use this to test your API.
● You do not need to hook your solution up to a database. Reading directly from the
supplied csv documents is sufficient. Results “saved to a database” can just be added to
a list called database.
● Your request and response formats MUST match the examples below.
● You do not need to create anything other than the three defined endpoints.
You do not get bonus points for providing anything other than what we have
asked.
We run a set of automated tests against your solution, failing to follow the rules above will result
in a failure.
The three endpoints are:
POST /metadata
GET /metadata/:movieId
GET /movies/stats

To run the application you will need to setup an API Key in user secrets replacing the 2nd parameter with a valid key:

dotnet user-secrets set "ApiKeys:Key", "XXXXX" --project Movies.API