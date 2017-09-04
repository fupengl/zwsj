using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ZhiDiExt
{
    public static class PushMessage
    {
        private const string PushKey = "10c42a44867caec9933571c9";
        private const string PushSecret = "77e26f30eb336c8b25be9c60";
        private const string AndroidAppKey = PushKey;//"bed16fdc185dcb60c2824b56";//old"377e0a450084ca84a3646e5d";
        private const string AndroidMasterSecret = PushSecret;//"8d7f195fb0bc6bd520ca2a02";//old"0184f419c06e859a2c173f74";

        private const string IosAppKey = PushKey;//"10c42a44867caec9933571c9";// "447aa91f9950ce19eff60658";//old"c8f965d1602338f2c4939390";
        private const string IosMasterSecret = PushSecret;//"77e26f30eb336c8b25be9c60";//"66d564e6273f1ff6b0ed6b95";//old"35ed532ab4b6cdee54281d4a";


        //private const string IosAppKey = "c8aa7ee5d3405446799514fd";// "447aa91f9950ce19eff60658";//old"c8f965d1602338f2c4939390";
        //private const string IosMasterSecret = "227cc1ea37cecc9d5952b66c";//"66d564e6273f1ff6b0ed6b95";//old"35ed532ab4b6cdee54281d4a";

        //private const string UserInfo1 = "377e0a450084ca84a3646e5d";
        //private const string UserInfo2 = "0184f419c06e859a2c173f74";

        //public static void PushIosMessageToTag(string deviceType, string message, int notificationType, string tag,
        //    string updateCity = "", string updateDocumentType = "")
        //{
        //    const string url = "https://api.jpush.cn/v3/push";
        //    WebRequest request = WebRequest.Create(url);
        //    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", IosAppKey, IosMasterSecret)));
        //    request.Method = "POST";
        //    request.PreAuthenticate = true;
        //    request.ContentType = "application/json";
        //    request.Credentials = new NetworkCredential(IosAppKey, IosMasterSecret);
        //    byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(UpdateMessage(deviceType, message, tag, notificationType, updateCity, updateDocumentType));//("\'{\"platform\":\"all\",\"audience\": \"all\",\"notification\":{\"alert\":\"更新了版本\", \"android\" : {\"extras\" : {\"NotificationType\" : 1}}}}\'");
        //    Stream reqstr = request.GetRequestStream();
        //    reqstr.Write(buffer, 0, buffer.Length);
        //    reqstr.Close();
        //    WebResponse response = request.GetResponse();
        //}

        public static void PushUpdateMessageToTag(string deviceType, string message, int notificationType, string tag, string updateCity = "", string updateDocumentType = "")
        {

            var deviceTypes = deviceType.Split(';');
            foreach (var d in deviceTypes)
            {
                //if (d == "android")
                //    continue;
                string tmpAppKey;
                string tmpMasterSecret;
                if (d == "android")
                {
                    tmpAppKey = AndroidAppKey;
                    tmpMasterSecret = AndroidMasterSecret;
                }
                else
                {
                    tmpAppKey = IosAppKey;
                    tmpMasterSecret = IosMasterSecret;
                }
                Push(tmpAppKey, tmpMasterSecret, d, message, notificationType, tag, updateCity, updateDocumentType);
            }
        }

        private static void Push(string appKey, string masterSecret, string deviceType, string message,
            int notificationType, string tag, string updateCity = "", string updateDocumentType = "")
        {
            try
            {
                const string url = "https://api.jpush.cn/v3/push/validate";
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.PreAuthenticate = true;
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " +
                                                   Convert.ToBase64String(
                                                       Encoding.ASCII.GetBytes(string.Format("{0}:{1}", appKey,
                                                           masterSecret)));

                request.Credentials = new NetworkCredential(appKey, masterSecret);
                byte[] buffer =
                    Encoding.GetEncoding("UTF-8")
                        .GetBytes(UpdateMessage(deviceType, message, tag, notificationType, updateCity,
                            updateDocumentType));
                Stream reqstr = request.GetRequestStream();
                reqstr.Write(buffer, 0, buffer.Length);
                reqstr.Close();
                request.GetResponse();
            }
            catch (Exception ex)
            {

            }

        }


        private static string UpdateMessage(string deviceType, string message, string tag, int notificationType, string updateCity, string updateDocumentType)
        {
            var j = new JObject { { "platform", JPlatform(deviceType) }, { "audience", AudienceToken(tag) } };
            if (deviceType == "android")
            {
                var jExtras = new JObject { { "extras", JobjectExtras(notificationType, updateCity, updateDocumentType) } };

                var j1 = new JObject
                {
                    {"alert", message},
                    {deviceType, jExtras}
                };

                j.Add("notification", j1);
            }
            else
            {
                var j2 = new JObject
                {
                    //{"content-available","true"},
                    {"alert", message},
                    {"sound", "default"},
                    {"badge", 1},
                    {"extras", JobjectExtras(notificationType, updateCity, updateDocumentType)}
                };
                var j1 = new JObject { { deviceType, j2 } };
                j.Add("notification", j1);
            }

            return j.ToString();
        }

        private static JToken JPlatform(string deviceType)
        {
            if (deviceType == "all")
            {
                return deviceType;
            }
            return new JArray { deviceType };//return "all";

        }

        private static JObject JobjectExtras(int notificationType, string updateCity = "",
            string updateDocumentType = "")
        {
            var jExtras = new JObject { { "NotificationType", notificationType } };
            if (!string.IsNullOrEmpty(updateCity))
            {
                jExtras.Add("UpdateCity", updateCity);
            }
            if (!string.IsNullOrEmpty(updateDocumentType))
            {
                jExtras.Add("UpdateDocumentType", updateDocumentType);
            }
            return jExtras;
        }

        private static JToken AudienceToken(string tag)
        {
            if (tag == "all")
            {
                return tag;
            }
            return new JObject
            {
                {"tag", new JArray {tag}}
            };
        }
    }
}