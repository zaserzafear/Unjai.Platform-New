INSERT INTO public.tenants (
    id,
    code,
    name,
    is_active,
    created_at,
    updated_at,
    is_deleted,
    deleted_at
)
SELECT
    UUIDV7(),
    'TENANT_' || lpad(gs::text, 6, '0'),
    'Tenant ' || gs,

    (random() < 0.75),

    now() - (random() * interval '365 days'),
    now(),

    is_deleted,

    CASE
        WHEN is_deleted
            THEN now() - (random() * interval '90 days')
        ELSE NULL
    END
FROM (
    SELECT
        gs,
        (random() < 0.15) AS is_deleted
    FROM generate_series(1, 100000) gs
) t;
