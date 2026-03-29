-- Friend / blacklist tables (aligned with saga-revised list_friends / list_blacklist).
-- Apply to the same MySQL schema as `char`, `inventory`, etc.

CREATE TABLE IF NOT EXISTS `list_friends` (
  `CharId` int(10) unsigned NOT NULL,
  `FriendName` varchar(32) NOT NULL,
  PRIMARY KEY (`CharId`,`FriendName`),
  KEY `idx_friendname` (`FriendName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `list_blacklist` (
  `CharId` int(10) unsigned NOT NULL,
  `FriendName` varchar(32) NOT NULL,
  `Reason` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`CharId`,`FriendName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
