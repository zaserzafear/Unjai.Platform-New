START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_refresh_tokens DROP CONSTRAINT "FK_tenant_admin_refresh_tokens_tenants_admin_tenant_admin_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin DROP CONSTRAINT "FK_tenants_admin_tenants_admin_roles_role_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_role_permissions DROP CONSTRAINT "FK_tenants_admin_role_permissions_tenants_admin_permissions_pe~";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_role_permissions DROP CONSTRAINT "FK_tenants_admin_role_permissions_tenants_admin_roles_role_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_roles DROP CONSTRAINT "PK_tenants_admin_roles";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_role_permissions DROP CONSTRAINT "PK_tenants_admin_role_permissions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_permissions DROP CONSTRAINT "PK_tenants_admin_permissions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin DROP CONSTRAINT "PK_tenants_admin";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE jwt_signing_key DROP CONSTRAINT "PK_jwt_signing_key";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_roles RENAME TO tenant_admin_roles;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_role_permissions RENAME TO tenant_admin_role_permissions;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin_permissions RENAME TO tenant_admin_permissions;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenants_admin RENAME TO tenant_admins;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE jwt_signing_key RENAME TO jwt_signing_keys;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX "IX_tenants_admin_roles_code" RENAME TO "IX_tenant_admin_roles_code";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX "IX_tenants_admin_role_permissions_permission_id" RENAME TO "IX_tenant_admin_role_permissions_permission_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX "IX_tenants_admin_permissions_code" RENAME TO "IX_tenant_admin_permissions_code";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX "IX_tenants_admin_username" RENAME TO "IX_tenant_admins_username";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX "IX_tenants_admin_role_id" RENAME TO "IX_tenant_admins_role_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX idx_tenants_admin_active_created_at_id RENAME TO idx_tenant_admins_active_created_at_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER INDEX idx_jwt_signing_key_active RENAME TO idx_jwt_signing_keys_active;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_refresh_tokens ALTER COLUMN token TYPE character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_roles ADD CONSTRAINT "PK_tenant_admin_roles" PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_role_permissions ADD CONSTRAINT "PK_tenant_admin_role_permissions" PRIMARY KEY (role_id, permission_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_permissions ADD CONSTRAINT "PK_tenant_admin_permissions" PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admins ADD CONSTRAINT "PK_tenant_admins" PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE jwt_signing_keys ADD CONSTRAINT "PK_jwt_signing_keys" PRIMARY KEY (key_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    CREATE UNIQUE INDEX "IX_tenant_admin_refresh_tokens_token" ON tenant_admin_refresh_tokens (token);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_refresh_tokens ADD CONSTRAINT "FK_tenant_admin_refresh_tokens_tenant_admins_tenant_admin_id" FOREIGN KEY (tenant_admin_id) REFERENCES tenant_admins (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_role_permissions ADD CONSTRAINT "FK_tenant_admin_role_permissions_tenant_admin_permissions_perm~" FOREIGN KEY (permission_id) REFERENCES tenant_admin_permissions (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admin_role_permissions ADD CONSTRAINT "FK_tenant_admin_role_permissions_tenant_admin_roles_role_id" FOREIGN KEY (role_id) REFERENCES tenant_admin_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    ALTER TABLE tenant_admins ADD CONSTRAINT "FK_tenant_admins_tenant_admin_roles_role_id" FOREIGN KEY (role_id) REFERENCES tenant_admin_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260405100025_RenameNaming20260504') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260405100025_RenameNaming20260504', '10.0.5');
    END IF;
END $EF$;
COMMIT;

