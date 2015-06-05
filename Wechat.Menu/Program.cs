﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP;
using Senparc.Weixin.HttpUtility;

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
            string appWebUrl = "http://wechatweb.apphb.com";
            string[] titles = new[] { "立即预约", "我", "我的预约", "我的车辆", "我要绑定", "使用指南" };
#endif
            var oauth2 = appWebUrl + "/oauth2";
            var url =
                string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type={2}&scope={3}&state={4}#wechat_redirect",
                                appId, oauth2.UrlEncode(), "code", "snsapi_userinfo", "1");
            string bookUrl = url;
            string userOrderUrl = appWebUrl + "/myorder";
            string carManageUrl = appWebUrl + "/orders";

            var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret);

            ButtonGroup btGroup = new ButtonGroup();

            var bookButton = new SingleViewButton() { 
                name = titles[0],
                url = bookUrl
            };

            var userButton = new SubButton() { name = titles[1] };
            userButton.sub_button.Add(new SingleViewButton() { 
                name = titles[2],
                url = userOrderUrl
            });
            userButton.sub_button.Add(new SingleViewButton()
            {
                name = titles[3],
                url = carManageUrl
            });
            userButton.sub_button.Add(new SingleClickButton()
            {
                name = titles[4],
                key = "Bind"
            });
 
            var helpButton = new SingleClickButton()
            {
                name = titles[5],
                type = ButtonType.click.ToString(),
                key = "Help"
            };

            btGroup.button.Add(bookButton);
            btGroup.button.Add(userButton);
            btGroup.button.Add(helpButton);

            WxJsonResult result = CommonApi.CreateMenu(accessToken, btGroup);
            if (result.errmsg != null && result.errmsg != "ok")
            {
                Console.WriteLine(result.errmsg);
                Console.WriteLine("------------------->创建失败！");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("创建成功，要看效果需要先取消再重新关注公众号");
                Console.ReadKey();
            }
        }
    }
}
