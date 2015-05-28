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
using System.Web.Configuration;
using System.IO;
using Senparc.Weixin.MP.Agent;
using System.Text;

using Wechat.Server.Utility;
using System.Xml.Linq;

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
        private string userInfoPath = Utility.Server.GetMapPath("~/App_Data/UserInfo.xml");

        public VehicleMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            WeixinContext.ExpireMinutes = 3;
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
        /// 默认消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            string errorInfo = @"该消息未被识别，如需帮助，请查看使用指南！";
            responseMessage.Content = errorInfo;
            return responseMessage;
        }
        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            bool isBound = CheckBound(requestMessage);

            if(requestMessage.Content=="BookClick"|| requestMessage.Content=="MyBooking"||
                requestMessage.Content =="VehicleManager"){
                    if (!isBound) {
                        var responseMessage = CreateResponseMessage<ResponseMessageText>();
                        responseMessage.Content = GetBindUserInfo();
                        return responseMessage;
                    }                
            }
            return null;
        }

        /// <summary>
        /// 文本信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
             //调用远程Webservice，校验工号和密码
             //问题1，响应时间>5秒，微信服务器不响应了
             //问题2，校验方式，安全问题
             //这里跳过校验，只实现简单的6/2打头的5位数字校验，后续待实现
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            bool isBound = CheckBound(requestMessage);
            if (isBound) return responseMessage;
            else
            {
                int Uid;
                try
                {
                    Uid = Convert.ToInt32(requestMessage.Content);
                }
                catch (Exception ex)
                {
                    var exType = ex.GetType();
                    if (exType.Name == "FormatException")
                    {
                        responseMessage.Content = GetBindUserInfo();
                    }
                    else if (exType.Name == "OverflowException")
                    {
                        responseMessage.Content = @"您输入的工号太大了！\r\n更多信息请阅读【使用指南】";
                    }
                    return responseMessage;
                }
                if (Uid.ToString().Length != 5 || (Uid.ToString().Substring(0,1)!="6" && Uid.ToString().Substring(0,1) != "2"))
                {
                    responseMessage.Content = @"您输入的工号不正确！\r\n更多信息请阅读【使用指南】";
                    return responseMessage;
                }
                else 
                { 
                    try{
                    XElement root = XElement.Load(userInfoPath);
                    root.Element("Users").Add(
                        new XElement("User",
                            new XElement("Openid",requestMessage.FromUserName),
                            new XElement("Pernr",Uid.ToString()),
                            new XElement("BindingTime",DateTime.Now.ToShortDateString()),
                            new XElement("Status","OK")
                            )
                        );
                    root.Save(userInfoPath);
                    }catch(Exception ex){
                        //Log for debug
                        string error;
                        error = string.Format("xml错误: {0}",ex.Message);
                        responseMessage.Content = error;
                        return responseMessage;
                    }
                    responseMessage.Content = @"绑定成功！";
                    return responseMessage;
                }
            }
        }

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
                    Title = "卡斯柯微信平台-车辆预约",
                    Description = "使用指南",
                    PicUrl = "http://weixin.senparc.com/Images/qrcode.jpg",
                    Url = "http://weixin.senparc.com"
                });
                return responseMessage;
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
        /// 提醒
        /// </summary>
        /// <returns></returns>
        private string GetBindUserInfo()
        {
            return string.Format(
                @"欢迎使用卡斯柯移动车辆预定服务！\r\n
目前您还未对此微信号进行绑定，无法使用预约服务，
请输入工号，例如：61322，发送此消息进行绑定。更多信息请点击
菜单【使用指南】。"
                );
        }
        /// <summary>
        /// 判断绑定
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private bool CheckBound(RequestMessageText requestMessage)
        {
            bool isBound;
            string fromUser = requestMessage.FromUserName;
            XElement root = XElement.Load(userInfoPath);
            var User = root.Element("Users").Elements("User")
                .Where(s => s.Element("Openid").Value.ToString() == fromUser)
                .SingleOrDefault();
            isBound = User == null ? false : true;
            return isBound;
        }
        /// <summary>
        /// 周
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