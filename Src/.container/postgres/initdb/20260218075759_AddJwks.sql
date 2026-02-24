START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218075759_AddJwks') THEN
    CREATE TABLE jwt_signing_key (
        key_id text NOT NULL,
        public_key_pem text NOT NULL,
        private_key_pem text,
        is_active boolean NOT NULL DEFAULT TRUE,
        created_at timestamp with time zone NOT NULL DEFAULT (NOW()),
        CONSTRAINT "PK_jwt_signing_key" PRIMARY KEY (key_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218075759_AddJwks') THEN
    CREATE UNIQUE INDEX idx_jwt_signing_key_active ON jwt_signing_key (is_active) WHERE "is_active" = true;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260218075759_AddJwks') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260218075759_AddJwks', '10.0.2');
    END IF;
END $EF$;
COMMIT;

