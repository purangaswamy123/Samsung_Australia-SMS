using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Net;
using System.Globalization;
using System.Web;
using System.Net.Mail;
using System.Net.NetworkInformation;
using MessageMedia;
using MessageMedia.Common;
using MessageMedia.Gateway;
using MessageMedia.SMS;

namespace eQMSMessage_FormApp
{
    public partial class Form1 : Form
    {
        #region SMS - Variable Intiallization

        //public event SerialDataReceivedEventHandler DataReceived;

        SerialPort MySerialPort = new SerialPort();
        SMSController smscontroller = new SMSController();
        DataTable QueueGenerationdt = new DataTable();
        DataTable MissedQueuedt = new DataTable();
        DataTable AutoQueuedt = new DataTable();
        WebClient client = new WebClient();
        //Thread ReadSMSThread;
        string toAddress;
        string mobile;
        string PhoneNumber, FinalQueueNo, MessageNumber;
        //  string QueueToken1;
        string RecievedData;
        string QueueToken;
        Queue<string> RecData = new Queue<string>();

        int TotalWaitingQueue;
        int QueueCount;
        //int i = 0;
        int j = 0;

        string[] result;
        string[] splitted;
        int pos = 0;
        int i = 0;
        string messageid = null;
        string text = string.Empty;
        // int ThreadRelease;

        string JustNumbers;
        string BiggestNumbers;
        int SwappingNumbers = 1;

        // private BackgroundWorker BackgroundWorker;


        //int SMSEdit = 0;

        // Getting Connection String & Auto SMS

        // string connectionstring = eQMSMessage_FormApp.Properties.Resources.ConnectionString;
        String connectionstring;
        string AutoSMS = eQMSMessage_FormApp.Properties.Resources.AutoSMS;

        SqlDependency sqldep, sqldepen;

        SMSView smsview;

        static AutoResetEvent AutoEvent = new AutoResetEvent(true);

        #endregion SMS - Variable Intiallization

        #region Form1

        public Form1()
        {
            InitializeComponent();

            string[] lines = System.IO.File.ReadAllLines(@"C:\QMS\Display Config1.txt");
            foreach (String line in lines)
            {
                connectionstring = line;
            }
        }

        #endregion Form1

        #region SMS - Queue Token Generation SMS

        private void QueueTokenGenerationSMS()
        {
            SMSController smscontroller = new SMSController();
            SMSView smsview = new SMSView();
            DataTable QueueTokenGeneration = new DataTable();
            DataTable QueueTokenGenerationSentSMS = new DataTable();
            WebClient oWeb = new WebClient();
            Byte[] bytHeaders;


            try
            {
                QueueTokenGeneration = null;
                QueueTokenGenerationSentSMS = null;

                smsview = new SMSView();

                QueueTokenGeneration = smscontroller.GetGeneratedQueue();
                foreach (DataRow dr in QueueTokenGeneration.Rows)
                {
                    //  ThreadRelease = 1;
                    // Thread.Sleep(2000);
                    DataTable dt = new DataTable();
                    smsview.QueueTransaction = (Convert.ToInt32(dr["queu_visit_tnxid"].ToString()));
                    string QueueTokenGenerationSMS = (dr["visit_queue_no_show"].ToString());
                    string QueueTokenGenerationPhoneNo = (Convert.ToString(dr["members_mobile"].ToString()));
                    long QueueCustomerId = (Convert.ToInt64(dr["visit_customer_id"].ToString()));
                    smsview.DepartmentID = (Convert.ToInt32(dr["queue_department_id"]));
                    long memberId = (Convert.ToInt64(dr["members_id"].ToString()));
                    // dt = smscontroller.GetQueuePosition(smsview);
                    // string QueueTokenGenerationPhoneNo = 61 + QueueTokenGenerationPhoneNo1;
                    smsview.CustId = QueueCustomerId;
                    smsview.PhoneNo = QueueTokenGenerationPhoneNo;
                    int pos = 0;
                    dt = smscontroller.GetQueuePosition123(smsview);

                    foreach (DataRow dc in dt.Rows)
                    {
                        smsview.QueueTransaction = (Convert.ToInt32(dc["queu_visit_tnxid"].ToString()));
                        string dname = (dc["department_desc"].ToString());
                        // Thread.Sleep(100);
                        try
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string queno = (dc["visit_queue_no_show"].ToString());
                                smsview.MySms = queno;
                                // QueueToken = (dc["visit_queue_no_show"].ToString());
                                if (queno == QueueTokenGenerationSMS)
                                {
                                    if (dt != null && dt.Rows.Count > 0 && Convert.ToString(dt.Rows[i]["visit_queue_no_show"]).Equals(queno))
                                    // if (dt.Rows[i]["visit_queue_no_show"] == QueueToken)
                                    {
                                        DataTable CheckMessage = new DataTable();
                                        CheckMessage = smscontroller.GetNewMessageExistance(smsview);
                                        if (CheckMessage.Rows.Count <= 0)
                                        {
                                            pos = i;
                                            #region f pos>0
                                            if (pos > 0)
                                            {
                                                DataTable dtCustName = new DataTable();
                                                smsview.QueueNo = QueueTokenGenerationSMS;
                                                DataTable dtc = new DataTable();
                                                dtc = smscontroller.GetCustId(smsview);
                                                foreach (DataRow drc in dtc.Rows)
                                                {
                                                    long custid = (Convert.ToInt64(drc["visit_customer_id"].ToString()));
                                                    smsview.MenberId = (Convert.ToInt32(drc["members_id"].ToString()));
                                                    smsview.CustId = custid;

                                                    //retrieve name
                                                    dtCustName = smscontroller.GetCustomerName(smsview);
                                                    foreach (DataRow Custname in dtCustName.Rows)
                                                    {
                                                        string custfname = (Custname["members_firstname"].ToString());
                                                        string custlname = (Custname["members_lastname"].ToString());
                                                        string Cname = custfname + " " + custlname;
                                                        string mobileno = (Custname["members_mobile"].ToString());
                                                        string toAddress = (Custname["members_email"].ToString());
                                                        //string mobileno = 61 + mobileno1;
                                                        //retrieve name
                                                        string SmsStatusMsg = string.Empty;
                                                        string SmsDeliveryStatus = string.Empty;
                                                        try
                                                        {
                                                            #region email
                                                            //if (toAddress != "")
                                                            //{
                                                            //    string subject = "Q Number Details from Welcome to Ampulatory Care Centre";
                                                            //    string body = "Dear  " + Cname + ",<br> Welcome to Ampulatory Care Centre <br/> Your Queue Number is :" + QueueTokenGenerationSMS + "<br/>" + "Selected Department is :" + dname + "<br/>" + "Date Time :" + DateTime.Now + "<br/>" + "Please do not reply to this email. If you have any questions or " + "<br />" + "require further information about the operation of this site," + "<br />" + " please contact: Helpdesk" + "<br />" + "Ph: +61 422889101" + "<br />" + "Email: helpdesk@attsystemsgroup.com" + "<br />" + "";
                                                            //    MailMessage msgMail = new MailMessage("qsoft@attsystems.com.au", toAddress, subject, body);
                                                            //    msgMail.IsBodyHtml = true;
                                                            //    SmtpClient smtp = new SmtpClient();
                                                            //    smtp.Host = "mail.attsystemsgroup.com";
                                                            //    smtp.UseDefaultCredentials = true;
                                                            //    smtp.Credentials = new System.Net.NetworkCredential("qsoft@attsystems.com.au", "User@123");
                                                            //    smtp.Send(msgMail);
                                                            //}
                                                            #endregion email
                                                            if (mobileno != "")
                                                            {

                                                                if (mobileno.Length == 11)
                                                                {
                                                                    int t = 5 * pos;

                                                                    TimeSpan span = TimeSpan.FromMinutes(t);
                                                                    string apxtime = span.ToString(@"hh\:mm");
                                                                    string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                    //string strmsg1 = "Dear" + " " + Cname + ", Welcome to the Ambulatory Care Centre.\r\nYour ticket number is:" + QueueTokenGenerationSMS + " .\r\nTicket number will be called and displayed in the waiting room TV based on your appointment time. Please wait in the waiting room. Thanks";
                                                                    //\r\n\rTo track status of your queue no send SMS to 9214002002 e.g.: ATT<space><your Q number>";
                                                                    #region Proactive SMS Gateway
                                                                    //string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + mobileno + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                                    #endregion Proactive SMS Gateway

                                                                    #region AussieSMS Gateway

                                                                    // string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + mobileno + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                                    //string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + mobileno + "&text=" + strmsg1 + "";
                                                                    //Hospital Gateway

                                                                    //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                                    //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + mobileno + "&text=" + strmsg1 + "";

                                                                    //SMS for Samsung gateway
                                                                    // Set the username of the account holder.
                                                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                                    // Set the password of the account holder.
                                                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                                    // Set the first name of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                                    // Set the last name of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                                    // Set the mobile phone number of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                                    // Set the landline phone number of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                                    // Set the contact email of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                                    // Set the country of origin of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                                    bool testOK = false;
                                                                    try
                                                                    {
                                                                        // Test the user account settings.
                                                                        Account testAccount = Messaging.MessageController.UserAccount;
                                                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        // An exception was thrown. Display the details of the exception and return.
                                                                        string message = "There was an error testing the connection details:\n" +
                                                                        ex.Message;
                                                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                        return;
                                                                    }
                                                                    if (testOK)
                                                                    {
                                                                        // The user account settings were valid. Display a success message
                                                                        // box with the number of credits.
                                                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                                                        string message = string.Format("You have {0} message credits available.",
                                                                        balance);
                                                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                                    }
                                                                    else
                                                                    {
                                                                        // The username or password were incorrect. Display a failed message box.
                                                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                        // "Connection Failed", MessageBoxButtons.OK);
                                                                    }

                                                                    Messaging.MessageController.Settings.TimeOut = 60;
                                                                    // Set the batch size (number of messages to be sent at once) to 200.
                                                                    Messaging.MessageController.Settings.BatchSize = 200;
                                                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                                                    SMSMessage smsobj = new SMSMessage(mobileno, strmsg);
                                                                    Messaging.MessageController.AddToQueue(smsobj);
                                                                    Messaging.MessageController.SendMessages();
                                                                    //end of Samsung SMS
                                                                    #endregion AussieSMS Gateway
                                                                    //SmsStatusMsg = oWeb.DownloadString(URL);
                                                                    // if (SmsStatusMsg.Contains("<br>"))
                                                                    //  {
                                                                    //     SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                                    //  }
                                                                    smsview.SMSStatusFlag = "A";
                                                                    QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                    Thread.Sleep(100);
                                                                    DataTable dt1 = new DataTable();
                                                                    dt1 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                    foreach (DataRow dr123 in dt1.Rows)
                                                                    {
                                                                        string Sflag = (dr123["message_status_flag"].ToString());
                                                                        string uflag = Convert.ToString("N");
                                                                        if (Sflag == uflag)
                                                                        {
                                                                            smsview.SMSStatusFlag = "A";
                                                                            QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                        }
                                                                        else
                                                                        {
                                                                            #region xml for messageid
                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                                            //XmlDocument xml = new XmlDocument();
                                                                            //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                            //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                                            //foreach (XmlNode xn in xnList)
                                                                            //{
                                                                            //    XmlNode example = xn.SelectSingleNode("sms");
                                                                            //    if (example != null)
                                                                            //    {
                                                                            //        string na = example["messageid"].InnerText;
                                                                            //        string no = example["smsclientid"].InnerText;
                                                                            //        string mobile_no = example["mobile-no"].InnerText;
                                                                            #endregion xml for messageid

                                                                            #region message id from Aussie Gateway

                                                                            //char[] delimiterChars = { ':' };

                                                                            //text = SmsStatusMsg;
                                                                            //System.Console.WriteLine("Original text: '{0}'", text);
                                                                            //string[] words = text.Split(delimiterChars);
                                                                            //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                            //foreach (string s in words)
                                                                            //{
                                                                            //    for (i = 0; i < words.Length; i++)
                                                                            //    {
                                                                            //        if (pos == 1)
                                                                            //        {
                                                                            //            string[] digits = Regex.Split(s, @"\D+");
                                                                            //            //
                                                                            //            // Now we have each number string.
                                                                            //            //
                                                                            //            foreach (string value in digits)
                                                                            //            {
                                                                            //                //
                                                                            //                // Parse the value to get the number.
                                                                            //                //
                                                                            //                int number;
                                                                            //                if (int.TryParse(value, out number))
                                                                            //                {
                                                                            //                    messageid = value;
                                                                            //                }
                                                                            //            }
                                                                            //        }
                                                                            //    }

                                                                            //    // rsbel.QueueNo = s;
                                                                            //    pos++;
                                                                            //}

                                                                            #endregion message id from Aussie Gateway

                                                                            #region proactive delivery report

                                                                            //string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                                                            #endregion proactive delivery report

                                                                            #region Aussie Delivery report

                                                                            //string URL1 = "https://api.aussiesms.com.au/?querymessage&mobileID=61422889101&password=att0424&msgid=20150617121452";
                                                                            //string URL1 = "https://api.aussiesms.com.au/?querymessage&mobileID=61422889101&password=att0424&msgid=" + messageid + "";


                                                                            #endregion Aussie Delivery report

                                                                            // SmsDeliveryStatus = client.DownloadString(URL1);
                                                                            #region xml for delivery report
                                                                            //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                                            //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                                            //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                                            //XmlDocument xml1 = new XmlDocument();
                                                                            //xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                            ////XmlNodeList xnList1 = xml.SelectNodes("response");

                                                                            ////foreach (XmlNode xn1 in xnList1)
                                                                            ////{
                                                                            //XmlNode example1 = xml.SelectSingleNode("response");
                                                                            //if (example1 != null)
                                                                            //{
                                                                            //    //string rscode = example1["responsecode"].InnerText;
                                                                            //    smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                                            //    //string dlrcount = example1["dlristcount"].InnerText;
                                                                            //    //string pendingcount = example1["pendingdrcount"].InnerText;

                                                                            //}

                                                                            // }
                                                                            #endregion xml for delivery report

                                                                            #region Aussie Delivery report

                                                                            //char[] delimiterChars1 = { ':' };
                                                                            //text = SmsDeliveryStatus;
                                                                            //System.Console.WriteLine("Original text: '{0}'", text);
                                                                            //words = text.Split(delimiterChars1);
                                                                            //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                            //foreach (string s in words)
                                                                            //{

                                                                            //    smsview.DeliveryReport = s;
                                                                            //}


                                                                            #endregion Aussie Delivery report
                                                                            smsview.MySms = strmsg;
                                                                            smsview.QueueNo = QueueTokenGenerationSMS;
                                                                            smsview.IncomingsmsFlag = "N";
                                                                            smsview.SMSDateTime = System.DateTime.Now;
                                                                            string success;
                                                                            success = smscontroller.GetInsertNewSMS(smsview);

                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (mobileno.Length == 9)
                                                                    {
                                                                        mobileno = 61 + mobileno;

                                                                        int t = 5 * pos;

                                                                        TimeSpan span = TimeSpan.FromMinutes(t);
                                                                        string apxtime = span.ToString(@"hh\:mm");
                                                                        string strmsg = "Dear" + " " + Cname + ", Welcome to the Samsung Experience Store .\r\nYour ticket number is:" + QueueTokenGenerationSMS + " . Thanks";
                                                                        //\r\n\rTo track status of your queue no send SMS to 9214002002 e.g.: ATT<space><your Q number>";
                                                                        #region Proactive SMS Gateway
                                                                        //string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + mobileno + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                                        #endregion Proactive SMS Gateway

                                                                        #region AussieSMS Gateway

                                                                        // string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + mobileno + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                                        //string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + mobileno + "&text=" + strmsg1 + "";
                                                                        //Hospital Gateway

                                                                        //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                                        //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                                        //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                                        //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + mobileno + "&text=" + strmsg1 + "";

                                                                        #endregion AussieSMS Gateway

                                                                        #region Samsung SMS gateway
                                                                        //SMS for Samsung gateway
                                                                        // Set the username of the account holder.
                                                                        Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                                        // Set the password of the account holder.
                                                                        Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                                        // Set the first name of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                                        // Set the last name of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                                        // Set the mobile phone number of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                                        // Set the landline phone number of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                                        // Set the contact email of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                                        // Set the country of origin of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                                        bool testOK = false;
                                                                        try
                                                                        {
                                                                            // Test the user account settings.
                                                                            Account testAccount = Messaging.MessageController.UserAccount;
                                                                            testOK = Messaging.MessageController.TestAccount(testAccount);
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            // An exception was thrown. Display the details of the exception and return.
                                                                            string message = "There was an error testing the connection details:\n" +
                                                                            ex.Message;
                                                                            // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                            return;
                                                                        }
                                                                        if (testOK)
                                                                        {
                                                                            // The user account settings were valid. Display a success message
                                                                            // box with the number of credits.
                                                                            int balance = Messaging.MessageController.UserAccount.Balance;
                                                                            string message = string.Format("You have {0} message credits available.",
                                                                            balance);
                                                                            // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                                        }
                                                                        else
                                                                        {
                                                                            // The username or password were incorrect. Display a failed message box.
                                                                            //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                            // "Connection Failed", MessageBoxButtons.OK);
                                                                        }

                                                                        Messaging.MessageController.Settings.TimeOut = 60;
                                                                        // Set the batch size (number of messages to be sent at once) to 200.
                                                                        Messaging.MessageController.Settings.BatchSize = 200;
                                                                        //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                        //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                        //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                                        Messaging.MessageController.Settings.DeliveryReport = true;
                                                                        SMSMessage smsobj = new SMSMessage(mobileno, strmsg);
                                                                        Messaging.MessageController.AddToQueue(smsobj);
                                                                        Messaging.MessageController.SendMessages();
                                                                        //end of Samsung SMS
                                                                        #endregion Samsung SMS gateway

                                                                        //SmsStatusMsg = oWeb.DownloadString(URL);
                                                                        if (SmsStatusMsg.Contains("<br>"))
                                                                        {
                                                                            SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                                        }
                                                                        smsview.SMSStatusFlag = "A";
                                                                        QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                        // Thread.Sleep(100);
                                                                        DataTable dt1 = new DataTable();
                                                                        dt1 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                        foreach (DataRow dr123 in dt1.Rows)
                                                                        {
                                                                            string Sflag = (dr123["message_status_flag"].ToString());
                                                                            string uflag = Convert.ToString("N");
                                                                            if (Sflag == uflag)
                                                                            {
                                                                                smsview.SMSStatusFlag = "A";
                                                                                QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                            }
                                                                            else
                                                                            {
                                                                                #region xml for messageid
                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                                                //XmlDocument xml = new XmlDocument();
                                                                                //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                                                //foreach (XmlNode xn in xnList)
                                                                                //{
                                                                                //    XmlNode example = xn.SelectSingleNode("sms");
                                                                                //    if (example != null)
                                                                                //    {
                                                                                //        string na = example["messageid"].InnerText;
                                                                                //        string no = example["smsclientid"].InnerText;
                                                                                //        string mobile_no = example["mobile-no"].InnerText;
                                                                                #endregion xml for messageid

                                                                                #region message id from Aussie Gateway

                                                                                //char[] delimiterChars = { ':' };

                                                                                //text = SmsStatusMsg;
                                                                                //System.Console.WriteLine("Original text: '{0}'", text);
                                                                                //string[] words = text.Split(delimiterChars);
                                                                                //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                                //foreach (string s in words)
                                                                                //{
                                                                                //    for (i = 0; i < words.Length; i++)
                                                                                //    {
                                                                                //        if (pos == 1)
                                                                                //        {
                                                                                //            string[] digits = Regex.Split(s, @"\D+");
                                                                                //            //
                                                                                //            // Now we have each number string.
                                                                                //            //
                                                                                //            foreach (string value in digits)
                                                                                //            {
                                                                                //                //
                                                                                //                // Parse the value to get the number.
                                                                                //                //
                                                                                //                int number;
                                                                                //                if (int.TryParse(value, out number))
                                                                                //                {
                                                                                //                    messageid = value;
                                                                                //                }
                                                                                //            }
                                                                                //        }
                                                                                //    }

                                                                                //    // rsbel.QueueNo = s;
                                                                                //    pos++;
                                                                                //}

                                                                                #endregion message id from Aussie Gateway

                                                                                #region proactive delivery report

                                                                                //string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                                                                #endregion proactive delivery report

                                                                                #region Aussie Delivery report

                                                                                //string URL1 = "https://api.aussiesms.com.au/?querymessage&mobileID=61422889101&password=att0424&msgid=20150617121452";
                                                                                //string URL1 = "https://api.aussiesms.com.au/?querymessage&mobileID=61422889101&password=att0424&msgid=" + messageid + "";


                                                                                #endregion Aussie Delivery report

                                                                                // SmsDeliveryStatus = client.DownloadString(URL1);
                                                                                #region xml for delivery report
                                                                                //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                                                //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                                                //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                                                //XmlDocument xml1 = new XmlDocument();
                                                                                //xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                ////XmlNodeList xnList1 = xml.SelectNodes("response");

                                                                                ////foreach (XmlNode xn1 in xnList1)
                                                                                ////{
                                                                                //XmlNode example1 = xml.SelectSingleNode("response");
                                                                                //if (example1 != null)
                                                                                //{
                                                                                //    //string rscode = example1["responsecode"].InnerText;
                                                                                //    smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                                                //    //string dlrcount = example1["dlristcount"].InnerText;
                                                                                //    //string pendingcount = example1["pendingdrcount"].InnerText;

                                                                                //}

                                                                                // }
                                                                                #endregion xml for delivery report

                                                                                #region Aussie Delivery report

                                                                                //char[] delimiterChars1 = { ':' };
                                                                                //text = SmsDeliveryStatus;
                                                                                //System.Console.WriteLine("Original text: '{0}'", text);
                                                                                //words = text.Split(delimiterChars1);
                                                                                //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                                //foreach (string s in words)
                                                                                //{

                                                                                //    smsview.DeliveryReport = s;
                                                                                //}


                                                                                #endregion Aussie Delivery report
                                                                                smsview.MySms = strmsg;
                                                                                smsview.QueueNo = QueueTokenGenerationSMS;
                                                                                smsview.IncomingsmsFlag = "N";
                                                                                smsview.SMSDateTime = System.DateTime.Now;
                                                                                string success;
                                                                                success = smscontroller.GetInsertNewSMS(smsview);

                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                            }


                                                            //smsview.SMSStatusFlag = "A";
                                                            //QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                        }
                                                        catch (WebException e1)
                                                        {
                                                            SmsStatusMsg = e1.Message;
                                                        }
                                                        catch (Exception e2)
                                                        {
                                                            SmsStatusMsg = e2.Message;
                                                        }
                                                    }
                                                }
                                                // Thread.Sleep(2000);

                                            }
                                            #endregion if pos>0

                                            #region if pos<0
                                            else
                                            {
                                                DataTable dtCustName = new DataTable();
                                                smsview.QueueNo = QueueTokenGenerationSMS;
                                                DataTable dtc = new DataTable();
                                                dtc = smscontroller.GetCustId(smsview);
                                                foreach (DataRow drc in dtc.Rows)
                                                {
                                                    long custid = (Convert.ToInt64(drc["visit_customer_id"].ToString()));
                                                    smsview.MenberId = (Convert.ToInt32(drc["members_id"].ToString()));
                                                    smsview.CustId = custid;
                                                    //retrieve name
                                                    dtCustName = smscontroller.GetCustomerName(smsview);
                                                    foreach (DataRow Custname in dtCustName.Rows)
                                                    {
                                                        string custfname = (Custname["members_firstname"].ToString());
                                                        string custlname = (Custname["members_lastname"].ToString());
                                                        string Cname = custfname + " " + custlname;
                                                        string mobileno = (Custname["members_mobile"].ToString());
                                                        toAddress = (Custname["members_email"].ToString());
                                                        //retrieve name
                                                        string SmsStatusMsg = string.Empty;
                                                        string SmsDeliveryStatus = string.Empty;

                                                        try
                                                        {
                                                            #region email
                                                            //if (toAddress != "")
                                                            //{
                                                            //    string subject = "Q Number Details from Welcome to Ampulatory Care Centre";
                                                            //    string body = " Dear  " + Cname + ",<br>Welcome to Ampulatory Care Centre <br/> Your Queue Number is :" + QueueTokenGenerationSMS + "<br/>" + "Selected Department is :" + dname + "<br/>" + "Date Time :" + DateTime.Now + "<br/>" + "<br/>Please do not reply to this email. If you have any questions or " + "<br />" + "require further information about the operation of this site," + "<br />" + " please contact: Helpdesk" + "<br />" + "Ph: +61 422889101" + "<br />" + "Email: helpdesk@attsystemsgroup.com" + "<br />" + "";
                                                            //    MailMessage msgMail = new MailMessage("qsoft@attsystems.com.au", toAddress, subject, body);
                                                            //    msgMail.IsBodyHtml = true;
                                                            //    SmtpClient smtp = new SmtpClient();
                                                            //    smtp.Host = "mail.attsystemsgroup.com";
                                                            //    smtp.UseDefaultCredentials = true;
                                                            //    smtp.Credentials = new System.Net.NetworkCredential("qsoft@attsystems.com.au", "User@123");
                                                            //    smtp.Send(msgMail);
                                                            //}
                                                            #endregion email
                                                            if (QueueTokenGenerationPhoneNo != "")
                                                            {
                                                                if (QueueTokenGenerationPhoneNo.Length == 11)
                                                                {
                                                                    int t = 0;
                                                                    TimeSpan span = TimeSpan.FromMinutes(t);
                                                                    string apxtime = span.ToString(@"hh\:mm");
                                                                    string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";
                                                                    //\r\n\rTo track status of your queue no send SMS to 9214002002 e.g.: ATT<space><your Q number>";
                                                                    //string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + QueueTokenGenerationPhoneNo + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                                    //string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + QueueTokenGenerationPhoneNo + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                                    //string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + QueueTokenGenerationPhoneNo + "&text=" + strmsg1 + "";


                                                                    //Hospital Gateway

                                                                    //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                                    //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + QueueTokenGenerationPhoneNo + "&text=" + strmsg1 + "";

                                                                    #region Samsung SMS gateway
                                                                    //SMS for Samsung gateway
                                                                    // Set the username of the account holder.
                                                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                                    // Set the password of the account holder.
                                                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                                    // Set the first name of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                                    // Set the last name of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                                    // Set the mobile phone number of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                                    // Set the landline phone number of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                                    // Set the contact email of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                                    // Set the country of origin of the account holder (optional).
                                                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                                    bool testOK = false;
                                                                    try
                                                                    {
                                                                        // Test the user account settings.
                                                                        Account testAccount = Messaging.MessageController.UserAccount;
                                                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        // An exception was thrown. Display the details of the exception and return.
                                                                        string message = "There was an error testing the connection details:\n" +
                                                                        ex.Message;
                                                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                        return;
                                                                    }
                                                                    if (testOK)
                                                                    {
                                                                        // The user account settings were valid. Display a success message
                                                                        // box with the number of credits.
                                                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                                                        string message = string.Format("You have {0} message credits available.",
                                                                        balance);
                                                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                                    }
                                                                    else
                                                                    {
                                                                        // The username or password were incorrect. Display a failed message box.
                                                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                        // "Connection Failed", MessageBoxButtons.OK);
                                                                    }

                                                                    Messaging.MessageController.Settings.TimeOut = 60;
                                                                    // Set the batch size (number of messages to be sent at once) to 200.
                                                                    Messaging.MessageController.Settings.BatchSize = 200;
                                                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                                                    SMSMessage smsobj = new SMSMessage(mobileno, strmsg);
                                                                    Messaging.MessageController.AddToQueue(smsobj);
                                                                    Messaging.MessageController.SendMessages();
                                                                    //end of Samsung SMS
                                                                    #endregion Samsung SMS gateway

                                                                    //SmsStatusMsg = oWeb.DownloadString(URL);
                                                                    if (SmsStatusMsg.Contains("<br>"))
                                                                    {
                                                                        SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                                    }
                                                                    smsview.SMSStatusFlag = "A";
                                                                    smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                    // Thread.Sleep(100);
                                                                    DataTable dt1 = new DataTable();
                                                                    dt1 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                    foreach (DataRow dr123 in dt1.Rows)
                                                                    {
                                                                        string Sflag = (dr123["message_status_flag"].ToString());
                                                                        string uflag = Convert.ToString("N");
                                                                        if (Sflag == uflag)
                                                                        {
                                                                            smsview.SMSStatusFlag = "A";
                                                                            QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                        }
                                                                        else
                                                                        {

                                                                            #region xml for message id
                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                                            //XmlDocument xml = new XmlDocument();
                                                                            //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                            //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                                            //foreach (XmlNode xn in xnList)
                                                                            //{
                                                                            //    XmlNode example = xn.SelectSingleNode("sms");
                                                                            //    if (example != null)
                                                                            //    {
                                                                            //        string na = example["messageid"].InnerText;
                                                                            //        string no = example["smsclientid"].InnerText;
                                                                            //        string mobileno = example["mobile-no"].InnerText;
                                                                            #endregion xml for message id

                                                                            #region message id from Aussie Gateway

                                                                            //char[] delimiterChars = { ':' };

                                                                            //text = SmsStatusMsg;
                                                                            //System.Console.WriteLine("Original text: '{0}'", text);
                                                                            //string[] words = text.Split(delimiterChars);
                                                                            //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                            //foreach (string s in words)
                                                                            //{
                                                                            //    for (i = 0; i < words.Length; i++)
                                                                            //    {
                                                                            //        if (pos == 1)
                                                                            //        {
                                                                            //            string[] digits = Regex.Split(s, @"\D+");
                                                                            //            //
                                                                            //            // Now we have each number string.
                                                                            //            //
                                                                            //            foreach (string value in digits)
                                                                            //            {
                                                                            //                //
                                                                            //                // Parse the value to get the number.
                                                                            //                //
                                                                            //                int number;
                                                                            //                if (int.TryParse(value, out number))
                                                                            //                {
                                                                            //                    messageid = value;
                                                                            //                }
                                                                            //            }
                                                                            //        }
                                                                            //    }

                                                                            //    // rsbel.QueueNo = s;
                                                                            //    pos++;
                                                                            //}

                                                                            #endregion message id from Aussie Gateway

                                                                            #region Proactive messageid
                                                                            //string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";
                                                                            #endregion Proactive messageid

                                                                            #region Aussie Delivery report

                                                                            //string URL1 = "https://api.aussiesms.com.au/?querymessage&mobileID=61422889101&password=att0424&msgid=" + messageid + "";

                                                                            #endregion Aussie Delivery Report
                                                                            //SmsDeliveryStatus = client.DownloadString(URL1);
                                                                            #region Proactive Delivery report
                                                                            //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                                            //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                                            //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                                            //XmlDocument xml1 = new XmlDocument();
                                                                            //xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                            ////XmlNodeList xnList1 = xml.SelectNodes("response");

                                                                            ////foreach (XmlNode xn1 in xnList1)
                                                                            ////{
                                                                            //XmlNode example1 = xml.SelectSingleNode("response");
                                                                            //if (example1 != null)
                                                                            //{
                                                                            //    //string rscode = example1["responsecode"].InnerText;
                                                                            //    smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                                            //    //string dlrcount = example1["dlristcount"].InnerText;
                                                                            //    //string pendingcount = example1["pendingdrcount"].InnerText;
                                                                            // }
                                                                            //}
                                                                            #endregion Proactive Delivery report

                                                                            #region Aussie Delivery report
                                                                            //char[] delimiterChars1 = { ':' };
                                                                            //text = SmsDeliveryStatus;
                                                                            //System.Console.WriteLine("Original text: '{0}'", text);
                                                                            //words = text.Split(delimiterChars1);
                                                                            //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                            //foreach (string s in words)
                                                                            //{

                                                                            //    smsview.DeliveryReport = s;
                                                                            //}
                                                                            #endregion Aussie Delivery report

                                                                            smsview.MySms = strmsg;
                                                                            smsview.QueueNo = QueueTokenGenerationSMS;
                                                                            smsview.IncomingsmsFlag = "N";
                                                                            smsview.SMSDateTime = System.DateTime.Now;
                                                                            string success;
                                                                            success = smscontroller.GetInsertNewSMS(smsview);

                                                                        }

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (QueueTokenGenerationPhoneNo.Length == 9)
                                                                    {
                                                                        QueueTokenGenerationPhoneNo = 61 + QueueTokenGenerationPhoneNo;

                                                                        int t = 0;
                                                                        TimeSpan span = TimeSpan.FromMinutes(t);
                                                                        string apxtime = span.ToString(@"hh\:mm");
                                                                        string strmsg = "Dear" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";
                                                                        //\r\n\rTo track status of your queue no send SMS to 9214002002 e.g.: ATT<space><your Q number>";
                                                                        // string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + QueueTokenGenerationPhoneNo + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                                        //string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + QueueTokenGenerationPhoneNo + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                                        //string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + QueueTokenGenerationPhoneNo + "&text=" + strmsg1 + "";
                                                                        //Hospital Gateway

                                                                        //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                                        //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                                        //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                                        //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + QueueTokenGenerationPhoneNo + "&text=" + strmsg1 + "";

                                                                        #region Samsung SMS gateway
                                                                        //SMS for Samsung gateway
                                                                        // Set the username of the account holder.
                                                                        Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                                        // Set the password of the account holder.
                                                                        Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                                        // Set the first name of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                                        // Set the last name of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                                        // Set the mobile phone number of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                                        // Set the landline phone number of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                                        // Set the contact email of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                                        // Set the country of origin of the account holder (optional).
                                                                        Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                                        bool testOK = false;
                                                                        try
                                                                        {
                                                                            // Test the user account settings.
                                                                            Account testAccount = Messaging.MessageController.UserAccount;
                                                                            testOK = Messaging.MessageController.TestAccount(testAccount);
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            // An exception was thrown. Display the details of the exception and return.
                                                                            string message = "There was an error testing the connection details:\n" +
                                                                            ex.Message;
                                                                            // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                            return;
                                                                        }
                                                                        if (testOK)
                                                                        {
                                                                            // The user account settings were valid. Display a success message
                                                                            // box with the number of credits.
                                                                            int balance = Messaging.MessageController.UserAccount.Balance;
                                                                            string message = string.Format("You have {0} message credits available.",
                                                                            balance);
                                                                            // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                                        }
                                                                        else
                                                                        {
                                                                            // The username or password were incorrect. Display a failed message box.
                                                                            //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                            // "Connection Failed", MessageBoxButtons.OK);
                                                                        }

                                                                        Messaging.MessageController.Settings.TimeOut = 60;
                                                                        // Set the batch size (number of messages to be sent at once) to 200.
                                                                        Messaging.MessageController.Settings.BatchSize = 200;
                                                                        //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                        //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                        //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                                        Messaging.MessageController.Settings.DeliveryReport = true;
                                                                        SMSMessage smsobj = new SMSMessage(mobileno, strmsg);
                                                                        Messaging.MessageController.AddToQueue(smsobj);
                                                                        Messaging.MessageController.SendMessages();
                                                                        //end of Samsung SMS
                                                                        #endregion Samsung SMS gateway

                                                                        //SmsStatusMsg = oWeb.DownloadString(URL);
                                                                        if (SmsStatusMsg.Contains("<br>"))
                                                                        {
                                                                            SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                                        }
                                                                        smsview.SMSStatusFlag = "A";
                                                                        smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                        // Thread.Sleep(100);
                                                                        DataTable dt1 = new DataTable();
                                                                        dt1 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                        foreach (DataRow dr123 in dt1.Rows)
                                                                        {
                                                                            string Sflag = (dr123["message_status_flag"].ToString());
                                                                            string uflag = Convert.ToString("N");
                                                                            if (Sflag == uflag)
                                                                            {
                                                                                smsview.SMSStatusFlag = "A";
                                                                                QueueTokenGenerationSentSMS = smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                            }
                                                                            else
                                                                            {

                                                                                #region xml for message id
                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                                                //XmlDocument xml = new XmlDocument();
                                                                                //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                                                //foreach (XmlNode xn in xnList)
                                                                                //{
                                                                                //    XmlNode example = xn.SelectSingleNode("sms");
                                                                                //    if (example != null)
                                                                                //    {
                                                                                //        string na = example["messageid"].InnerText;
                                                                                //        string no = example["smsclientid"].InnerText;
                                                                                //        string mobileno = example["mobile-no"].InnerText;
                                                                                #endregion xml for message id

                                                                                #region message id from Aussie Gateway

                                                                                //char[] delimiterChars = { ':' };

                                                                                //text = SmsStatusMsg;
                                                                                //System.Console.WriteLine("Original text: '{0}'", text);
                                                                                //string[] words = text.Split(delimiterChars);
                                                                                //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                                //foreach (string s in words)
                                                                                //{
                                                                                //    for (i = 0; i < words.Length; i++)
                                                                                //    {
                                                                                //        if (pos == 1)
                                                                                //        {
                                                                                //            string[] digits = Regex.Split(s, @"\D+");
                                                                                //            //
                                                                                //            // Now we have each number string.
                                                                                //            //
                                                                                //            foreach (string value in digits)
                                                                                //            {
                                                                                //                //
                                                                                //                // Parse the value to get the number.
                                                                                //                //
                                                                                //                int number;
                                                                                //                if (int.TryParse(value, out number))
                                                                                //                {
                                                                                //                    messageid = value;
                                                                                //                }
                                                                                //            }
                                                                                //        }
                                                                                //    }

                                                                                //    // rsbel.QueueNo = s;
                                                                                //    pos++;
                                                                                //}

                                                                                #endregion message id from Aussie Gateway

                                                                                #region Proactive messageid
                                                                                //string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";
                                                                                #endregion Proactive messageid

                                                                                #region Aussie Delivery report

                                                                                //string URL1 = "https://api.aussiesms.com.au/?querymessage&mobileID=61422889101&password=att0424&msgid=" + messageid + "";

                                                                                #endregion Aussie Delivery Report
                                                                                //SmsDeliveryStatus = client.DownloadString(URL1);
                                                                                #region Proactive Delivery report
                                                                                //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                                                //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                                                //SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                                                //XmlDocument xml1 = new XmlDocument();
                                                                                //xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                ////XmlNodeList xnList1 = xml.SelectNodes("response");

                                                                                ////foreach (XmlNode xn1 in xnList1)
                                                                                ////{
                                                                                //XmlNode example1 = xml.SelectSingleNode("response");
                                                                                //if (example1 != null)
                                                                                //{
                                                                                //    //string rscode = example1["responsecode"].InnerText;
                                                                                //    smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                                                //    //string dlrcount = example1["dlristcount"].InnerText;
                                                                                //    //string pendingcount = example1["pendingdrcount"].InnerText;
                                                                                // }
                                                                                //}
                                                                                #endregion Proactive Delivery report

                                                                                #region Aussie Delivery report
                                                                                //char[] delimiterChars1 = { ':' };
                                                                                //text = SmsDeliveryStatus;
                                                                                //System.Console.WriteLine("Original text: '{0}'", text);
                                                                                //words = text.Split(delimiterChars1);
                                                                                //System.Console.WriteLine("{0} words in text:", words.Length);

                                                                                //foreach (string s in words)
                                                                                //{

                                                                                //    smsview.DeliveryReport = s;
                                                                                //}
                                                                                #endregion Aussie Delivery report

                                                                                smsview.MySms = strmsg;
                                                                                smsview.QueueNo = QueueTokenGenerationSMS;
                                                                                smsview.IncomingsmsFlag = "N";
                                                                                smsview.SMSDateTime = System.DateTime.Now;
                                                                                string success;
                                                                                success = smscontroller.GetInsertNewSMS(smsview);

                                                                            }

                                                                        }

                                                                    }
                                                                }



                                                            }
                                                        }
                                                        catch (WebException e1)
                                                        {
                                                            SmsStatusMsg = e1.Message;
                                                        }
                                                        catch (Exception e2)
                                                        {
                                                            SmsStatusMsg = e2.Message;
                                                        }

                                                    }
                                                }
                                                // Thread.Sleep(100);
                                            }
                                            #endregion if pos<0
                                        }

                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    // Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // Thread.Sleep(100);

        }
        #endregion SMS - Queue Token Generation SMS

        #region SMS - Missed Queue SMS

        private void MissedQueueSendingSMS()
        {
            SMSView smsview = new SMSView();
            SMSController smscontroller = new SMSController();
            DataTable MissedQueue = new DataTable();
            DataTable MissedQueueSentSMS = new DataTable();
            MissedQueue = null;
            //WebClient oWeb = new WebClient();
            //Byte[] bytHeaders;
            MissedQueueSentSMS = null;
            smsview = new SMSView();
            MissedQueue = smscontroller.GetMissedQueueSendingSMS();
            foreach (DataRow dr in MissedQueue.Rows)
            {
                smsview.QueueTransaction = (Convert.ToInt32(dr["queue_visit_tnxid"].ToString()));
                string MissedQueueNo = (dr["visit_queue_no_show"].ToString());
                string MissedPhoneNo = (Convert.ToString(dr["customer_mobile"].ToString()));
                long CustId = (Convert.ToInt64(dr["customer_id"].ToString()));
                // string mobileno = (Custname["members_mobile"].ToString());
                // string MissedPhoneNo = 61 + MissedPhoneNo1;
                string SmsStatusMsg = string.Empty;
                string SmsDeliveryStatus = string.Empty;

                // SMSView smsview = new SMSView();
                string cname = dr["visit_customer_name"].ToString();
                string appt = dr["customer_appointment_time"].ToString();

                DateTime apt = Convert.ToDateTime(appt);
                string cappt = apt.ToString("HH:mm");
                string custaptime = apt.ToShortTimeString();
                string mobnum = dr["customer_mobile"].ToString();

                try
                {
                    DataTable CheckMessage = new DataTable();
                    // CheckMessage = smscontroller.GetMessedQMessageExistance(smsview);
                    // if (CheckMessage.Rows.Count <= 0)
                    // {
                    if (MissedPhoneNo != "")
                    {

                        if (MissedPhoneNo.Length == 11)
                        {
                            //string strmsg = "Hi " + " " + Cname + ", It seems you missed your appointment at the Samsung Experience Stor. \r\nShould you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au";
                            // string strmsg = "Hi " + " " + MissedQueueNo +",It seems you missed your appointment at the Samsung Experience Stor. \r\nShould you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au";
                            string strmsg = "Hi " + cname + " It seems you missed your appointment at the Samsung Experience Store at " + cappt + "\r\nShould you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au.";
                            //Hi Kara, It seems you missed your appointment at the Samsung Experience Store at XX:XX, Should you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au
                            //"Hi " + cname + "It seems you missed your appointment at the Samsung Experience Store at " + cappt + "\r\nShould you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au."; 
                            smsview.SmsDesc = strmsg;
                            //string strmsg = "Hi: " + MissedQueueNo + " has been called but due to no show your number has been moved to missed queue. \r\nPlease contact reception for assistance";
                            //string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + MissedPhoneNo + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                            //string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + MissedPhoneNo + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                            //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                            //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                            //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                            ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                            //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + MissedPhoneNo + "&text=" + strmsg1 + "";

                            #region Samsung SMS gateway
                            //SMS for Samsung gateway
                            // Set the username of the account holder.
                            Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                            // Set the password of the account holder.
                            Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                            // Set the first name of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactFirstName = "David";
                            // Set the last name of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                            // Set the mobile phone number of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                            // Set the landline phone number of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                            // Set the contact email of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                            // Set the country of origin of the account holder (optional).
                            Messaging.MessageController.UserAccount.Country = Countries.Australia;
                            bool testOK = false;
                            try
                            {
                                // Test the user account settings.
                                Account testAccount = Messaging.MessageController.UserAccount;
                                testOK = Messaging.MessageController.TestAccount(testAccount);
                            }
                            catch (Exception ex)
                            {
                                // An exception was thrown. Display the details of the exception and return.
                                string message = "There was an error testing the connection details:\n" +
                                ex.Message;
                                // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                return;
                            }
                            if (testOK)
                            {
                                // The user account settings were valid. Display a success message
                                // box with the number of credits.
                                int balance = Messaging.MessageController.UserAccount.Balance;
                                string message = string.Format("You have {0} message credits available.",
                                balance);
                                // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                            }
                            else
                            {
                                // The username or password were incorrect. Display a failed message box.
                                //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                // "Connection Failed", MessageBoxButtons.OK);
                            }

                            Messaging.MessageController.Settings.TimeOut = 60;
                            // Set the batch size (number of messages to be sent at once) to 200.
                            Messaging.MessageController.Settings.BatchSize = 200;
                            //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                            //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                            //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                            //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                            //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                            Messaging.MessageController.Settings.DeliveryReport = true;
                            SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                            //SMSMessage smsobj = new SMSMessage(mobileno, strmsg);
                            Messaging.MessageController.AddToQueue(smsobj);
                            Messaging.MessageController.SendMessages();

                            //end of Samsung SMS
                            #endregion Samsung SMS gateway
                            //SmsStatusMsg = oWeb.DownloadString(URL);
                            //if (SmsStatusMsg.Contains("<br>"))
                            //{
                            //    SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                            //}
                            //smsview.SMSStatusFlag = "S";
                            //smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                            //// Thread.Sleep(100);
                            //DataTable dt1 = new DataTable();
                            //dt1 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                            //foreach (DataRow dr123 in dt1.Rows)
                            //{
                            //    string Sflag = (dr123["message_status_flag"].ToString());
                            //    string uflag = Convert.ToString("M");
                            //    if (Sflag == uflag)
                            //    {
                            //        smsview.SMSStatusFlag = "S";
                            //        smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                            //    }
                            //    else
                            //    {
                            //        #region delivery Report
                            //        //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                            //        //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                            //        //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                            //        //XmlDocument xml = new XmlDocument();
                            //        //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                            //        //XmlNodeList xnList = xml.SelectNodes("smslist");
                            //        //foreach (XmlNode xn in xnList)
                            //        //{
                            //        //    XmlNode example = xn.SelectSingleNode("sms");
                            //        //    if (example != null)
                            //        //    {
                            //        //        string na = example["messageid"].InnerText;
                            //        //        string no = example["smsclientid"].InnerText;
                            //        //        string mobileno = example["mobile-no"].InnerText;
                            //        //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                            //        //        SmsDeliveryStatus = client.DownloadString(URL1);
                            //        //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                            //        //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                            //        //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                            //        //        XmlDocument xml1 = new XmlDocument();
                            //        //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                            //        //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                            //        //        //foreach (XmlNode xn1 in xnList1)
                            //        //        //{
                            //        //        XmlNode example1 = xml.SelectSingleNode("response");
                            //        //        if (example1 != null)
                            //        //        {
                            //        //            //string rscode = example1["responsecode"].InnerText;
                            //        //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                            //        //            //string dlrcount = example1["dlristcount"].InnerText;
                            //        //            //string pendingcount = example1["pendingdrcount"].InnerText;

                            //        //        }
                            //        //    }
                            //        #endregion delivery Report
                            //        // smsview.QueueTransaction = Qtnxid;
                                   
                                   //// smsview.MySms = strmsg;
                                   // smsview.SmsDesc = strmsg;
                                   // smsview.QueueNo = MissedQueueNo;
                                   // smsview.PhoneNo = MissedPhoneNo;
                                   // smsview.DeliveryReport = "y";
                                   //// smsview.IncomingsmsFlag = "M";
                                   // smsview.SMSDateTime = System.DateTime.Now;
                                   // smsview.SMSStatusFlag = "S";
                                   // smsview.SmstnxId = 1;
                                   // smsview.SmsVisittnxId = 2;
                                   // smsview.CentreId = "";
                           
                            #region inserting to tbl_sms_tnx
                            smsview.CustId = CustId;
                            smsview.SmsDesc = strmsg;
                            smsview.PhoneNo = mobnum;
                            smsview.DeliveryReport = "y";
                            smsview.SmsDesc = strmsg;
                            smsview.IncomingsmsFlag = "M";
                            smsview.SmsVisittnxId = 2;
                            smsview.SMSDateTime = System.DateTime.Now;
                            smsview.SMSStatusFlag = "M";
                            smsview.QueueNo = Convert.ToString("1");
                            smsview.CentreId = "";
                            smsview.SMSDateTime = System.DateTime.Now;
                            string i;
                            i = smscontroller.getInsertAppointmentAlertSms(smsview);
                            #endregion into tbl_sms_tnx
                            // string d;
                            // d = smscontroller.GetInsertMissedQSMS(smsview);
                            // DataTable QueueTokenGenerationSentSMS = new DataTable();
                            // }
                            //smsview.SmsUpdatedDateTime = System.DateTime.Now;
                            //smsview.SmsActive = 'Y';
                            //smsview.SMSContentTypeId = 2;
                            //smsview.SmsAlert = 2;
                            //smsview.SmsUpdatedBy = "Admin";
                           // string i;
                            //#region inserting to tbl_sms_tnx
                            //smsview.CustId = custid;
                            //smsview.SmsDesc = strmsg;
                            //smsview.PhoneNo = mobnum;
                            //smsview.DeliveryReport = "y";
                            //smsview.SmsDesc = strmsg;
                            //smsview.IncomingsmsFlag = "M";
                            //smsview.SmstnxId = 1;
                            //smsview.SmsVisittnxId = 2;
                            //smsview.SMSDateTime = System.DateTime.Now;
                            //smsview.SMSStatusFlag = "M";
                            //smsview.QueueNo = Convert.ToString("1");
                            //smsview.CentreId = "";
                            //smsview.SMSDateTime = System.DateTime.Now;
                            //string i;
                            //i = smscontroller.getInsertAppointmentAlertSms(smsview);
                            //#endregion into tbl_sms_tnx

                           // i = smscontroller.getInsertAppointmentAlertSms(smsview);
                            // }
                        }
                        //}
                        else
                        {
                            if (MissedPhoneNo.Length == 9)
                            {
                                MissedPhoneNo = 61 + MissedPhoneNo;
                                //string strmsg1 = "Your ticket number: " + MissedQueueNo + " has been called but due to no show your number has been moved to missed queue. \r\nPlease contact reception for assistance";
                                //string strmsg = "Hi " + " " + MissedQueueNo + ",It seems you missed your appointment at the Samsung Experience Stor. \r\nShould you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au";
                                string strmsg = "Hi " + cname + " It seems you missed your appointment at the Samsung Experience Store at " + cappt + "\r\n Should you require a new appointment, please contact 1300 362 603 or visit www.samsung.com.au.";
                                smsview.SmsDesc = strmsg;
                                // string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + MissedPhoneNo + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                //string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + MissedPhoneNo + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + MissedPhoneNo + "&text=" + strmsg1 + "";

                                #region Samsung SMS gateway
                                //SMS for Samsung gateway
                                // Set the username of the account holder.
                                Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                // Set the password of the account holder.
                                Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                // Set the first name of the account holder (optional).
                                Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                // Set the last name of the account holder (optional).
                                Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                // Set the mobile phone number of the account holder (optional).
                                Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                // Set the landline phone number of the account holder (optional).
                                Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                // Set the contact email of the account holder (optional).
                                Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                // Set the country of origin of the account holder (optional).
                                Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                bool testOK = false;
                                try
                                {
                                    // Test the user account settings.
                                    Account testAccount = Messaging.MessageController.UserAccount;
                                    testOK = Messaging.MessageController.TestAccount(testAccount);
                                }
                                catch (Exception ex)
                                {
                                    // An exception was thrown. Display the details of the exception and return.
                                    string message = "There was an error testing the connection details:\n" +
                                    ex.Message;
                                    // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                    return;
                                }
                                if (testOK)
                                {
                                    // The user account settings were valid. Display a success message
                                    // box with the number of credits.
                                    int balance = Messaging.MessageController.UserAccount.Balance;
                                    string message = string.Format("You have {0} message credits available.",
                                    balance);
                                    // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    // The username or password were incorrect. Display a failed message box.
                                    //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                    // "Connection Failed", MessageBoxButtons.OK);
                                }

                                Messaging.MessageController.Settings.TimeOut = 60;
                                // Set the batch size (number of messages to be sent at once) to 200.
                                Messaging.MessageController.Settings.BatchSize = 200;
                                //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                Messaging.MessageController.Settings.DeliveryReport = true;
                                SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                                Messaging.MessageController.AddToQueue(smsobj);
                                Messaging.MessageController.SendMessages();
                                //end of Samsung SMS
                                #endregion Samsung SMS gateway

                                //SmsStatusMsg = oWeb.DownloadString(URL);
                                //if (SmsStatusMsg.Contains("<br>"))
                                //{
                                //    SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                //}
                                //smsview.SMSStatusFlag = "S";
                                //smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                ////Thread.Sleep(100);
                                //DataTable dt1 = new DataTable();
                                //dt1 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                //foreach (DataRow dr123 in dt1.Rows)
                                //{
                                //    string Sflag = (dr123["message_status_flag"].ToString());
                                //    string uflag = Convert.ToString("M");
                                //    if (Sflag == uflag)
                                //    {
                                //        smsview.SMSStatusFlag = "S";
                                //        smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                //    }
                                //    else
                                //    {
                                //        #region delivery Report
                                //        //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                //        //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                //        //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                //        //XmlDocument xml = new XmlDocument();
                                //        //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                //        //XmlNodeList xnList = xml.SelectNodes("smslist");
                                //        //foreach (XmlNode xn in xnList)
                                //        //{
                                //        //    XmlNode example = xn.SelectSingleNode("sms");
                                //        //    if (example != null)
                                //        //    {
                                //        //        string na = example["messageid"].InnerText;
                                //        //        string no = example["smsclientid"].InnerText;
                                //        //        string mobileno = example["mobile-no"].InnerText;
                                //        //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                //        //        SmsDeliveryStatus = client.DownloadString(URL1);
                                //        //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                //        //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                //        //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                //        //        XmlDocument xml1 = new XmlDocument();
                                //        //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                //        //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                //        //        //foreach (XmlNode xn1 in xnList1)
                                //        //        //{
                                //        //        XmlNode example1 = xml.SelectSingleNode("response");
                                //        //        if (example1 != null)
                                //        //        {
                                //        //            //string rscode = example1["responsecode"].InnerText;
                                //        //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                //        //            //string dlrcount = example1["dlristcount"].InnerText;
                                //        //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                //        //        }
                                //        //    }
                                //        #endregion delivery Report
                                // smsview.QueueTransaction = Qtnxid;
                               // smsview.CustId = CustId;
                               // smsview.SmsDesc = strmsg;
                               // smsview.QueueNo = MissedQueueNo;
                               // smsview.PhoneNo = MissedPhoneNo;
                               //// smsview.IncomingsmsFlag = "M";
                               // smsview.SMSDateTime = System.DateTime.Now;
                               // // string d;
                               // // d = smscontroller.GetInsertMissedQSMS(smsview);
                               // // DataTable QueueTokenGenerationSentSMS = new DataTable();
                               // // }
                               // //smsview.SmsUpdatedDateTime = System.DateTime.Now;
                               // smsview.SmsActive ='Y';
                               // smsview.SMSContentTypeId = 2;
                               // smsview.SmsAlert = 2;
                               // smsview.SmsUpdatedBy = "Admin";
                               // string i;
                               // i = smscontroller.getInsertAppointmentAlertSms(smsview);
                                // }
                                // }
                                // }
                                // }
                                #region inserting to tbl_sms_tnx
                                smsview.CustId = CustId;
                                smsview.SmsDesc = strmsg;
                                smsview.PhoneNo = mobnum;
                                smsview.DeliveryReport = "y";
                                smsview.SmsDesc = strmsg;
                                smsview.IncomingsmsFlag = "M";
                                smsview.SmsVisittnxId = 2;
                                smsview.SMSDateTime = System.DateTime.Now;
                                smsview.SMSStatusFlag = "M";
                                smsview.QueueNo = Convert.ToString("1");
                                smsview.CentreId = "";
                                smsview.SMSDateTime = System.DateTime.Now;
                                string i;
                                i = smscontroller.getInsertAppointmentAlertSms(smsview);
                                #endregion into tbl_sms_tnx

                            }
                        }
                    }
                }
                catch (WebException e1)
                {
                    //  SmsStatusMsg = e1.Message;
                }
                catch (Exception e2)
                {
                    // SmsStatusMsg = e2.Message;
                }

                //smsview.SMSStatusFlag = "S";

                //MissedQueueSentSMS = smscontroller.GetMissedQueueSentSMS(smsview);
            }
        }

        #endregion SMS - Send SMS

        #region AllMethod Recursive Techniq

        private void AllMetheds()
        {
            bool connection = NetworkInterface.GetIsNetworkAvailable();
            if (connection == true)
            {
                label3.Text = "Internet Is Available";
                label4.Text = "Application Is Running...";
                QueueTokenGenerationSMS();
                MissedQueueSendingSMS();
                appointmentalert();
                AppRemender();
                ExpiredAppointmentNotification();
                System.Data.SqlClient.SqlConnection.ClearAllPools();
            }
            else if (connection == false)
            {
                label3.Text = "Internet Is Not Available";
                label4.Text = "SMS Are Not Sending!!!";
                AllMetheds();
            }
        }

        #endregion AllMethod Recursive Techniq

        #region SMS - Reply Send SMS

        private void ReplySendSMS()
        {
            string URL;
            DataTable MtIncomingSMS = new DataTable();
            WebClient oWeb = new WebClient();
            Byte[] bytHeaders;
            SMSView smsview = new SMSView();
            DataTable dtinsms = new DataTable();
            DataTable dtqstatus = new DataTable();
            DataTable flagupdate = new DataTable();
            SMSController smscontroller = new SMSController();
            dtinsms = smscontroller.GetIncomingSMS(smsview);
            if (dtinsms.Rows.Count > 0)
            {

                foreach (DataRow dr in dtinsms.Rows)
                {
                    string PhoneNumber = (Convert.ToString(dr["sms_phoneno"].ToString()));
                    smsview.MySms = (Convert.ToString(dr["sms_content"].ToString()));
                    string SmsStatusMsg = string.Empty;
                    string SmsDeliveryStatus = string.Empty;
                    dtqstatus = smscontroller.GetQueueStatus(smsview);
                    DataTable dtWcount = new DataTable();
                    smsview.QueueStatusID = Convert.ToInt32(dtqstatus.Rows[0][0]);
                    if (dtqstatus.Rows.Count > 0)
                    {
                        if (smsview.QueueStatusID == 1)
                        {

                            try
                            {
                                // SMSWaitingQueueCount
                                DataTable dttnx = new DataTable();
                                dttnx = smscontroller.Getvisittnxidbyusingrepliedsms(smsview);
                                foreach (DataRow drtnx in dttnx.Rows)
                                {
                                    smsview.QueueTransaction = (Convert.ToInt32(drtnx["visit_tnx_id"].ToString()));

                                    // SMSWaitingQueueCount
                                    DataTable dttab = new DataTable();
                                    dttab = smscontroller.GetDeptid(smsview);
                                    foreach (DataRow dttb in dttab.Rows)
                                    {
                                        smsview.DepartmentID = (Convert.ToInt32(dttb["queue_department_id"].ToString()));
                                        DataTable dtcount = new DataTable();
                                        dtcount = smscontroller.GetStatusCount(smsview);
                                        foreach (DataRow c in dtcount.Rows)
                                        {
                                            QueueToken = (c["visit_queue_no_show"].ToString());
                                            for (int i = 0; i < dtcount.Rows.Count; i++)
                                            {
                                                int pos = 0;
                                                if (dtcount != null && dtcount.Rows.Count > 0 && Convert.ToString(dtcount.Rows[i]["visit_queue_no_show"]).Equals(QueueToken))
                                                // if (dt.Rows[i]["visit_queue_no_show"] == QueueToken)
                                                {
                                                    pos = i;

                                                    if (pos > 0)
                                                    {
                                                        int t = pos * 5;
                                                        TimeSpan span = TimeSpan.FromMinutes(t);
                                                        string aptime = span.ToString(@"hh\:mm");
                                                        DataTable dtcid = new DataTable();
                                                        dtcid = smscontroller.GetCustIDByUsingQueueno(smsview);
                                                        foreach (DataRow drcid in dtcid.Rows)
                                                        {
                                                            smsview.CustId = (Convert.ToInt32(drcid["visit_customer_id"].ToString()));

                                                            string str1 = "Your Queue NO:" + QueueToken + " is in waiting state.\r\nYou have " + pos + " person yet to be served.Approximate waiting time " + aptime + " Hours.\r\nYour will be called Shortly.\r\nplease proceed to the waiting area.";
                                                            //URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + PhoneNumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                            //URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + PhoneNumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                            // URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&from=Qsoft&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                                            //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                            //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                            //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                            ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                                            //URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + PhoneNumber + "&text=" + strmsg1 + "";


                                                            // SmsStatusMsg = client.DownloadString(URL);
                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                            //XmlDocument xml = new XmlDocument();
                                                            //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                            //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                            //foreach (XmlNode xn in xnList)
                                                            //{
                                                            //    XmlNode example = xn.SelectSingleNode("sms");
                                                            //    if (example != null)
                                                            //    {
                                                            //        string na = example["messageid"].InnerText;
                                                            //        string no = example["smsclientid"].InnerText;
                                                            //        string mobileno = example["mobile-no"].InnerText;
                                                            //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                                            //        SmsDeliveryStatus = client.DownloadString(URL1);
                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                            //        XmlDocument xml1 = new XmlDocument();
                                                            //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                            //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                                            //        //foreach (XmlNode xn1 in xnList1)
                                                            //        //{
                                                            //        XmlNode example1 = xml.SelectSingleNode("response");
                                                            //        if (example1 != null)
                                                            //        {
                                                            //            //string rscode = example1["responsecode"].InnerText;
                                                            //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                            //            //string dlrcount = example1["dlristcount"].InnerText;
                                                            //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                                            //        }
                                                            //    }

                                                            //}

                                                            #region Samsung SMS gateway
                                                            //SMS for Samsung gateway
                                                            // Set the username of the account holder.
                                                            Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                            // Set the password of the account holder.
                                                            Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                            // Set the first name of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                            // Set the last name of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                            // Set the mobile phone number of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                            // Set the landline phone number of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                            // Set the contact email of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                            // Set the country of origin of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                            bool testOK = false;
                                                            try
                                                            {
                                                                // Test the user account settings.
                                                                Account testAccount = Messaging.MessageController.UserAccount;
                                                                testOK = Messaging.MessageController.TestAccount(testAccount);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                // An exception was thrown. Display the details of the exception and return.
                                                                string message = "There was an error testing the connection details:\n" +
                                                                ex.Message;
                                                                // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                return;
                                                            }
                                                            if (testOK)
                                                            {
                                                                // The user account settings were valid. Display a success message
                                                                // box with the number of credits.
                                                                int balance = Messaging.MessageController.UserAccount.Balance;
                                                                string message = string.Format("You have {0} message credits available.",
                                                                balance);
                                                                // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                            }
                                                            else
                                                            {
                                                                // The username or password were incorrect. Display a failed message box.
                                                                //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                // "Connection Failed", MessageBoxButtons.OK);
                                                            }

                                                            Messaging.MessageController.Settings.TimeOut = 60;
                                                            // Set the batch size (number of messages to be sent at once) to 200.
                                                            Messaging.MessageController.Settings.BatchSize = 200;
                                                            //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                            //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                            //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                            //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                            //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                            Messaging.MessageController.Settings.DeliveryReport = true;
                                                            // SMSMessage smsobj = new SMSMessage(smsview.QueueStatusID, str1);
                                                            // Messaging.MessageController.AddToQueue(smsobj);
                                                            Messaging.MessageController.SendMessages();
                                                            //end of Samsung SMS
                                                            #endregion Samsung SMS gateway



                                                            if (SmsStatusMsg.Contains("<br>"))
                                                            {
                                                                SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                            }
                                                            smsview.QueueNo = QueueToken;
                                                            DateTime dattime = System.DateTime.Now;
                                                            smsview.SMSDateTime = dattime;
                                                            smsview.SMSStatusFlag = "R";
                                                            smsview.Message = str1;
                                                            smsview.PhoneNo = PhoneNumber;
                                                            smscontroller.GetInsertReplySMS(smsview);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int t = 5;
                                                        TimeSpan span = TimeSpan.FromMinutes(t);
                                                        string aptime = span.ToString(@"hh\:mm");
                                                        DataTable dtcid = new DataTable();
                                                        dtcid = smscontroller.GetCustIDByUsingQueueno(smsview);
                                                        foreach (DataRow drcid in dtcid.Rows)
                                                        {
                                                            smsview.CustId = (Convert.ToInt32(drcid["visit_customer_id"].ToString()));

                                                            string strmsg1 = "Your Queue NO:" + QueueToken + " is in waiting state.\r\nYou are the next person to be served.Approximate waiting time " + aptime + " Hours\r\nplease proceed to the waiting area.";
                                                            //URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + PhoneNumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                            //URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + PhoneNumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                            // URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&from=Qsoft&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                                            //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                            //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                            //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                            ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                                            //URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + PhoneNumber + "&text=" + strmsg1 + "";


                                                            //SmsStatusMsg = client.DownloadString(URL);
                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                            //XmlDocument xml = new XmlDocument();
                                                            //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                            //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                            //foreach (XmlNode xn in xnList)
                                                            //{
                                                            //    XmlNode example = xn.SelectSingleNode("sms");
                                                            //    if (example != null)
                                                            //    {
                                                            //        string na = example["messageid"].InnerText;
                                                            //        string no = example["smsclientid"].InnerText;
                                                            //        string mobileno = example["mobile-no"].InnerText;
                                                            //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                                            //        SmsDeliveryStatus = client.DownloadString(URL1);
                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                            //        XmlDocument xml1 = new XmlDocument();
                                                            //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                            //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                                            //        //foreach (XmlNode xn1 in xnList1)
                                                            //        //{
                                                            //        XmlNode example1 = xml.SelectSingleNode("response");
                                                            //        if (example1 != null)
                                                            //        {
                                                            //            //string rscode = example1["responsecode"].InnerText;
                                                            //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                            //            //string dlrcount = example1["dlristcount"].InnerText;
                                                            //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                                            //        }
                                                            //    }

                                                            //}
                                                            #region Samsung SMS gateway
                                                            //SMS for Samsung gateway
                                                            // Set the username of the account holder.
                                                            Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                            // Set the password of the account holder.
                                                            Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                            // Set the first name of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                            // Set the last name of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                            // Set the mobile phone number of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                            // Set the landline phone number of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                            // Set the contact email of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                            // Set the country of origin of the account holder (optional).
                                                            Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                            bool testOK = false;
                                                            try
                                                            {
                                                                // Test the user account settings.
                                                                Account testAccount = Messaging.MessageController.UserAccount;
                                                                testOK = Messaging.MessageController.TestAccount(testAccount);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                // An exception was thrown. Display the details of the exception and return.
                                                                string message = "There was an error testing the connection details:\n" +
                                                                ex.Message;
                                                                // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                return;
                                                            }
                                                            if (testOK)
                                                            {
                                                                // The user account settings were valid. Display a success message
                                                                // box with the number of credits.
                                                                int balance = Messaging.MessageController.UserAccount.Balance;
                                                                string message = string.Format("You have {0} message credits available.",
                                                                balance);
                                                                // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                            }
                                                            else
                                                            {
                                                                // The username or password were incorrect. Display a failed message box.
                                                                //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                // "Connection Failed", MessageBoxButtons.OK);
                                                            }

                                                            Messaging.MessageController.Settings.TimeOut = 60;
                                                            // Set the batch size (number of messages to be sent at once) to 200.
                                                            Messaging.MessageController.Settings.BatchSize = 200;
                                                            //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                            //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                            //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                            //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                            //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                            Messaging.MessageController.Settings.DeliveryReport = true;
                                                            //SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                                                            //Messaging.MessageController.AddToQueue(smsobj);
                                                            Messaging.MessageController.SendMessages();
                                                            //end of Samsung SMS
                                                            #endregion Samsung SMS gateway


                                                            if (SmsStatusMsg.Contains("<br>"))
                                                            {
                                                                SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                            }
                                                            smsview.QueueNo = QueueToken;
                                                            DateTime dattime = System.DateTime.Now;
                                                            smsview.SMSDateTime = dattime;
                                                            smsview.SMSStatusFlag = "R";
                                                            smsview.Message = strmsg1;
                                                            smsview.PhoneNo = PhoneNumber;
                                                            smscontroller.GetInsertReplySMS(smsview);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (WebException e1)
                            {
                                SmsStatusMsg = e1.Message;
                            }
                            catch (Exception e2)
                            {
                                SmsStatusMsg = e2.Message;
                            }
                            //smsview.InsmsStatus = "S";
                            //dtinsms = smscontroller.GetIncomingSMS(smsview);
                            //smsview.MySms = (Convert.ToString(dr["incomingsms"].ToString()));
                            //flagupdate = smscontroller.GetsmsStatusFlag(smsview);
                        }
                        else if (smsview.QueueStatusID == 2)
                        {
                            try
                            {
                                DataTable dtcid = new DataTable();
                                dtcid = smscontroller.GetCustIDByUsingQueueno(smsview);
                                foreach (DataRow drcid in dtcid.Rows)
                                {
                                    smsview.CustId = (Convert.ToInt32(drcid["visit_customer_id"].ToString()));
                                    string strmsg1 = "Thank you for asking your Q status...\r\nYour Queue NO:" + QueueToken + " is Serving";
                                    //URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + PhoneNumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                    //URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + PhoneNumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                    // URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&from=Qsoft&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                    //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                    ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                    //URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + PhoneNumber + "&text=" + strmsg1 + "";


                                    // SmsStatusMsg = client.DownloadString(URL);
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                    //XmlDocument xml = new XmlDocument();
                                    //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //XmlNodeList xnList = xml.SelectNodes("smslist");
                                    //foreach (XmlNode xn in xnList)
                                    //{
                                    //    XmlNode example = xn.SelectSingleNode("sms");
                                    //    if (example != null)
                                    //    {
                                    //        string na = example["messageid"].InnerText;
                                    //        string no = example["smsclientid"].InnerText;
                                    //        string mobileno = example["mobile-no"].InnerText;
                                    //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                    //        SmsDeliveryStatus = client.DownloadString(URL1);
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                    //        XmlDocument xml1 = new XmlDocument();
                                    //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                    //        //foreach (XmlNode xn1 in xnList1)
                                    //        //{
                                    //        XmlNode example1 = xml.SelectSingleNode("response");
                                    //        if (example1 != null)
                                    //        {
                                    //            //string rscode = example1["responsecode"].InnerText;
                                    //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                    //            //string dlrcount = example1["dlristcount"].InnerText;
                                    //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                    //        }
                                    //    }

                                    // }
                                    #region Samsung SMS gateway
                                    //SMS for Samsung gateway
                                    // Set the username of the account holder.
                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                    // Set the password of the account holder.
                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                    // Set the first name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                    // Set the last name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                    // Set the mobile phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                    // Set the landline phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                    // Set the contact email of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                    // Set the country of origin of the account holder (optional).
                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                    bool testOK = false;
                                    try
                                    {
                                        // Test the user account settings.
                                        Account testAccount = Messaging.MessageController.UserAccount;
                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                    }
                                    catch (Exception ex)
                                    {
                                        // An exception was thrown. Display the details of the exception and return.
                                        string message = "There was an error testing the connection details:\n" +
                                        ex.Message;
                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                        return;
                                    }
                                    if (testOK)
                                    {
                                        // The user account settings were valid. Display a success message
                                        // box with the number of credits.
                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                        string message = string.Format("You have {0} message credits available.",
                                        balance);
                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                    }
                                    else
                                    {
                                        // The username or password were incorrect. Display a failed message box.
                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                        // "Connection Failed", MessageBoxButtons.OK);
                                    }

                                    Messaging.MessageController.Settings.TimeOut = 60;
                                    // Set the batch size (number of messages to be sent at once) to 200.
                                    Messaging.MessageController.Settings.BatchSize = 200;
                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                    //SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                                    //Messaging.MessageController.AddToQueue(smsobj);
                                    Messaging.MessageController.SendMessages();
                                    //end of Samsung SMS
                                    #endregion Samsung SMS gateway



                                    if (SmsStatusMsg.Contains("<br>"))
                                    {
                                        SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                    }
                                    smsview.QueueNo = QueueToken;
                                    DateTime dattime = System.DateTime.Now;
                                    smsview.SMSDateTime = dattime;
                                    smsview.SMSStatusFlag = "R";
                                    smsview.Message = strmsg1;
                                    smsview.PhoneNo = PhoneNumber;
                                    smscontroller.GetInsertReplySMS(smsview);
                                }
                            }
                            catch (WebException e1)
                            {
                                SmsStatusMsg = e1.Message;
                            }
                            catch (Exception e2)
                            {
                                SmsStatusMsg = e2.Message;
                            }
                            //smsview.InsmsStatus = "S";
                            //dtinsms = smscontroller.GetIncomingSMS(smsview);
                            //smsview.MySms = (Convert.ToString(dr["incomingsms"].ToString()));
                            //flagupdate = smscontroller.GetsmsStatusFlag(smsview);
                        }
                        else if (smsview.QueueStatusID == 3)
                        {
                            try
                            {
                                DataTable dtcid = new DataTable();
                                dtcid = smscontroller.GetCustIDByUsingQueueno(smsview);
                                foreach (DataRow drcid in dtcid.Rows)
                                {
                                    string strmsg1 = "Thank you for asking your Q status...\r\nYour Queue NO:" + QueueToken + " is Ended\r\nThank you, Please visit again";
                                    //URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + PhoneNumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                    //URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + PhoneNumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                    // URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&from=Qsoft&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                    //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                    ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                    //URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + PhoneNumber + "&text=" + strmsg1 + "";


                                    // SmsStatusMsg = client.DownloadString(URL);
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                    //XmlDocument xml = new XmlDocument();
                                    //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //XmlNodeList xnList = xml.SelectNodes("smslist");
                                    //foreach (XmlNode xn in xnList)
                                    //{
                                    //    XmlNode example = xn.SelectSingleNode("sms");
                                    //    if (example != null)
                                    //    {
                                    //        string na = example["messageid"].InnerText;
                                    //        string no = example["smsclientid"].InnerText;
                                    //        string mobileno = example["mobile-no"].InnerText;
                                    //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                    //        SmsDeliveryStatus = client.DownloadString(URL1);
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                    //        XmlDocument xml1 = new XmlDocument();
                                    //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                    //        //foreach (XmlNode xn1 in xnList1)
                                    //        //{
                                    //        XmlNode example1 = xml.SelectSingleNode("response");
                                    //        if (example1 != null)
                                    //        {
                                    //            //string rscode = example1["responsecode"].InnerText;
                                    //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                    //            //string dlrcount = example1["dlristcount"].InnerText;
                                    //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                    //        }
                                    //    }

                                    //}

                                    #region Samsung SMS gateway
                                    //SMS for Samsung gateway
                                    // Set the username of the account holder.
                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                    // Set the password of the account holder.
                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                    // Set the first name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                    // Set the last name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                    // Set the mobile phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                    // Set the landline phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                    // Set the contact email of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                    // Set the country of origin of the account holder (optional).
                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                    bool testOK = false;
                                    try
                                    {
                                        // Test the user account settings.
                                        Account testAccount = Messaging.MessageController.UserAccount;
                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                    }
                                    catch (Exception ex)
                                    {
                                        // An exception was thrown. Display the details of the exception and return.
                                        string message = "There was an error testing the connection details:\n" +
                                        ex.Message;
                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                        return;
                                    }
                                    if (testOK)
                                    {
                                        // The user account settings were valid. Display a success message
                                        // box with the number of credits.
                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                        string message = string.Format("You have {0} message credits available.",
                                        balance);
                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                    }
                                    else
                                    {
                                        // The username or password were incorrect. Display a failed message box.
                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                        // "Connection Failed", MessageBoxButtons.OK);
                                    }

                                    Messaging.MessageController.Settings.TimeOut = 60;
                                    // Set the batch size (number of messages to be sent at once) to 200.
                                    Messaging.MessageController.Settings.BatchSize = 200;
                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                    //SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                                    //Messaging.MessageController.AddToQueue(smsobj);
                                    Messaging.MessageController.SendMessages();
                                    //end of Samsung SMS
                                    #endregion Samsung SMS gateway


                                    if (SmsStatusMsg.Contains("<br>"))
                                    {
                                        SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                    }
                                    smsview.QueueNo = QueueToken;
                                    DateTime dattime = System.DateTime.Now;
                                    smsview.SMSDateTime = dattime;
                                    smsview.SMSStatusFlag = "R";
                                    smsview.Message = strmsg1;
                                    smsview.PhoneNo = PhoneNumber;
                                    smscontroller.GetInsertReplySMS(smsview);
                                }
                            }
                            catch (WebException e1)
                            {
                                SmsStatusMsg = e1.Message;
                            }
                            catch (Exception e2)
                            {
                                SmsStatusMsg = e2.Message;
                            }
                            //smsview.InsmsStatus = "S";
                            //dtinsms = smscontroller.GetIncomingSMS(smsview);
                            //smsview.MySms = (Convert.ToString(dr["incomingsms"].ToString()));
                            //flagupdate = smscontroller.GetsmsStatusFlag(smsview);
                        }
                        else if (smsview.QueueStatusID == 4)
                        {
                            try
                            {
                                DataTable dtcid = new DataTable();
                                dtcid = smscontroller.GetCustIDByUsingQueueno(smsview);
                                foreach (DataRow drcid in dtcid.Rows)
                                {
                                    string strmsg1 = "Thank you for asking your Q status...\r\nYour Queue is missed. Please contact reception for assistance.";
                                    //URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + PhoneNumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                    //URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + PhoneNumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                    //URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&from=Qsoft&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                    //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                    ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                    //URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + PhoneNumber + "&text=" + strmsg1 + "";


                                    // SmsStatusMsg = client.DownloadString(URL);
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                    //XmlDocument xml = new XmlDocument();
                                    //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //XmlNodeList xnList = xml.SelectNodes("smslist");
                                    //foreach (XmlNode xn in xnList)
                                    //{
                                    //    XmlNode example = xn.SelectSingleNode("sms");
                                    //    if (example != null)
                                    //    {
                                    //        string na = example["messageid"].InnerText;
                                    //        string no = example["smsclientid"].InnerText;
                                    //        string mobileno = example["mobile-no"].InnerText;
                                    //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                    //        SmsDeliveryStatus = client.DownloadString(URL1);
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                    //        XmlDocument xml1 = new XmlDocument();
                                    //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                    //        //foreach (XmlNode xn1 in xnList1)
                                    //        //{
                                    //        XmlNode example1 = xml.SelectSingleNode("response");
                                    //        if (example1 != null)
                                    //        {
                                    //            //string rscode = example1["responsecode"].InnerText;
                                    //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                    //            //string dlrcount = example1["dlristcount"].InnerText;
                                    //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                    //        }
                                    //    }

                                    //}
                                    #region Samsung SMS gateway
                                    //SMS for Samsung gateway
                                    // Set the username of the account holder.
                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                    // Set the password of the account holder.
                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                    // Set the first name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                    // Set the last name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                    // Set the mobile phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                    // Set the landline phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                    // Set the contact email of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                    // Set the country of origin of the account holder (optional).
                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                    bool testOK = false;
                                    try
                                    {
                                        // Test the user account settings.
                                        Account testAccount = Messaging.MessageController.UserAccount;
                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                    }
                                    catch (Exception ex)
                                    {
                                        // An exception was thrown. Display the details of the exception and return.
                                        string message = "There was an error testing the connection details:\n" +
                                        ex.Message;
                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                        return;
                                    }
                                    if (testOK)
                                    {
                                        // The user account settings were valid. Display a success message
                                        // box with the number of credits.
                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                        string message = string.Format("You have {0} message credits available.",
                                        balance);
                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                    }
                                    else
                                    {
                                        // The username or password were incorrect. Display a failed message box.
                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                        // "Connection Failed", MessageBoxButtons.OK);
                                    }

                                    Messaging.MessageController.Settings.TimeOut = 60;
                                    // Set the batch size (number of messages to be sent at once) to 200.
                                    Messaging.MessageController.Settings.BatchSize = 200;
                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                    //SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                                    //Messaging.MessageController.AddToQueue(smsobj);
                                    Messaging.MessageController.SendMessages();
                                    //end of Samsung SMS
                                    #endregion Samsung SMS gateway



                                    if (SmsStatusMsg.Contains("<br>"))
                                    {
                                        SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                    }
                                    smsview.QueueNo = QueueToken;
                                    DateTime dattime = System.DateTime.Now;
                                    smsview.SMSDateTime = dattime;
                                    smsview.SMSStatusFlag = "R";
                                    smsview.Message = strmsg1;
                                    smsview.PhoneNo = PhoneNumber;
                                    smscontroller.GetInsertReplySMS(smsview);
                                }
                            }
                            catch (WebException e1)
                            {
                                SmsStatusMsg = e1.Message;
                            }
                            catch (Exception e2)
                            {
                                SmsStatusMsg = e2.Message;
                            }
                            //smsview.InsmsStatus = "S";
                            //dtinsms = smscontroller.GetIncomingSMS(smsview);
                            //smsview.MySms = (Convert.ToString(dr["incomingsms"].ToString()));
                            //flagupdate = smscontroller.GetsmsStatusFlag(smsview);
                        }
                        else if (smsview.QueueStatusID == 5)
                        {
                            try
                            {
                                DataTable dtcid = new DataTable();
                                dtcid = smscontroller.GetCustIDByUsingQueueno(smsview);
                                foreach (DataRow drcid in dtcid.Rows)
                                {
                                    string strmsg1 = "Thank you for asking your Q status...\r\nYour Queue is On hold";
                                    //URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + PhoneNumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                    //URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + PhoneNumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                    // URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&from=Qsoft&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                    //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                    ////string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + MissedPhoneNo + "&text=" + strmsg1 + "";
                                    //URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?UserName=qsoft@attsytems.com.au&Password=Qsoft123&destination=" + PhoneNumber + "&text=" + strmsg1 + "";

                                    // SmsStatusMsg = client.DownloadString(URL);
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                    //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                    //XmlDocument xml = new XmlDocument();
                                    //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //XmlNodeList xnList = xml.SelectNodes("smslist");
                                    //foreach (XmlNode xn in xnList)
                                    //{
                                    //    XmlNode example = xn.SelectSingleNode("sms");
                                    //    if (example != null)
                                    //    {
                                    //        string na = example["messageid"].InnerText;
                                    //        string no = example["smsclientid"].InnerText;
                                    //        string mobileno = example["mobile-no"].InnerText;
                                    //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                    //        SmsDeliveryStatus = client.DownloadString(URL1);
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                    //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                    //        XmlDocument xml1 = new XmlDocument();
                                    //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                    //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                    //        //foreach (XmlNode xn1 in xnList1)
                                    //        //{
                                    //        XmlNode example1 = xml.SelectSingleNode("response");
                                    //        if (example1 != null)
                                    //        {
                                    //            //string rscode = example1["responsecode"].InnerText;
                                    //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                    //            //string dlrcount = example1["dlristcount"].InnerText;
                                    //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                    //        }
                                    //    }

                                    //}

                                    #region Samsung SMS gateway
                                    //SMS for Samsung gateway
                                    // Set the username of the account holder.
                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                    // Set the password of the account holder.
                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                    // Set the first name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                    // Set the last name of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                    // Set the mobile phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                    // Set the landline phone number of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                    // Set the contact email of the account holder (optional).
                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                    // Set the country of origin of the account holder (optional).
                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                    bool testOK = false;
                                    try
                                    {
                                        // Test the user account settings.
                                        Account testAccount = Messaging.MessageController.UserAccount;
                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                    }
                                    catch (Exception ex)
                                    {
                                        // An exception was thrown. Display the details of the exception and return.
                                        string message = "There was an error testing the connection details:\n" +
                                        ex.Message;
                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                        return;
                                    }
                                    if (testOK)
                                    {
                                        // The user account settings were valid. Display a success message
                                        // box with the number of credits.
                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                        string message = string.Format("You have {0} message credits available.",
                                        balance);
                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                    }
                                    else
                                    {
                                        // The username or password were incorrect. Display a failed message box.
                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                        // "Connection Failed", MessageBoxButtons.OK);
                                    }

                                    Messaging.MessageController.Settings.TimeOut = 60;
                                    // Set the batch size (number of messages to be sent at once) to 200.
                                    Messaging.MessageController.Settings.BatchSize = 200;
                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                    //SMSMessage smsobj = new SMSMessage(MissedPhoneNo, strmsg);
                                    //Messaging.MessageController.AddToQueue(smsobj);
                                    Messaging.MessageController.SendMessages();
                                    //end of Samsung SMS
                                    #endregion Samsung SMS gateway


                                    if (SmsStatusMsg.Contains("<br>"))
                                    {
                                        SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                    }
                                    smsview.QueueNo = QueueToken;
                                    DateTime dattime = System.DateTime.Now;
                                    smsview.SMSDateTime = dattime;
                                    smsview.SMSStatusFlag = "R";
                                    smsview.Message = strmsg1;
                                    smsview.PhoneNo = PhoneNumber;
                                    smscontroller.GetInsertReplySMS(smsview);
                                }
                            }
                            catch (WebException e1)
                            {
                                SmsStatusMsg = e1.Message;
                            }
                            catch (Exception e2)
                            {
                                SmsStatusMsg = e2.Message;
                            }
                            //smsview.InsmsStatus = "S";
                            //dtinsms = smscontroller.GetIncomingSMS(smsview);
                            //smsview.MySms = (Convert.ToString(dr["incomingsms"].ToString()));
                            //flagupdate = smscontroller.GetsmsStatusFlag(smsview);
                        }
                    }



                }
            }
        }
        #endregion SMS - Reply Send SMS

        #region formload
        private void Form1_Load(object sender, EventArgs e)
        {
            bool connection = NetworkInterface.GetIsNetworkAvailable();
            if (connection == true)
            {


                label3.Text = "Internet Is Available";
                label4.Text = "Application Is Running...";

            }
            else if (connection == false)
            {
                label3.Text = "Internet Is Not Available";
                label4.Text = "SMS Are Not Sending!!!";

            }
            label2.Text = "Copyright " + Convert.ToChar(169) + " ATT Systems Group 2016";///169 is copyright symbol
            // AllMetheds();
        }
        #endregion formload

        #region Send Alert SMS
        public void SendAlertSMS()
        {
            SMSView smsview = new SMSView();
            DataTable dt = new DataTable();
            WebClient oWeb = new WebClient();
            // Byte[] bytHeaders;
            dt = smscontroller.GetQueuePosition(smsview);
            foreach (DataRow dr in dt.Rows)
            {
                //Thread.Sleep(100);
                try
                {
                    smsview.MySms = (dr["visit_queue_no_show"].ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                // smsview.MySms = QueueToken1;
                DataTable dt11 = new DataTable();
                dt11 = smscontroller.Getvisittnxidbyusingrepliedsms(smsview);
                foreach (DataRow dr11 in dt11.Rows)
                {
                    smsview.QueueTransaction = (Convert.ToInt32(dr11["visit_tnx_id"].ToString()));
                    DataTable dt12 = new DataTable();
                    dt12 = smscontroller.GetDeptid(smsview);
                    foreach (DataRow dr12 in dt12.Rows)
                    {
                        smsview.DepartmentID = (Convert.ToInt32(dr12["queue_department_id"].ToString()));
                        int dept = smsview.DepartmentID;
                        DataTable btnevent = new DataTable();
                        btnevent = smscontroller.GetButtonEvent(smsview);
                        if (btnevent.Rows.Count > 0)
                        {
                            foreach (DataRow dr1x in btnevent.Rows)
                            {
                                DataTable dbe = new DataTable();
                                smsview.ButtonVisitTnx = Convert.ToInt32(dr1x["visit_tnx_id"].ToString());
                                int dep1 = Convert.ToInt32(dr1x["queue_department_id"].ToString());
                                if (dept == dep1)
                                {
                                    smsview.ButtonEventFlag = "N";
                                    dbe = smscontroller.getUpdatebuttoneventflag(smsview);


                                    DataTable dt13 = new DataTable();
                                    dt13 = smscontroller.GetStatusCount(smsview);
                                    foreach (DataRow dr13 in dt13.Rows)
                                    {
                                        QueueToken = (dr13["visit_queue_no_show"].ToString());
                                        // SMSController smscontroller = new SMSController();
                                        DataTable dt14 = new DataTable();
                                        dt14 = smscontroller.GetStatusCount123(smsview);
                                        foreach (DataRow dr14 in dt14.Rows)
                                        {

                                            string qtoken = (dr14["visit_queue_no_show"].ToString());

                                            for (j = 0; j < dt14.Rows.Count; j++)
                                            {
                                                if (dt14 != null && dt14.Rows.Count > 0 && Convert.ToString(dt14.Rows[j]["visit_queue_no_show"]).Equals(QueueToken))
                                                {
                                                    smsview.MySms = QueueToken;
                                                    int pos = 0;
                                                    DataTable CheckMessage = smscontroller.GetAlertQMessageExistance(smsview);
                                                    if (CheckMessage.Rows.Count <= 0)
                                                    {


                                                        for (i = 0; i < dt13.Rows.Count; i++)
                                                        {
                                                            if (qtoken == QueueToken)
                                                            {


                                                                if (dt13 != null && dt13.Rows.Count > 0 && Convert.ToString(dt13.Rows[i]["visit_queue_no_show"]).Equals(qtoken))
                                                                {
                                                                    pos = j;
                                                                    if (pos == 2)
                                                                    {
                                                                        DataTable dt111 = new DataTable();
                                                                        dt111 = smscontroller.Getvisittnxidbyusingrepliedsms(smsview);
                                                                        foreach (DataRow dr111 in dt111.Rows)
                                                                        {

                                                                            smsview.QueueTransaction = (Convert.ToInt32(dr111["visit_tnx_id"].ToString()));

                                                                            DataTable dt157 = new DataTable();
                                                                            dt157 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                            foreach (DataRow dr1234 in dt157.Rows)
                                                                            {
                                                                                string Sflag3 = (dr1234["message_status_flag"].ToString());
                                                                                string uflag2 = Convert.ToString("A");
                                                                                if (Sflag3 == uflag2)
                                                                                {

                                                                                    DataTable dt1 = new DataTable();
                                                                                    smsview.MySms = qtoken;
                                                                                    dt1 = smscontroller.GetCustIDByUsingQueueno(smsview);
                                                                                    foreach (DataRow dr1 in dt1.Rows)
                                                                                    {
                                                                                        smsview.CustId = (Convert.ToInt64(dr1["visit_customer_id"].ToString()));
                                                                                        smsview.MenberId = (Convert.ToInt32(dr1["members_id"].ToString()));
                                                                                        DataTable dt2 = new DataTable();
                                                                                        dt2 = smscontroller.GetCustomerNameMobile(smsview);
                                                                                        foreach (DataRow dr2 in dt2.Rows)
                                                                                        {
                                                                                            string fname = (dr2["customer_firstname"].ToString());
                                                                                            string lname = (dr2["customer_lastname"].ToString());
                                                                                            string cname = fname + " " + lname;
                                                                                            string phonenumber = (dr2["members_mobile"].ToString());
                                                                                            // string phonenumber = phonenumber1;
                                                                                            string SmsStatusMsg = string.Empty;
                                                                                            string SmsDeliveryStatus = string.Empty;
                                                                                            if (phonenumber != "")
                                                                                            {

                                                                                                if (phonenumber.Length == 11)
                                                                                                {
                                                                                                    string strmsg = "Dear " + cname + ",\r\nYour ticket number is in 3rd position. If you are away, please return back to the waiting room.";
                                                                                                    //create Request
                                                                                                    //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(@"https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + phonenumber + "&text=" + strmsg1 + "");
                                                                                                    //oWeb.Credentials = new NetworkCredential("islhd-wolacckiosk@sesiahs.health.nsw.gov.au", "EqMs2015");

                                                                                                    //string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + phonenumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                                                                    //string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + phonenumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                                                                    //string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                                                                                    // bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");

                                                                                                    //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015"));

                                                                                                    //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                                                                    //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                                                                    // oWeb.Headers[HttpRequestHeader.Authorization] = "Basic" + credentials;
                                                                                                    //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + phonenumber + "&text=" + strmsg1 + "";

                                                                                                    #region Samsung SMS gateway
                                                                                                    //SMS for Samsung gateway
                                                                                                    // Set the username of the account holder.
                                                                                                    Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                                                                    // Set the password of the account holder.
                                                                                                    Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                                                                    // Set the first name of the account holder (optional).
                                                                                                    Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                                                                    // Set the last name of the account holder (optional).
                                                                                                    Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                                                                    // Set the mobile phone number of the account holder (optional).
                                                                                                    Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                                                                    // Set the landline phone number of the account holder (optional).
                                                                                                    Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                                                                    // Set the contact email of the account holder (optional).
                                                                                                    Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                                                                    // Set the country of origin of the account holder (optional).
                                                                                                    Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                                                                    bool testOK = false;
                                                                                                    try
                                                                                                    {
                                                                                                        // Test the user account settings.
                                                                                                        Account testAccount = Messaging.MessageController.UserAccount;
                                                                                                        testOK = Messaging.MessageController.TestAccount(testAccount);
                                                                                                    }
                                                                                                    catch (Exception ex)
                                                                                                    {
                                                                                                        // An exception was thrown. Display the details of the exception and return.
                                                                                                        string message = "There was an error testing the connection details:\n" +
                                                                                                        ex.Message;
                                                                                                        // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                                                        return;
                                                                                                    }
                                                                                                    if (testOK)
                                                                                                    {
                                                                                                        // The user account settings were valid. Display a success message
                                                                                                        // box with the number of credits.
                                                                                                        int balance = Messaging.MessageController.UserAccount.Balance;
                                                                                                        string message = string.Format("You have {0} message credits available.",
                                                                                                        balance);
                                                                                                        // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        // The username or password were incorrect. Display a failed message box.
                                                                                                        //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                                                        // "Connection Failed", MessageBoxButtons.OK);
                                                                                                    }

                                                                                                    Messaging.MessageController.Settings.TimeOut = 60;
                                                                                                    // Set the batch size (number of messages to be sent at once) to 200.
                                                                                                    Messaging.MessageController.Settings.BatchSize = 200;
                                                                                                    //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                                                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                                                    //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                                                    //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                                                    //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                                                                    Messaging.MessageController.Settings.DeliveryReport = true;
                                                                                                    SMSMessage smsobj = new SMSMessage(phonenumber, strmsg);
                                                                                                    Messaging.MessageController.AddToQueue(smsobj);
                                                                                                    Messaging.MessageController.SendMessages();
                                                                                                    //end of Samsung SMS
                                                                                                    #endregion Samsung SMS gateway
                                                                                                    //SmsStatusMsg = oWeb.DownloadString(URL);

                                                                                                    oWeb.Dispose();
                                                                                                    smsview.SMSStatusFlag = "S";
                                                                                                    smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                                                    if (SmsStatusMsg.Contains("<br>"))
                                                                                                    {
                                                                                                        SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                                                                    }

                                                                                                    // Thread.Sleep(100);
                                                                                                    DataTable dt15 = new DataTable();
                                                                                                    dt15 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                                                    foreach (DataRow dr123 in dt15.Rows)
                                                                                                    {
                                                                                                        string Sflag = (dr123["message_status_flag"].ToString());
                                                                                                        string uflag = Convert.ToString("A");
                                                                                                        if (Sflag == uflag)
                                                                                                        {
                                                                                                            smsview.SMSStatusFlag = "S";
                                                                                                            smsview.ButtonEventFlag = "N";
                                                                                                            smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                                                        }
                                                                                                        else
                                                                                                        {

                                                                                                            #region Delivery Report
                                                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                                                                            //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                                                                            //XmlDocument xml = new XmlDocument();
                                                                                                            //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                                            //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                                                                            //foreach (XmlNode xn in xnList)
                                                                                                            //{
                                                                                                            //    XmlNode example = xn.SelectSingleNode("sms");
                                                                                                            //    if (example != null)
                                                                                                            //    {
                                                                                                            //        string na = example["messageid"].InnerText;
                                                                                                            //        string no = example["smsclientid"].InnerText;
                                                                                                            //        string mobileno = example["mobile-no"].InnerText;
                                                                                                            //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                                                                                            //        SmsDeliveryStatus = client.DownloadString(URL1);
                                                                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                                                                            //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                                                                            //        XmlDocument xml1 = new XmlDocument();
                                                                                                            //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                                            //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                                                                                            //        //foreach (XmlNode xn1 in xnList1)
                                                                                                            //        //{
                                                                                                            //        XmlNode example1 = xml.SelectSingleNode("response");
                                                                                                            //        if (example1 != null)
                                                                                                            //        {
                                                                                                            //            //string rscode = example1["responsecode"].InnerText;
                                                                                                            //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                                                                            //            //string dlrcount = example1["dlristcount"].InnerText;
                                                                                                            //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                                                                                            //        }
                                                                                                            //    }

                                                                                                            //}
                                                                                                            #endregion Delivery report


                                                                                                            smsview.MySms = QueueToken;
                                                                                                            DataTable dt5 = new DataTable();
                                                                                                            dt5 = smscontroller.Getvisittnxidbyusingrepliedsms(smsview);
                                                                                                            foreach (DataRow dr5 in dt5.Rows)
                                                                                                            {
                                                                                                                smsview.QueueTransaction = (Convert.ToInt32(dr5["visit_tnx_id"].ToString()));
                                                                                                            }
                                                                                                            smsview.QueueNo = QueueToken;
                                                                                                            smsview.PhoneNo = phonenumber;
                                                                                                            smsview.IncomingsmsFlag = "A";
                                                                                                            smsview.SMSDateTime = System.DateTime.Now;
                                                                                                            smsview.MySms = strmsg;
                                                                                                            string a;
                                                                                                            a = smscontroller.GetInsertAlertSMS(smsview);

                                                                                                            // DataTable QueueTokenGenerationSentSMS = new DataTable();

                                                                                                            // Thread.Sleep(100);
                                                                                                        }

                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (phonenumber.Length == 9)
                                                                                                    {
                                                                                                        phonenumber = 61 + phonenumber;
                                                                                                        string strmsg = "Dear " + cname + ",\r\nYour ticket number is in 3rd position. If you are away, please return back to the waiting room.";

                                                                                                        //string URL = "http://sms.proactivesms.in/sendsms.jsp?user=attsystm&password=attsystm&mobiles=" + phonenumber + "&sms=" + strmsg1 + "&senderid=ATTIPL";
                                                                                                        //string URL = "https://api.aussiesms.com.au/?sendsms&mobileID=61422889101&password=att0424&to=" + phonenumber + "&text=" + strmsg1 + "&from=QSoft&msg_type=SMS_TEXT";
                                                                                                        //string URL = "http://www.smsglobal.com/http-api.php?action=sendsms&user=yahtz6o2&password=46yAfp0i&api=1&to=" + PhoneNumber + "&text=" + strmsg1 + "";
                                                                                                        //bytHeaders = System.Text.ASCIIEncoding.UTF8.GetBytes("islhd-wolacckiosk@sesiahs.health.nsw.gov.au" + ":" + "EqMs2015");
                                                                                                        //oWeb.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytHeaders));
                                                                                                        //oWeb.Headers.Add("Content-Type", "text/plain;charset=utf-8");
                                                                                                        //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(@"https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + phonenumber + "&text=" + strmsg1 + "");
                                                                                                        //oWeb.Credentials = new NetworkCredential("islhd-wolacckiosk@sesiahs.health.nsw.gov.au", "EqMs2015");

                                                                                                        //string URL = "https://tim.telstra.com/cgphttp/servlet/sendmsg?destination=" + phonenumber + "&text=" + strmsg1 + "";


                                                                                                        //SmsStatusMsg = oWeb.DownloadString(URL);
                                                                                                        #region Samsung SMS gateway
                                                                                                        //SMS for Samsung gateway
                                                                                                        // Set the username of the account holder.
                                                                                                        Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                                                                                                        // Set the password of the account holder.
                                                                                                        Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                                                                                                        // Set the first name of the account holder (optional).
                                                                                                        Messaging.MessageController.UserAccount.ContactFirstName = "David";
                                                                                                        // Set the last name of the account holder (optional).
                                                                                                        Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                                                                                                        // Set the mobile phone number of the account holder (optional).
                                                                                                        Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                                                                                                        // Set the landline phone number of the account holder (optional).
                                                                                                        Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                                                                                                        // Set the contact email of the account holder (optional).
                                                                                                        Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                                                                                                        // Set the country of origin of the account holder (optional).
                                                                                                        Messaging.MessageController.UserAccount.Country = Countries.Australia;
                                                                                                        bool testOK = false;
                                                                                                        try
                                                                                                        {
                                                                                                            // Test the user account settings.
                                                                                                            Account testAccount = Messaging.MessageController.UserAccount;
                                                                                                            testOK = Messaging.MessageController.TestAccount(testAccount);
                                                                                                        }
                                                                                                        catch (Exception ex)
                                                                                                        {
                                                                                                            // An exception was thrown. Display the details of the exception and return.
                                                                                                            string message = "There was an error testing the connection details:\n" +
                                                                                                            ex.Message;
                                                                                                            // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                                                                                            return;
                                                                                                        }
                                                                                                        if (testOK)
                                                                                                        {
                                                                                                            // The user account settings were valid. Display a success message
                                                                                                            // box with the number of credits.
                                                                                                            int balance = Messaging.MessageController.UserAccount.Balance;
                                                                                                            string message = string.Format("You have {0} message credits available.",
                                                                                                            balance);
                                                                                                            // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            // The username or password were incorrect. Display a failed message box.
                                                                                                            //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                                                                                            // "Connection Failed", MessageBoxButtons.OK);
                                                                                                        }

                                                                                                        Messaging.MessageController.Settings.TimeOut = 60;
                                                                                                        // Set the batch size (number of messages to be sent at once) to 200.
                                                                                                        Messaging.MessageController.Settings.BatchSize = 200;
                                                                                                        //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                                                                                                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                                                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                                                                                                        //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                                                                                                        //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                                                                                                        Messaging.MessageController.Settings.DeliveryReport = true;
                                                                                                        SMSMessage smsobj = new SMSMessage(phonenumber, strmsg);
                                                                                                        Messaging.MessageController.AddToQueue(smsobj);
                                                                                                        Messaging.MessageController.SendMessages();
                                                                                                        //end of Samsung SMS
                                                                                                        #endregion Samsung SMS gateway
                                                                                                        smsview.SMSStatusFlag = "S";
                                                                                                        smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                                                        if (SmsStatusMsg.Contains("<br>"))
                                                                                                        {
                                                                                                            SmsStatusMsg = SmsStatusMsg.Replace("<br>", ", ");
                                                                                                        }

                                                                                                        //Thread.Sleep(100);
                                                                                                        DataTable dt15 = new DataTable();
                                                                                                        dt15 = smscontroller.GetRetrieveSMSstatusFlag(smsview);

                                                                                                        foreach (DataRow dr123 in dt15.Rows)
                                                                                                        {
                                                                                                            string Sflag = (dr123["message_status_flag"].ToString());
                                                                                                            string uflag = Convert.ToString("A");
                                                                                                            if (Sflag == uflag)
                                                                                                            {
                                                                                                                smsview.SMSStatusFlag = "S";
                                                                                                                smscontroller.GetQueueTokenGenerationSentSMS(smsview);
                                                                                                            }
                                                                                                            else
                                                                                                            {

                                                                                                                #region Delivery Report
                                                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\r\n", "");
                                                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\t", "");
                                                                                                                //SmsStatusMsg = SmsStatusMsg.Replace("\n", "");
                                                                                                                //XmlDocument xml = new XmlDocument();
                                                                                                                //xml.LoadXml(SmsStatusMsg); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                                                //XmlNodeList xnList = xml.SelectNodes("smslist");
                                                                                                                //foreach (XmlNode xn in xnList)
                                                                                                                //{
                                                                                                                //    XmlNode example = xn.SelectSingleNode("sms");
                                                                                                                //    if (example != null)
                                                                                                                //    {
                                                                                                                //        string na = example["messageid"].InnerText;
                                                                                                                //        string no = example["smsclientid"].InnerText;
                                                                                                                //        string mobileno = example["mobile-no"].InnerText;
                                                                                                                //        string URL1 = "http://sms.proactivesms.in/getDLR.jsp?userid=attsystm&password=attsystm&messageid=" + na + "redownload=yes&responce type=xml";

                                                                                                                //        SmsDeliveryStatus = client.DownloadString(URL1);
                                                                                                                //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\r\n", "");
                                                                                                                //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\t", "");
                                                                                                                //        SmsDeliveryStatus = SmsDeliveryStatus.Replace("\n", "");
                                                                                                                //        XmlDocument xml1 = new XmlDocument();
                                                                                                                //        xml.LoadXml(SmsDeliveryStatus); //myXmlString is the xml file in string //copying xml to string: string myXmlString = xmldoc.OuterXml.ToString();
                                                                                                                //        //XmlNodeList xnList1 = xml.SelectNodes("response");

                                                                                                                //        //foreach (XmlNode xn1 in xnList1)
                                                                                                                //        //{
                                                                                                                //        XmlNode example1 = xml.SelectSingleNode("response");
                                                                                                                //        if (example1 != null)
                                                                                                                //        {
                                                                                                                //            //string rscode = example1["responsecode"].InnerText;
                                                                                                                //            smsview.DeliveryReport = example1["resposedescription"].InnerText;
                                                                                                                //            //string dlrcount = example1["dlristcount"].InnerText;
                                                                                                                //            //string pendingcount = example1["pendingdrcount"].InnerText;

                                                                                                                //        }
                                                                                                                //    }

                                                                                                                //}
                                                                                                                #endregion Delivery report


                                                                                                                smsview.MySms = QueueToken;
                                                                                                                DataTable dt5 = new DataTable();
                                                                                                                dt5 = smscontroller.Getvisittnxidbyusingrepliedsms(smsview);
                                                                                                                foreach (DataRow dr5 in dt5.Rows)
                                                                                                                {
                                                                                                                    smsview.QueueTransaction = (Convert.ToInt32(dr5["visit_tnx_id"].ToString()));
                                                                                                                }
                                                                                                                smsview.QueueNo = QueueToken;
                                                                                                                smsview.PhoneNo = phonenumber;
                                                                                                                smsview.IncomingsmsFlag = "A";
                                                                                                                smsview.SMSDateTime = System.DateTime.Now;
                                                                                                                smsview.MySms = strmsg;
                                                                                                                string a;
                                                                                                                a = smscontroller.GetInsertAlertSMS(smsview);

                                                                                                                // DataTable QueueTokenGenerationSentSMS = new DataTable();

                                                                                                                // Thread.Sleep(100);
                                                                                                            }

                                                                                                        }
                                                                                                    }
                                                                                                }

                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Send Alert SMS

        #region timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            AllMetheds();
        }
        #endregion timer

        public void appointmentalert()
        {
            try
            {
                DataTable Qtokendt = new DataTable();
                SMSController smscont = new SMSController();
                Qtokendt = smscont.appointmentalert();


                foreach (DataRow dr in Qtokendt.Rows)
                {
                    SMSView smsview = new SMSView();
                    // int appid = Convert.ToInt32(dr["appointment_id"].ToString());
                    // string adt = dr["appointment_time"].ToString();
                    // TimeSpan t1 = new TimeSpan(1, 0, 0);
                    // DateTime dtime1 = Convert.ToDateTime(adt);
                    // DateTime dtime2 = dtime1 - t1;              //appointment time adjustment for before one hour
                    // string apptime = dtime2.ToString("HH:mm:ss");
                    // string cmobn = dr["appointment_mobileno"].ToString();
                    // string cname = dr["customer_firstname"].ToString();
                    //// string cdt = DateTime.Now.ToString("HH:mm:ss");         //current system time
                    // string cdt = "::00";  //      Manual system time for testing
                    // if (apptime == cdt)
                    // {
                    int appid = Convert.ToInt32(dr["appointment_id"].ToString());
                    string appt = dr["appointment_time"].ToString();
                    DateTime apt = Convert.ToDateTime(appt);
                    string custaptime = apt.ToShortTimeString();
                    //string cmobn = dr["appointment_customer_id"].ToString();
                    string cname = dr["customer_firstname"].ToString();
                    string mobnum = dr["customer_mobile"].ToString();
                    //DateTime crrsysdate = DateTime.Now;
                    DateTime fdt = DateTime.Now;//Convert.ToDateTime("06:01");
                    string fd = fdt.ToString("HH:mm");
                    //DateTime onehour = Convert.ToDateTime("01:00");
                    string sfd = fdt.ToString("HH:mm");
                    //string sonehour = onehour.ToString("HH:MM");

                    string cappt = apt.ToString("HH:mm");
                    DateTime Result = apt.AddMinutes(-59);
                    //DateTime r=Convert.ToDateTime(Result);
                    string Resultdt = Result.ToString();
                    DateTime re = Convert.ToDateTime(Resultdt);
                    string r = re.ToString("HH:mm");
                    string cdt = fdt.ToString();//DateTime.Now.ToString();
                    if (r == fd)
                    {

                        string strmsg = "Dear " + cname + ",\r\n Your appointment with Samsung is an hour to go, If you are away, please return back to the waiting room.";
                        smsview.SmsDesc = strmsg;
                        #region Samsung SMS gateway
                        //SMS for Samsung gateway
                        // Set the username of the account holder.
                        Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                        // Set the password of the account holder.
                        Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                        // Set the first name of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactFirstName = "David";
                        // Set the last name of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                        // Set the mobile phone number of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                        // Set the landline phone number of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                        // Set the contact email of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                        // Set the country of origin of the account holder (optional).
                        Messaging.MessageController.UserAccount.Country = Countries.Australia;
                        bool testOK = false;

                        try
                        {
                            // Test the user account settings.
                            Account testAccount = Messaging.MessageController.UserAccount;
                            testOK = Messaging.MessageController.TestAccount(testAccount);
                        }
                        catch (Exception ex)
                        {
                            // An exception was thrown. Display the details of the exception and return.
                            string message = "There was an error testing the connection details:\n" +
                            ex.Message;
                            // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                            return;
                        }
                        if (testOK)
                        {
                            // The user account settings were valid. Display a success message
                            // box with the number of credits.
                            int balance = Messaging.MessageController.UserAccount.Balance;
                            string message = string.Format("You have {0} message credits available.",
                            balance);
                            // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                        }
                        else
                        {
                            // The username or password were incorrect. Display a failed message box.
                            //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                            // "Connection Failed", MessageBoxButtons.OK);
                        }

                        Messaging.MessageController.Settings.TimeOut = 60;
                        // Set the batch size (number of messages to be sent at once) to 200.
                        Messaging.MessageController.Settings.BatchSize = 200;
                        //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                        //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                        //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                        Messaging.MessageController.Settings.DeliveryReport = true;
                        SMSMessage smsobj = new SMSMessage(mobnum, strmsg);
                        Messaging.MessageController.AddToQueue(smsobj);
                        Messaging.MessageController.SendMessages();
                        //end of Samsung SMS

                        //smsview.SmsUpdatedDateTime = System.DateTime.Now;
                        //smsview.SmsActive = 'Y';
                        //smsview.SMSContentTypeId = 1;
                        //smsview.SmsAlert = 1;
                        //smsview.SmsUpdatedBy="Admin";
                        //string i;
                        //i = smscontroller.getInsertAppointmentAlertSms(smsview);
                        #endregion Samsung SMS gateway

                        #region Update SMS_Alert statsus flag

                        smsview.AppointmentID = appid;
                        smsview.SMSalert = 'B';
                        smscont.updatesmsalert(smsview);
                        #endregion Update SMS_Alert statsus flag

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region SMS Appointment Remender

        public void AppRemender()
        {
            try
            {
                DataTable Appdetails = new DataTable();
                SMSController smscont = new SMSController();
                Appdetails = smscont.AppRemender();

                DateTime re = Convert.ToDateTime("07:00");
                String ret = re.ToString("hh:mm");
                DateTime syst = DateTime.Now;
                string systime = syst.ToString("hh:mm");
                if (ret == systime)
                {
                    foreach (DataRow dr in Appdetails.Rows)
                    {
                        SMSView smsview = new SMSView();
                        int appid = Convert.ToInt32(dr["appointment_id"].ToString());
                        int custid = Convert.ToInt32(dr["appointment_customer_id"].ToString());
                        string cname = dr["customer_firstname"].ToString();
                        string appt = dr["appointment_time"].ToString();

                        DateTime apt = Convert.ToDateTime(appt);
                        string cappt = apt.ToString("HH:mm");
                        string custaptime = apt.ToShortTimeString();
                        string mobnum = dr["customer_mobile"].ToString();



                        //String systime="";


                        string strmsg = "Dear " + cname + ",\r\n your appointment with the Samsung Experience Store, Sydney is at " + cappt + " today.\r\nPlease bring a copy of your purchase invoice and back-up your data before your appointment to avoid data loss, Look forward to seeing you.";
                        //“Hi Kara, Reminder your appointment with the Samsung Experience Store, Sydney is at 11:15 today. Please bring a copy of your purchase invoice and back-up your data before your appointment to avoid data loss. Look forward to seeing you.”
                        smsview.SmsDesc = strmsg;
                        #region Samsung SMS gateway
                        //SMS for Samsung gateway
                        // Set the username of the account holder.
                        Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                        // Set the password of the account holder.
                        Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                        // Set the first name of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactFirstName = "David";
                        // Set the last name of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                        // Set the mobile phone number of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                        // Set the landline phone number of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                        // Set the contact email of the account holder (optional).
                        Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                        // Set the country of origin of the account holder (optional).
                        Messaging.MessageController.UserAccount.Country = Countries.Australia;
                        bool testOK = true;

                        try
                        {
                            // Test the user account settings.
                            Account testAccount = Messaging.MessageController.UserAccount;
                            testOK = Messaging.MessageController.TestAccount(testAccount);
                        }
                        catch (Exception ex)
                        {
                            // An exception was thrown. Display the details of the exception and return.
                            string message = "There was an error testing the connection details:\n" +
                            ex.Message;
                            // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                            return;
                        }
                        if (testOK)
                        {
                            // The user account settings were valid. Display a success message
                            // box with the number of credits.
                            int balance = Messaging.MessageController.UserAccount.Balance;
                            string message = string.Format("You have {0} message credits available.",
                            balance);
                            // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                        }
                        else
                        {
                            // The username or password were incorrect. Display a failed message box.
                            //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                            // "Connection Failed", MessageBoxButtons.OK);
                        }

                        Messaging.MessageController.Settings.TimeOut = 60;
                        // Set the batch size (number of messages to be sent at once) to 200.
                        Messaging.MessageController.Settings.BatchSize = 200;
                        //string strmsg = "To confirm an appointment with the Samsung Experience Store,\r\nyou will need 4 characters password. The password is  " + strrandom + "";
                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                        //string strmsg = "Hi " + " " + Cname + ", To finalize your appointment with the Samsung Experience Store  at Sydney Central Plaza,\r\nplease enter these 4 characters" + strrandom + "password on the Confirmation screen. Thank you";
                        //string strmsg = "Hi" + " " + Cname + ",Your ticket number is:" + QueueTokenGenerationSMS + " . Thanks";

                        //"Hi Kara, your ticket number is 040, Approximate waiting time is 00:40 minutes/hours”

                        Messaging.MessageController.Settings.DeliveryReport = true;
                        SMSMessage smsobj = new SMSMessage(mobnum, strmsg);
                        Messaging.MessageController.AddToQueue(smsobj);
                        Messaging.MessageController.SendMessages();
                        //end of Samsung SMS

                        //smsview.SmsUpdatedDateTime = System.DateTime.Now;
                        //smsview.SmsActive = 'Y';
                        //smsview.SMSContentTypeId = 1;
                        //smsview.SmsAlert = 1;
                        //smsview.SmsUpdatedBy="Admin";
                        //string i;
                        //i = smscontroller.getInsertAppointmentAlertSms(smsview);
                        #endregion Samsung SMS gateway

                        #region Update SMS_Alert statsus flag

                        smsview.AppointmentID = appid;
                        smsview.SMSalert = 'B';
                        smscont.updatesmsalert(smsview);
                        #endregion Update SMS_Alert statsus flag
                        
                        #region inserting to tbl_sms_tnx
                        smsview.CustId = custid;
                        smsview.SmsDesc = strmsg;
                        smsview.PhoneNo = mobnum;
                        smsview.DeliveryReport = "y";
                        smsview.SmsDesc = strmsg;
                        smsview.IncomingsmsFlag = "M";
                        smsview.SmsVisittnxId = 2;
                        smsview.SMSDateTime = System.DateTime.Now;
                        smsview.SMSStatusFlag = "M";
                        smsview.QueueNo = Convert.ToString("1");
                        smsview.CentreId = "";
                        smsview.SMSDateTime = System.DateTime.Now;
                        string i;
                        i = smscontroller.getInsertAppointmentAlertSms(smsview);
                       #endregion into tbl_sms_tnx
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion SMS Appointment Remender

        #region SMS Appointment alertexpire
        public void ExpiredAppointmentNotification()
        {
            SMSView smsview = new SMSView();
            try
            {
                DataTable appointment = new DataTable();
                SMSController sms = new SMSController();
                appointment = sms.appointmentexpired(smsview);

                foreach (DataRow dr in appointment.Rows)
                {
                    SMSView smsview1 = new SMSView();
                    string cname = dr["customer_firstname"].ToString();
                    int appid = Convert.ToInt32(dr["appointment_id"].ToString());
                    string app = dr["appointment_time"].ToString();
                    DateTime adt = Convert.ToDateTime(app);
                    string mobileno = dr["appointment_mobileno"].ToString();
                    long custid = Convert.ToInt64(dr["appointment_customer_id"].ToString());
                    DateTime syst = DateTime.Now;
                    string apptime = adt.ToString("HH:mm");
                    DateTime timeapp1 = Convert.ToDateTime(app);
                    TimeSpan t1 = new TimeSpan(01, 00, 00);
                    timeapp1 = timeapp1 + t1;
                    string timeapp = timeapp1.ToString("HH:mm");
                    //string systemtime = syst.ToString("HH:mm");
                    string systemtime = "20:20";
                    //DateTime re = Convert.ToDateTime("10:31");
                    //String ret = re.ToString("HH:mm");
                    DataTable dtsmsview = new DataTable();
                    dtsmsview = smscontroller.appointmenTranstexpired(smsview1);
                    if (dtsmsview.Rows.Count == 0)
                    {
                        if (timeapp == systemtime)
                        {
                            string strmsg = "Dear " + cname + ",\r\n  your appointment for " + apptime + " has been Expired, please contact 18xxxxxx to reschedule";
                            smsview.SmsDesc = strmsg;
                            #region Samsung SMS gateway
                            //SMS for Samsung gateway
                            // Set the username of the account holder.
                            Messaging.MessageController.UserAccount.User = "SAMSUNGELECTR213";
                            // Set the password of the account holder.
                            Messaging.MessageController.UserAccount.Password = "y2M28DQD";
                            // Set the first name of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactFirstName = "David";
                            // Set the last name of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactLastName = "Smith";
                            // Set the mobile phone number of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactPhone = "0423612367";
                            // Set the landline phone number of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactLandLine = "0338901234";
                            // Set the contact email of the account holder (optional).
                            Messaging.MessageController.UserAccount.ContactEmail = "david.smith@email.com";
                            // Set the country of origin of the account holder (optional).
                            Messaging.MessageController.UserAccount.Country = Countries.Australia;
                            bool testOK = false;

                            try
                            {
                                // Test the user account settings.
                                Account testAccount = Messaging.MessageController.UserAccount;
                                testOK = Messaging.MessageController.TestAccount(testAccount);
                            }
                            catch (Exception ex)
                            {
                                // An exception was thrown. Display the details of the exception and return.
                                string message = "There was an error testing the connection details:\n" +
                                ex.Message;
                                // MessageBox.Show(this, message, "Connection Failed", MessageBoxButtons.OK);
                                return;
                            }
                            if (testOK)
                            {
                                // The user account settings were valid. Display a success message
                                // box with the number of credits.
                                int balance = Messaging.MessageController.UserAccount.Balance;
                                string message = string.Format("You have {0} message credits available.",
                                balance);
                                // MessageBox.Show(this, message, "Connection Succeeded", MessageBoxButtons.OK);
                            }
                            else
                            {
                                // The username or password were incorrect. Display a failed message box.
                                //  MessageBox.Show(this, "The username or password you entered were incorrect.",
                                // "Connection Failed", MessageBoxButtons.OK);
                            }

                            Messaging.MessageController.Settings.TimeOut = 5;
                            // Set the batch size (number of messages to be sent at once) to 200.
                            Messaging.MessageController.Settings.BatchSize = 200;
                            Messaging.MessageController.Settings.DeliveryReport = true;
                            SMSMessage smsobj = new SMSMessage(mobileno, strmsg);
                            Messaging.MessageController.AddToQueue(smsobj);
                            Messaging.MessageController.SendMessages();
                            #endregion Samsung SMS gateway

                            #region Update SMS_Alert statsus flag

                            smsview.AppointmentID = appid;
                            smsview.SMSalert = 'E';
                            sms.updatesmsalert(smsview);
                            #endregion Update SMS_Alert statsus flag

                            //#region inserting to tbl_sms_content_mst
                            //smsview.CustId = CustId;
                            //smsview.SmsDesc = strmsg;
                            ////smsview.QueueNo = MissedQueueNo;
                            ////smsview.PnoneNo = MissedPhoneNo;
                            //smsview.IncomingsmsFlag = "M";
                            //smsview.SMSDateTime = System.DateTime.Now;
                            //smsview.SmsUpdatedDateTime = System.DateTime.Now;
                            //smsview.SmsActive = 'Y';
                            //smsview.SMSContentTypeId = 2;
                            //smsview.SmsAlert = 2;
                            //smsview.SmsUpdatedBy = "Admin";
                            //string i;
                            //i = smscontroller.getInsertAppointmentAlertSms(smsview);

                            //#endregion into tbl_sms_content_mst
                            #region inserting to tbl_sms_tnx
                            smsview.CustId = custid;
                            smsview.SmsDesc = strmsg;
                            smsview.PhoneNo = mobileno;
                            smsview.DeliveryReport = "y";
                            smsview.SmsDesc = strmsg;
                            smsview.IncomingsmsFlag = "M";
                            smsview.SmsVisittnxId = 2;
                            smsview.SMSDateTime = System.DateTime.Now;
                            smsview.SMSStatusFlag = "M";
                            smsview.QueueNo = Convert.ToString("1");
                            smsview.CentreId = "";
                            smsview.SMSDateTime = System.DateTime.Now;
                            string i;
                            i = smscontroller.getInsertAppointmentAlertSms(smsview);
                            #endregion into tbl_sms_tnx
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion SMS Appointmentexpired

    }
}
