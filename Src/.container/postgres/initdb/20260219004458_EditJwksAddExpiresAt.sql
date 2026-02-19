START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260219004458_EditJwksAddExpiresAt') THEN
    ALTER TABLE jwt_signing_key ADD expires_at timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260219004458_EditJwksAddExpiresAt') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260219004458_EditJwksAddExpiresAt', '10.0.2');
    END IF;
END $EF$;
COMMIT;

