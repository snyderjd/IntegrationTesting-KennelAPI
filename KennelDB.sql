DROP TABLE IF EXISTS [dbo].[Animal]

CREATE TABLE [dbo].[Animal] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [Breed]    VARCHAR (40) NOT NULL,
    [Age]      INT          NOT NULL,
    [HasShots] BIT          NOT NULL
);

INSERT INTO Animal (Name, Breed, Age, HasShots) VALUES ('Jack', 'Cocker Spaniel', 5, 1);
INSERT INTO Animal (Name, Breed, Age, HasShots) VALUES ('Wilson', 'Poodle', 2, 0);
INSERT INTO Animal (Name, Breed, Age, HasShots) VALUES ('Lucy', 'Collie', 5, 1);
INSERT INTO Animal (Name, Breed, Age, HasShots) VALUES ('Jenny', 'Mutt', 10, 0);

