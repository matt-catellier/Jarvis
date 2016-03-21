
-- ADMIN
-- only needs to have a log in
GO -- Create account data
INSERT INTO Account VALUES('abc@123.com', 'admin', 'john', 'smith', '2015-11-14', 'Admin');

-- CONSUMER
-- will have login, provider log in, and device data
GO -- Create account data
INSERT INTO Account VALUES('john@hotmail.com', 'consumer', 'john', 'smith', '2015-11-14', 'Consumer');
INSERT INTO Details VALUES(1001, 4, 2, 2, 6, '1234 2nd Street');
INSERT INTO ProviderAccount VALUES(1001, 'john@hotmail.com', 'consumer', 'Nest'); 

GO -- create device data
INSERT INTO DeviceData VALUES(1, '78 Degrees F', 25.50);
INSERT INTO Device VALUES(1, 'Thermostat', 1001, 1);
INSERT INTO StoredData VALUES(1, '78 Degrees F', DEFAULT);

-- CONSUMER
-- will have login, provider log in, and device data
GO -- Create account data
INSERT INTO Account VALUES('bob@hotmail.com', 'consumer', 'bob', 'miller', '2015-11-14', 'Consumer');
INSERT INTO Details VALUES(1002, 4, 2, 2, 6, '1234 56th Street');
INSERT INTO ProviderAccount VALUES(1002, 'bob@hotmail.com', 'consumer', 'Samsung'); 

GO -- create device data
INSERT INTO DeviceData VALUES(2, 'On', 25.50);
INSERT INTO Device VALUES(2, 'Smart Bulb', 1002, 2);
INSERT INTO StoredData VALUES(2, 'On', DEFAULT);

-- CONSUMER
-- will have login, provider log in, and device data
GO -- Create account data
INSERT INTO Account VALUES('reggie@hotmail.com', 'consumer', 'reggie', 'bush', '2015-11-14', 'Consumer');
INSERT INTO Details VALUES(1003, 4, 2, 2, 6, '1234 New orleans Street');
INSERT INTO ProviderAccount VALUES(1003, 'reggie@hotmail.com', 'consumer', 'Nest'); 

GO -- create device data
INSERT INTO DeviceData VALUES(3, '54 Degrees F', 25.50);
INSERT INTO Device VALUES(3, 'Thermostat', 1003, 3);
INSERT INTO StoredData VALUES(3, '54 Degrees F', DEFAULT);




-- BUSINESS
-- will have login, and gather anonymous device data, no provider account
GO -- Create account data
INSERT INTO Account VALUES('david@hotmail.com', 'business', 'david', 'attenborough', '2015-11-14', 'Business');
INSERT INTO Details VALUES(1004, 4, 2, 2, 6, '1234 3nd Street'); 


GO -- View data
SELECT * FROM Account;
SELECT * FROM Details;
SELECT * FROM ProviderAccount;
SELECT * FROM DeviceData;
SELECT * FROM Device;
SELECT * FROM StoredData;