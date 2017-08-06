﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using WeiXinApi.Util;
using WeiXinApi.Model;
using System.Net;
using System.Web.UI;
namespace WeiXinApi
{
    /// <summary>
    /// weixinapi 的摘要说明
    /// </summary>
    public class weixinapi : IHttpHandler
    {
        //公众号上接口字段
        string Token = "key";
        /// <summary>
        /// AppId 要与 微信公共平台 上的 AppId 保持一致
        /// </summary>
        string AppId = "wx4e9ab80a5ee0e214";
        /// <summary>
        /// 加密用 
        /// </summary>
        string AESKey = "UU9ETyPbqgva4UP164HEJipLpCKY47TzFFsM1za7GH5";

       
        wxmessage wx = new wxmessage();
        FunctionAll fuc = new FunctionAll();

        string RequestLocation = String.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            MyMenu();
        }

        public void ProcessRequest(HttpContext context)
        {

            try
            {
                Stream stream = context.Request.InputStream;
                byte[] byteArray = new byte[stream.Length];
                stream.Read(byteArray, 0, (int)stream.Length);
                string postXmlStr = System.Text.Encoding.UTF8.GetString(byteArray);
                if (!string.IsNullOrEmpty(postXmlStr))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(postXmlStr);
                    //if (string.IsNullOrWhiteSpace(AppId))
                    //{
                    //    //DataTable dt = ConfigDal.GetConfig(WXMsgUtil.GetFromXML(doc, "ToUserName"));
                    //    //DataRow dr = dt.Rows[0];
                    //    //sToken = dr["Token"].ToString();
                    //    //sAppID = dr["AppID"].ToString();
                    //    //sEncodingAESKey = dr["EncodingAESKey"].ToString();


                    //}

                    //if (!string.IsNullOrWhiteSpace(sAppID))  //没有AppID则不解密(订阅号没有AppID)
                    //{
                    //    //解密
                    //    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(sToken, sEncodingAESKey, sAppID);
                    //    string signature = context.Request["msg_signature"];
                    //    string timestamp = context.Request["timestamp"];
                    //    string nonce = context.Request["nonce"];
                    //    string stmp = "";
                    //    int ret = wxcpt.DecryptMsg(signature, timestamp, nonce, postXmlStr, ref stmp);
                    //    if (ret == 0)
                    //    {
                    //        doc = new XmlDocument();
                    //        doc.LoadXml(stmp);

                    //        try
                    //        {
                    //            responseMsg(context, doc);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            FileLogger.WriteErrorLog(context, ex.Message);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        FileLogger.WriteErrorLog(context, "解密失败，错误码：" + ret);
                    //    }
                    //}
                    //else
                    //{
                    //    responseMsg(context, doc);
                    //}
                    string result;
                    var rootElement = doc.DocumentElement;
                    if (rootElement == null)
                    {
                        return;
                    }
                    else
                    {
                        //获取用户发来的信息
                        //string msgType =WeiXinXML.GetFromXML(doc, "MsgType");
                        FunctionAll fuc = new FunctionAll();
                        fuc.GetMsgModel(doc);
                        wx.MsgType = WeiXinXML.GetFromXML(doc, "MsgType");
                        switch (wx.MsgType)
                        {
                            case "text"://文本
                                string Content = rootElement.SelectSingleNode("Content").InnerText;
                                RequestLocation = Content;
                                result = WeiXinXML.CreateTextMsg(doc, Content);
                                LogHelper.WriteLog(result);
                                break;
                            //case "location"://文本
                            //    wx.Latitude = rootElement.SelectSingleNode("Latitude").InnerText;
                            //    wx.Longitude = rootElement.SelectSingleNode("Longitude").InnerText;
                            //    wx.Precision = rootElement.SelectSingleNode("Precision").InnerText;
                            //    string sbChr = "您的纬度是：" + wx.Latitude + "您的经度是：" + wx.Longitude;
                            //    LogHelper.WriteLog(sbChr);
                            //    break;

                            default:
                                break;
                        }
                    }

                    //回复消息
                    responseMsg(context, doc);

                }
                else
                {
                    valid(context);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);

            }
        }


        public void valid(HttpContext context)
        {
            var echostr = context.Request["echoStr"].ToString();
            if (checkSignature(context) && !string.IsNullOrEmpty(echostr))
            {
                context.Response.Write(echostr);
                context.Response.Flush();       //推送...不然微信平台无法验证token
            }
        }

        /// <summary>
        /// 第一步，公众号开发者验证方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool checkSignature(HttpContext context)
        {
            var signature = context.Request["signature"].ToString();
            var timestamp = context.Request["timestamp"].ToString();
            var nonce = context.Request["nonce"].ToString();
            var token = "key";
            string[] ArrTmp = { token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void responseMsg(HttpContext context, XmlDocument xmlDoc)
        {

            fuc.GetMsgModel(xmlDoc);
            string fromUser =WeiXinXML.GetFromXML(xmlDoc, "FromUserName");
            string toUser = WeiXinXML.GetFromXML(xmlDoc, "ToUserName");
            //位置没有获取，待完成。
            string Longitude = WeiXinXML.GetFromXML(xmlDoc, "Longitude");
            string Latitude = WeiXinXML.GetFromXML(xmlDoc, "Latitude");
            string result = "";
            //string msgType = WeiXinXML.GetFromXML(xmlDoc, "MsgType");
            switch (wx.MsgType)
            {
                case "event":
                    switch (WeiXinXML.GetFromXML(xmlDoc, "Event"))
                    {
                        case "subscribe": //订阅
                            result = WeiXinXML.subscribeRes(xmlDoc);
                            break;
                        case "unsubscribe": //取消订阅
                            break;
                        case "CLICK":

                            break;
                        default:
                            break;
                    }
                    break;
                case "text":
                    string text = WeiXinXML.GetFromXML(xmlDoc, "Content");
                    //if (text == "位置")
                    //{
                    //    result = WeiXinXML.ReArticle(fromUser, toUser, "点击图片查看您附近的防疫站", "啦啦啦啦啦", "http://119.29.20.29/test.jpg", "http://m.amap.com/around/?locations=" + Longitude + "," + Latitude + "&keywords=" + RequestLocation + "&defaultIndex=3&defaultView=map&searchRadius=5000&key=33fe5b1e0fc0023eb1cd28b392d5e70f");
                    //}
                    //else 
                if (text != null)
                    {
                       // result = WeiXinXML.CreateTextMsg(xmlDoc, TuLing.GetTulingMsg(text));
                        result = WeiXinXML.ReArticle(fromUser, toUser, "点击图片查看您附近的" + RequestLocation + "", @"测试测试测试测试", "http://119.29.20.29/image/navi.jpg", "http://m.amap.com/around/?locations=" + Longitude + "," + Latitude + "&keywords=" + RequestLocation + "&defaultIndex=3&defaultView=map&searchRadius=5000&key=33fe5b1e0fc0023eb1cd28b392d5e70f");
                    }
                    else
                    {
                        result = WeiXinXML.CreateTextMsg(xmlDoc, "回复错误！");
                    }

                    break;
                case "location":
                    result = WeiXinXML.ReArticle(wx.FromUserName, wx.ToUserName, "点击图片查看您附近的防疫站", "啦啦啦啦啦", "http://119.29.20.29/navi.jpg", "http://m.amap.com/around/?locations=" + wx.Longitude + "," + wx.Latitude + "&keywords=疾控中心&defaultIndex=3&defaultView=map&searchRadius=5000&key=33fe5b1e0fc0023eb1cd28b392d5e70f");
                    break;
            }



            /////消息加密
            //if (!string.IsNullOrWhiteSpace(sAppID))
            //{
            //    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(sToken, sEncodingAESKey, sAppID);
            //    string sEncryptMsg = ""; //xml格式的密文
            //    string timestamp = context.Request["timestamp"];
            //    string nonce = context.Request["nonce"];
            //    int ret = wxcpt.EncryptMsg(result, timestamp, nonce, ref sEncryptMsg);
            //    if (ret != 0)
            //    {
            //        LogHelper.WriteLog(this.GetType(), context+ "加密失败，错误码：" + ret);
            //        return;
            //    }

            //    context.Response.Write(sEncryptMsg);
            //    context.Response.Flush();
            //}
            //else
            //{ 

            context.Response.Write(result);
            context.Response.Flush();
            LogHelper.WriteLog(result);
            //}
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void MyMenu()
        {
            string weixin1 = "";
            weixin1 = @" {
     ""button"":[
     {	
          ""type"":""click"",
          ""name"":""你好!"",
          ""key"":""HELLO""
      },
      {
           ""type"":""click"",
           ""name"":""歌手简介"",
           ""key"":""V1001_TODAY_SINGER""
      },
      {
           ""name"":""公司网站"",
           ""sub_button"":[
            {
               ""type"":""view"",
               ""name"":""hello word"",
                ""url"":""http://www.4ugood.net""
            },
            {
               ""type"":""click"",
               ""name"":""赞一下我们"",
               ""key"":""V1001_GOOD""
            }]
       }]
 }
";
             GetPage("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + GetAccess_token() + "", weixin1);
            
            //LogHelper.WriteLog("菜单下面这个i的值" + i);
        }



        //可以通用!!
        public string GetPage(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据当前日期 判断Access_Token 是否超期  如果超期返回新的Access_Token   否则返回之前的Access_Token
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string IsExistAccess_Token()
        {

            string Token = string.Empty;
            DateTime YouXRQ;
            // 读取XML文件中的数据，并显示出来 ，注意文件路径
            string filepath = HttpContext.Current.Server.MapPath("XMLFile.xml");

            StreamReader str = new StreamReader(filepath, System.Text.Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            Token = xml.SelectSingleNode("xml").SelectSingleNode("Access_Token").InnerText;
            YouXRQ = Convert.ToDateTime(xml.SelectSingleNode("xml").SelectSingleNode("Access_YouXRQ").InnerText);

            if (DateTime.Now > YouXRQ)
            {
                DateTime _youxrq = DateTime.Now;
                Access_token mode = GetAccess_token();
                xml.SelectSingleNode("xml").SelectSingleNode("Access_Token").InnerText = mode.access_token;
                _youxrq = _youxrq.AddSeconds(int.Parse(mode.expires_in));
                xml.SelectSingleNode("xml").SelectSingleNode("Access_YouXRQ").InnerText = _youxrq.ToString();
                xml.Save(filepath);
                Token = mode.access_token;
            }
            return Token;
        }



        public static Access_token GetAccess_token()
        {
            string appid = "wx3fd9b86495ce9609";
            string secret = "6f6a79781e36f7c69e48533f209bc226";
            string strUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
            Access_token mode = new Access_token();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);

            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();

                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                string content = reader.ReadToEnd();
                //Response.Write(content);
                //在这里对Access_token 赋值
                Access_token token = new Access_token();
                token = JsonHelper.ParseFromJson<Access_token>(content);
                mode.access_token = token.access_token;
                mode.expires_in = token.expires_in;
            }
            return mode;
        }

    }
}

   