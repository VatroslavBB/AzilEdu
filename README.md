# AzilEdu

Aplikacija za vođenje azila za životinje. Sastoji se od tri projekta:

| Projekt | Uloga |
|---|---|
| `AzilEdu.Api` | ASP.NET Core Web API, EF Core i SQLite baza, JWT autentifikacija |
| `AzilEdu.App` | Blazor Server (MudBlazor) korisničko sučelje |
| `AzilEdu.Shared` | Zajednički modeli i DTO klase koje koriste oba projekta |

## Pokretanje

Potreban je .NET 10 SDK.

```bash
dotnet restore
dotnet build
```

API i App moraju raditi istovremeno, svaki u svojem terminalu:

```bash
dotnet run --project AzilEdu.Api
dotnet run --project AzilEdu.App
```

| Projekt | HTTP | HTTPS |
|---|---|---|
| API | http://localhost:5086 | https://localhost:7205 |
| App | http://localhost:5062 | https://localhost:7094 |

Swagger je dostupan na `/swagger` na adresi API-ja.

App čita adresu API-ja iz `ApiBaseUrl` u `AzilEdu.App/appsettings.json`. Ako se
mijenja profil pokretanja API-ja, mora se uskladiti i ta vrijednost.

U Visual Studiju se oba projekta pokreću odjednom preko spremljenog launch
profila (`AzilEdu.slnLaunch.user`).

### Baza

Baza je SQLite datoteka `AzilEdu.Api/AzilEdu.db`. Migracije se primjenjuju
automatski pri pokretanju API-ja (`db.Database.MigrateAsync()`), a nakon toga se
seedaju demo podatci. Za rad ispočetka dovoljno je obrisati
`AzilEdu.db` i ponovno pokrenuti API.

Ručno upravljanje migracijama:

```bash
dotnet ef database update --project AzilEdu.Api
dotnet ef database update 0 --project AzilEdu.Api
```

## Demo računi

Lozinke se ne navode u repozitoriju. Sve demo račune i njihove početne lozinke
postavlja `AzilEdu.Api/Data/AppUserSeeder.cs` i vidljive su samo u tom kodu.

| E-mail | Uloge | Povezani profil |
|---|---|---|
| `user@aziledu.local` | User | - |
| `admin@aziledu.local` | User, Admin | - |
| `employee@aziledu.local` | User, Employee | prvi djelatnik iz baze |
| `volunteer@aziledu.local` | User, Volunteer | prvi volonter iz baze |
| `donor@aziledu.local` | User, Donor | prvi donator iz baze |

Novi računi dodaju se kroz sučelje na `/users` (samo Admin) ili kroz
`POST api/users`.

## Relacije korisničkih računa

### AppUser – AppRole (više-na-više)

Jedan račun može imati više uloga, a jedna uloga pripada većem broju računa.
Zato veza ide kroz spojnu tablicu `AppUserRole` s ključevima `AppUserId` i
`AppRoleId`, umjesto jednog `Role` stupca na korisniku. Primjer: isti račun može
istovremeno biti `Employee` i `Donor`.

### AppUser – Volunteer, AppUser – Donor, AppUser – Employee (jedan-na-jedan, neobavezno)

`AppUser` ima nullable strane ključeve `VolunteerId`, `DonorId` i `EmployeeId`.
Račun ne mora biti povezan ni s jednim poslovnim profilom, a jedan poslovni
profil smije biti povezan s najviše jednim računom - to pravilo provjerava
`UsersController` prije spremanja.

Te veze su temelj `/mine` ruta: kada volonter traži svoje zadatke, API čita
`volunteer_id` iz tokena, a ne iz zahtjeva preglednika.

## 401 i 403

| Status | Značenje | Kada se javlja |
|---|---|---|
| **401 Unauthorized** | identitet nije potvrđen | token nedostaje, neispravan je ili je istekao |
| **403 Forbidden** | identitet je potvrđen, ali ovlasti nisu dovoljne | prijavljeni korisnik nema traženu ulogu ili traženi poslovni profil |


Rezultati autorizacijskog testiranja:

| Zahtjev | Admin | Employee | Volunteer | Donor | Bez tokena |
|---|---|---|---|---|---|
| `GET api/animals` | 200 | 200 | 200 | 200 | 401 |
| `POST api/animals` | 201 | 201 | 403 | 403 | 401 |
| `GET api/donations` | 200 | 200 | 403 | 403 | 401 |
| `GET api/donations/mine` | 403 | 403 | 403 | 200 | 401 |
| `GET api/volunteertasks/mine` | 403 | 403 | 200 | 403 | 401 |
| `GET api/users` | 200 | 403 | 403 | 403 | 401 |

Zaštita je definirana u `AzilEdu.Api/Program.cs`: `FallbackPolicy` traži
prijavljenog korisnika za svaki endpoint bez vlastitog atributa, a politike
`Staff` (Admin i Employee) i `AdminOnly` (Admin) definirane su u
`AzilEdu.Api/Security/AuthorizationPolicies.cs`.

## Multimedija

Datoteke se spremaju na disk u `AzilEdu.Api/wwwroot/uploads/animals`, a u bazi
je samo zapis o datoteci (`AnimalMedia`: putanja, tip, naziv, oznaka naslovne
slike). Dopušteni formati su JPG, PNG, WEBP, MP4 i WEBM, uz ograničenje od
25 MB po datoteci. Upload i brisanje smiju samo Admin i Employee, dok galeriju
vidi svaki prijavljeni korisnik.

## Poznata ograničenja

- Token se čuva u sesiji preglednika i nema refresh token; nakon isteka
  potrebna je nova prijava.
- Promjena uloga vrijedi tek nakon sljedeće prijave jer se uloge čitaju iz
  postojećeg tokena.
- `UsersController` nema `DELETE` akciju - račun se može deaktivirati, ali ne i
  obrisati kroz sučelje.
- Stranice `Adopters`, `Adoptions` i `Breeds` zasad su samo najave modula bez
  vlastitog API-ja i baze.
- Baza je SQLite datoteka namijenjena lokalnom razvoju, bez migracijske
  strategije za produkciju.

## Prijedlozi za sljedeću verziju

1. **Dovršiti modul udomljavanja** - `Adopters` i `Adoptions` povezati sa
   životinjama, uz status udomljenja na profilu životinje i prikaz povijesti.
2. **Refresh token i evidencija prijava** - produžiti sesiju bez ponovnog
   upisivanja lozinke te bilježiti neuspjele pokušaje prijave radi zaključavanja
   računa nakon više pogrešaka.
