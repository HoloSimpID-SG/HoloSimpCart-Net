DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'item_type') THEN
    CREATE TYPE item_type AS (
      item_name TEXT,
      item_link TEXT,
      price_sgd DOUBLE PRECISION
    );
  END IF;
END
$$ LANGUAGE plpgsql;

CREATE TABLE IF NOT EXISTS simps (
  u_dex SERIAL PRIMARY KEY,
  dc_user_name TEXT NOT NULL,
  simp_name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS carts (
  u_dex SERIAL PRIMARY KEY,
  cart_name TEXT NOT NULL,
  owner_id INT REFERENCES simps(u_dex),
  cart_date_start TIMESTAMPTZ,
  cart_date_plan TIMESTAMPTZ,
  cart_date_end TIMESTAMPTZ,
  cost_shipping DOUBLE PRECISION
);

CREATE TABLE IF NOT EXISTS cart_items (
  cart_id INT REFERENCES carts(u_dex),
  simp_id INT REFERENCES simps(u_dex),
  items item_type[],
  quantities INT[],
  PRIMARY KEY (cart_id, simp_id)
);