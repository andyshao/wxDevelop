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
using System.Web.SessionState;
using System.Web.Script.Serialization;
namespace WeiXinApi
{
    
    /// <summary>
    /// weixinapi 的摘要说明
    /// </summary>
    public class weixinapi : IHttpHandler, IRequiresSessionState
    {
        public static string Lat = "";
        public static string Lon = "";
        public void ProcessRequest(HttpContext context)
        {
            wxmessage wxGlobal = new wxmessage();
            FunctionAll fuc = new FunctionAll();
            XmlDocument doc = new XmlDocument();
            try
            {
                fuc.MyMenu();
                string postXmlStr = PostInput();
                if (!string.IsNullOrEmpty(postXmlStr))
                {
                    doc.LoadXml(postXmlStr);
                    XmlElement root = doc.DocumentElement;
                    wxGlobal = fuc.GetWxMessage(doc);
                    //switch (wxGlobal.MsgType)
                    //{
                    //    case "text"://文本
                    //        wxGlobal.Content = doc.SelectSingleNode("xml").SelectSingleNode("Content").InnerText;
                    //        break;
                    //    case "event"://文本
                    //        wxGlobal.EventName = doc.SelectSingleNode("xml").SelectSingleNode("Event").InnerText;
                    //        wxGlobal.EventKey = doc.SelectSingleNode("xml").SelectSingleNode("EventKey").InnerText;
                    //        wxGlobal.Latitude = doc.SelectSingleNode("xml").SelectSingleNode("Latitude").InnerText;
                    //        wxGlobal.Longitude = doc.SelectSingleNode("xml").SelectSingleNode("Longitude").InnerText;
                    //        break;
                    //    case "LOCATION"://文本
                    //        wxGlobal.Location_X = doc.SelectSingleNode("xml").SelectSingleNode("Location_X").InnerText;
                    //        wxGlobal.Location_Y = doc.SelectSingleNode("xml").SelectSingleNode("Location_Y").InnerText;
                    //        break;
                    //}
                    //}///消息加密
                    //if (!string.IsNullOrWhiteSpace(sAppId))
                    //{
                    //    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(sToken, AESKey, sAppId);
                    //    string sEncryptMsg = ""; //xml格式的密文
                    //    string timestamp = context.Request["timestamp"];
                    //    string nonce = context.Request["nonce"];
                    //    int ret = wxcpt.EncryptMsg(result, timestamp, nonce, ref sEncryptMsg);
                    //    if (ret != 0)
                    //    {
                    //        LogHelper.WriteLog("加密失败，错误码：" + ret);
                    //        return;
                    //    }

                    //    context.Response.Write(sEncryptMsg);
                    //    context.Response.Flush();
                    //    LogHelper.WriteLog("系统回复的加密文:" + sEncryptMsg);
                    //}
                    
                    string result="";
                    string requestContent = "";
                    //var rootElement = doc.DocumentElement;
                    if (wxGlobal.MsgType == null)
                    {
                        return;
                    }
                    else
                    {
                        //获取用户发来的信息
                        switch (wxGlobal.MsgType)
                        {

                            case "text"://文本
                                requestContent = WeiXinXML.CreateTextMsg(doc, wxGlobal.Content);
                                LogHelper.WriteLog(requestContent);
                                result = WeiXinXML.CreateTextMsg(doc, TuLing.GetTulingMsg(wxGlobal.Content));
                                break;
                            case "location"://文本
                                //LogHelper.WriteXMLLog(doc);
                                //result = WeiXinXML.ReArticle(wxGlobal.FromUserName, wxGlobal.ToUserName, "您附近的XXX", "XXXXXXXX", "http://119.29.20.29/image/test.jpg", "http://m.amap.com/around/?locations=" + wxGlobal.Location_Y + "" + wxGlobal.Location_X + "&keywords=疾控中心&defaultIndex=3&defaultView=map&searchRadius=5000&key=33fe5b1e0fc0023eb1cd28b392d5e70f");
                                result = WeiXinXML.ReArticle(wxGlobal.FromUserName, wxGlobal.ToUserName, "您附近的XXX", "XXXXXXXX", "http://119.29.20.29/image/test.jpg", FunctionAll.GetCodeUrl(null));
                                break;
                            case "event":
                                switch (wxGlobal.EventName)
                                {
                                    case "subscribe": //订阅
                                        result = WeiXinXML.subscribeRes(doc);
                                        break;
                                    case "unsubscribe": //取消订阅
                                        break;
                                    case "CLICK":

                                        if (wxGlobal.EventKey == "HELLO")
                                            result = WeiXinXML.CreateTextMsg(doc, "微医app - 原名 挂号网，用手机挂号，十分方便！更有医生咨询、智能分诊、院外候诊、病历管理等强大功能。\r\n" +
  "预约挂号 聚合全国超过900家重点医院的预约挂号资源\r\n" +
  "咨询医生 支持医患之间随时随地图文、语音、视频方式的沟通交流\r\n" +
  "智能分诊 根据分诊自测系统分析疾病类型，提供就诊建议\r\n" +
  "院外候诊 时间自由可控，不再无谓浪费\r\n" +
  "病历管理 病历信息统一管理，个人健康及时监测\r\n" +
  "贴心服务 医疗支付、报告提取、医院地图\r\n" +
  "权威保障 国家卫计委（原卫生部）指定的全国健康咨询及就医指导平台\r\n" +
  "[微医] 目前用户量大，有些不足我们正加班加点的努力完善，希望大家用宽容的心给 [微医] 一点好评，给我们一点激励，让 [微医] 和大家的健康诊疗共成长。");
                                        if (wxGlobal.EventKey == "myprofile")
                                            result = WeiXinXML.CreateTextMsg(doc, "功能测试中，敬请期待！");
                                        if (wxGlobal.EventKey == "jkzx")
                                            result = WeiXinXML.ReArticle(wxGlobal.FromUserName, wxGlobal.ToUserName, "点击图片查看您附近的疾控中心", @"测试测试测试测试", "http://119.29.20.29/image/navi.jpg", "http://m.amap.com/around/?locations=&keywords=疾控中心&defaultIndex=3&defaultView=map&searchRadius=5000&key=33fe5b1e0fc0023eb1cd28b392d5e70f");

                                        break;
                                    case "LOCATION": //获取地理位置
                                        Lat = wxGlobal.Latitude;Lon = wxGlobal.Longitude;
                                        //string city = fuc.GetLocation(wxGlobal.Latitude,wxGlobal.Longitude);
                                        //result = WeiXinXML.CreateTextMsg(doc,city);
                                        //context.Session["wxGlobal.Latitude"] = wxGlobal.Latitude;
                                        //context.Session["wxGlobal.Longitude"] = wxGlobal.Longitude;
                                        //string wx = GetLocation(wxGlobal.Latitude, wxGlobal.Longitude);
                                        //HttpCookie Latitude = new HttpCookie("Latitude");
                                        //Latitude.Value = wxGlobal.Latitude;
                                        //HttpCookie Longitude = new HttpCookie("Longitude");
                                        //Longitude.Value = wxGlobal.Latitude;
                                        //Latitude.Expires = DateTime.Now.AddDays(1);   Longitude.Expires = DateTime.Now.AddDays(1);
                                        //context.Response.Cookies.Add(Latitude);  context.Response.Cookies.Add(Longitude); 
                                        //wxmessage xx = GetAddress(wxGlobal.Latitude,wxGlobal.Longitude);
                                        break;
                                        
                                }

                                break;
                        }
                        //if (!string.IsNullOrWhiteSpace(sAppId))  //根据appid解密字符串
                        //{

                        //    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(sToken, AESKey, sAppId);
                        //    string signature = context.Request["msg_signature"];
                        //    string timestamp = context.Request["timestamp"];
                        //    string nonce = context.Request["nonce"];
                        //    string stmp = "";
                        //    int ret = wxcpt.DecryptMsg(signature, timestamp, nonce, postXmlStr, ref stmp);
                        //    if (ret == 0)
                        //    {
                        //        doc = new XmlDocument();
                        //        doc.LoadXml(stmp);
                        //    }
                        //}
                    }
                    
                    context.Response.Write(result);
                    context.Response.Flush();
                    LogHelper.WriteLog("系统回复的明文" + result);
                }
                else
                {
                    valid(context);
                    return;
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);

            }
        }

        public void valid(HttpContext context)
        {
            var echostr = context.Request["echostr"].ToString();
            if (checkSignature(context) && !string.IsNullOrEmpty(echostr))
            {
                context.Response.Write(echostr);
                context.Response.Flush();       //推送...不然微信平台无法验证token
            }
        }
        #region 获取post请求数据
        /// <summary>
        /// 获取post请求数据
        /// </summary>
        /// <returns></returns>
        private string PostInput()
        {
            string data = "";
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            data= Encoding.UTF8.GetString(b);
            return data;
        }
        #endregion
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
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public static wxmessage GetAddress(string lat,string lon)
        {
            //string appid = "wx3fd9b86495ce9609";
            //string secret = "6f6a79781e36f7c69e48533f209bc226";
            string strUrl = "http://api.map.baidu.com/geoconv/v1/?coords="+lat+","+lon+"&from=1&to=5&ak=MGIAm9Vi0b7bKAanmtGluzTxUryGef9K";
            wxmessage mode = new wxmessage();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);

            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();

                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                string content = reader.ReadToEnd();
                //Response.Write(content);
                //在这里对Access_token 赋值
                wxmessage token = new wxmessage();
                token = JsonHelper.ParseFromJson<wxmessage>(content);
                //mode.Latitude = token.Location_X;
                //mode.expires_in = token.expires_in;
            }
            return mode;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}

   