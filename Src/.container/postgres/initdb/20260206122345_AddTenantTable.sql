START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260206122345_AddTenantTable') THEN
    CREATE TABLE tenants (
        id uuid NOT NULL DEFAULT (uuidv7()),
        code character varying(50) NOT NULL,
        name character varying(200) NOT NULL,
        is_active boolean NOT NULL DEFAULT TRUE,
        created_at timestamp with time zone NOT NULL DEFAULT (NOW()),
        updated_at timestamp with time zone NOT NULL DEFAULT (NOW()),
        is_deleted boolean NOT NULL DEFAULT FALSE,
        deleted_at timestamp with time zone,
        CONSTRAINT "PK_tenants" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260206122345_AddTenantTable') THEN
    CREATE UNIQUE INDEX "IX_tenants_code" ON tenants (code);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260206122345_AddTenantTable') THEN
    CREATE UNIQUE INDEX "IX_tenants_name" ON tenants (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260206122345_AddTenantTable') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260206122345_AddTenantTable', '10.0.2');
    END IF;
END $EF$;
COMMIT;

