using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Senparc.Weixin.Context;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.HttpUtility;
using System.Web.Configuration;
using System.IO;
using Senparc.Weixin.MP.Agent;
using System.Text;

using Wechat.Server.Utility;
using System.Xml.Linq;
using System.Diagnostics;

namespace Wechat.Server.Wx.Handler
{
   public partial class VehicleMessageHandler : MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        #if DEBUG
                string agentUrl = "http://localhost:12222/App/Weixin/4";
                string agentToken = "27C455F496044A87";
                string wiweihiKey = "CNadjJuWzyX5bz5Gn+/XoyqiqMa5DjXQ";
        #else
                //下面的Url和Token可以用其他平台的消息
                private string agentUrl = WebConfigurationManager.AppSettings["WeixinAgentUrl"];
                private string agentToken = WebConfigurationManager.AppSettings["WeixinAgentToken"];//Token
                private string wiweihiKey = WebConfigurationManager.AppSettings["WeixinAgentWeiweihiKey"];
        #endif

        private string appId = WebConfigurationManager.AppSettings["WeixinAppId"];
        private string appSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];
        private string bindToken = WebConfigurationManager.AppSettings["WeixinAppBindToken"];
        private string webUrl = WebConfigurationManager.AppSettings["WeixinAppWebUrl"];
        private string userInfoPath = Utility.Server.GetMapPath("~/App_Data/UserInfo.xml");

        public VehicleMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            WeixinContext.ExpireMinutes = 3;
            bindToken = string.IsNullOrEmpty(bindToken) ? "casco.zz2j" : bindToken;
            webUrl = string.IsNullOrEmpty(webUrl) ? "wechatweb.apphb.com" : webUrl;
        }

        public override void OnExecuting()
        {
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }


        #region message handler
       /// <summary>
       /// Subscribe
       /// </summary>
       /// <param name="requestMessage"></param>
       /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            string text = "欢迎关注卡斯柯微信服务号！";
            responseMessage.Content = text;
            return responseMessage;
            //return base.OnEvent_SubscribeRequest(requestMessage);
        }
        /// <summary>
        /// Default Message
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            string errorInfo = @"该消息未被识别，如需帮助，请查看使用指南或者联系信息管理部相关人员！";
            responseMessage.Content = errorInfo;
            return responseMessage;
        }
        /// <summary>
        /// Pre-process
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Text message
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "呵呵";
            return responseMessage;
        }
        //click event:Help/Bind
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase responseMessage = null;
            
            if(requestMessage.EventKey == "Help")
            {
                //todo
                var enhancedResponseMessage = CreateResponseMessage<ResponseMessageNews>();
                responseMessage = enhancedResponseMessage;
                enhancedResponseMessage.Articles.Add(new Article()
                {
                    Title = "使用指南",
                    Description = "如需帮助，请联系信息管理部相关负责人",
                    PicUrl = "http://www.jhmmxxzx.com/upload/image/20140716/20140716162584538453.jpg",
                    Url = "http://www.casco.com.cn"
                });
                return responseMessage;
            }
            if (requestMessage.EventKey == "Bind")
            {
                var enhancedResponseMessage = CreateResponseMessage<ResponseMessageText>();
                //enhancedResponseMessage.Content = "www.baidu.com";
                //return enhancedResponseMessage;

                StringBuilder sb = new StringBuilder();
                Random rd = new Random();
                string nonce = string.Empty;
                for (int i = 0; i < 8; i++)
                {
                    int r = rd.Next(122);
                    char c = (char)r;
                    sb.Append(c);
                }
                nonce = sb.ToString();

                DateTime ancient = new DateTime(1970, 1, 1);
                long timestamp = (DateTime.UtcNow.Ticks - ancient.Ticks) / 10000000;

                var arr = new[] { bindToken, timestamp.ToString(), nonce }.OrderBy(z => z).ToArray();
                var arrString = string.Join("", arr);
                var sha1 = System.Security.Cryptography.SHA1.Create();
                var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
                StringBuilder enText = new StringBuilder();
                foreach (var b in sha1Arr)
                {
                    enText.AppendFormat("{0:x2}", b);
                }

                string loginUrl = string.Format("{0}/Login?openid={1}&signature={2}&timestamp={3}&nonce={4}",
                    webUrl, requestMessage.FromUserName, enText.ToString(), timestamp.ToString(), nonce);

                string text = string.Format("欢迎使用卡斯柯微信服务号，绑定账号请访问下面的地址：\r\n\r\n<a href=\"{0}\">点击这里</a>",loginUrl.UrlEncode());
                enhancedResponseMessage.Content = text;
                return enhancedResponseMessage;
            }
            return base.OnEvent_ClickRequest(requestMessage);
        }
        
        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "Web page: " + requestMessage.EventKey;
            return responseMessage;
        }
        #endregion

        #region private method
        /// <summary>
        ///Bind information
        /// </summary>
        /// <returns></returns>
        private string GetBindUserInfo()
        {
            return string.Format(
                @"欢迎使用卡斯柯移动车辆预定服务！
目前您还未对此微信号进行绑定，无法使用预约服务，
请输入工号，例如：61322，发送此消息进行绑定。
更多信息请点击菜单【使用指南】。

"
                );
        }
        /// <summary>
        /// Check Bound
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private bool CheckBound(RequestMessageText requestMessage)
        {
            bool isBound;
            string fromUser = requestMessage.FromUserName;
            XElement User = null;
            try
            {
                XElement root = XElement.Load(userInfoPath);
                User = root.Element("Users").Elements("User")
                    .Where(s => s.Element("Openid").Value.ToString() == fromUser)
                    .SingleOrDefault();
            }
            catch(Exception ex){
                
            }
            isBound = User == null ? false : true;
            return isBound;
        }
        /// <summary>
        /// Week
        /// </summary>
        /// <returns></returns>
        private static string GetWeekDay()
        {
            string day = string.Empty;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    day = "星期五";
                    break;
                case DayOfWeek.Monday:
                    day = "星期一";
                    break;
                case DayOfWeek.Saturday:
                    day = "星期六";
                    break;
                case DayOfWeek.Sunday:
                    day = "星期天";
                    break;
                case DayOfWeek.Thursday:
                    day = "星期四";
                    break;
                case DayOfWeek.Tuesday:
                    day = "星期五";
                    break;
                case DayOfWeek.Wednesday:
                    day = "星期二";
                    break;
                default:
                    break;
            }
            return day;
        }
        #endregion
    }
}