CREATE EXTENSION IF NOT EXISTS "pgcrypto";

DROP TABLE IF EXISTS Trainer_Subs CASCADE;
DROP TABLE IF EXISTS Member CASCADE;
DROP TABLE IF EXISTS Subscription CASCADE;
DROP TABLE IF EXISTS Trainer CASCADE;

CREATE TABLE Subscription ( 
Subs_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
weekly INT NOT NULL,
duration INT NOT NULL,
price NUMERIC(10,2) NOT NULL
);

CREATE TABLE Trainer (
Trainer_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
name VARCHAR(100) NOT NULL,
email VARCHAR(100) NOT NULL
);

CREATE TABLE Member ( 
Mem_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
name VARCHAR(100) NOT NULL,
email VARCHAR(100) NOT NULL,
Subs_Id UUID NOT NULL,
CONSTRAINT fk_subs_id FOREIGN KEY (Subs_Id) REFERENCES Subscription(Subs_Id) ON DELETE CASCADE
);

CREATE TABLE Trainer_Subs (
Subs_Id UUID NOT NULL,
Trainer_Id UUID NOT NULL,
PRIMARY KEY (Subs_Id,Trainer_Id),
CONSTRAINT fk_ts_s FOREIGN KEY (Subs_Id) REFERENCES Subscription(Subs_Id) ON DELETE CASCADE,
CONSTRAINT fk_ts_t FOREIGN KEY (Trainer_Id) REFERENCES Trainer(Trainer_Id) ON DELETE CASCADE
);

INSERT INTO Subscription (weekly,duration, price) VALUES
(7,1,15.00),   -- Tjedna članarina
(3,4,45.00),   -- Mjesečna članarina
(3,12,120.00), -- Tromjesečna članarina
(3,52,400.00); -- Godišnja članarina

INSERT INTO Trainer (name, email) VALUES
('Ivan Horvat', 'ivan.horvat@gym.hr'),
('Marko Kovač', 'marko.kovac@gym.hr'),
('Ana Jurić', 'ana.juric@gym.hr'),
('Petra Babić', 'petra.babic@gym.hr');

-- UNOS ČLANOVA (Dohvaćanje stvarnosg UUID-a na temelju trajanja pretplate)
INSERT INTO Member (name, email, subs_id) VALUES
('Luka Modrić', 'luka@gmail.com', (SELECT subs_id FROM Subscription WHERE duration = 4)),        
('Mateo Kovačić', 'mateo@gmail.com', (SELECT subs_id FROM Subscription WHERE duration = 52)),   
('Joško Gvardiol', 'josko@gmail.com', (SELECT subs_id FROM Subscription WHERE duration = 4)),  
('Dominik Livaković', 'dominik@gmail.com', (SELECT subs_id FROM Subscription WHERE duration = 1)),
('Ivana Perić', 'ivana@gmail.com', (SELECT subs_id FROM Subscription WHERE duration = 12));     

-- UNOS U SPOJNU TABLICU TRAINER_SUBS (Dohvaćanje UUID-a za pretplatu i trenera)
INSERT INTO Trainer_Subs (subs_id, trainer_id) VALUES
((SELECT subs_id FROM Subscription WHERE duration = 1), (SELECT trainer_id FROM Trainer WHERE email = 'ivan.horvat@gym.hr')),
((SELECT subs_id FROM Subscription WHERE duration = 4), (SELECT trainer_id FROM Trainer WHERE email = 'ivan.horvat@gym.hr')),
((SELECT subs_id FROM Subscription WHERE duration = 4), (SELECT trainer_id FROM Trainer WHERE email = 'marko.kovac@gym.hr')),
((SELECT subs_id FROM Subscription WHERE duration = 12), (SELECT trainer_id FROM Trainer WHERE email = 'ana.juric@gym.hr')),
((SELECT subs_id FROM Subscription WHERE duration = 52), (SELECT trainer_id FROM Trainer WHERE email = 'ivan.horvat@gym.hr')),
((SELECT subs_id FROM Subscription WHERE duration = 52), (SELECT trainer_id FROM Trainer WHERE email = 'ana.juric@gym.hr')),
((SELECT subs_id FROM Subscription WHERE duration = 52), (SELECT trainer_id FROM Trainer WHERE email = 'petra.babic@gym.hr'));

ALTER TABLE Member
ADD COLUMN phone_num VARCHAR(20);

UPDATE Member
SET phone_num = '0919874563'
WHERE email = 'luka@gmail.com';

DELETE FROM Member
WHERE email = 'ivana@gmail.com';

--clanovi koji placaju pretplatu vise od 40€ poredani abecedno
SELECT m.name, m.email, s.price 
FROM Member m
JOIN Subscription s ON m.subs_id=s.Subs_Id
WHERE s.price>40.00
ORDER BY m.name ASC;

--broj clanova po tipu pretplate
SELECT s.subs_id, s.price, COUNT(m.Mem_Id)
FROM Subscription s
LEFT JOIN Member m on s.subs_id=m.subs_id
GROUP BY s.subs_id, s.price
ORDER BY COUNT(m.Mem_Id) DESC;

SELECT * 
FROM Member;


