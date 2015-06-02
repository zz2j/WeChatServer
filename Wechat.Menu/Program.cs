using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP;

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
            string appWebUrl = "http://wechatweb.apphb.com/";
#endif
            bool test = false;
            string test_host = "http://www.casco.com.cn";

            string menu1_url = appWebUrl;
            string menu2_1_url = appWebUrl + "/MyCar";
            string menu2_2_url = appWebUrl + "/VehicleManage";

            if (test)
            {
                menu1_url = test_host;
                menu2_1_url = test_host;
                menu2_2_url = test_host;
            }

            string menu1 = "立即预约";
            string menu2 = "我";
            string menu2_1 = "我的预约";
            string menu2_2 = "我的车辆";
            string menu2_3 = "我要绑定";
            string menu3 = "使用帮助";

            var accessToken = AccessTokenContainer.TryGetToken(appId, appSecret);
            Console.WriteLine("Token： " + accessToken);

            ButtonGroup btGroup = new ButtonGroup();

            var bookButton = new SingleViewButton() { 
                name = menu1,
                url = menu1_url
            };

            var meButton = new SubButton() { name = menu2 };
            meButton.sub_button.Add(new SingleViewButton() { 
                name = menu2_1,
                url = menu2_1_url
            });
            meButton.sub_button.Add(new SingleViewButton()
            {
                name = menu2_2,
                url = menu2_2_url
            });
            meButton.sub_button.Add(new SingleClickButton()
            {
                name = menu2_3,
                key = "Bind"
            });
 
            var helpButton = new SingleClickButton()
            {
                name = menu3,
                type = ButtonType.click.ToString(),
                key = "Help"
            };

            btGroup.button.Add(bookButton);
            btGroup.button.Add(meButton);
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
