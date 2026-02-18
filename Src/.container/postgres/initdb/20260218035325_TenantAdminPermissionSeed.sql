START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218035325_TenantAdminPermissionSeed') THEN
    INSERT INTO tenants_admin_permissions (id, code, name)
    VALUES (1, 'READTENANTS', 'Read Tenants');
    INSERT INTO tenants_admin_permissions (id, code, name)
    VALUES (2, 'CREATETENANTS', 'Create Tenants');
    INSERT INTO tenants_admin_permissions (id, code, name)
    VALUES (3, 'UPDATETENANTS', 'Update Tenants');
    INSERT INTO tenants_admin_permissions (id, code, name)
    VALUES (4, 'DELETETENANTS', 'Delete Tenants');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218035325_TenantAdminPermissionSeed') THEN
    PERFORM setval(
        pg_get_serial_sequence('tenants_admin_permissions', 'id'),
        GREATEST(
            (SELECT MAX(id) FROM tenants_admin_permissions) + 1,
            nextval(pg_get_serial_sequence('tenants_admin_permissions', 'id'))),
        false);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218035325_TenantAdminPermissionSeed') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260218035325_TenantAdminPermissionSeed', '10.0.2');
    END IF;
END $EF$;
COMMIT;

