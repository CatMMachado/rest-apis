# Testing Internal vs External API Access in Swagger UI

## 🎯 Overview
Your Swagger UI is now configured to test different client access patterns. You can simulate different client types by selecting specific scopes during authentication.

## 🔧 Swagger UI Testing Setup

### 1. Start Your Application
Run your API and IdentityServer:
```bash
dotnet run
```

### 2. Open Swagger UI
Navigate to: `http://localhost:5000/swagger`

### 3. Authenticate in Swagger UI
1. Click the **🔒 Authorize** button in Swagger UI
2. In the OAuth2 dialog, you'll see all available scopes:
   - ☑️ `devices.read` - Read access to devices
   - ☑️ `devices.write` - Write access to devices  
   - ☑️ `devices.internal` - Internal API access
   - ☑️ `devices.external` - External API access

## 🧪 Test Scenarios

### Scenario 1: Internal Client (Read-Only)
**Select scopes:** `devices.read` + `devices.internal`

**Expected Results:**
- ✅ `GET /device/internal/management` - Should work
- ✅ `GET /device/public` - Should work (AllowAnonymous)
- ✅ `GET /device/private` - Should work (has devices.read)
- ❌ `GET /device/external/partner-data` - Should fail (403 Forbidden)
- ❌ `PUT /device/internal/{id}/config` - Should fail (needs write access)

### Scenario 2: External Client (Full Access)  
**Select scopes:** `devices.read` + `devices.write` + `devices.external`

**Expected Results:**
- ✅ `GET /device/external/partner-data` - Should work
- ✅ `GET /device/public` - Should work (AllowAnonymous)
- ✅ `GET /device/private` - Should work (has devices.read)
- ❌ `GET /device/internal/management` - Should fail (403 Forbidden)
- ❌ `PUT /device/internal/{id}/config` - Should fail (not internal)

### Scenario 3: Internal Client (Full Access)
**Select scopes:** `devices.read` + `devices.write` + `devices.internal`

**Expected Results:**
- ✅ `GET /device/internal/management` - Should work
- ✅ `PUT /device/internal/{id}/config` - Should work (has both internal + write)
- ✅ `GET /device/public` - Should work (AllowAnonymous)
- ✅ `GET /device/private` - Should work (has devices.read)
- ❌ `GET /device/external/partner-data` - Should fail (403 Forbidden)

### Scenario 4: Read-Only Client
**Select scopes:** `devices.read` only

**Expected Results:**
- ✅ `GET /device/public` - Should work (AllowAnonymous)
- ✅ `GET /device/private` - Should work (has devices.read)
- ❌ `GET /device/internal/management` - Should fail (needs internal)
- ❌ `GET /device/external/partner-data` - Should fail (needs external)
- ❌ `PUT /device/internal/{id}/config` - Should fail (needs write + internal)

## 🎛️ How to Test Different Combinations

### Method 1: Change Scopes in Swagger UI
1. Click **🔒 Authorize** again
2. **Uncheck/check different scopes**
3. Click **Authorize** 
4. Test endpoints with new token

### Method 2: Use Different Clients via Postman/curl

#### Get Token for Internal Read-Only Client:
```bash
curl -X POST "http://localhost:5001/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=internal-client-read-only-id&client_secret=internal-client-read-only&scope=devices.read devices.internal"
```

#### Get Token for External Client:
```bash
curl -X POST "http://localhost:5001/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=external-client-allow-all-id&client_secret=external-client-allow-all&scope=devices.read devices.write devices.external"
```

#### Get Token for Internal Full Access:
```bash
curl -X POST "http://localhost:5001/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=internal-client-allow-all-id&client_secret=internal-client-allow-all&scope=devices.read devices.write devices.internal"
```

## 🔍 Understanding the Results

### ✅ Success (200 OK)
- The client has the required scopes for that endpoint
- Authorization policies passed

### ❌ Forbidden (403)
- The client is authenticated but lacks required scopes
- Example: External client trying to access internal endpoint

### ❌ Unauthorized (401)
- Token is missing, expired, or invalid
- IdentityServer is not running or misconfigured

## 🛠️ Troubleshooting

### If Swagger UI doesn't show scopes:
1. Check that IdentityServer is running on `http://localhost:5001`
2. Verify the scopes are correctly defined in `IdentityServerConfig.cs`
3. Check browser console for CORS errors

### If all requests return 401:
1. Verify the JWT audience matches `device-management-api`
2. Check that IdentityServer and API are using the same signing key
3. Ensure `UseAuthentication()` comes before `UseAuthorization()` in Program.cs

### If scopes don't seem to work:
1. Check that the scope names in policies match exactly
2. Verify the claim type is "scope" (not "scopes")
3. Look at the JWT token claims in a debugger like jwt.io

## 📋 Client Summary

| Client ID | Available Scopes | Use Case |
|-----------|------------------|----------|
| `internal-client-read-only-id` | `devices.read`, `devices.internal` | Internal read-only access |
| `internal-client-allow-all-id` | `devices.read`, `devices.write`, `devices.internal` | Internal full access |
| `external-client-allow-all-id` | `devices.read`, `devices.write`, `devices.external` | External partner access |
| `swagger-ui-client` | All scopes | Development/testing in Swagger UI |

The `swagger-ui-client` is special - it has access to all scopes, allowing you to test any combination by selecting different scopes in the Swagger UI authorization dialog.
