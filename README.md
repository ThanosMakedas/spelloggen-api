# Spelloggen API

The backend for Spelloggen. It keeps the games in a database and answers the web app.
Made with ASP.NET Core Web API, Entity Framework Core and a SQLite database.

The web app is in another repo: https://github.com/ThanosMakedas/spelloggen-web
Its README has the steps for starting both of them.

## Run it

Needs the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```
git clone https://github.com/ThanosMakedas/spelloggen-api.git
cd spelloggen-api
dotnet run
```

The API runs on **http://localhost:5080**. The first time it creates the file `spelloggen.db`
and puts six games in it, so there is something to see at once. Nothing else to install.

To try the endpoints in the browser: http://localhost:5080/swagger

## Endpoints

| Method | Route | What it does |
|---|---|---|
| GET | `/api/spel` | All the games |
| GET | `/api/spel/{id}` | One game |
| POST | `/api/spel` | Add a game |
| PUT | `/api/spel/{id}` | Change a game |
| POST | `/api/spel/{id}/bild` | Upload a cover image (jpg, png or webp, max 5 MB) |
| DELETE | `/api/spel/{id}` | Delete a game |

The file `SpelloggenApi.http` has a ready request for each one.

## Why I built it this way

**SQLite.** The database is one single file in the project folder. Nothing to install and
nothing to set up, and the games are still there after a restart.

**Controllers.** All the endpoints are in one class, `SpelController`, the way we did it in the
course. The controller talks to the database directly, because there is nothing complicated to
put in between.

**The app creates the database itself.** When it starts, it makes the file and the six games if
they are not there yet. That way starting the project is only `dotnet run`.

**CORS.** The web app runs on port 5173 and the API on 5080. The browser treats those as two
different places and blocks the calls, unless the API says that this one is allowed. That is
what the short CORS part in `Program.cs` does.

**Port 5080.** A fixed port, so the web app always knows where the API is. Not 5000, because on
a Mac that port is already used by the system.

**No HTTPS.** HTTPS on your own machine needs a certificate that has to be installed first.
Everything runs locally, so plain HTTP is enough and it is one step less.

**Images are saved as files.** An uploaded image goes into `wwwroot/uploads` with a new random
name, and the database keeps only the path to it. The database stays small and the browser can
load the image directly. Only jpg, png and webp are accepted.

**Two games without a rank and one without a cover image.** On purpose, so that empty fields
are tested in the web app every time.

## The files

```
Controllers/SpelController.cs   the endpoints
Data/SpelloggenContext.cs       the database and the six games
Models/Spel.cs                  what a game is
Program.cs                      the setup
wwwroot/uploads/                the cover images
```
