# TripPlanner

## Google Places API Key Setup

This project requires a Google Places API key for location autocomplete.

The key is **not included in the repository**, so you need to configure it locally.

---

## Option 1: Environment Variable (recommended)

### Windows (CMD)

```cmd
setx GooglePlaces__ApiKey "YOUR_API_KEY"
```

### PowerShell

```powershell
$env:GooglePlaces__ApiKey="YOUR_API_KEY"
```

### Linux / macOS

```bash
export GooglePlaces__ApiKey="YOUR_API_KEY"
```

> Restart your terminal or IDE after setting the variable.

---

## Option 2: User Secrets (development)

```bash
dotnet user-secrets init
dotnet user-secrets set "GooglePlaces:ApiKey" "YOUR_API_KEY"
```

---

## Notes

* Do not commit API keys to the repository
* Make sure your key is enabled for Google Places API
