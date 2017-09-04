using System.Web;
using System.Web.Security;
using ServiceStack.Text;

namespace ZhiDiExt
{
    public static class HttpResponseBaseExtensions
    {
        public static int SetAuthCookie<T>(this HttpResponseBase responseBase, string name, bool rememberMe, T userData)
        {
            // In order to pickup the settings from config, we create a default cookie and use its values to create a
            // new one.
            var cookie = FormsAuthentication.GetAuthCookie(name, rememberMe);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);

            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration,
                ticket.IsPersistent, userData.ToJson(), ticket.CookiePath);
            var encTicket = FormsAuthentication.Encrypt(newTicket);

            // Use existing cookie. Could create new one but would have to copy settings over...
            cookie.Value = encTicket;

            responseBase.Cookies.Add(cookie);

            return encTicket.Length;
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}