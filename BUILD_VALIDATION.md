# Build Validation Checklist

After refactoring, verify the project builds and runs:

## 1. Compilación

```bash
cd src/server-core
dotnet build
```

**Esperado**: Sin errores. Advertencias son OK si existen pre-refactoring.

---

## 2. Verificaciones Rápidas

### Verificar que ProjectRoles constantes están disponibles

```bash
dotnet build /t:ResolvePackageAssets
grep -r "ProjectRoles.Owner" src/
```

**Esperado**: Múltiples referencias en PresenceTracker, VoiceHub, ProjectService

### Verificar que ErrorCode enum está disponible

```bash
grep -r "ErrorCode." src/ | grep -v ".xml" | head -5
```

**Esperado**: Referencias en ProjectService, AuthService, Controllers

### Verificar que TokenVersionValidator existe

```bash
test -f src/server-core/Layla.Api/Middleware/TokenVersionValidator.cs && echo "✓ Found"
```

**Esperado**: ✓ Found

### Verificar que Result<T> tiene ErrorCode

```bash
grep "ErrorCode?" src/server-core/Layla.Core/Common/Result.cs
```

**Esperado**: Una línea con `ErrorCode? ErrorCode { get; set; }`

---

## 3. Tests (si existen)

```bash
dotnet test src/server-core/
```

**Notas**:
- Si hay tests para `PresenceTracker.IsProjectActive`, pueden fallar si esperaban "Owner" (sin los cambios de casing)
- Esto es ESPERADO — los tests validarán el fix
- Si hay tests que mockean `Result<T>.Failure("magic string")`, deben actualizarse a `Result<T>.Failure(ErrorCode.xxx)`

---

## 4. Compilación de Proyectos Individuales

Verifica que cada proyecto compila:

```bash
cd src/server-core/Layla.Core && dotnet build
cd ../Layla.Infrastructure && dotnet build
cd ../Layla.Api && dotnet build
```

**Esperado**: 0 errores en los 3 proyectos

---

## 5. Verificación de Breaking Changes

### GetProjectsByUserIdAsync

La query ahora retorna TODOS los proyectos, no solo OWNER.

- Archivos afectados: Cualquier endpoint que use `GetProjectsByUserIdAsync`
- **Verificar**: `ProjectsController.GetProjects` devuelve ahora proyectos donde el user es OWNER, EDITOR, o READER
- Si frontend asume solo OWNER projects, actualizarlo

### Token Validation

`OnTokenValidated` ahora está en `TokenVersionValidator` (middleware dedicado)

- Comportamiento: idéntico, no breaking
- Sí se cambió la estructura del código

### Event Publishing Order

Eventos se publican DESPUÉS del `CommitTransactionAsync`

- Si hay código que espera eventos ANTES del commit, fallará
- Busca: "ProjectCreatedEvent" consumers en Node.js backend
- Verifica que Node.js puede manejar el retardo

---

## 6. Smoke Test Manual

Si tienes un cliente de prueba (Postman, etc.):

1. **POST /api/users** (register nuevo usuario) → Debe usar `ErrorCode.DuplicateEmail` si email existe
2. **POST /api/tokens** (login) → Debe usar `ErrorCode.AccountLocked` si está bloqueado
3. **POST /api/projects** (crear proyecto) → Debe retornar 201, proyecto en DB, eventos en RabbitMQ
4. **GET /api/projects/{id}/collaborators** (sin ser member) → Debe retornar 403 (Forbidden), no 400

---

## 7. Logging

Verifica que no hay `Debug.WriteLine` en Output:

```bash
grep -r "Debug.WriteLine" src/server-core/ --include="*.cs" | grep -v "\.xaml"
```

**Esperado**: 0 resultados (todos fueron removidos)

---

## ✅ All Clear?

Si todos los checks pasaron:

```bash
echo "✓ Refactoring validation complete. Ready to commit."
```

---

## 🚨 Si algo falla

1. **Compilation error**: Lee el error exacto, identifica el archivo
2. **Test failure**: Verifica que es por cambios de casing/ErrorCode (intencional)
3. **Breaking behavior**: Compara con REFACTORING_SUMMARY.md para entender el cambio
