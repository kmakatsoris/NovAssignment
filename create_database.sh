#!/bin/bash

# Dependencies:
# ------------------------------------------------------------
# mysql -u root -p
# CREATE USER 'test'@'localhost' IDENTIFIED BY 'A1b2@c3d!';
# GRANT ALL PRIVILEGES ON CurrencyDB.* TO 'test'@'localhost';
# FLUSH PRIVILEGES;
# EXIT;
# chmod +x create_database.sh

USER="test"
PASSWORD="A1b2@c3d!"
DATABASE="CurrencyDB"
TABLE="CurrencyRates"

mysql -u "$USER" -p"$PASSWORD" -e "CREATE DATABASE IF NOT EXISTS $DATABASE; USE $DATABASE; CREATE TABLE IF NOT EXISTS $TABLE (
    Date DATE NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    Rate DECIMAL(18, 6) NOT NULL,
    PRIMARY KEY (Date, Currency)
);"

if [ $? -eq 0 ]; then
    echo "Database '$DATABASE' and table '$TABLE' created successfully."
else
    echo "Failed to create database '$DATABASE' or table '$TABLE'."
fi
