﻿using RestWithASPNETUdemy.Data.Converter.VO;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;

namespace RestWithASPNETUdemy.Repository
{
    public interface IUserRepository
    {
        User ValidadeCredentials(UserVO user);

        User RefreshUserInfo(User user);
    }
}
