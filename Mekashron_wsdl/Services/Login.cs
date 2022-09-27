using Mekashron_wsdl.Models;
using Mekashron_wsdl.Services.Interfaces;
using System.Net;
using System.Xml;

namespace Mekashron_wsdl.Services
{
    public class Login : ISendRequest
    {
        public UserData Send(User user)
        {
            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();

            soapEnvelopeXml.LoadXml(@$"<?xml version=""1.0"" encoding=""UTF-8""?>
                <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope"" 
                xmlns:ns1=""urn:ICUTech.Intf-IICUTech"" 
                xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                xmlns:enc=""http://www.w3.org/2003/05/soap-encoding"">
                <env:Body><ns1:Login env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
                <UserName xsi:type=""xsd:string"">{user.Name}</UserName>
                <Password xsi:type=""xsd:string"">{user.Password}</Password>
                <IPs xsi:type=""xsd:string"">{user.Ip}</IPs></ns1:Login></env:Body>
                </env:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            string soapResult;
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResult);

            int resultCode = GetResultCode(xmlDoc.InnerText);
            if(resultCode > -1)
            {
                UserData userData = GetUserAttributes(xmlDoc.InnerText);
                return userData;
            }
            else
            {
                UserData userData = new UserData() { EntityId = "-1" };
                return userData;
            }
        }

        private UserData GetUserAttributes(string innerText)
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>();
            foreach(var attr in innerText.Split(','))
            {
                string normalizedAttr_1 = attr.Split(':')[0]
                    .Replace("\r\n", string.Empty)
                    .Replace("{", string.Empty)
                    .Replace("}", string.Empty)
                    .Replace("\\", string.Empty)
                    .Replace("\"", string.Empty)
                    .Trim();
                string normalizedAttr_2 = attr.Split(':')[1]
                    .Replace("\r\n", string.Empty)
                    .Replace("{", string.Empty)
                    .Replace("}", string.Empty)
                    .Replace("\\", string.Empty)
                    .Replace("\"", string.Empty)
                    .Trim();
                attrs.Add(normalizedAttr_1, normalizedAttr_2);
            }

            UserData result = new UserData() 
            { 
                EntityId      = attrs["EntityId"],
                FirstName     = attrs["FirstName"],
                LastName      = attrs["LastName"],
                Company       = attrs["Company"],
                Address       = attrs["Address"],
                City          = attrs["City"],
                Country       = attrs["Country"],
                Zip           = attrs["Zip"],
                Mobile        = attrs["Mobile"],
                Email         = attrs["Email"],
                EmailConfirm  = attrs["EmailConfirm"],
                MobileConfirm = attrs["MobileConfirm"],
                CountryID     = attrs["CountryID"],
                Status        = attrs["Status"],
                lid           = attrs["lid"],
                FTPHost       = attrs["FTPHost"],
                FTPPort       = attrs["FTPPort"]
            };

            return result;
        }

        private int GetResultCode(string innerText)
        {
            var test = innerText.Split(',');
            int resultCode = int.Parse(test[0].Split(':')[1]);
            return resultCode;
        }

        private HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://isapi.icu-tech.com/icutech-test.dll/soap/IICUTech");
            webRequest.Headers.Add(@"SOAP:Login");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "Post";
            return webRequest;
        }
    }
}
