using MVCIntegrationKit.Models;
using System;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVCIntegrationKit.Controllers
{
    public class ReturnController : Controller
    {
        //
        // GET: /Return/
        BussinessLogic obj = new BussinessLogic();
        public ActionResult Index()
        {
            return View();
        }


        public async Task<string> Return(FormCollection form)
        {
            try
            {
                int s_id = 0;
                string html = "";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string amount = form["amount"].ToString();
                ViewData["amount"] = amount;

                string udf1 = form["udf1"].ToString();
                ViewData["udf1"] = udf1;
                string status = form["status"].ToString();
                ViewData["status"] = status;
                ViewData["txnid"] = form["txnid"].ToString(); ;

                string result = "";

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];// "key | txnid |  amount | productInfo | firstName |email |udf1 |||||||||| salt";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        var data = Convert.ToString(form[merc_hash_var]);
                        merc_hash_string = merc_hash_string + (data != null ? data : "");

                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != form["hash"])
                    {
                        //updatetodb(order_id, status, amount, udf1);

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        // updatetodb(order_id, status,amount, udf1);                        
                        result = "Status is successful. Hash value is matched " + order_id;
                        ViewData["Message"] = result;
                        s_id = 1;
                        // Response.Write("<span style='color:green'>" + result + "</span>");

                        //Hash value did not matched
                    }

                }

                else
                {
                    //updatetodb(order_id, status, amount, udf1);
                    result = "Status is unsuccessful. Hash value did not matched " + order_id;
                    ViewData["Message"] = result;
                    //Response.Write("<span style='color:green'>" + result + "</span>");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }

                updatetodb(order_id, status, amount, udf1);
                if (s_id == 1)
                {


                    DateTime paymentDate = DateTime.Now;


                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: green;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .success-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
    .amount {{
      font-size: 20px;
      font-weight: bold;
    }}
  </style>

  <div class=""success-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Success!</h1>
    <p>Thank you for your payment.</p>
    <p>Payment Details:</p>
    <p>Invoice Number: <strong>{udf1}</strong></p>
    <p>Payment Date: <strong>{paymentDate}</strong></p>
    <p>Amount Paid: <span class=""amount"">{amount}</span></p>  
    <p>Transaction ID: <strong>{order_id}</strong></p>
    <p>For any queries, please contact our support team.</p>
  </div>

";

                }
                else
                {
                    // Sample dynamic values
                    string errorReason = result;
                    string supportEmail = "mishelpdesk@zeelearn.com ";

                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";


                }
                return html;
            }

            catch (Exception ex)
            {
                string errorReason = ex.Message;
                string supportEmail = "mishelpdesk@zeelearn.com ";

                // Create the HTML string with dynamic values
                return $@"

  <title>Payment Failed</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


        }

        public async Task<string> zllReturn(FormCollection form)
        {
            try
            {
                int s_id = 0;
                string html = "";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string amount = form["amount"].ToString();
                string CustomerName = form["firstname"].ToString();
                string txnid = form["txnid"].ToString();
                ViewData["amount"] = amount;
                //ViewData["CustomerName"] = CustomerName;

                string udf1 = form["udf1"].ToString();
                ViewData["udf1"] = udf1;
                string status = form["status"].ToString();
                ViewData["status"] = status;
                ViewData["txnid"] = form["txnid"].ToString();

                string result = "";

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];// "key | txnid |  amount | productInfo | firstName |email |udf1 |||||||||| salt";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        var data = Convert.ToString(form[merc_hash_var]);
                        merc_hash_string = merc_hash_string + (data != null ? data : "");

                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != form["hash"])
                    {
                        //updatetodb(order_id, status, amount, udf1);

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        // updatetodb(order_id, status,amount, udf1);                        
                        result = "Status is successful. Hash value is matched " + order_id;
                        ViewData["Message"] = result;
                        s_id = 1;
                        // Response.Write("<span style='color:green'>" + result + "</span>");

                        //Hash value did not matched
                    }

                }

                else
                {
                    //updatetodb(order_id, status, amount, udf1);
                    result = "Status is unsuccessful. Hash value did not matched " + order_id;
                    ViewData["Message"] = result;
                    //Response.Write("<span style='color:green'>" + result + "</span>");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }

                updatetodbzll(order_id, status, amount, udf1);
                if (s_id == 1)
                {


                    DateTime paymentDate = DateTime.Now;


                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: green;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .success-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
    .amount {{
      font-size: 20px;
      font-weight: bold;
    }}
  </style>

  <div class=""success-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Success!</h1>
    <p>Thank you for your payment.</p>
    <p>Payment Details:</p>
    <p>Reference Number: <strong>{txnid}</strong></p>
    <p>Payment Date: <strong>{paymentDate}</strong></p>
    <p>Name: <strong>{CustomerName}</strong></p>
    <p>Amount Paid: <span class=""amount"">{amount}</span></p>  
    <p>Nature: <strong>{udf1}</strong></p>
    <p>The above is subject to clearance and subject to issue of Final GST Receipt</strong></p>
    <p>For any queries, please contact our support team.</p>
  </div>

";

                }
                else
                {
                    // Sample dynamic values
                    string errorReason = result;
                    string supportEmail = "mishelpdesk@zeelearn.com ";

                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";


                }
                return html;
            }

            catch (Exception ex)
            {
                string errorReason = ex.Message;
                string supportEmail = "mishelpdesk@zeelearn.com ";

                // Create the HTML string with dynamic values
                return $@"

  <title>Payment Failed</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


        }

        public async Task<string> mlzsReturn(FormCollection form)
        {
            try
            {
                int s_id = 0;
                string html = "";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string amount = form["amount"].ToString();
                ViewData["amount"] = amount;

                string udf1 = form["udf1"].ToString();
                ViewData["udf1"] = udf1;
                string status = form["status"].ToString();
                ViewData["status"] = status;
                ViewData["txnid"] = form["txnid"].ToString(); ;

                string result = "";

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];// "key | txnid |  amount | productInfo | firstName |email |udf1 |||||||||| salt";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["mlzs_SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        var data = Convert.ToString(form[merc_hash_var]);
                        merc_hash_string = merc_hash_string + (data != null ? data : "");

                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != form["hash"])
                    {
                        //updatetodb(order_id, status, amount, udf1);

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        // updatetodb(order_id, status,amount, udf1);                        
                        result = "Status is successful. Hash value is matched " + order_id;
                        ViewData["Message"] = result;
                        s_id = 1;
                        // Response.Write("<span style='color:green'>" + result + "</span>");

                        //Hash value did not matched
                    }

                }

                else
                {
                    //updatetodb(order_id, status, amount, udf1);
                    result = "Status is unsuccessful. Hash value did not matched " + order_id;
                    ViewData["Message"] = result;
                    //Response.Write("<span style='color:green'>" + result + "</span>");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }

                updatetodbmlzs(order_id, status, amount, udf1);
                if (s_id == 1)
                {


                    DateTime paymentDate = DateTime.Now;


                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: green;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .success-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
    .amount {{
      font-size: 20px;
      font-weight: bold;
    }}
  </style>

  <div class=""success-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Success!</h1>
    <p>Thank you for your payment.</p>
    <p>Payment Details:</p>
    <p>Invoice Number: <strong>{udf1}</strong></p>
    <p>Payment Date: <strong>{paymentDate}</strong></p>
    <p>Amount Paid: <span class=""amount"">{amount}</span></p>  
    <p>Transaction ID: <strong>{order_id}</strong></p>
    <p>For any queries, please contact our support team.</p>
  </div>

";

                }
                else
                {
                    // Sample dynamic values
                    string errorReason = result;
                    string supportEmail = "mishelpdesk@zeelearn.com ";

                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";


                }
                return html;
            }

            catch (Exception ex)
            {
                string errorReason = ex.Message;
                string supportEmail = "mishelpdesk@zeelearn.com ";

                // Create the HTML string with dynamic values
                return $@"

  <title>Payment Failed</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zeelearn.com/wp-content/uploads/2017/02/zeelearnlogo_new171.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


        }

        public async Task<string> zicaReturn(FormCollection form)
        {
            try
            {
                int s_id = 0;
                string html = "";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string amount = form["amount"].ToString();
                ViewData["amount"] = amount;

                string udf1 = form["udf1"].ToString();
                ViewData["udf1"] = udf1;
                string status = form["status"].ToString();
                ViewData["status"] = status;
                ViewData["txnid"] = form["txnid"].ToString(); ;

                string result = "";

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];// "key | txnid |  amount | productInfo | firstName |email |udf1 |||||||||| salt";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["zica_SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        var data = Convert.ToString(form[merc_hash_var]);
                        merc_hash_string = merc_hash_string + (data != null ? data : "");

                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != form["hash"])
                    {
                        //updatetodb(order_id, status, amount, udf1);

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        // updatetodb(order_id, status,amount, udf1);                        
                        result = "Status is successful. Hash value is matched " + order_id;
                        ViewData["Message"] = result;
                        s_id = 1;
                        // Response.Write("<span style='color:green'>" + result + "</span>");

                        //Hash value did not matched
                    }

                }

                else
                {
                    //updatetodb(order_id, status, amount, udf1);
                    result = "Status is unsuccessful. Hash value did not matched " + order_id;
                    ViewData["Message"] = result;
                    //Response.Write("<span style='color:green'>" + result + "</span>");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }

                updatetodbzica(order_id, status, amount, udf1);
                if (s_id == 1)
                {


                    DateTime paymentDate = DateTime.Now;


                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: green;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .success-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
    .amount {{
      font-size: 20px;
      font-weight: bold;
    }}
  </style>

  <div class=""success-container"">
    <div class=""logo"">
      <img src=""https://www.zicaindia.com/assets/img/zica/ZICA_Logo_finle.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Success!</h1>
    <p>Thank you for your payment.</p>
    <p>Payment Details:</p>
    <p>Invoice Number: <strong>{udf1}</strong></p>
    <p>Payment Date: <strong>{paymentDate}</strong></p>
    <p>Amount Paid: <span class=""amount"">{amount}</span></p>  
    <p>Transaction ID: <strong>{order_id}</strong></p>
    <p>For any queries, please contact our support team.</p>
  </div>

";

                }
                else
                {
                    // Sample dynamic values
                    string errorReason = result;
                    string supportEmail = "mishelpdesk@zeelearn.com ";

                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://www.zicaindia.com/assets/img/zica/ZICA_Logo_finle.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";


                }
                return html;
            }

            catch (Exception ex)
            {
                string errorReason = ex.Message;
                string supportEmail = "mishelpdesk@zeelearn.com ";

                // Create the HTML string with dynamic values
                return $@"

  <title>Payment Failed</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://www.zicaindia.com/assets/img/zica/ZICA_Logo_finle.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


        }

        public async Task<string> zimaStudentReturn(FormCollection form)
        {
            try
            {
                int s_id = 0;
                string html = "";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string amount = form["amount"].ToString();
                ViewData["amount"] = amount;

                string udf1 = form["udf1"].ToString();
                ViewData["udf1"] = udf1;
                string status = form["status"].ToString();
                ViewData["status"] = status;
                ViewData["txnid"] = form["txnid"].ToString(); ;

                string result = "";

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];// "key | txnid |  amount | productInfo | firstName |email |udf1 |||||||||| salt";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["zica_SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        var data = Convert.ToString(form[merc_hash_var]);
                        merc_hash_string = merc_hash_string + (data != null ? data : "");

                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != form["hash"])
                    {
                        //updatetodb(order_id, status, amount, udf1);

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        // updatetodb(order_id, status,amount, udf1);                        
                        result = "Status is successful. Hash value is matched " + order_id;
                        ViewData["Message"] = result;
                        s_id = 1;
                        // Response.Write("<span style='color:green'>" + result + "</span>");

                        //Hash value did not matched
                    }

                }

                else
                {
                    //updatetodb(order_id, status, amount, udf1);
                    result = "Status is unsuccessful. Hash value did not matched " + order_id;
                    ViewData["Message"] = result;
                    //Response.Write("<span style='color:green'>" + result + "</span>");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }

                updatetodbzima(order_id, status, amount, "0");
                if (s_id == 1)
                {


                    DateTime paymentDate = DateTime.Now;


                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: green;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .success-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
    .amount {{
      font-size: 20px;
      font-weight: bold;
    }}
  </style>

  <div class=""success-container"">
    <div class=""logo"">
      <img src=""https://zima.co.in/images/logo%402x.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Success!</h1>
    <p>Thank you for your payment.</p>
    <p>Payment Details:</p>
    <p>Invoice Number: <strong>{udf1}</strong></p>
    <p>Payment Date: <strong>{paymentDate}</strong></p>
    <p>Amount Paid: <span class=""amount"">{amount}</span></p>  
    <p>Transaction ID: <strong>{order_id}</strong></p>
    <p>For any queries, please contact our support team.</p>
  </div>

";

                }
                else
                {
                    // Sample dynamic values
                    string errorReason = result;
                    string supportEmail = "mishelpdesk@zeelearn.com ";

                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zima.co.in/images/logo%402x.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";


                }
                return html;
            }

            catch (Exception ex)
            {
                string errorReason = ex.Message;
                string supportEmail = "mishelpdesk@zeelearn.com ";

                // Create the HTML string with dynamic values
                return $@"

  <title>Payment Failed</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://zima.co.in/images/logo%402x.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


        }

        public async Task<string> zicastudentReturn(FormCollection form)
        {
            try
            {
                int s_id = 0;
                string html = "";
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string amount = form["amount"].ToString();
                ViewData["amount"] = amount;

                string udf1 = form["udf1"].ToString();
                ViewData["udf1"] = udf1;
                string status = form["status"].ToString();
                ViewData["status"] = status;
                ViewData["txnid"] = form["txnid"].ToString(); ;

                string result = "";

                string hash_seq = ConfigurationManager.AppSettings["hashSequence"];// "key | txnid |  amount | productInfo | firstName |email |udf1 |||||||||| salt";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["zica_SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        var data = Convert.ToString(form[merc_hash_var]);
                        merc_hash_string = merc_hash_string + (data != null ? data : "");

                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();



                    if (merc_hash != form["hash"])
                    {
                        //updatetodb(order_id, status, amount, udf1);

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];

                        // updatetodb(order_id, status,amount, udf1);                        
                        result = "Status is successful. Hash value is matched " + order_id;
                        ViewData["Message"] = result;
                        s_id = 1;
                        // Response.Write("<span style='color:green'>" + result + "</span>");

                        //Hash value did not matched
                    }

                }

                else
                {
                    //updatetodb(order_id, status, amount, udf1);
                    result = "Status is unsuccessful. Hash value did not matched " + order_id;
                    ViewData["Message"] = result;
                    //Response.Write("<span style='color:green'>" + result + "</span>");
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));

                }

                updatetodbzica(order_id, status, amount, "0");
                if (s_id == 1)
                {


                    DateTime paymentDate = DateTime.Now;


                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: green;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .success-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
    .amount {{
      font-size: 20px;
      font-weight: bold;
    }}
  </style>

  <div class=""success-container"">
    <div class=""logo"">
      <img src=""https://www.zicaindia.com/assets/img/zica/ZICA_Logo_finle.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Success!</h1>
    <p>Thank you for your payment.</p>
    <p>Payment Details:</p>
    <p>Invoice Number: <strong>{udf1}</strong></p>
    <p>Payment Date: <strong>{paymentDate}</strong></p>
    <p>Amount Paid: <span class=""amount"">{amount}</span></p>  
    <p>Transaction ID: <strong>{order_id}</strong></p>
    <p>For any queries, please contact our support team.</p>
  </div>

";

                }
                else
                {
                    // Sample dynamic values
                    string errorReason = result;
                    string supportEmail = "mishelpdesk@zeelearn.com ";

                    // Create the HTML string with dynamic values
                    html = $@"

  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://www.zicaindia.com/assets/img/zica/ZICA_Logo_finle.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";


                }
                return html;
            }

            catch (Exception ex)
            {
                string errorReason = ex.Message;
                string supportEmail = "mishelpdesk@zeelearn.com ";

                // Create the HTML string with dynamic values
                return $@"

  <title>Payment Failed</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      margin: 20px;
    }}
    h1 {{
      font-size: 24px;
      color: red;
    }}
    p {{
      font-size: 16px;
      margin-bottom: 10px;
    }}
    .error-container {{
      border: 1px solid #ccc;
      padding: 20px;
      max-width: 500px;
      margin: 0 auto;
    }}
    .logo {{
      text-align: center;
      margin-bottom: 20px;
    }}
  </style>

  <div class=""error-container"">
    <div class=""logo"">
      <img src=""https://www.zicaindia.com/assets/img/zica/ZICA_Logo_finle.png"" alt=""Company Logo"" width=""150"">
    </div>
    <h1>Payment Failed</h1>
    <p>We're sorry, but your payment could not be processed.</p>
    <p>Error Reason: <strong>{errorReason}</strong></p>
    <p>Please check your payment details and try again.</p>
    <p>If you continue to experience issues, please contact our support team at <a href=""mailto:{supportEmail}"">{supportEmail}</a> for assistance.</p>
  </div>

";
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");

            }


        }
        public string updatetodbzll(string TXN_ID, string PaymentStatus, string Indent_Amount, string Indent_Id)
        {
            string result = "Failed";
            try
            {
                var data = "{ 'TXN_ID' : '" + TXN_ID + "','PaymentStatus' : '" + PaymentStatus + "','Indent_Amount' : '" + Indent_Amount + "'}";
                DataTable dt = obj.GetDataSet("pr_updateCustomerPaymentDetails", data, "BPMSconnectionstring").Tables[0];
                result = "Success";
            }
            catch
            {


            }
            return result;

        }

        public string updatetodbmlzs(string TXN_ID, string PaymentStatus, string Indent_Amount, string Indent_Id)
        {
            string result = "Failed";
            try
            {
                var data = "{ 'TXN_ID' : '" + TXN_ID + "','PaymentStatus' : '" + PaymentStatus + "','Indent_Amount' : '" + Indent_Amount + "','Indent_Id':'"+ Indent_Id + "'}";
                DataTable dt = obj.GetDataSet("pr_UpdateOnlinePaymentHistoryAndPosted_Collection", data, "MLZSconnectionstring").Tables[0];
                result = "Success";

            }
            catch(Exception ex)
            {

                result = ex.Message;
            }
            return result;


        }

        public string updatetodbzica(string TXN_ID, string PaymentStatus, string Indent_Amount, string Indent_Id)
        {
            string result = "Failed";
            try
            {
                var data = "{ 'TXN_ID' : '" + TXN_ID + "','PaymentStatus' : '" + PaymentStatus + "','Indent_Amount' : '" + Indent_Amount + "','Indent_Id':'" + Indent_Id + "'}";
                DataTable dt = obj.GetDataSet("pr_UpdateOnlinePaymentHistory", data, "ZICAconnectionstring").Tables[0];
                result = "Success";

            }
            catch (Exception ex)
            {

                result = ex.Message;
            }
            return result;


        }

        public string updatetodbzima(string TXN_ID, string PaymentStatus, string Indent_Amount, string Indent_Id)
        {
            string result = "Failed";
            try
            {
                var data = "{ 'TXN_ID' : '" + TXN_ID + "','PaymentStatus' : '" + PaymentStatus + "','Indent_Amount' : '" + Indent_Amount + "','Indent_Id':'" + Indent_Id + "'}";
                DataTable dt = obj.GetDataSet("pr_UpdateOnlinePaymentHistory", data, "ZIMAconnectionstring").Tables[0];
                result = "Success";

            }
            catch (Exception ex)
            {

                result = ex.Message;
            }
            return result;


        }

        public string updatetodb(string TXN_ID, string PaymentStatus, string Indent_Amount, string Indent_Id)
        {
            string result = "Failed";
            try
            {
                var data = "{ 'TXN_ID' : '" + TXN_ID + "','PaymentStatus' : '" + PaymentStatus + "','Indent_Amount' : '" + Indent_Amount + "','Indent_Id' : '" + Indent_Id + "'}";
                DataTable dt = obj.GetDataSet("pr_UpdateOnlinePaymentHistory_BPMS", data, "connectionstring").Tables[0];
                result = "Success";
            }
            catch
            {


            }
            return result;

        }

        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }










    }
}
