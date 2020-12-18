using APIBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MigLibUtils.Services.LearnWorlds
{
    public class Main
    {
       public static SSOReturn RegisterUser(string email, string username, string avatar, string redirectUrl, string userId, out string error)
        {
            var ret = BaseApi.CallService<SSOReturn>("sso", out error, 
                "email".AndValue(email), 
                "username".AndValue(username), 
                "avatar".AndValue(avatar), 
                "redirectUrl".AndValue(redirectUrl), 
                "userId".AndValue(userId));
            return ret;
        }

    }
}
