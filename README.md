# MyPlanner

Benvenuto in **MyPlanner**: l'applicazione per gestire e analizzare le tue finanze personali in modo semplice, moderno e sicuro.

---

## Cosa troverai in questo progetto?

- **Backend**: Solution .NET (API RESTful, autenticazione JWT, database)
- **Frontend**: Applicazione React (dashboard, grafici, gestione transazioni)

---

## Funzionalità principali

- **Registrazione e Login** sicuri tramite JWT
- **Dashboard** con riepilogo di entrate, uscite e bilancio
- **Gestione transazioni**: aggiungi, visualizza, filtra e cancella movimenti
- **Storico mensile**: scegli mese/anno e vedi tutte le transazioni, totali e bilancio
- **Grafici avanzati**: analisi visiva di entrate/uscite per categoria e andamento temporale
- **Salvadanaio**: gestisci i tuoi risparmi separatamente
- **Logout automatico** dopo 1 ora di inattività/sessione scaduta

---

## Come avviare il progetto

### 1. Backend (.NET)

Apri un terminale e posizionati nella cartella `MyPlanner`, quindi esegui:

\`\`\`bash
dotnet run --launch-profile "https"
\`\`\`

Assicurati di aver configurato la stringa di connessione e i parametri JWT in `appsettings.json`.

### 2. Frontend (React)

Apri un altro terminale, spostati nella cartella `myplanner-frontend` ed esegui:

\`\`\`bash
npm install
npm run start
\`\`\`

L’app sarà disponibile su: [http://localhost:3000](http://localhost:3000)

---

## Struttura delle cartelle

\`\`\`
MyPlanner/               # Backend .NET
  Controllers/
  Models/
  Data/
  ...

myplanner-frontend/      # Frontend React
  src/
  components/
  services/
  App.js
  App.css
  ...
\`\`\`

---

## Note tecniche

- **Autenticazione**:  
  Il backend genera un token JWT valido 1 ora. Il frontend gestisce il logout automatico alla scadenza.

- **API**:  
  Tutte le chiamate API richiedono il token JWT nell’header `Authorization`.

- **Database**:  
  Il progetto usa **SQL Server** come RDBMS, gestito tramite Entity Framework Core.  
  Configura la stringa di connessione in `appsettings.json`.

- **Grafici**:  
  Realizzati con Chart.js tramite `react-chartjs-2`.

---


## Problemi comuni

- "Connection string not found"
Verifica che appsettings.json sia configurato correttamente
Controlla che SQL Server sia in esecuzione
Esegui dotnet ef database update per applicare le migrazioni

- "Token expired"
Il token JWT scade dopo 1 ora
L'applicazione dovrebbe gestire automaticamente il logout
Rieffettua il login se necessario

- "CORS errors"
Verifica che il backend sia configurato per accettare richieste da localhost:3000
Controlla la configurazione CORS in Program.cs

- Frontend non si connette al backend
Verifica che il backend sia in esecuzione con mode = "https"
Verifica che non ci siano problemi di certificato SSL

---


## Supporto e consigli

- Se hai problemi tecnici, controlla la console del browser e i log di Visual Studio o del terminale.
- Assicurati che il backend sia avviato correttamente prima di usare il frontend.
- Verifica che la configurazione JWT sia coerente tra backend e frontend.

