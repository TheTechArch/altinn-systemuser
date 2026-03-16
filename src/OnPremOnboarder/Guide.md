# OnPremOnboarder - Step-by-Step Guide

This guide walks through everything needed to set up the OnPremOnboarder tool, from requesting scopes to creating a system user.

## Prerequisites

- .NET 10 SDK installed
- An organization registered in Enhetsregisteret (Brønnøysundregistrene)
- Access to Samarbeidsportalen (DigDir)
- Access to Altinn Studio / TT02 test environment

---

## Step 1: Request Scopes from DigDir

You need two scopes from DigDir to use the System Register and System User Request APIs.

### 1.1 Log in to Samarbeidsportalen

Go to [Samarbeidsportalen](https://samarbeid.digdir.no) and log in with your organization.

### 1.2 Request the following scopes

Navigate to **Selvbetjening** → **Integrasjoner** → **API-tilgang** and request access to:

| Scope | Purpose |
|-------|---------|
| `altinn:authentication/systemregister.write` | Create and update systems in the Altinn System Register |
| `altinn:authentication/systemuser.request.write` | Create system user requests on behalf of organizations |

For test environments (TT02), request these in the **Ver2** environment.

> **Note:** Scope approval may take some time. You will be notified when access is granted.

---

## Step 2: Install Altinn CLI and Generate JWK

### 2.1 Install Altinn CLI

```bash
dotnet tool install -g altinn.jwks
```

### 2.2 Generate a JSON Web Key (JWK)

Create an RSA key pair:

```bash
altinn-jwks create maskinportclientkey
```

### 2.3 Export the public key

Export the public key in the format Maskinporten expects. You will upload this when configuring the Maskinporten client (Step 3).

```bash
altinn-jwks export key maskinportclientkey -r Public
```

### 2.4 Export the private key as Base64

The OnPremOnboarder expects the JWK private key as a Base64-encoded string. Export it directly:

```bash
altinn-jwks export key maskinportclientkey -b
```

Copy the Base64 output — this is the value for `EncodedJwk` in the configuration.

---

## Step 3: Create a Maskinporten Client

### 3.1 Log in to Samarbeidsportalen

Go to **Selvbetjening** → **Integrasjoner** → **Selvbetjening**.

### 3.2 Create a new integration (client)

1. Click **Ny integrasjon**
2. Fill in the details:
   - **Difi-tjeneste:** Select `API-klient`
   - **Scopes:** Add the scopes you were granted:
     - `altinn:authentication/systemregister.write`
     - `altinn:authentication/systemuser.request.write`
   - **Navn:** Give it a descriptive name (e.g., `OnPremOnboarder`)
   - **Beskrivelse:** Optional description

3. Click **Opprett**

### 3.3 Note the Client ID

After creation, you will see a **Client ID** (a GUID like `8bcdfd2a-f136-49ca-b294-13664b758f33`). Save this.

### 3.4 Upload the JWK public key

1. Open the integration you just created
2. Navigate to the **Nøkler** (Keys) section
3. Upload the **public key** portion of the JWK you generated in Step 2

---

## Step 4: Assign Scopes to the Client

If you haven't already assigned scopes during client creation:

1. Open the integration in Samarbeidsportalen
2. Go to **Scopes**
3. Add:
   - `altinn:authentication/systemregister.write`
   - `altinn:authentication/systemuser.request.write`
4. Save

---

## Step 5: Configure OnPremOnboarder

### 5.1 Edit appsettings.json

Open `src/OnPremOnboarder/appsettings.json` and fill in your values:

```json
{
  "OnboarderConfig": {
    "ClientId": "<your-maskinporten-client-id>",
    "EncodedJwk": "<your-base64-encoded-jwk>",
    "Environment": "test",
    "SystemOwnerOrgNumber": "<your-org-number>",
    "SystemUserOrgNumber": "<target-org-number-or-leave-empty>",
    "SystemId": "my-system-id",
    "SystemName": "My System - Display Name",
    "SystemUserType": "standard",
    "AltinnBaseAddress": "https://platform.tt02.altinn.no",
    "SystemRegisterScope": "altinn:authentication/systemregister.write",
    "SystemUserRequestScope": "altinn:authentication/systemuser.request.write",
    "Packages": [
      "urn:altinn:accesspackage:a-ordning",
      "urn:altinn:accesspackage:renovasjon"
    ],
    "SingleResources": [],
    "RedirectUrl": ""
  }
}
```

### 5.2 Configuration reference

| Setting | Required | Description |
|---------|----------|-------------|
| `ClientId` | Yes | Maskinporten client ID from Step 3 |
| `EncodedJwk` | Yes | Base64-encoded JWK from Step 2 |
| `Environment` | Yes | `test` for TT02, `prod` for production |
| `SystemOwnerOrgNumber` | Yes | Your organization number (the vendor/system owner) |
| `SystemUserOrgNumber` | No | Org number of the party that will use the system user. Defaults to `SystemOwnerOrgNumber` if not set |
| `SystemId` | Yes | Short identifier, no spaces (e.g., `my-erp-system`). Combined with org number to form full ID: `{orgNumber}_{systemId}` |
| `SystemName` | Yes | Display name shown to users in Altinn (can contain spaces) |
| `SystemUserType` | No | Type of system user, default: `standard` |
| `AltinnBaseAddress` | Yes | `https://platform.tt02.altinn.no` for test, `https://platform.altinn.no` for prod |
| `SystemRegisterScope` | Yes | Scope for system register API |
| `SystemUserRequestScope` | Yes | Scope for system user request API |
| `Packages` | No | List of access package URNs the system needs |
| `SingleResources` | No | List of single resource identifiers the system needs |
| `RedirectUrl` | No | Redirect URL after system user approval |

### 5.3 Finding access packages

To find available access package URNs, browse the Altinn resource registry or consult the Altinn documentation for the services your system needs to access.

---

## Step 6: Build the Tool

```bash
cd src/OnPremOnboarder
dotnet build
```

---

## Step 7: Create a System

Register your system in the Altinn System Register:

```bash
dotnet run -- create-system
```

Or if using the built executable:

```bash
.\bin\Debug\net10.0\OnPremOnboarder.exe create-system
```

**Expected output:**
```
Creating system: 312268876_my-system-id
System created successfully!
System ID: 312268876_my-system-id
Response: { ... }
```

This registers the system with:
- The system ID: `{SystemOwnerOrgNumber}_{SystemId}`
- The display name from `SystemName` (in nb, nn, and en)
- Your Maskinporten `ClientId` as an allowed client
- The configured access packages and/or single resources

---

## Step 8: Create a System User Request

Create a request for an organization to approve a system user:

```bash
dotnet run -- create-request
```

**Expected output:**
```
Creating system user request for system: 312268876_my-system-id
Target organization: 314165543

System user request created successfully!
Request ID: <guid>

=== APPROVAL URL ===
https://tt02.altinn.no/...
====================

Open the URL above in a browser to approve the system user request.
```

### 8.1 Approve the system user request

1. Copy the **approval URL** from the output
2. Open it in a browser
3. Log in as an administrator for the target organization (`SystemUserOrgNumber`)
4. Review and approve the system user request

---

## Step 9: Verify the System User Token

After the request has been approved, verify that everything works:

```bash
dotnet run -- verify-token
```

**Expected output:**
```
Requesting system user token for org: 314165543
Scope: altinn:authentication/systemregister.write

Token obtained successfully!
Token type: Bearer
Scope: altinn:authentication/systemregister.write
Expires in: 120s

=== TOKEN CLAIMS ===
  iss: https://test.maskinporten.no/
  scope: altinn:authentication/systemregister.write
  authorization_details: ...
  ...
====================

System user is working correctly!
```

If the request has not been approved yet, you will see:
```
Failed to obtain system user token: Maskinporten error: ...
This likely means the system user request has not been approved yet.
```

---

## Command Summary

| Command | Description |
|---------|-------------|
| `OnPremOnboarder help` | Show help and configuration reference |
| `OnPremOnboarder create-system` | Register the system in Altinn System Register |
| `OnPremOnboarder create-request` | Create a system user request (returns approval URL) |
| `OnPremOnboarder verify-token` | Verify the system user by obtaining a token |

---

## Typical Workflow

```
1. Request scopes from DigDir (one-time)
2. Generate JWK and create Maskinporten client (one-time)
3. Configure appsettings.json
4. Run: create-system          → Registers the system
5. Run: create-request         → Get approval URL
6. Org admin approves the URL  → System user is created
7. Run: verify-token           → Confirm it works
```

---

## Troubleshooting

| Error | Cause | Solution |
|-------|-------|----------|
| `Maskinporten error: invalid_grant` | JWK or ClientId mismatch | Verify `EncodedJwk` and `ClientId` match the Maskinporten integration |
| `Maskinporten error: invalid_scope` | Scope not assigned | Ensure scopes are assigned to your client in Samarbeidsportalen |
| `Token exchange failed (401)` | Invalid Maskinporten token | Check that `Environment` matches your Maskinporten setup (test/prod) |
| `Failed to create system (Conflict)` | System ID already exists | Choose a different `SystemId` or update the existing system |
| `Failed to create system (InternalServerError)` with constraint error | Missing language translations | Ensure all language codes (nb, nn, en) are provided |
| `Failed to create system (Forbidden)` | Org number mismatch | `SystemOwnerOrgNumber` must match the org that owns the Maskinporten client |
| `Failed to obtain system user token` | Request not approved | Open the approval URL and approve the request first |

---

## Production Checklist

When moving from test (TT02) to production:

1. Request scopes in the **Prod** environment in Samarbeidsportalen
2. Create a new Maskinporten client in the **Prod** environment
3. Generate a new JWK for production (do not reuse test keys)
4. Update `appsettings.json`:
   - `Environment`: `prod`
   - `AltinnBaseAddress`: `https://platform.altinn.no`
   - `ClientId`: Production client ID
   - `EncodedJwk`: Production JWK
