START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260217082446_TenantAdminRoleSeed') THEN
    INSERT INTO tenants_admin_roles (id, code, name)
    VALUES (1, 'SUPERADMIN', 'Super Admin');
    INSERT INTO tenants_admin_roles (id, code, name)
    VALUES (2, 'ADMIN', 'Admin');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260217082446_TenantAdminRoleSeed') THEN
    PERFORM setval(
        pg_get_serial_sequence('tenants_admin_roles', 'id'),
        GREATEST(
            (SELECT MAX(id) FROM tenants_admin_roles) + 1,
            nextval(pg_get_serial_sequence('tenants_admin_roles', 'id'))),
        false);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260217082446_TenantAdminRoleSeed') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260217082446_TenantAdminRoleSeed', '10.0.2');
    END IF;
END $EF$;
COMMIT;

