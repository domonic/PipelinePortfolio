ALTER TABLE products ADD COLUMN sku VARCHAR(50) UNIQUE AFTER name;
CREATE INDEX idx_products_sku ON products (sku);
