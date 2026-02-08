START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260208121852_AllowDuplicateTenantNames') THEN
    DROP INDEX "IX_tenants_name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260208121852_AllowDuplicateTenantNames') THEN
    CREATE INDEX "IX_tenants_name" ON tenants (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260208121852_AllowDuplicateTenantNames') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260208121852_AllowDuplicateTenantNames', '10.0.2');
    END IF;
END $EF$;
COMMIT;

