START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405090322_TenantAdminRefreshToken') THEN
    CREATE TABLE tenant_admin_refresh_tokens (
        id uuid NOT NULL DEFAULT (uuidv7()),
        tenant_admin_id uuid NOT NULL,
        token text NOT NULL,
        expires_at timestamp with time zone NOT NULL,
        is_revoked boolean NOT NULL DEFAULT FALSE,
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_tenant_admin_refresh_tokens" PRIMARY KEY (id),
        CONSTRAINT "FK_tenant_admin_refresh_tokens_tenants_admin_tenant_admin_id" FOREIGN KEY (tenant_admin_id) REFERENCES tenants_admin (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405090322_TenantAdminRefreshToken') THEN
    CREATE INDEX "IX_tenant_admin_refresh_tokens_tenant_admin_id" ON tenant_admin_refresh_tokens (tenant_admin_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405090322_TenantAdminRefreshToken') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260405090322_TenantAdminRefreshToken', '10.0.5');
    END IF;
END $EF$;
COMMIT;

