START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218043424_TenantAdminRolePermissionSeed') THEN
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (1, 1);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (2, 1);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (3, 1);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (4, 1);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (1, 2);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (2, 2);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (3, 2);
    INSERT INTO tenants_admin_role_permissions (permission_id, role_id)
    VALUES (4, 2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218043424_TenantAdminRolePermissionSeed') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260218043424_TenantAdminRolePermissionSeed', '10.0.2');
    END IF;
END $EF$;
COMMIT;

