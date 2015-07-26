-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server versie:                5.6.25-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Versie:              8.0.0.4396
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Databasestructuur van music wordt geschreven
DROP DATABASE IF EXISTS `music`;
CREATE DATABASE IF NOT EXISTS `music` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `music`;


-- Structuur van  tabel music.album wordt geschreven
DROP TABLE IF EXISTS `album`;
CREATE TABLE IF NOT EXISTS `album` (
  `ID` mediumint(9) NOT NULL AUTO_INCREMENT,
  `Album` varchar(100) NOT NULL,
  `ArtistID` smallint(6) NOT NULL,
  `Cover` mediumblob,
  PRIMARY KEY (`Album`,`ArtistID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporteren was gedeselecteerd


-- Structuur van  tabel music.artist wordt geschreven
DROP TABLE IF EXISTS `artist`;
CREATE TABLE IF NOT EXISTS `artist` (
  `ID` smallint(6) NOT NULL AUTO_INCREMENT,
  `Artist` varchar(50) NOT NULL,
  `Image` mediumblob,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Artist` (`Artist`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporteren was gedeselecteerd


-- Structuur van  tabel music.genre wordt geschreven
DROP TABLE IF EXISTS `genre`;
CREATE TABLE IF NOT EXISTS `genre` (
  `ID` smallint(6) NOT NULL AUTO_INCREMENT,
  `Genre` varchar(50) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Genre` (`Genre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporteren was gedeselecteerd


-- Structuur van  tabel music.song wordt geschreven
DROP TABLE IF EXISTS `song`;
CREATE TABLE IF NOT EXISTS `song` (
  `ID` mediumint(9) NOT NULL AUTO_INCREMENT,
  `AlbumID` smallint(6) NOT NULL,
  `ArtistID` smallint(6) NOT NULL,
  `Song` varchar(150) NOT NULL,
  `Track` smallint(6) NOT NULL,
  `Length` mediumint(9) DEFAULT NULL,
  `Played` smallint(3) NOT NULL DEFAULT '0',
  `Path` varchar(500) NOT NULL,
  `Starred` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`AlbumID`,`ArtistID`,`Song`,`Track`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporteren was gedeselecteerd
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
