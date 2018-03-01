/*
Navicat MySQL Data Transfer

Source Server         : local
Source Server Version : 50719
Source Host           : localhost:3306
Source Database       : messagecenter

Target Server Type    : MYSQL
Target Server Version : 50719
File Encoding         : 65001

Date: 2017-10-28 13:28:22
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for app
-- ----------------------------
DROP TABLE IF EXISTS `app`;
CREATE TABLE `app` (
  `AppID` varchar(45) NOT NULL,
  `AppName` varchar(45) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`AppID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for exchange
-- ----------------------------
DROP TABLE IF EXISTS `exchange`;
CREATE TABLE `exchange` (
  `AppID` varchar(45) NOT NULL,
  `SysNo` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) DEFAULT NULL,
  `Memo` varchar(200) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`SysNo`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for message
-- ----------------------------
DROP TABLE IF EXISTS `message`;
CREATE TABLE `message` (
  `MessageId` char(36) NOT NULL,
  `Exchange` varchar(45) DEFAULT NULL,
  `Topic` varchar(45) DEFAULT NULL,
  `MessageText` varchar(500) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `HashFingerprint` VARCHAR(45) DEFAULT NULL,
  `ReferenceIdentifier` VARCHAR(45) DEFAULT NULL,
  PRIMARY KEY (`MessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for processfailrecord
-- ----------------------------
DROP TABLE IF EXISTS `processfailrecord`;
CREATE TABLE `processfailrecord` (
  `MessageId` char(36) NOT NULL,
  `Topic` varchar(45) DEFAULT NULL,
  `FailRecord` text,
  `TimePeriod` bigint(64) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for subscribemessage
-- ----------------------------
DROP TABLE IF EXISTS `subscribemessage`;
CREATE TABLE `subscribemessage` (
  `SysNo` int(11) NOT NULL AUTO_INCREMENT,
  `MessageId` char(36) NOT NULL,
  `Topic` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `RetryCount` varchar(45) DEFAULT '0',
  `ProcessSuccessDate` datetime DEFAULT NULL,
  `TimePeriod` bigint(64) DEFAULT NULL,
  PRIMARY KEY (`SysNo`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for topic
-- ----------------------------
DROP TABLE IF EXISTS `topic`;
CREATE TABLE `topic` (
  `AppID` varchar(45) NOT NULL,
  `ExchangeSysNo` int(11) DEFAULT NULL,
  `SysNo` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) DEFAULT NULL,
  `Memo` varchar(200) DEFAULT NULL,
  `Status` int(11) NOT NULL,
  `ProcessorType` int(11) DEFAULT NULL,
  `ProcessorConfig` varchar(200) DEFAULT NULL,
  `ProcessFailNotifyEmails` varchar(200) DEFAULT NULL,
  `ProcessorMaintainer` varchar(45) DEFAULT NULL,
  `CreateUserSysNo` int(11) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdateUserSysNo` int(11) DEFAULT NULL,
  `UpdateDate` datetime DEFAULT NULL,
  PRIMARY KEY (`SysNo`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `server`;
CREATE TABLE `server` (
  `ServerID` char(36) NOT NULL,
  `ServerHost` varchar(45) DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ServerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
