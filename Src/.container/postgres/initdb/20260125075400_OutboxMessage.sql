START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260125075400_OutboxMessage') THEN
    CREATE TABLE outbox_messages (
        id uuid NOT NULL DEFAULT (uuidv7()),
        type character varying(255) NOT NULL,
        content jsonb NOT NULL,
        occurred_on_utc timestamp with time zone NOT NULL,
        processed_on_utc timestamp with time zone,
        error text,
        CONSTRAINT "PK_outbox_messages" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260125075400_OutboxMessage') THEN
    CREATE INDEX idx_outbox_messages_unprocessed ON outbox_messages (occurred_on_utc, processed_on_utc) INCLUDE (id, type, content) WHERE "processed_on_utc" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260125075400_OutboxMessage') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260125075400_OutboxMessage', '10.0.2');
    END IF;
END $EF$;
COMMIT;

