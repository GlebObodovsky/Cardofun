CREATE PROCEDURE spCitiesSeeding
@FolderPath NVARCHAR(MAX)
AS
BEGIN
	CREATE TABLE countryTempTable
	(
	CountryName NVARCHAR(100) NULL,
	CountryIsoCode NVARCHAR(2) NULL,
	ContinentName NVARCHAR(30) NULL,
	)

	DECLARE @sql VARCHAR(MAX)
	SET @sql = 'BULK INSERT countryTempTable
	FROM ''' + CONCAT(@FolderPath, '\Continents.txt') + '''
	WITH (FIELDTERMINATOR = ''|'')'
	EXEC (@sql)

	INSERT INTO Continents
	SELECT DISTINCT ContinentName 
	FROM countryTempTable
	WHERE ContinentName IS NOT NULL

	INSERT INTO Countries (IsoCode, Name, ContinentName)
	SELECT CountryIsoCode, CountryName, ContinentName
	FROM countryTempTable

	CREATE TABLE cityTempTable
	(
	CountryIsoCode NVARCHAR(2) NULL,
	CityName NVARCHAR(150) NULL
	)

	SET @sql = 'BULK INSERT cityTempTable
	FROM ''' + CONCAT(@FolderPath, '\Cities.txt') + '''
	WITH (FIELDTERMINATOR = ''|'')'
	EXEC (@sql)

	INSERT INTO Cities (Name, CountryIsoCode)
	SELECT CityName, CountryIsoCode
	FROM cityTempTable

	DROP TABLE countryTempTable
	DROP TABLE cityTempTable
END