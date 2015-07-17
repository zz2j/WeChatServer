using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.Helpers;

namespace Wechat.Menu
{
    class Program
    {
        static void Main(string[] args)
        {
#if SETTING
            string appId = System.Configuration.ConfigurationManager.AppSettings["AppId"].ToString();
            string appSecret = System.Configuration.ConfigurationManager.AppSettings["AppSecret"].ToString();
            string appWebUrl = System.Configuration.ConfigurationManager.AppSettings["AppWebUrl"].ToString();
#else
            string appId = "wx4df221a4e9845aea";
            string appSecret = "c2c6e3220de9609c79e3d1f425bba0fb";
            string appHost = "http://recruit.casco.com.cn";
            
#endif
            
            var appOauth2 = appHost + "/oauth";
            var appUrl =
                string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type={2}&scope={3}&state={4}#wechat_redirect",
                                appId, appOauth2.UrlEncode(), "code", "snsapi_userinfo", "1");

            ButtonGroup btGroup = new ButtonGroup();
            var serviceButton = new SingleViewButton() { 
                name = "在线服务",
                url = appUrl
            };
            var userButton = new SubButton() { name = "个人中心" };
            userButton.sub_button.Add(new SingleClickButton()
            {
                name = "账号绑定",
                type = ButtonType.click.ToString(),
                key = "UserBind"
            });
            userButton.sub_button.Add(new SingleClickButton()
            {
                name = "个人信息",
                type = ButtonType.click.ToString(),
                key = "UserInfo"
            });

            btGroup.button.Add(serviceButton);
            btGroup.button.Add(userButton);

            var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret);
            WxJsonResult result = CommonApi.CreateMenu(accessToken, btGroup);
            if (result.errmsg != null && result.errmsg != "ok")
            {
                Console.WriteLine(result.errmsg);
                Console.WriteLine("------------------->Build Error！");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Success!");
                Console.ReadKey();
            }
            
        }
    }
}
