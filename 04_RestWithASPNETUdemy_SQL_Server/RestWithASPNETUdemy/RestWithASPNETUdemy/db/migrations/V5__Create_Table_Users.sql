CREATE TABLE IF NOT EXISTS `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_name` varchar(50) NOT NULL DEFAULT '0',
  `password` varchar(130) NOT NULL DEFAULT '0',
  `full_name` varchar(120) NOT NULL DEFAULT '0',
  `refresh_token` varchar(500) DEFAULT '0',
  `refresh_token_expiry_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;