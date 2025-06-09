using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Xml;

namespace MVCIntegrationKit.Models
{
    public class BussinessLogic
    {
        public BussinessLogic()
        {

        }
        public XmlDocument mergeallxml(DataTable dt)
        {


            XmlDocument doc = new XmlDocument();

            string str = "";
            if (dt.Rows.Count >= 1)
            {
                for (var i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    str = str + dt.Rows[i][0].ToString();
                }
            }
            else { str = "<root><subroot></subroot></root>"; }
            doc.LoadXml(str);

            return doc;


        }
        public DataSet GetDataSetmMembership(string proc, string variable)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["membershipconnectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (variable != "" && variable != null)
                {
                    xml.LoadXml(variable);
                    XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds;
        }

        public DataSet GetDataSet(string proc, string variable)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (variable != "" && variable != null)
                {
                    xml.LoadXml(variable);
                    XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds;
        }
        public DataSet GetDataSet(string proc, string variable, string conn)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[conn].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (variable != "" && variable != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(variable);
                    foreach (var kv in dict)
                    {

                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            cmd.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds;
        }

        public string savejsonobject(string proc, string variable, string connection)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (variable != "" && variable != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(variable);
                    foreach (var kv in dict)
                    {

                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);


            }
            string JSONresult = "";
            if (ds != null && ds.Tables.Count > 0)
            {
                JSONresult = JsonConvert.SerializeObject(ds.Tables[0]);
            };
            return JSONresult;

        }

        public XmlDocument getXmlData(string proc, string variable, string connectionstring)
        {

            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionstring].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (variable != "" && variable != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(variable);
                    foreach (var kv in dict)
                    {

                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                try
                {
                    da.Fill(ds);
                }
                catch
                {
                    xml.LoadXml("<root><subroot></subroot></root>");
                    return xml;
                }
            }

            try
            {
                string str = "";
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        str = str + ds.Tables[0].Rows[i][0].ToString();

                    }
                    if (str == "")
                    {
                        str = "<root><subroot></subroot></root>";
                    }

                    xml.LoadXml(str);


                }
                else
                {
                    str = "<root><subroot><error>error</error></subroot></root>";
                    xml.LoadXml(str);
                }

                return xml;
            }
            catch
            {
                xml.LoadXml("<root><subroot><messege>There is some error while fetching data.Plz contact service provider</messege></subroot></root>");
                return xml;

            }
        }
        public string getjsondata(string proc, string variable)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (variable != "" && variable != null)
                {
                    xml.LoadXml(variable);
                    XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return JsonConvert.SerializeObject(ds.Tables[0]);
        }
        public XmlDocument getjson(string proc, string jsonparam, string connectionstring)
        {

            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionstring].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                if (jsonparam != "" && jsonparam != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonparam);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }

                //if (jsonparam != "" && jsonparam != null)
                //{
                //    xml.LoadXml(jsonparam);
                //    XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                //    foreach (XmlNode node in nodes)
                //    {
                //        string param = node.Attributes["paramname"].Value;
                //        string value = node.Attributes["paramvalue"].Value;
                //        sqlComm.Parameters.AddWithValue("@" + param, value);
                //    }
                //}
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
            }

            try
            {
                string str = "";
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        str = str + ds.Tables[0].Rows[i][0].ToString();

                    }
                    if (str == "")
                    {
                        str = "<root><subroot></subroot></root>";
                    }

                    xml.LoadXml(str);


                }
                else
                {
                    str = "<root><subroot><error>error</error></subroot></root>";
                    xml.LoadXml(str);
                }

                return xml;
            }
            catch
            {
                xml.Load("<root><subroot><messege>There is some error while fetching data.Plz contact service provider</messege></subroot></root>");
                return xml;

            }
        }
        //public void SendMailSMS(GetAll objdata)
        //{
        //    try
        //    {
        //        string CustomerId = "", toEmailBody = "", toSMSBody = "", EType = "", AttachedFiles = "", UserId = "", ProjectId = "", CatId = "", toEmail = "", toSMS = "", subject = "";

        //        string jsonparam = objdata.FilterParameter;
        //        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        dynamic obj = serializer.Deserialize<RootObject>(jsonparam);

        //        CustomerId = obj.CustomerId;
        //        toEmailBody = obj.SendMailBody;
        //        toSMSBody = obj.SendSMSBody;
        //        EType = obj.EType;
        //        AttachedFiles = obj.AttachedFiles;
        //        UserId = obj.HisUserId;
        //        CatId = obj.CatId;
        //        ProjectId = obj.ProjectId;
        //        toEmail = obj.toEmail;
        //        toSMS = obj.toSMS;
        //        subject = obj.subject;

        //        var html = obj.invoiceHtml;

        //        DataTable dt = new DataTable();
        //        dt.Clear();

        //        dt.Columns.Add("File1", typeof(SqlBinary));
        //        dt.Columns.Add("filename1");
        //        dt.Columns.Add("size");


        //        // Send html as pdf in mail
        //        if (html != null)
        //        {
        //            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
        //            var pdfBytes = htmlToPdf.GeneratePdf(html);
        //            DataRow dr1 = dt.NewRow();
        //            dr1["File1"] = pdfBytes;
        //            dr1["filename1"] = "InvoiceDetails.pdf";

        //            dr1["size"] = (pdfBytes.Length / 1024f) / 1024f;
        //            if ((pdfBytes.Length / 1024f) / 1024f <= 25)
        //            {
        //                dt.Rows.Add(dr1);
        //            }
        //        }

        //        // Send files in mail

        //        string[] Filespath = AttachedFiles.Split('~');
        //        if (Filespath.Length > 0)
        //        {
        //            foreach (var path in Filespath)
        //            {
        //                if (path != "")
        //                {

        //                    FileStream stream = File.OpenRead(HttpContext.Current.Server.MapPath("~/images/Mailimages/" + path));
        //                    //FileStream stream = File.OpenRead(context.Request.MapPath(path));
        //                    byte[] fileBytes = new byte[stream.Length];
        //                    stream.Read(fileBytes, 0, fileBytes.Length);
        //                    stream.Close();
        //                    //AddFileToManipulate(fileBytes, Path.GetFileName(path));
        //                    DataRow dr = dt.NewRow();
        //                    dr["File1"] = fileBytes;
        //                    dr["filename1"] = Path.GetFileName(path);

        //                    dr["size"] = (fileBytes.Length / 1024f) / 1024f;
        //                    if ((fileBytes.Length / 1024f) / 1024f <= 25)
        //                    {
        //                        dt.Rows.Add(dr);
        //                    }
        //                }
        //            }
        //        }

        //        // SQL Server Connection String
        //        string sqlConnectionString = ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString;

        //        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("Communication_SendMailSMS"))
        //            {
        //                try
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandTimeout = 0;
        //                    cmd.Connection = con;
        //                    cmd.Parameters.AddWithValue("@tblfileattachment", dt);
        //                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
        //                    cmd.Parameters.AddWithValue("@toEmailBody", toEmailBody);
        //                    cmd.Parameters.AddWithValue("@toSMSBody", toSMSBody);
        //                    cmd.Parameters.AddWithValue("@EType", EType);
        //                    cmd.Parameters.AddWithValue("@AttachedFiles", AttachedFiles);
        //                    cmd.Parameters.AddWithValue("@UserId", UserId);
        //                    cmd.Parameters.AddWithValue("@CatId", CatId);
        //                    cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
        //                    cmd.Parameters.AddWithValue("@toEmail", toEmail);
        //                    cmd.Parameters.AddWithValue("@toSMS", toSMS);
        //                    cmd.Parameters.AddWithValue("@subject", subject);
        //                    con.Open();
        //                    cmd.ExecuteNonQuery();
        //                    con.Close();
        //                }
        //                catch (Exception ex)
        //                {
        //                    con.Close();
        //                }
        //            }
        //        }
        //        foreach (var fileNamePath in Filespath)
        //        {
        //            // Delete a file by using File class static method...
        //            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/images/Mailimages/" + fileNamePath)))
        //            {
        //                // Use a try block to catch IOExceptions, to
        //                // handle the case of the file already being
        //                // opened by another process.
        //                try
        //                {
        //                    File.SetAttributes(HttpContext.Current.Server.MapPath("~/images/Mailimages/" + fileNamePath), FileAttributes.Normal);
        //                    System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/images/Mailimages/" + fileNamePath));
        //                }
        //                catch (System.IO.IOException e)
        //                {

        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        public string getdatatablejsondata(string proc, string json, string connection, int NewMethod)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);


            }
            string JSONresult;
            return JsonConvert.SerializeObject(ds.Tables[0]);
        }

        //public string getdatatablejsondata(string proc, string variable)
        //{
        //    DataSet ds = new DataSet("data1");
        //    XmlDocument xml = new XmlDocument();

        //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
        //    {
        //        SqlCommand sqlComm = new SqlCommand(proc, conn);
        //        sqlComm.CommandType = CommandType.StoredProcedure;
        //        if (variable != "" && variable != null)
        //        {
        //            xml.LoadXml(variable);
        //            XmlNodeList nodes = xml.GetElementsByTagName("Definition");
        //            foreach (XmlNode node in nodes)
        //            {
        //                string param = node.Attributes["paramname"].Value;
        //                string value = node.Attributes["paramvalue"].Value;
        //                sqlComm.Parameters.AddWithValue("@" + param, value);
        //            }
        //        }
        //        SqlDataAdapter da = new SqlDataAdapter();
        //        da.SelectCommand = sqlComm;
        //        da.Fill(ds);


        //    }
        //    string JSONresult;
        //    return JsonConvert.SerializeObject(ds.Tables[0]);
        //}
        public string getmultipletablejson(string proc, string variable)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (variable != "" && variable != null)
                {
                    xml.LoadXml(variable);
                    XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        sqlComm.Parameters.AddWithValue("@" + param, value);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);


            }
            string JSONresult;
            return JsonConvert.SerializeObject(ds.Tables);
        }
        public string getdatatablejsondata(string proc, string json, string connection)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            string JSONresult = "";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                try
                {
                    da.Fill(ds);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        JSONresult = JsonConvert.SerializeObject(ds.Tables[0]);
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0},{1}", ex.Message, ex.Source);

                }
            }
            return JSONresult;
        }
        public string getdatatablejsondata1(string proc, string json, string connection)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);


            }
            string JSONresult;
            return JsonConvert.SerializeObject(ds.Tables[0]);
        }
        public string getdatatablejsondata(string proc, string json, string otherparameter, string connection)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (otherparameter != "")
                {
                    doc = (XmlDocument)JsonConvert.DeserializeXmlNode(otherparameter);
                    sqlComm.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                }
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {

                        sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);


            }
            string JSONresult;
            return JsonConvert.SerializeObject(ds.Tables[0]);
        }
        public string getmultipletablejson(string proc, string json, string connection)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }

                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);


            }
            string JSONresult;
            return JsonConvert.SerializeObject(ds.Tables);
        }
        public string updatexml(string proc, string xml, int id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(xml);
                cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                cmd.Parameters.AddWithValue("@id", id);
                int i = 0;
                i = cmd.ExecuteNonQuery();
                try
                {
                    if (i > 0)
                    {
                        return ("{'status':'1'}");
                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {
                    return ("{'status':'0'}");
                }
            }
        }
        public string SaveUpdateDeleteData(string proc, string otherparameter)
        {
            XmlDataDocument doc = new XmlDataDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                doc.LoadXml(otherparameter);
                XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                foreach (XmlNode node in nodes)
                {
                    string param = node.Attributes["paramname"].Value;
                    string value = node.Attributes["paramvalue"].Value;
                    cmd.Parameters.AddWithValue("@" + param, value);

                }
                int i = 0;
                i = cmd.ExecuteNonQuery();
                try
                {
                    if (i > 0)
                    {
                        return ("1");
                    }
                    else
                    {
                        return ("0");
                    }
                }
                catch
                {
                    return ("0");
                }
            }
        }
        public DataSet getdataset(string proc)
        {

            DataSet ds = new DataSet("data1");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
            }
            return ds;


        }
        public DataSet getdatasetWithparameter(string proc, string data)
        {
            DataSet ds = new DataSet("data1");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.Parameters.AddWithValue("@docId", data);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
            }
            return ds;
        }
        public DataSet getdat(string proc)
        {

            DataSet ds = new DataSet("data1");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
            }
            return ds;


        }
        public DataSet Returndataset(string proc, string json, string otherparameter, int Flag)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                if (json != "")
                {
                    doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                    cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                }
                if (otherparameter != "" && otherparameter != null)
                {
                    doc.LoadXml(otherparameter);

                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                try
                {

                    return ds;
                }
                catch
                {

                    return ds;

                }
            }

        }
        public XmlDocument Convertdatasettoxml(DataSet ds)
        {
            string str = "";
            XmlDocument xml = new XmlDocument();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                {
                    str = str + ds.Tables[0].Rows[i][0].ToString();

                }
                if (str == "")
                {
                    str = "<root><subroot></subroot></root>";
                }

                xml.LoadXml(str);


            }
            return xml;

        }
        public string SaveMultixml(string proc, string json1, string json2, string json3, string otherparameter)
        {
            XmlDocument doc = new XmlDocument();
            XmlDocument doc1 = new XmlDocument();
            XmlDocument doc2 = new XmlDocument();
            XmlDocument doc3 = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json1);
                cmd.Parameters.AddWithValue("@XMLPG", doc.InnerXml.ToString());
                doc1 = (XmlDocument)JsonConvert.DeserializeXmlNode(json2);
                cmd.Parameters.AddWithValue("@XMLUG", doc1.InnerXml.ToString());
                if (json3 != "" && json3 != null)
                {
                    doc2 = (XmlDocument)JsonConvert.DeserializeXmlNode(json3);
                    cmd.Parameters.AddWithValue("@XMLOther", doc2.InnerXml.ToString());
                }
                if (otherparameter != "" && otherparameter != null)
                {
                    doc3.LoadXml(otherparameter);
                    XmlNodeList nodes = doc3.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }
                int i = 0;
                i = cmd.ExecuteNonQuery();

                try
                {
                    if (i > 0)
                    {
                        return ("1");

                    }
                    else
                    {
                        return ("0");
                    }
                }
                catch
                {

                    return ("0");
                }


            }



        }
        public XmlDocument getjson(string proc, string jsonparam)
        {

            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (jsonparam != "" && jsonparam != null)
                {
                    xml.LoadXml(jsonparam);
                    XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        sqlComm.Parameters.AddWithValue("@" + param, value);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
            }

            try
            {
                string str = "";
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        str = str + ds.Tables[0].Rows[i][0].ToString();

                    }
                    if (str == "")
                    {
                        str = "<root><subroot></subroot></root>";
                    }

                    xml.LoadXml(str);


                }
                else
                {
                    str = "<root><subroot><error>error</error></subroot></root>";
                    xml.LoadXml(str);
                }

                return xml;
            }
            catch
            {
                xml.Load("<root><subroot><messege>There is some error while fetching data.Plz contact service provider</messege></subroot></root>");
                return xml;

            }
        }
        public string savexml(string proc, string json)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                int i = 0;
                i = cmd.ExecuteNonQuery();

                try
                {
                    if (i > 0)
                    {
                        // return ("{'status':'1'}");
                        return "status1";

                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {

                    return ("{'status':'0'}");
                }


            }



        }
        public string savexmlWithJson(string proc, string json, string jsonparam)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                XmlDocument doc = new XmlDocument();
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                DataSet ds = new DataSet();
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (json != "")
                {
                    doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                    cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                }
                if (jsonparam != "" && jsonparam != null)
                {
                    doc.LoadXml(jsonparam);
                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                return JsonConvert.SerializeObject(ds.Tables[0]);


            }



        }
        public XmlDocument Returnsavexml(string proc, string json, string otherparameter, int Flag)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                    }

                }
                if (otherparameter != "" && otherparameter != null)
                {
                    doc.LoadXml(otherparameter);

                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                try
                {
                    string str = "";
                    if (ds.Tables[0].Rows.Count >= 1)
                    {
                        for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            str = str + ds.Tables[0].Rows[i][0].ToString();

                        }
                        xml.LoadXml(str);
                    }

                    return xml;
                }
                catch
                {
                    xml.LoadXml("<root><subroot><messege>There is some error while fetching data.Plz contact service provider</messege></subroot></root>");
                    return xml;

                }
            }

        }
        public string savexml(string proc, string json, string otherparameter)
        {
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (json != "")
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            doc = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }
                }
                if (otherparameter != "" && otherparameter != null)
                {
                    doc.LoadXml(otherparameter);
                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }
                int i = 0;
                i = cmd.ExecuteNonQuery();

                try
                {
                    if (i > 0)
                    {
                        return "status1";

                    }
                    else
                    {
                        return "status0";
                    }
                }
                catch
                {

                    return "status0";
                }


            }



        }
        public string savexml(string proc, string json, string otherparameter, string connstring)
        {
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[connstring].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (json != "")
                {
                    doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                    cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                }
                if (otherparameter != "" && otherparameter != null)
                {
                    doc.LoadXml(otherparameter);
                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }
                int i = 0;
                i = cmd.ExecuteNonQuery();

                try
                {
                    if (i > 0)
                    {
                        return "status1";

                    }
                    else
                    {
                        return "status0";
                    }
                }
                catch
                {

                    return "status0";
                }


            }



        }

        public string savexmlnew(string proc, string json, string connectionstring)
        {
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionstring].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                if (json != "")
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            doc = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }
                }

                int i = 0;
                i = cmd.ExecuteNonQuery();

                try
                {
                    if (i > 0)
                    {
                        return "status1";

                    }
                    else
                    {
                        return "status0";
                    }
                }
                catch
                {

                    return "status0";
                }


            }



        }
        public string ImageToBase64(string _imagePath)
        {


            byte[] imageBytes = System.IO.File.ReadAllBytes(_imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return "data:image/jpg;base64," + base64String;

            //string _base64String = null;

            //using (System.Drawing.Image _image = System.Drawing.Image.FromFile(_imagePath))
            //{
            //    using (MemoryStream _mStream = new MemoryStream())
            //    {
            //        _image.Save(_mStream, _image.RawFormat);
            //        byte[] _imageBytes = _mStream.ToArray();
            //        _base64String = Convert.ToBase64String(_imageBytes);

            //        return "data:image/jpg;base64," + _base64String;
            //    }
            //}
        }
        public string SaveImage(string base64, string path, string imagename)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    bm2.Save(path + imagename + ".Jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            return path + imagename + ".Jpeg";
        }
        public string savexmlwithmulti(string proc, string json2, string otherparameter)
        {
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json2);
                cmd.Parameters.AddWithValue("@XML", doc.InnerXml.ToString());
                if (otherparameter != "" && otherparameter != null)
                {
                    doc.LoadXml(otherparameter);
                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }
                int i = 0;
                i = cmd.ExecuteNonQuery();

                try
                {
                    if (i > 0)
                    {
                        return ("1");

                    }
                    else
                    {
                        return ("0");
                    }
                }
                catch
                {

                    return ("0");
                }


            }



        }
        public string deleteData(string proc, int id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@id", id);
                int i = 0;
                i = cmd.ExecuteNonQuery();
                try
                {
                    if (i > 0)
                    {
                        return ("{'status':'1'}");
                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {
                    return ("{'status':'0'}");
                }
            }
        }

        public string SendEmailClient(string ToEmailId, string subject, string body)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["client_connection"].ConnectionString))
            {
                string query = "Pr_insertmaillwk";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@toemail", ToEmailId);
                cmd.Parameters.AddWithValue("@subject", subject);
                cmd.Parameters.AddWithValue("@body", body);
                int i = 0;
                i = cmd.ExecuteNonQuery();
                try
                {
                    if (i > 0)
                    {
                        return ("{'status':'1'}");
                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {
                    return ("{'status':'0'}");
                }
            }
        }


        public async Task<String> SendMail(string ToEmailId, string ccEmail, string subject, string body, string Attachment)
        {
            try
            {
                MailMessage Msg = new MailMessage();
                //string ToEmailIdU = ToEmailId.Remove(ToEmailId.Length - 1, 1);
                Msg.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_EmailId"]);
                Msg.To.Add(ToEmailId);
                Msg.Subject = subject;
                Msg.IsBodyHtml = true;
                Msg.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["SMPT_HostLink"];
                smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["SMPT_Port"]);
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTP_EmailId"], ConfigurationManager.AppSettings["EmailPass"]);
                smtp.EnableSsl = true;
                smtp.Send(Msg);
            }
            catch (Exception ex)
            {

                return "2";
            }

            return "1";
        }
        public string SendNotification(string deviceId, string message, string title)
        {
            string SERVER_API_KEY = "AIzaSyDKhQm9mCzSKp7wrKCm6F3-uG8bijIfTyY";
            var SENDER_ID = "900123701505";
            var value = message;
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            string postData = "data.body=" + value + "&data.title=" + title + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";
            Console.WriteLine(postData);
            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;
            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tResponse = tRequest.GetResponse();
            dataStream = tResponse.GetResponseStream();
            StreamReader tReader = new StreamReader(dataStream);
            String sResponseFromServer = tReader.ReadToEnd();
            tReader.Close();
            dataStream.Close();
            tResponse.Close();
            return sResponseFromServer;
        }
        public DataSet savedatawithxml(string proc, string json, string jsonparam)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "")
                {
                    xml = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                    sqlComm.Parameters.AddWithValue("@XML", xml.InnerXml.ToString());
                }
                xml.LoadXml(jsonparam);
                XmlNodeList nodes = xml.GetElementsByTagName("Definition");
                foreach (XmlNode node in nodes)
                {
                    string param = node.Attributes["paramname"].Value;
                    string value = node.Attributes["paramvalue"].Value;
                    sqlComm.Parameters.AddWithValue("@" + param, value);
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
            }
            return ds;
        }
        public string EmailCount(int practiceId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = "Update_EmailCount";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@practiceId", practiceId);
                int i = 0;
                i = cmd.ExecuteNonQuery();
                try
                {
                    if (i > 0)
                    {

                        return "status1";

                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {

                    return ("{'status':'0'}");
                }


            }
        }
        public string SmsCount(int practiceId)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = "Update_SmsCount";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@practiceId", practiceId);
                int i = 0;
                i = cmd.ExecuteNonQuery();
                try
                {
                    if (i > 0)
                    {

                        return "status1";

                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {

                    return ("{'status':'0'}");
                }


            }
        }

        public string SendSMSByconnexion(string sendToPhoneNumber, string msg, string TiemStamp, string type)
        {

            sendToPhoneNumber = sendToPhoneNumber.Replace(",", "%20%7C%20");


            string result = "";
            string res = "";

            String[] strArr = null;
            String[] strTrnsId = null;

            //--------------------------Transactional SMS----------------------------------//

            string GupshupUrl = "https://smsc5.smsconnexion.com/sendsms/bulksms.php?username=kidzee.p&password=12345678&type=TEXT&sender=ZEELRN&entityId=1001155581128617495&templateId=";


            if (sendToPhoneNumber.Trim() == "")
            {
                return "Mobile Number Not Found.";
            }

            else if (!IsPositiveNumber(sendToPhoneNumber))
            {
                return "Invalid Mobile Number.";
            }
            else
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                WebRequest request = null;
                HttpWebResponse response = null;

                string msg1 = msg;
                Regex rgxLines = new Regex("\\n");
                try
                {
                    string url = "";


                    msg = msg.Replace(" ", "%20");
                    url = GupshupUrl;
                    url = url + "&mobile=" + sendToPhoneNumber;
                    url = url + "&message=" + msg1;

                    // url = url + "&timestamp=" + TiemStamp;

                    request = WebRequest.Create(url);
                    response = (HttpWebResponse)request.GetResponse();

                    Stream stream = response.GetResponseStream();
                    Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader reader = new StreamReader(stream, ec);
                    result = reader.ReadToEnd();
                    reader.Close();
                    stream.Close();

                }

                catch (Exception ex)
                {
                    Console.WriteLine("{0},{1}", ex.Message, ex.Source);
                    response.Close();
                }
                finally
                {

                    response.Close();

                }
            }
            if (!result.Contains("success"))
            {

                res = "0";


                return res + "@" + "0";
            }
            if (result.Contains("error"))
            {

                res = "0";


                return res + "@" + "0";
            }
            else
            {
                res = "1";

                strArr = result.Split('|');
                strTrnsId = strArr[2].Split('-');

            }

            return res + "@" + strTrnsId[0];

            // return "";
        }
        public string SendSMSByGupshup(string sendToPhoneNumber, string msg, string TiemStamp, string type)
        {

            sendToPhoneNumber = sendToPhoneNumber.Replace(",", "%20%7C%20");


            string result = "";
            string res = "";

            String[] strArr = null;
            String[] strTrnsId = null;

            //--------------------------Transactional SMS----------------------------------//

            string GupshupUrl = "http://enterprise.smsgupshup.com/GatewayAPI/rest?method=sendMessage";
            string GupshupId = "2000074159";
            string GupshupPwd = "xeeseM";
            string GupshupMask = "IDAHOM";

            //--------------------------Pramotional SMS----------------------------------//
            string GupshupIdPrm = "";
            string GupshupPWDPrm = "";

            if (sendToPhoneNumber.Trim() == "")
            {
                return "Mobile Number Not Found.";
            }

            else if (!IsPositiveNumber(sendToPhoneNumber))
            {
                return "Invalid Mobile Number.";
            }
            else
            {
                WebRequest request = null;
                HttpWebResponse response = null;

                string msg1 = msg;
                Regex rgxLines = new Regex("\\n");
                try
                {
                    string url = "";


                    msg = msg.Replace(" ", "%20");
                    url = GupshupUrl;
                    url = url + "&send_to=" + sendToPhoneNumber;
                    url = url + "&msg=" + msg1;

                    if (type == "Birthday" || type == "Anniversary")
                    {
                        url = url + "&userid=" + GupshupIdPrm;
                        url = url + "&password=" + GupshupPWDPrm;
                    }
                    else
                    {
                        url = url + "&userid=" + GupshupId;
                        url = url + "&password=" + GupshupPwd;
                    }

                    url = url + "&v=1.1";
                    url = url + "&mask=" + GupshupMask;
                    // url = url + "&timestamp=" + TiemStamp;

                    request = WebRequest.Create(url);
                    response = (HttpWebResponse)request.GetResponse();

                    Stream stream = response.GetResponseStream();
                    Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader reader = new StreamReader(stream, ec);
                    result = reader.ReadToEnd();
                    reader.Close();
                    stream.Close();

                }

                catch (Exception ex)
                {
                    Console.WriteLine("{0},{1}", ex.Message, ex.Source);
                    response.Close();
                }
                finally
                {

                    response.Close();

                }
            }
            if (!result.Contains("success"))
            {

                res = "0";


                return res + "@" + "0";
            }
            if (result.Contains("error"))
            {

                res = "0";


                return res + "@" + "0";
            }
            else
            {
                res = "1";

                strArr = result.Split('|');
                strTrnsId = strArr[2].Split('-');

            }

            return res + "@" + strTrnsId[0];

            // return "";
        }
        public static bool IsPositiveNumber(String strNumber)
        {
            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !objNotPositivePattern.IsMatch(strNumber) &&
                objPositivePattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber);
        }
        public string SendNotification(string deviceId, string tickerText, string contentTitle, string message, string dateval, string data)
        {

            string postData = "";

            // {"to":"/topics/ida","data": {"tickerText":"example test GCM","contentTitle":"Sync data","message": "Enter your message"}}
            postData =
            "{ \"condition\":\"'ida_news_app' in topics\", " +
            "\"data\": {" +
            "\"msgdata\": {\"type\":\"" + tickerText + "\", " +
            "\"title\":\"" + contentTitle + "\", " +
            "\"imageuri\":\"" + "/images/ic_launcher.png" + "\", " +
            "\"content\": \"" + message + "\"," +
            "\"date\": \"" + dateval + "\"," +
            "\"data\": " + data + "}}}";



            string SERVER_API_KEY = "AAAAt9Ji1U4:APA91bHAj-MvGsV-K4AT0RE3_MyLgHWN9BK17dFxpcDomjobkhuIgc7Zsp8IfdSI2yr8aKpihm7UVSk1FuEbewIWkI9vEAeZW-ymCS7eBHIR6E7MbMHEoub5PYnPW68SQw7NMt6mIyke";
            //var SENDER_ID = "900123701505"; //"724912804047";

            WebRequest tRequest;
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
            // tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            //string postData = "data.body=" + value + "&data.title=" + title + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";
            //string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"message\" : " + "\"" + value + "\",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\"},\"registration_ids\":[\"" + deviceId + "\"]}";

            //Console.WriteLine(postData);
            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;
            //Stream dataStream = tRequest.GetRequestStream();
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();
            //WebResponse tResponse = tRequest.GetResponse();
            //dataStream = tResponse.GetResponseStream();
            //StreamReader tReader = new StreamReader(dataStream);
            //String sResponseFromServer = tReader.ReadToEnd();
            //tReader.Close();
            //dataStream.Close();
            //tResponse.Close();
            return "";
        }
        public string SaveJson(string proc, string json)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            //using (SqlConnection con = conn.sqlCon("connectionstring"))
            {
                conn.Open();
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                XmlDocument xml = new XmlDocument();
                XmlDocument doc = new XmlDocument();
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }
                }
                int i = 0;
                i = sqlComm.ExecuteNonQuery();
                conn.Close();
                try
                {
                    if (i > 0)
                    {
                        // return ("{'status':'1'}");
                        return "status1";

                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {

                    return ("{'status':'0'}");
                }
            }
        }

        public string SaveJson(string proc, string json,string connection)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            //using (SqlConnection con = conn.sqlCon("connectionstring"))
            {
                conn.Open();
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                XmlDocument xml = new XmlDocument();
                XmlDocument doc = new XmlDocument();
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }
                }
                int i = 0;
                i = sqlComm.ExecuteNonQuery();
                conn.Close();
                try
                {
                    if (i > 0)
                    {
                        // return ("{'status':'1'}");
                        return "status1";

                    }
                    else
                    {
                        return ("{'status':'0'}");
                    }
                }
                catch
                {

                    return ("{'status':'0'}");
                }
            }
        }
        public string GetDataXmltoJson(string proc, string json, string connection)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            if (connection == "" || connection == null) connection = "connectionstring";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            //using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            //using (SqlConnection con = conn.sqlConnection("connectionstring"))
            //using (SqlConnection con = conn.sqlCon(connection))
            {
                conn.Open();
                SqlCommand sqlComm = new SqlCommand(proc, conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                if (json != "" && json != null)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    foreach (var kv in dict)
                    {
                        //string value = kv.Value;
                        //sqlComm.Parameters.AddWithValue("@" + kv.Key, value);
                        if (kv.Key == "xml")
                        {
                            xml = (XmlDocument)JsonConvert.DeserializeXmlNode(kv.Value);
                            sqlComm.Parameters.AddWithValue("@xml", xml.InnerXml.ToString());
                        }
                        else
                        {
                            sqlComm.Parameters.AddWithValue("@" + kv.Key, kv.Value);
                        }
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = sqlComm;
                da.Fill(ds);
                string str = "";
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        str = str + ds.Tables[0].Rows[i][0].ToString();

                    }
                    if (str == "")
                    {
                        str = "<root><subroot></subroot></root>";
                    }

                    xml.LoadXml(str);
                }
                else
                {
                    str = "<root><subroot><error>error</error></subroot></root>";
                    xml.LoadXml(str);
                }
            }
            return JsonConvert.SerializeXmlNode(xml);
        }

        public XmlDocument SaveDatatable(string proc, string json, string otherparameter, string datatablename, DataTable dt)
        {
            DataSet ds = new DataSet("data1");
            XmlDocument xml = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString))
            {
                string query = proc;
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                if (json != "")
                {
                    doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                    cmd.Parameters.AddWithValue("@xml", doc.InnerXml.ToString());
                }
                if (otherparameter != "" && otherparameter != null)
                {
                    doc.LoadXml(otherparameter);

                    XmlNodeList nodes = doc.GetElementsByTagName("Definition");
                    foreach (XmlNode node in nodes)
                    {
                        string param = node.Attributes["paramname"].Value;
                        string value = node.Attributes["paramvalue"].Value;
                        cmd.Parameters.AddWithValue("@" + param, value);

                    }
                }

                cmd.Parameters.AddWithValue("@" + datatablename, dt);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                try
                {
                    string str = "";
                    if (ds.Tables[0].Rows.Count >= 1)
                    {
                        for (var i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            str = str + ds.Tables[0].Rows[i][0].ToString();

                        }
                        xml.LoadXml(str);
                    }

                    return xml;
                }
                catch
                {
                    xml.LoadXml("<root><subroot><messege>There is some error while fetching data.Plz contact service provider</messege></subroot></root>");
                    return xml;

                }
            }

        }

        public void SaveResizedImage(Image image, double lnRatiom, string url)
        {

            try
            {
                var newidthm = (int)(image.Width * lnRatiom);
                var newHeightm = (int)(image.Height * lnRatiom);
                var thumbnailImgm = new Bitmap(newidthm, newHeightm);
                var thumbGraphm = Graphics.FromImage(thumbnailImgm);
                thumbGraphm.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraphm.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraphm.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectanglem = new Rectangle(0, 0, newidthm, newHeightm);
                thumbGraphm.DrawImage(image, imageRectanglem);
                thumbnailImgm.Save(url, image.RawFormat);
            }
            catch
            {

            }


        }


        public int Upload(Stream st, string filename, string folder)
        {
            FtpWebRequest request =
                (FtpWebRequest)WebRequest.Create("ftp://103.241.181.144:2112/www/Insurance/" + folder + "/" + filename);
            //request.Credentials = new NetworkCredential("dentsoftinftechocom", "SDtT4T68");
            request.Credentials = new NetworkCredential("insuranceal", "inSu545EaLff");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.KeepAlive = false;
            var per = 0;
            using (Stream fileStream = st)
            using (Stream ftpStream = request.GetRequestStream())
            {
                byte[] buffer = new byte[fileStream.Length];
                int read;
                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ftpStream.Write(buffer, 0, read);
                    per = (int)fileStream.Position;

                }
                return per;
            }
        }
        public int CreateDirectory(string directory)
        {
            try
            {
                string ftp = "ftp://103.241.181.144:2112/";

                //FTP Folder name. Leave blank if you want to list files from root folder.
                string ftpFolder = "WWW/Insurance/" + directory;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);

                //FtpWebRequest request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/", "103.241.181.144:2112", directory))) as FtpWebRequest;
                //request.Credentials = new NetworkCredential("dentsoftinftechocom", "SDtT4T68");
                request.Credentials = new NetworkCredential("insuranceal", "inSu545EaLff");
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int DeleteDirectory(string directory)
        {
            try
            {
                string ftp = "ftp://103.241.181.144:2112/";

                //FTP Folder name. Leave blank if you want to list files from root folder.
                string ftpFolder = "WWW/Insurance/" + directory;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);

                //FtpWebRequest request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/", "103.241.181.144:2112", directory))) as FtpWebRequest;
                //request.Credentials = new NetworkCredential("dentsoftinftechocom", "SDtT4T68");
                request.Credentials = new NetworkCredential("insuranceal", "inSu545EaLff");
                if (directory.Split('.').Length > 1)
                {
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                }
                else
                {
                    request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                }
                FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool MoveFile(string sourceFile, string destinationFile)
        {
            string ftp = "ftp://103.241.181.144:2112/";

            //FTP Folder name. Leave blank if you want to list files from root folder.
            string ftpFolder = "WWW/Insurance/" + sourceFile;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);

            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.Rename;
            request.Credentials = new NetworkCredential("insuranceal", "inSu545EaLff");
            request.RenameTo = destinationFile;
            FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
            bool Success = ftpResponse.StatusCode == FtpStatusCode.CommandOK || ftpResponse.StatusCode == FtpStatusCode.FileActionOK;
            return Success;
        }
        public void DownloadFile(string directory, string localFile)
        {

            try
            {
                string ftp = "ftp://103.241.181.144:2112/";

                //FTP Folder name. Leave blank if you want to list files from root folder.
                string ftpFolder = "WWW/Insurance/" + directory;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);
                int bufferSize = 2048;
                //FtpWebRequest request = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/", "103.241.181.144:2112", directory))) as FtpWebRequest;
                //request.Credentials = new NetworkCredential("dentsoftinftechocom", "SDtT4T68");
                request.Credentials = new NetworkCredential("insuranceal", "inSu545EaLff");
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = ftpResponse.GetResponseStream();

                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                request = null;
            }
            catch (Exception ex)
            {

            }
        }

        public string DataTableToJSON(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }


        public string getpath(string oldString)
        {
            return ConfigurationManager.AppSettings["ScheduleTypePath"] + (oldString.Substring(oldString.LastIndexOf("WebGallery")));
        }

        public string websitepath = ConfigurationManager.AppSettings["websitepath"];


        public string GetThumbnail(string video, string thumbnail)
        {
            var cmd = "ffmpeg  -itsoffset -1  -i " + '"' + video + '"' + " -vcodec mjpeg -vframes 1 -an -f rawvideo -s 320x240 " + '"' + thumbnail + '"';

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + cmd
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit(5000);
            LoadImage(thumbnail);
            return thumbnail;
        }

        static Bitmap LoadImage(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path));
            return (Bitmap)Image.FromStream(ms);
        }


        public string GetReceiptData(string variable)
        {
            BussinessLogic obj = new BussinessLogic();
            DataSet ds = new DataSet();
            string str = obj.getmultipletablejson("usp_getInvoiceReceipt", variable, "connectionstring");

            dynamic objdata = Json.Decode(str);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("/ReceiptDefault.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("[LogoPath]", ConfigurationManager.AppSettings["LogoUrl"].ToString());
            body = body.Replace("[ReceiptId]", objdata[0][0].billno.ToString());
            body = body.Replace("[ReceiptDate]", objdata[0][0].updatedate.ToString());
            body = body.Replace("[FullName]", objdata[0][0].billing_name.ToString());
            body = body.Replace("[Address1]", objdata[0][0].billing_address1.ToString());
            body = body.Replace("[Address2]", objdata[0][0].billing_address2.ToString());
            body = body.Replace("[Address3]", objdata[0][0].billing_address3.ToString());
            body = body.Replace("[CityName]", objdata[0][0].billing_city.ToString());
            body = body.Replace("[StateName]", objdata[0][0].billing_state.ToString());
            body = body.Replace("[ZipCode]", objdata[0][0].billing_pincode.ToString());
            body = body.Replace("[EmailId]", objdata[0][0].billing_email.ToString());
            body = body.Replace("[Mobile]", objdata[0][0].billing_mobile.ToString());
            body = body.Replace("[Shipping_Name]", objdata[0][0].delivery_name.ToString());
            body = body.Replace("[Shipping_Add1]", objdata[0][0].delivery_address.ToString());
            body = body.Replace("[Shipping_Add2]", objdata[0][0].delivery_address2.ToString());
            body = body.Replace("[Shipping_Add3]", objdata[0][0].delivery_address3.ToString());
            body = body.Replace("[Shipping_CityName]", objdata[0][0].delivery_city.ToString());
            body = body.Replace("[Shipping_StateName]", objdata[0][0].delivery_state.ToString());
            body = body.Replace("[Shipping_pincode]", objdata[0][0].delivery_pincode.ToString());
            body = body.Replace("[Shipping_Mobile]", objdata[0][0].delivery_Mobile.ToString());
            body = body.Replace("[Currency]", objdata[0][0].Currency.ToString());

            var temprow = "";
            var Req_data = "";
            for (int i = 0; i < objdata[2].Length; i++)
            {
                temprow = temprow + "<tr>";
                temprow = temprow + "<td style = 'padding:5px;width: 230px;' ><strong >" + objdata[2][i].item.ToString() + "</strong ></td >";
                temprow = temprow + "<td style = 'padding:5px;width: 230px;' ><strong >" + objdata[1][0].VenderName.ToString() + "</strong ></td >";
                temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;' ><i class='fa fa-inr'></i>&nbsp;" + (objdata[2][i].finalamount + objdata[2][i].disamount).ToString() + "</td>";
                temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;' >" + objdata[2][i].unit.ToString() + "</td >";
                temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;'><i class='fa fa-inr'></i>&nbsp;" + ((objdata[2][i].finalamount + objdata[2][i].disamount) * objdata[2][i].unit).ToString() + "</td>";
                //temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;' >" + objdata[2][i].tax.ToString() + "</td>";
                //temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;'><i class='fa fa-inr'></i>&nbsp;" + objdata[2][i].taxamount.ToString() + "</td>";
                temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;' ><i class='fa fa-inr'></i>&nbsp;" + objdata[2][i].disamount.ToString() + "</td>";
                temprow = temprow + "<td style = 'padding:5px;white-space:nowrap;' ><i class='fa fa-inr'></i>&nbsp;" + objdata[2][i].finalamount.ToString() + "</td>";
                temprow = temprow + "</tr>";

                //Req_data = (i + 1) + ". <b>" + objdata[2][i].item.ToString() + "</b>";
                // Req_data = Req_data + "<div>" + objdata[2][i].Requirment.ToString() + "</div>";
            }
            body = body.Replace("[TransactionRow]", temprow);
            body = body.Replace("[total_amount_Inwords]", ConvertNumbertoWords(Convert.ToInt64(objdata[0][0].total_amount)).ToString());
            body = body.Replace("[total_amount]", objdata[0][0].finalamount.ToString());
            body = body.Replace("[Requirement]", Req_data);

            return body.ToString();
        }

        public string ConvertNumbertoWords(long number)
        {
            if (number == 0) return "ZERO";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 100000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " LAKH ";
                number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            //if ((number / 10) > 0)  
            //{  
            // words += ConvertNumbertoWords(number / 10) + " RUPEES ";  
            // number %= 10;  
            //}  
            if (number > 0)
            {
                if (words != "") words += "AND ";
                var unitsMap = new[]
                {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
        };
                var tensMap = new[]
                {
            "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
        };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }
    }
}