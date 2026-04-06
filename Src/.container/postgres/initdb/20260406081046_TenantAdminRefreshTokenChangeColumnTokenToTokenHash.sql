START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260406081046_TenantAdminRefreshTokenChangeColumnTokenToTokenHash') THEN
    ALTER TABLE tenant_admin_refresh_tokens RENAME COLUMN token TO token_hash;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260406081046_TenantAdminRefreshTokenChangeColumnTokenToTokenHash') THEN
    ALTER INDEX "IX_tenant_admin_refresh_tokens_token" RENAME TO "IX_tenant_admin_refresh_tokens_token_hash";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260406081046_TenantAdminRefreshTokenChangeColumnTokenToTokenHash') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260406081046_TenantAdminRefreshTokenChangeColumnTokenToTokenHash', '10.0.5');
    END IF;
END $EF$;
COMMIT;

