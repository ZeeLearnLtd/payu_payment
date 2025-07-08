using MVCIntegrationKit.Models;
using System;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace MVCIntegrationKit.Controllers
{
    public class PaymentController : Controller
    {
        private const string V = "Indent_Id";
        BussinessLogic obj = new BussinessLogic();


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Payment(string id)
        {

            ViewBag.indent = id;
            return View();
        }
        public ActionResult ZllPayment(string id)
        {

            ViewBag.indent = id;
            return View();
        }
        public ActionResult mlzsPayment(string id)
        {

            ViewBag.indent = id;
            return View();
        }

        public ActionResult zicaPayment(string id)
        {

            ViewBag.indent = id;
            return View();
        }

        public ActionResult zicaStudentPayment(string id)
        {

            ViewBag.indent = id;
            return View();
        }
        [HttpPost]
        public void ZllPayment(FormCollection form)
        {
            try
            {
                var identa = form["indent"].ToString();

                var data = "{txn_id :'" + identa + "'}";
                DataTable dt = obj.GetDataSet("pr_getCustomerPaymentDetails", data, "BPMSconnectionstring").Tables[0];
                string firstName = dt.Rows[0]["Franchisee_Name"].ToString();
                string amount = dt.Rows[0]["Indent_Amount"].ToString();
                string productInfo = dt.Rows[0]["Indent_Type"].ToString();
                string email = dt.Rows[0]["Email_Id"].ToString();
                string phone = dt.Rows[0]["Mobile_No"].ToString();
                string payment_nature = dt.Rows[0]["Payment_Nature"].ToString();
                string Created_By = dt.Rows[0]["Created_By"].ToString();
                string Location = dt.Rows[0]["Location"].ToString();
                string surl = ConfigurationManager.AppSettings["PAYU_zllreturn_URL"].ToString();
                string furl = ConfigurationManager.AppSettings["PAYU_zllreturn_URL"].ToString();
                //RemotePost myremotepost = new RemotePost();
                string key = ConfigurationManager.AppSettings["MERCHANT_KEY"].ToString();
                string salt = ConfigurationManager.AppSettings["SALT"].ToString();
                ////posting all the parameters required for integration.
                //myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"].ToString();
                //myremotepost.Add("key", key);
                string txnid = dt.Rows[0]["TXN_ID"].ToString();// Generatetxnid();
                //myremotepost.Add("txnid", txnid);
                //myremotepost.Add("amount", amount);
                //myremotepost.Add("productinfo", productInfo);
                //myremotepost.Add("firstname", firstName);
                //myremotepost.Add("phone", phone);
                //myremotepost.Add("email", email);
                //myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
                //myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
                //myremotepost.Add("service_provider", "payu_paisa");
                //myremotepost.Add("udf1", identa);

                //string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                //// string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + " ||||||" + salt;
                //string hash = Generatehash512(hashString);
                //myremotepost.Add("hash", hash);
                //myremotepost.Post();
                RemotePost myremotepost = new RemotePost();

                myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                myremotepost.Add("key", key);
                myremotepost.Add("txnid", txnid);
                myremotepost.Add("amount", amount);
                myremotepost.Add("productinfo", productInfo);
                myremotepost.Add("firstname", firstName);
                myremotepost.Add("phone", phone);
                myremotepost.Add("email", email);
                myremotepost.Add("surl", ConfigurationManager.AppSettings["PAYU_zllreturn_URL"]);//Change the success url here depending upon the port number of your local system.  
                myremotepost.Add("furl", ConfigurationManager.AppSettings["PAYU_zllreturn_URL"]);//Change the failure url here depending upon the port number of your local system.  
                myremotepost.Add("service_provider", "payu_paisa");
                myremotepost.Add("udf1", payment_nature);
                myremotepost.Add("udf2", Created_By);
                myremotepost.Add("udf3", Location);
                //ConfigurationManager.AppSettings["hashSequence"];//
                //string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + payment_nature + "|"+ Created_By + "|"+ Location + "||||||||" + salt;
                string hash = Generatehash512(hashString);
                UpdateHashCode(hash, txnid, amount);
                myremotepost.Add("hash", hash);
                myremotepost.Post();
            }
            catch (Exception ex)
            {




            }

        }

        [HttpPost]
        public void mlzsPayment(FormCollection form)
        {
            try
            {
                var identa = form["indent"].ToString();
                var data = "{Indent_Id :" + identa + "}";
                DataTable dt = obj.GetDataSet("pr_CreateOnlinePaymentHistoryByIndentId", data, "MLZSconnectionstring").Tables[0];
                string firstName = dt.Rows[0]["Franchisee_Name"].ToString();
                string amount = dt.Rows[0]["Indent_Amount"].ToString();
                string productInfo = dt.Rows[0]["Indent_Type"].ToString();
                string email = dt.Rows[0]["Email_Id"].ToString();
                string phone = dt.Rows[0]["Mobile_No"].ToString();
                string surl = ConfigurationManager.AppSettings["PAYU_mlzsreturn_URL"].ToString();
                string furl = ConfigurationManager.AppSettings["PAYU_mlzsreturn_URL"].ToString();
                //RemotePost myremotepost = new RemotePost();
                string key = ConfigurationManager.AppSettings["mlzs_MERCHANT_KEY"].ToString();
                string salt = ConfigurationManager.AppSettings["mlzs_SALT"].ToString();
                ////posting all the parameters required for integration.
                //myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"].ToString();
                //myremotepost.Add("key", key);
                string txnid = dt.Rows[0]["TXN_ID"].ToString();// Generatetxnid();
                //myremotepost.Add("txnid", txnid);
                //myremotepost.Add("amount", amount);
                //myremotepost.Add("productinfo", productInfo);
                //myremotepost.Add("firstname", firstName);
                //myremotepost.Add("phone", phone);
                //myremotepost.Add("email", email);
                //myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
                //myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
                //myremotepost.Add("service_provider", "payu_paisa");
                //myremotepost.Add("udf1", identa);

                //string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                //// string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + " ||||||" + salt;
                //string hash = Generatehash512(hashString);
                //myremotepost.Add("hash", hash);
                //myremotepost.Post();
                RemotePost myremotepost = new RemotePost();

                myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                myremotepost.Add("key", key);
                myremotepost.Add("txnid", txnid);
                myremotepost.Add("amount", amount);
                myremotepost.Add("productinfo", productInfo);
                myremotepost.Add("firstname", firstName);
                myremotepost.Add("phone", phone);
                myremotepost.Add("email", email);
                myremotepost.Add("surl", ConfigurationManager.AppSettings["PAYU_mlzsreturn_URL"]);//Change the success url here depending upon the port number of your local system.  
                myremotepost.Add("furl", ConfigurationManager.AppSettings["PAYU_mlzsreturn_URL"]);//Change the failure url here depending upon the port number of your local system.  
                myremotepost.Add("service_provider", "payu_paisa");
                myremotepost.Add("udf1", identa);
                //ConfigurationManager.AppSettings["hashSequence"];//
                string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                string hash = Generatehash512(hashString);
                UpdateHashCodeEML(hash, txnid, amount);
                myremotepost.Add("hash", hash);
                myremotepost.Post();
            }
            catch (Exception ex)
            {




            }

        }

        [HttpPost]
        public void zicaPayment(FormCollection form)
        {
            try
            {
                var identa = form["indent"].ToString();
                var data = "{Indent_Id :" + identa + "}";
                DataTable dt = obj.GetDataSet("pr_CreateOnlinePaymentHistoryByIndentId", data, "ZICAconnectionstring").Tables[0];
                string firstName = dt.Rows[0]["Franchisee_Name"].ToString();
                string amount = dt.Rows[0]["Indent_Amount"].ToString();
                string productInfo = dt.Rows[0]["Indent_Type"].ToString();
                string email = dt.Rows[0]["Email_Id"].ToString();
                string phone = dt.Rows[0]["Mobile_No"].ToString();
                string fran_code = dt.Rows[0]["Franchisee_Code"].ToString();
                string surl = ConfigurationManager.AppSettings["PAYU_zicareturn_URL"].ToString();
                string furl = ConfigurationManager.AppSettings["PAYU_zicareturn_URL"].ToString();
                //RemotePost myremotepost = new RemotePost();
                string key = ConfigurationManager.AppSettings["zica_MERCHANT_KEY"].ToString();
                string salt = ConfigurationManager.AppSettings["zica_SALT"].ToString();
                ////posting all the parameters required for integration.
                //myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"].ToString();
                //myremotepost.Add("key", key);
                string txnid = dt.Rows[0]["TXN_ID"].ToString();// Generatetxnid();
                //myremotepost.Add("txnid", txnid);
                //myremotepost.Add("amount", amount);
                //myremotepost.Add("productinfo", productInfo);
                //myremotepost.Add("firstname", firstName);
                //myremotepost.Add("phone", phone);
                //myremotepost.Add("email", email);
                //myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
                //myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
                //myremotepost.Add("service_provider", "payu_paisa");
                //myremotepost.Add("udf1", identa);

                //string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                //// string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + " ||||||" + salt;
                //string hash = Generatehash512(hashString);
                //myremotepost.Add("hash", hash);
                //myremotepost.Post();
                RemotePost myremotepost = new RemotePost();

                myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                myremotepost.Add("key", key);
                myremotepost.Add("txnid", txnid);
                myremotepost.Add("amount", amount);
                myremotepost.Add("productinfo", productInfo);
                myremotepost.Add("firstname", firstName);
                myremotepost.Add("phone", phone);
                myremotepost.Add("email", email);
                myremotepost.Add("surl", ConfigurationManager.AppSettings["PAYU_zicareturn_URL"]);//Change the success url here depending upon the port number of your local system.  
                myremotepost.Add("furl", ConfigurationManager.AppSettings["PAYU_zicareturn_URL"]);//Change the failure url here depending upon the port number of your local system.  
                myremotepost.Add("service_provider", "payu_paisa");
                myremotepost.Add("udf1", identa);
                myremotepost.Add("udf2", fran_code);
                //ConfigurationManager.AppSettings["hashSequence"];//
                string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                string hash = Generatehash512(hashString);
                UpdateHashCodeZICA(hash, txnid, amount);
                myremotepost.Add("hash", hash);
                myremotepost.Post();
            }
            catch (Exception ex)
            {




            }

        }

        [HttpPost]
        public void zicaStudentPayment(FormCollection form)
        {
            try
            {
                var identa = form["indent"].ToString();
                var data = "{TXN_ID :'" + identa + "'}";
                DataTable dt = obj.GetDataSet("pr_getOnlineReceiptPaymentHistory", data, "ZICAconnectionstring").Tables[0];
                string firstName = dt.Rows[0]["Franchisee_Name"].ToString()+" - "+ dt.Rows[0]["Student_Name"].ToString();
                string amount = dt.Rows[0]["Indent_Amount"].ToString();
                string productInfo = dt.Rows[0]["Course_Name"].ToString();
                string email = dt.Rows[0]["Email_Id"].ToString();
                string phone = dt.Rows[0]["Mobile_No"].ToString();
                string fran_code = dt.Rows[0]["Franchisee_Code"].ToString();
                string student_id = dt.Rows[0]["User_Id"].ToString();
                string surl = ConfigurationManager.AppSettings["PAYU_zicastudentreturn_URL"].ToString();
                string furl = ConfigurationManager.AppSettings["PAYU_zicastudentreturn_URL"].ToString();
                //RemotePost myremotepost = new RemotePost();
                string key = ConfigurationManager.AppSettings["zica_MERCHANT_KEY"].ToString();
                string salt = ConfigurationManager.AppSettings["zica_SALT"].ToString();
                ////posting all the parameters required for integration.
                //myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"].ToString();
                //myremotepost.Add("key", key);
                string txnid = dt.Rows[0]["TXN_ID"].ToString();// Generatetxnid();
                //myremotepost.Add("txnid", txnid);
                //myremotepost.Add("amount", amount);
                //myremotepost.Add("productinfo", productInfo);
                //myremotepost.Add("firstname", firstName);
                //myremotepost.Add("phone", phone);
                //myremotepost.Add("email", email);
                //myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
                //myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
                //myremotepost.Add("service_provider", "payu_paisa");
                //myremotepost.Add("udf1", identa);

                //string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                //// string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + " ||||||" + salt;
                //string hash = Generatehash512(hashString);
                //myremotepost.Add("hash", hash);
                //myremotepost.Post();
                RemotePost myremotepost = new RemotePost();

                myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                myremotepost.Add("key", key);
                myremotepost.Add("txnid", txnid);
                myremotepost.Add("amount", amount);
                myremotepost.Add("productinfo", productInfo);
                myremotepost.Add("firstname", firstName);
                myremotepost.Add("phone", phone);
                myremotepost.Add("email", email);
                myremotepost.Add("surl", ConfigurationManager.AppSettings["PAYU_zicastudentreturn_URL"]);//Change the success url here depending upon the port number of your local system.  
                myremotepost.Add("furl", ConfigurationManager.AppSettings["PAYU_zicastudentreturn_URL"]);//Change the failure url here depending upon the port number of your local system.  
                myremotepost.Add("service_provider", "payu_paisa");
                myremotepost.Add("udf1", identa);
                myremotepost.Add("udf2", fran_code);
                myremotepost.Add("udf3", student_id);
                //ConfigurationManager.AppSettings["hashSequence"];//
                string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                string hash = Generatehash512(hashString);
                UpdateHashCodeZICA(hash, txnid, amount);
                myremotepost.Add("hash", hash);
                myremotepost.Post();
            }
            catch (Exception ex)
            {




            }

        }

        [HttpPost]
        public void Payment(FormCollection form)
        {
            try
            {
                var identa = form["indent"].ToString();
                var data = "{Indent_Id :" + identa + "}";
                DataTable dt = obj.GetDataSet("pr_CreateOnlinePaymentHistory_BPMS", data, "connectionstring").Tables[0];
                string firstName = dt.Rows[0]["Franchisee_Name"].ToString();
                string amount = dt.Rows[0]["Indent_Amount"].ToString();
                string productInfo = dt.Rows[0]["Indent_Type"].ToString();
                string email = dt.Rows[0]["Email_Id"].ToString();
                string phone = dt.Rows[0]["Mobile_No"].ToString();
                string surl = ConfigurationManager.AppSettings["PAYU_return_URL"].ToString();
                string furl = ConfigurationManager.AppSettings["PAYU_return_URL"].ToString();
                //RemotePost myremotepost = new RemotePost();
                string key = ConfigurationManager.AppSettings["MERCHANT_KEY"].ToString();
                string salt = ConfigurationManager.AppSettings["SALT"].ToString();
                ////posting all the parameters required for integration.
                //myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"].ToString();
                //myremotepost.Add("key", key);
                string txnid = dt.Rows[0]["TXN_ID"].ToString();// Generatetxnid();
                //myremotepost.Add("txnid", txnid);
                //myremotepost.Add("amount", amount);
                //myremotepost.Add("productinfo", productInfo);
                //myremotepost.Add("firstname", firstName);
                //myremotepost.Add("phone", phone);
                //myremotepost.Add("email", email);
                //myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
                //myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
                //myremotepost.Add("service_provider", "payu_paisa");
                //myremotepost.Add("udf1", identa);

                //string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                //// string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + "|" + txnid + " ||||||" + salt;
                //string hash = Generatehash512(hashString);
                //myremotepost.Add("hash", hash);
                //myremotepost.Post();
                RemotePost myremotepost = new RemotePost();

                myremotepost.Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"];
                myremotepost.Add("key", key);
                myremotepost.Add("txnid", txnid);
                myremotepost.Add("amount", amount);
                myremotepost.Add("productinfo", productInfo);
                myremotepost.Add("firstname", firstName);
                myremotepost.Add("phone", phone);
                myremotepost.Add("email", email);
                myremotepost.Add("surl", ConfigurationManager.AppSettings["PAYU_return_URL"]);//Change the success url here depending upon the port number of your local system.  
                myremotepost.Add("furl", ConfigurationManager.AppSettings["PAYU_return_URL"]);//Change the failure url here depending upon the port number of your local system.  
                myremotepost.Add("service_provider", "payu_paisa");
                myremotepost.Add("udf1", identa);
                //ConfigurationManager.AppSettings["hashSequence"];//
                string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + identa + "||||||||||" + salt;
                string hash = Generatehash512(hashString);
                myremotepost.Add("hash", hash);
                myremotepost.Post();
            }
            catch (Exception ex)
            {




            }

        }

        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();


            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
            }
        }

        //Hash generation Algorithm

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


        public string Generatetxnid()
        {

            Random rnd = new Random();
            string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);

            return txnid1;
        }

        public ActionResult PaymentLink() {
            return View();
        }

        private void UpdateHashCode(string vhashString, string vTXN_ID, string vIndent_Amount)
        {
            string data = "{'TXN_ID':'" + vTXN_ID + "','HashCode':'" + vhashString + "','Indent_Amount':" + vIndent_Amount + "}";
            string result = obj.SaveJson("pr_UpdateOnlinePaymentHashCode", data, "BPMSconnectionstring");
        }

        private void UpdateHashCodeEML(string vhashString, string vTXN_ID, string vIndent_Amount)
        {
            string data = "{'TXN_ID':'" + vTXN_ID + "','HashCode':'" + vhashString + "','Indent_Amount':" + vIndent_Amount + "}";
            string result = obj.SaveJson("pr_UpdateOnlinePaymentHashCode", data, "MLZSconnectionstring");
        }

        private void UpdateHashCodeZICA(string vhashString, string vTXN_ID, string vIndent_Amount)
        {
            string data = "{'TXN_ID':'" + vTXN_ID + "','HashCode':'" + vhashString + "','Indent_Amount':" + vIndent_Amount + "}";
            string result = obj.SaveJson("pr_UpdateOnlinePaymentHashCode", data, "ZICAconnectionstring");
        }


    }
}
