START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260208023135_OptimizeSoftDeleteIndexes') THEN
    CREATE INDEX idx_tenants_active_created_at_id ON tenants (created_at, id) WHERE "is_deleted" = false;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260208023135_OptimizeSoftDeleteIndexes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260208023135_OptimizeSoftDeleteIndexes', '10.0.2');
    END IF;
END $EF$;
COMMIT;

