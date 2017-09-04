using System;
using cn.jpush.api;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;

namespace ZhiDiExt
{
    public static class PushMessageNew
    {
        //public static string AppKey = "c8aa7ee5d3405446799514fd";
        //public static string MasterSecret = "227cc1ea37cecc9d5952b66c";
        private const string PushKey = "10c42a44867caec9933571c9";
        private const string PushSecret = "77e26f30eb336c8b25be9c60";
        private const string AndroidAppKey = PushKey;//"bed16fdc185dcb60c2824b56";//"bed16fdc185dcb60c2824b56";//old"377e0a450084ca84a3646e5d";
        private const string AndroidMasterSecret = PushSecret;//"8d7f195fb0bc6bd520ca2a02";//"8d7f195fb0bc6bd520ca2a02";//old"0184f419c06e859a2c173f74";

        private const string IosAppKey = PushKey;//"10c42a44867caec9933571c9";//"10c42a44867caec9933571c9";// "447aa91f9950ce19eff60658";//old"c8f965d1602338f2c4939390";
        private const string IosMasterSecret = PushSecret;// "77e26f30eb336c8b25be9c60";//"77e26f30eb336c8b25be9c60";//"66d564e6273f1ff6b0ed6b95";//old"35ed532ab4b6cdee54281d4a";
        public static string NullValue = "";

        public static void PushMessage(string message, int notificationType, int updateDocumentTypeId, string tag, string updateCity, string updateDocumentType, int updateCityId)
        {
            try
            {
                if (notificationType == 1)
                {
                    var client = new JPushClient(AndroidAppKey, AndroidMasterSecret);
                    PushPayload payload = PushObjectUpdate(message, notificationType);
                    client.SendPush(payload);
                }
                else
                {
                    var clientIos = new JPushClient(IosAppKey, IosMasterSecret);
                    var payload = PushAndroid_IosDocument(message, notificationType, tag, updateDocumentTypeId, updateCity, updateDocumentType, updateCityId);
                    //PushPayload payload = PushIosObjectDocument(message, notificationType, tag, updateDocumentTypeId, updateCity, updateDocumentType, updateCityId);
                    clientIos.SendPush(payload);
                    //var clientAndroid = new JPushClient(AndroidAppKey, AndroidMasterSecret);
                    //payload = PushAndroidObjectDocument(message, notificationType, tag, updateDocumentTypeId, updateCity, updateDocumentType, updateCityId);
                    //clientAndroid.SendPush(payload);
                }
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("PushMessageNew").Error(message, ex);
                throw;
            }
        }

        private static PushPayload PushObjectUpdate(string message, int notificationType)
        {
            var pushPayload = new PushPayload
            {
                platform = Platform.android_ios(),
                audience = Audience.all(),
            };
            var notification = new Notification().setAlert(message);
            notification.AndroidNotification =
               new AndroidNotification().setAlert(message)
                   .AddExtra("NotificationType", notificationType)
                   .AddExtra("message", message);
            pushPayload.notification = notification;
            return pushPayload;
        }

        /// <summary>
        /// IOS文档消息推送
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notificationType"></param>
        /// <param name="tag"></param>
        /// <param name="updateDocumentTypeId"></param>
        /// <param name="updateCity"></param>
        /// <param name="updateDocumentType"></param>
        /// <param name="updateCityId"></param>
        /// <returns></returns>
        private static PushPayload PushAndroid_IosDocument(string message, int notificationType, string tag, int updateDocumentTypeId, string updateCity, string updateDocumentType, int updateCityId)
        {
            var pushPayload = new PushPayload
            {
                platform = Platform.android_ios(),
                audience = tag == "all" ? Audience.all() : Audience.s_tag(tag)
            };

            var notification = new Notification().setAlert(message);
            notification.AndroidNotification =
                new AndroidNotification().setAlert(message)
                    .AddExtra("UpdateDocumentTypeID", updateDocumentTypeId)
                    .AddExtra("NotificationType", notificationType)
                    .AddExtra("message", message);
            notification.IosNotification =
                new IosNotification().setAlert(message)
                    .AddExtra("UpdateDocumentTypeID", updateDocumentTypeId)
                    .AddExtra("NotificationType", notificationType)
                    .AddExtra("message", message)
                    .setSound("default")
                    .setContentAvailable(true);

            if (!string.IsNullOrEmpty(updateCity))
            {
                notification.IosNotification.AddExtra("UpdateCity", updateCity);
                notification.AndroidNotification.AddExtra("UpdateCity", updateCity);
            }
            if (!string.IsNullOrEmpty(updateDocumentType))
            {
                notification.IosNotification.AddExtra("UpdateDocumentType", updateDocumentType);
                notification.AndroidNotification.AddExtra("UpdateDocumentType", updateDocumentType);
            }
            if (updateCityId > 0)
            {
                notification.IosNotification.AddExtra("UpdateCityId", updateCityId);
                notification.AndroidNotification.AddExtra("UpdateCityId", updateCityId);
            }
            pushPayload.notification = notification;
            return pushPayload;
        }

        /// <summary>
        /// IOS文档消息推送
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notificationType"></param>
        /// <param name="tag"></param>
        /// <param name="updateDocumentTypeId"></param>
        /// <param name="updateCity"></param>
        /// <param name="updateDocumentType"></param>
        /// <param name="updateCityId"></param>
        /// <returns></returns>
        private static PushPayload PushIosObjectDocument(string message, int notificationType, string tag, int updateDocumentTypeId, string updateCity, string updateDocumentType, int updateCityId)
        {
            var pushPayload = new PushPayload
            {
                platform = Platform.ios(),
                audience = tag == "all" ? Audience.all() : Audience.s_tag(tag)
            };

            var notification = new Notification().setAlert(message);
            //notification.AndroidNotification =
            //    new AndroidNotification().setAlert(NullValue)
            //        .AddExtra("UpdateDocumentTypeID", updateDocumentTypeId)
            //        .AddExtra("NotificationType", notificationType)
            //        .AddExtra("message", message);
            notification.IosNotification =
                new IosNotification().setAlert(message)
                    .AddExtra("UpdateDocumentTypeID", updateDocumentTypeId)
                    .AddExtra("NotificationType", notificationType)
                    .AddExtra("message", message)
                    .setSound("default")
                    .setContentAvailable(true);

            if (!string.IsNullOrEmpty(updateCity))
            {
                //notification.AndroidNotification.AddExtra("UpdateCity", updateCity);
                notification.IosNotification.AddExtra("UpdateCity", updateCity);
            }
            if (!string.IsNullOrEmpty(updateDocumentType))
            {
                //notification.AndroidNotification.AddExtra("UpdateDocumentType", updateDocumentType);
                notification.IosNotification.AddExtra("UpdateDocumentType", updateDocumentType);
            }
            if (updateCityId > 0)
            {
                notification.IosNotification.AddExtra("UpdateCityId", updateCityId);
            }
            pushPayload.notification = notification;
            return pushPayload;
        }

        /// <summary>
        /// Android文档消息推送
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notificationType"></param>
        /// <param name="tag"></param>
        /// <param name="updateDocumentTypeId"></param>
        /// <param name="updateCity"></param>
        /// <param name="updateDocumentType"></param>
        /// <param name="updateCityId"></param>
        /// <returns></returns>
        private static PushPayload PushAndroidObjectDocument(string message, int notificationType, string tag, int updateDocumentTypeId, string updateCity, string updateDocumentType, int updateCityId)
        {
            var pushPayload = new PushPayload
            {
                platform = Platform.android(),
                audience = tag == "all" ? Audience.all() : Audience.s_tag(tag)
            };

            var notification = new Notification().setAlert(message);
            notification.AndroidNotification =
                new AndroidNotification().setAlert(message)
                    .AddExtra("UpdateDocumentTypeID", updateDocumentTypeId)
                    .AddExtra("NotificationType", notificationType)
                    .AddExtra("message", message);

            if (!string.IsNullOrEmpty(updateCity))
            {
                notification.AndroidNotification.AddExtra("UpdateCity", updateCity);
            }
            if (!string.IsNullOrEmpty(updateDocumentType))
            {
                notification.AndroidNotification.AddExtra("UpdateDocumentType", updateDocumentType);
            }
            if (updateCityId > 0)
            {
                notification.AndroidNotification.AddExtra("UpdateCityId", updateCityId);
            }
            pushPayload.notification = notification;
            return pushPayload;
        }
    }
}