﻿CREATE TABLE BusinessEntity
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE Region
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

INSERT INTO Region (Name) VALUES ('Брестская область');
INSERT INTO Region (Name) VALUES ('Витебская область');
INSERT INTO Region (Name) VALUES ('Гомельская область');
INSERT INTO Region (Name) VALUES ('Гродненская область');
INSERT INTO Region (Name) VALUES ('Минская область');
INSERT INTO Region (Name) VALUES ('Могилёвская область');
INSERT INTO Region (Name) VALUES ('Минск');

CREATE TABLE Activity
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE ActivityDirection
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE Dealer
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    BusinessEntityId INTEGER NOT NULL,
    Name TEXT NOT NULL,
    ActivityId INTEGER NOT NULL,
    ActivityDirectionId INTEGER NOT NULL,
    Rating INTEGER NOT NULL,
    IsRelevant INTEGER NOT NULL,
    RegionId INTEGER NOT NULL,
    City TEXT NULL,
    Street TEXT NULL,
    House TEXT NULL,
    Block TEXT NULL,
    Room TEXT NULL,
    Note TEXT NULL,
    Conditions TEXT NULL,
    DateAdded TEXT NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME')),
    FOREIGN KEY (BusinessEntityId) REFERENCES BusinessEntity(Id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (ActivityId) REFERENCES Activity(Id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (ActivityDirectionId) REFERENCES ActivityDirection(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Event
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Header TEXT NOT NULL,
    Description TEXT NOT NULL,
    DateAdded TEXT NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME')),
    DealerId INTEGER NOT NULL,
    FOREIGN KEY (DealerId) REFERENCES Dealer(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Contact
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Surname TEXT NULL,
    Name TEXT NOT NULL,
    Patronymic TEXT NULL,
    Position TEXT NULL,
    DealerId INTEGER NOT NULL,
    FOREIGN KEY (DealerId) REFERENCES Dealer(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Phone
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Value TEXT NOT NULL,
    ContactId INTEGER NOT NULL,
    FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Fax
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Value TEXT NOT NULL,
    ContactId INTEGER NOT NULL,
    FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Email
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Value TEXT NOT NULL,
    ContactId INTEGER NOT NULL,
    FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE ON UPDATE CASCADE
);