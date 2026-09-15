# Spelloggen API

Backend for Spelloggen, a small log of the games I play.
ASP.NET Core Web API with EF Core and SQLite.

The web app is in a separate repo: https://github.com/ThanosMakedas/spelloggen-web
Its README has the startup steps for both the API and the web app.

## Run it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```
git clone https://github.com/ThanosMakedas/spelloggen-api.git
cd spelloggen-api
dotnet run
```

The API starts on http://localhost:5080. On the first run it creates `spelloggen.db`
and fills it with six games. There is nothing else to install or configure.

Swagger UI for trying the endpoints: http://localhost:5080/swagger

## Endpoints

| Method | Route | What it does |
|---|---|---|
| GET | `/api/spel` | List all games, most recently played first |
| GET | `/api/spel/{id}` | Get one game |
| POST | `/api/spel` | Create a game |
| PUT | `/api/spel/{id}` | Update a game |
| POST | `/api/spel/{id}/bild` | Upload a cover image. Form field `fil`, .jpg .jpeg .png or .webp, max 5 MB |
| DELETE | `/api/spel/{id}` | Delete a game. Not required by the assignment |

`SpelloggenApi.http` has a ready request for each endpoint.

## Technical choices

**SQLite instead of SQL Server or an in-memory list.**
SQLite is a single file next to the project. Nobody has to install or start a database
server, and there is no connection string to change. Unlike an in-memory list, the data
survives a restart. That matters because the mobile app in December reads the same data.

**Controllers instead of minimal API.**
All game endpoints live together in one class, `SpelController`, which is easy to read
and is the pattern used in the course. The controller talks directly to the `DbContext`.
There is no service or repository layer, because there is no logic to put in one.

**EnsureCreated and HasData instead of migrations.**
`EnsureCreated()` creates the database and the seed rows on the first run, so `dotnet run`
is the only step. Migrations would need the `dotnet ef` tool, which is one more thing to
install. The downside: if the model changes, delete `spelloggen.db` and it is created
again. For a project this size that is fine.

**CORS with a named policy.**
The web app runs on http://localhost:5173 and the API on http://localhost:5080.
Different ports count as different origins, so the browser blocks the web app's requests
unless the API allows them. The `frontend` policy in `Program.cs` allows exactly the web
app's origin (`localhost` and `127.0.0.1` on port 5173), with any header and method.
`AllowAnyOrigin()` would also work, but it would let any website call the API.

**No HTTPS redirect.**
HTTPS on localhost needs a trusted development certificate (`dotnet dev-certs https --trust`),
which is another setup step with a system prompt. Everything runs on the local machine,
so plain HTTP is enough here.

**Fixed port 5080.**
With a fixed port the web app knows where the API is without any configuration.
Not port 5000, because the AirPlay Receiver on macOS already uses it.

**Cover images as files, only the path in the database.**
Uploaded images are saved in `wwwroot/uploads` under a new random file name, and the
database only stores the path, for example `/uploads/3f2c9a.png`. `UseStaticFiles()`
serves them, so the browser loads them directly. The random name means the file name
from the client is never used as a path, and two uploads never overwrite each other.
Only .jpg, .jpeg, .png and .webp are accepted, because an SVG can contain scripts.
The five seeded covers are SVGs made for this project and are part of the repo.

**Gaps in the seed data on purpose.**
Two games have no `Rank` and one has no cover image. That way the nullable fields and
the image placeholder in the web app are tested every time the app starts from scratch.

## Project structure

```
Controllers/SpelController.cs   all endpoints
Data/SpelloggenContext.cs       EF Core context and the seed data
Models/Spel.cs                  the game model
wwwroot/uploads/                cover images
Program.cs                      services, CORS, static files, database creation
```

## Later: the mobile app

The Expo app in December will call this same API. A phone cannot reach `localhost` on the
computer, so the API will then also need to listen on the local network, for example
`dotnet run --urls http://0.0.0.0:5080`, and the app will use the computer's IP address.
CORS does not apply there. CORS is a browser rule, and the native app is not a browser.
