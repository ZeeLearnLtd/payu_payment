<%@ WebHandler Language="C#" Class="UpdateEasebuzzLinkHandler" %>

using System;
using System.IO;
using System.Web;
using MVCIntegrationKit.Models;
using Newtonsoft.Json.Linq;

public class UpdateEasebuzzLinkHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request.HttpMethod != "POST")
        {
            context.Response.StatusCode = 405;
            context.Response.Write("{\"Msg\":\"Method Not Allowed\"}");
            return;
        }

        try
        {
            string body;
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                body = reader.ReadToEnd();
            }

            JObject json = JObject.Parse(body);
            string txnId = json["TXN_ID"] != null ? json["TXN_ID"].ToString() : "";
            string paymentLink = json["payment_link"] != null ? json["payment_link"].ToString() : "";

            if (string.IsNullOrEmpty(txnId) || string.IsNullOrEmpty(paymentLink))
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"Msg\":\"TXN_ID and payment_link are required\"}");
                return;
            }

            // Build JSON for SP - same format as other methods use
            string data = "{\"TXN_ID\":\"" + txnId + "\",\"payment_link\":\"" + paymentLink + "\"}";

            BussinessLogic obj = new BussinessLogic();
            obj.savejsonobject("pr_UpdatePaymentLink", data, "BPMSconnectionstring");

            context.Response.Write("{\"Msg\":\"Link Updated Successfully\"}");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write("{\"Msg\":\"Error\"}");
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
