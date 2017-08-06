﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using WeiXinApi.Util;

namespace WeiXinApi.Util
{
    public class WeiXinXML
    {

        public static string CreateTextMsg(XmlDocument xmlDoc, string content)
        {
            string strTpl = string.Format(@"<xml>
                <ToUserName><![CDATA[{0}]]></ToUserName>
                <FromUserName><![CDATA[{1}]]></FromUserName>
                <CreateTime>{2}</CreateTime>
                <MsgType><![CDATA[text]]></MsgType>
                <Content><![CDATA[{3}]]></Content>
                </xml>", GetFromXML(xmlDoc, "FromUserName"), GetFromXML(xmlDoc, "ToUserName"),
                       DateTime2Int(DateTime.Now), content);

            return strTpl;
        }
        /// <summary>
        /// 订阅后回复
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static string subscribeRes(XmlDocument xmlDoc)
        {
            string fromUser =GetFromXML(xmlDoc, "FromUserName");
            string toUser = GetFromXML(xmlDoc, "ToUserName");
            string Title = "欢迎关注嘻嘻爸爸开发的公众号";
            string Description = "i love you my baby";
            string PicUrl = "http://119.29.20.29/image/test.jpg";
            string Url = "http://119.29.20.29/WebPage/index.html";
            string result = WeiXinXML.ReArticle(fromUser, toUser, Title, Description, PicUrl, Url);
            return result;
        }


        /// <summary>
        /// 回复单图文
        /// </summary>
        /// <param name="FromUserName">发送给谁(openid)</param>
        /// <param name="ToUserName">来自谁(公众账号ID)</param>
        /// <param name="Title">标题</param>
        /// <param name="Description">详情</param>
        /// <param name="PicUrl">图片地址</param>
        /// <param name="Url">地址</param>
        /// <returns>拼凑的XML</returns>
        public static string ReArticle(string FromUserName, string ToUserName, string Title, string Description, string PicUrl, string Url)
        {
            string XML = "<xml><ToUserName><![CDATA[" + FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + ToUserName + "]]></FromUserName>";//发送给谁(openid)，来自谁(公众账号ID)
            XML += "<CreateTime>" + DateTime2Int(DateTime.Now) + "</CreateTime>";//回复时间戳
            XML += "<MsgType><![CDATA[news]]></MsgType><Content><![CDATA[]]></Content><ArticleCount>1</ArticleCount><Articles>";
            XML += "<item><Title><![CDATA[" + Title + "]]></Title><Description><![CDATA[" + Description + "]]></Description><PicUrl><![CDATA[" + PicUrl + "]]></PicUrl><Url><![CDATA[" + Url + "]]></Url></item>";
            XML += "</Articles><FuncFlag>0</FuncFlag></xml>";
            return XML;
        }

       
        public static int DateTime2Int(DateTime dt)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(dt - startTime).TotalSeconds;
        }

        public static string GetFromXML(XmlDocument xmlDoc, string name)
        {
            XmlNode node = xmlDoc.SelectSingleNode("xml/" + name);
            if (node != null && node.ChildNodes.Count > 0)
            {
               
                return node.ChildNodes[0].Value;
                
            }
            return "";
        }
    }
}