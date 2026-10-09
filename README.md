# API gateway with JWT (Ocelot sample)

Local .NET sample of three processes behind one gateway. This is a learning exercise, not a production identity system.

## Projects

| Project | Role | URL |
| --- | --- | --- |
| `APIGateway` | Ocelot reverse proxy. Requires a JWT on `GET /customers`. | `http://localhost:62236` |
| `AuthServer` | Issues a short-lived JWT when the local demo user matches. | `http://localhost:62238` |
| `CustomerAPIServices` | Sample customers API. List requires `[Authorize]`. `GET /api/customers/{id}` does not. | `http://localhost:62237` |
| `ClientApp` | Console client that calls the gateway with and without a token. | — |

Routes live in `APIGateway/configuration.json`.

## Before you run

Put the same signing secret in both appsettings files. Put the demo user only on the auth server. Do not commit those values.

`AuthServer/appsettings.json`:

```json
"Audience": {
  "Secret": "a-long-local-secret",
  "Iss": "http://localhost:62238",
  "Aud": "local-gateway-demo",
  "DemoUser": "demo",
  "DemoPassword": "change-me"
}
```

`APIGateway/appsettings.json` and `CustomerAPIServices/appsettings.json` need the same `Secret`, `Iss`, and `Aud`. The customers API checks the bearer token again on `GET /api/customers`.

Then, in the shell that starts `ClientApp`:

```powershell
$env:DEMO_USER = "demo"
$env:DEMO_PASSWORD = "change-me"
```

Start `CustomerAPIServices`, `AuthServer`, and `APIGateway`, then run `ClientApp`. The client calls `http://localhost:62236`.

## What this sample shows

- The gateway rejects `GET /customers` until the client presents a bearer token signed with the shared secret.
- `GET /customers/1` is routed without that gateway auth check.
- Tokens expire after two minutes.

## What is not in this repo

Signing secrets, demo passwords, bearer tokens, and `tempkey.rsa` are local only. Visual Studio cache (`.vs/`) is gitignored.
