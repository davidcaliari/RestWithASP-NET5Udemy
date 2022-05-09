CREATE TABLE `users` (
  `id` INT(11) AUTO_INCREMENT PRIMARY KEY,
  `user_name` varchar(50) NOT NULL DEFAULT '0',
  `password` varchar(130) NOT NULL DEFAULT '0',
  `full_name` varchar(120) NOT NULL,
  `refresh_token` varchar(500) NULL DEFAULT '0',
  `refresh_toke_expiry_time` DATETIME NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
