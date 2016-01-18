using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;

namespace eQMSMessage_FormApp
{
    class SMSDAO
    {
        // SMS - Intiallizing Variables

        #region SMS - Intiallizing Variables

        string ConnectionString;

        SMSView smsview = new SMSView();

        SqlCommand cmd = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataAdapter sqlad = new SqlDataAdapter();

        #endregion SMS - Intiallizing Variables

        // SMS - Constructor

        #region SMS - Constructor

        public SMSDAO()
        {
            // ConnectionString = eQMSMessage_FormApp.Properties.Resources.ConnectionString;
            String[] lines = System.IO.File.ReadAllLines(@"C:\QMS\Display Config1.txt");
            foreach (String line in lines)
            {
                ConnectionString = line;
            }

        }

        #endregion SMS - Constructor

        // SMS - Set SMS Search Row

        #region SMS - Set SMS Search Row

        public DataTable GetDaoSetSMSSearchRow(SMSView smsview)
        {
            DataTable MySearchRow = new DataTable();
            DataSet dstsr = new DataSet();

            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("select q.queue_tnx_id,q.queue_department_id from tbl_customervisit_tnx c,tbl_queue_tnx q where c.visit_tnx_id=q.queu_visit_tnxid and q.queue_status_id=@QueueStatusID and cast(q.queue_datetime as date) = cast(getdate() as date) and c.visit_queue_no=@QueueNo", MySqlConnection);

                MySqlCommand.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
                MySqlCommand.Parameters.AddWithValue("@QueueStatusID", smsview.QueueStatusID);

                //SqlCommand MySqlCommand = new SqlCommand("select q.queu_visit_tnxid,v.visit_queue_no,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(queue_datetime as date) = cast(getdate() as date) and queue_status_id=1 and sms_status_flag='N'", MySqlConnection);

                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

                MySqlConnection.Open();

                //SqlDataReader dr = MySqlCommand.ExecuteReader();

                //SqlDataReader dr = MySqlCommand.ExecuteReader();

                //DataTable MyStatusQueueNO = new DataTable();

                //MyStatusQueueNO.Load(dr);


                //MySearchRow.Load(dr);

                //sqlad.SelectCommand = cmd;
                MySqlCommand.ExecuteNonQuery();

                sqlad.SelectCommand = MySqlCommand;
                sqlad.Fill(dstsr);
                MySearchRow = dstsr.Tables[0];

                //MySqlConnection.Close();

                return MySearchRow;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Set SMS Search Row

        // SMS - Set SMS Status R

        #region SMS - Set SMS Status R

        public DataTable GetDaoSetSMSStatusR(SMSView smsview)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string sql = "update tbl_queue_tnx set sms_status_flag=@smsstatusflag where queue_tnx_id=@queuetnxid and queue_department_id=@departmentid";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@smsstatusflag", smsview.SMSStatusFlag);
            cmd.Parameters.AddWithValue("@queuetnxid", smsview.QueueTransaction);
            cmd.Parameters.AddWithValue("@departmentid", smsview.DepartmentID);

            SqlDataReader dr = cmd.ExecuteReader();

            DataTable MyAutoSMS = new DataTable();

            MyAutoSMS.Load(dr);
            con.Close();
            return MyAutoSMS;

        }

        #endregion SMS - Set SMS Status R

        // SMS - Reply SMS Total Waiting Queue

        #region SMS - Reply SMS Total Waiting Queue


        public DataTable GetDaoTotalWaitingQueue(SMSView smsview)
        {

            try
            {
                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("select q.queue_tnx_id,v.visit_queue_no,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and v.visit_customer_id=c.customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and v.visit_queue_no<@queueno and q.queue_department_id=@departmentid and q.queue_Status_id=1", MySqlConnection);


                MySqlCommand.Parameters.AddWithValue("@queueno", smsview.QueueNo);
                MySqlCommand.Parameters.AddWithValue("@departmentid", smsview.DepartmentID);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();
                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }

        }

        #endregion SMS - Reply SMS Total Waiting Queue

        // SMS - Reply SMS Total Waiting Missed Queue

        #region SMS - Reply SMS Total Waiting Missed Queue


        public DataTable GetDaoTotalWaitingMissedQueue(SMSView smsview)
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("select q.queue_tnx_id,v.visit_queue_no,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and v.visit_customer_id=c.customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and v.visit_queue_no<@queueno and q.queue_department_id=@departmentid and q.queue_Status_id=1", MySqlConnection);
                //SqlCommand MySqlCommand = new SqlCommand("select q.queu_visit_tnxid,v.visit_queue_no,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(queue_datetime as date) = cast(getdate() as date) and queue_status_id=1 and sms_status_flag='N'", MySqlConnection);

                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

                MySqlCommand.Parameters.AddWithValue("@queueno", smsview.QueueNo);
                MySqlCommand.Parameters.AddWithValue("@departmentid", smsview.DepartmentID);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Reply SMS Total Waiting Missed Queue

        // SMS - Queue Generation SMS

        #region SMS - Queue Generation SMS

        public DataTable GetDaoQueueGenerationSMS()
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                // SqlCommand MySqlCommand = new SqlCommand("select q.queu_visit_tnxid,v.visit_queue_no_show,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                //SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queue_visit_tnxid,v.visit_queue_no_show,tc.members_customer_id,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queue_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);

               // SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queue_visit_tnxid,v.visit_queue_no_show,tc.members_customer_id,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queue_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Queue Generation SMS

        // SMS - Queue Token Generation Sent SMS
        #region SMS - Get Appointment Alert

        public DataTable GetDaoappointmentalert()
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
                //string sql = "select a.appointment_mobileno, c.customer_firstname,a.appointment_time from tbl_appointment_tnx a, tbl_customerreg_mst c where c.customer_id=a.appointment_customer_id and CONVERT(date,a.appointment_time)=CONVERT(date,GETDATE())";
                // SqlCommand MySqlCommand = new SqlCommand("select q.queu_visit_tnxid,v.visit_queue_no_show,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                //SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
               // SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.message_status_flag='N'", MySqlConnection);
                //SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection); 
               // SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queue_visit_tnxid,v.visit_queue_no_show,tc.members_customer_id,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queue_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);
                //By Beeta
                //SqlCommand MySqlCommand = new SqlCommand("select appointment_customer_id,customer_mobile, appointment_time,customer_firstname from tbl_appointment_tnx,tbl_customerreg_mst where customer_id=appointment_customer_id and CONVERT(date,appointment_time)=CONVERT(date,GETDATE())", MySqlConnection); 
                SqlCommand MySqlCommand = new SqlCommand("select appointment_id,appointment_customer_id,customer_mobile,appointment_time,customer_firstname,sms_alert,appointment_centre_id from tbl_appointment_tnx,tbl_customerreg_mst where customer_id=appointment_customer_id and CONVERT(date,appointment_time)=CONVERT(date,GETDATE()) and sms_alert='A'", MySqlConnection);
                

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Get Appointment Alert

        // SMS - Rememder SMS
        #region SMS - Send Appointment Remender

        public DataTable AppRemender()
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
                SqlCommand MySqlCommand = new SqlCommand("select appointment_id,appointment_customer_id,customer_mobile,appointment_time,customer_firstname,appointment_centre_id from tbl_appointment_tnx,tbl_customerreg_mst where customer_id=appointment_customer_id and CONVERT(date,appointment_time)=CONVERT(date,GETDATE())", MySqlConnection);


                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Send Appointment Remender

        #region SMS - Queue Token Generation Sent SMS

        public DataTable GetDaoQueueTokenGenerationSentSMS(SMSView smsview)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string sql = "update tbl_queue_tnx set message_status_flag=@smsstatusflag where queu_visit_tnxid=@queuevisittnxid";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@smsstatusflag", smsview.SMSStatusFlag);
            cmd.Parameters.AddWithValue("@queuevisittnxid", smsview.QueueTransaction);

            SqlDataReader dr = cmd.ExecuteReader();

            DataTable MyMissedQueueNO = new DataTable();

            MyMissedQueueNO.Load(dr);
            con.Close();
            return MyMissedQueueNO;
        }

        #endregion SMS - Queue Token Generation Sent SMS

        // SMS - Missed Queue Sending SMS

        #region retrieve status flag

        public DataTable RetrieveSMSstatusFlag(SMSView smsview)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string sql = "select message_status_flag from tbl_queue_tnx where queu_visit_tnxid=@queuevisittnxid";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@queuevisittnxid", smsview.QueueTransaction);

            SqlDataReader dr = cmd.ExecuteReader();

            DataTable MyMissedQueueNO = new DataTable();

            MyMissedQueueNO.Load(dr);
            con.Close();
            return MyMissedQueueNO;

        }

        #endregion retrieve status flag

        #region SMS - Missed Queue Sending SMS


        public DataTable GetDaoMissedQueueSendingSMS()
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
                string sql = "select DISTINCT q.queue_visit_tnxid,v.visit_queue_no_show,v.visit_customer_name,v.customer_appointment_time,c.customer_id,c.customer_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c, tbl_department_mst d  where q.queue_department_id=d.department_id and v.visit_tnx_id=q.queue_visit_tnxid and c.customer_id=v.visit_customer_id and CONVERT(date,q.queue_datetime)=CONVERT(date,GETDATE())  and queue_status_id=4 and sms_status_flag='M'";
                // SqlCommand MySqlCommand = new SqlCommand("select q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile,c.customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(queue_datetime as date) = cast(getdate() as date) and queue_status_id=4 and sms_status_flag='M'", MySqlConnection);
               // SqlCommand MySqlCommand = new SqlCommand("select q.queue_visit_tnxid,v.visit_queue_no_show,tc.members_customer_id,c.customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queue_visit_tnxid and c.customer_id=v.visit_customer_id and cast(queue_datetime as date) = cast(getdate() as date) and queue_status_id=4 and sms_status_flag='M'", MySqlConnection);
                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);
                SqlCommand MySqlCommand = new SqlCommand(sql,MySqlConnection);
                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Missed Queue Sending SMS

        // SMS - Missed Queue Sent SMS

        #region SMS - Missed Queue Sent SMS

        public DataTable GetDaoMissedQueueSentSMS(SMSView smsview)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string sql = "update tbl_queue_tnx set sms_status_flag=@smsstatusflag where queue_tnx_id=@queuetnxid";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@smsstatusflag", smsview.SMSStatusFlag);
            cmd.Parameters.AddWithValue("@queuetnxid", smsview.QueueTransaction);

            SqlDataReader dr = cmd.ExecuteReader();

            DataTable MyMissedQueueNO = new DataTable();

            MyMissedQueueNO.Load(dr);
            con.Close();
            return MyMissedQueueNO;
        }

        #endregion SMS - Missed Queue Sent SMS

        // SMS - Total Waiting Queue

        #region SMS - TotalWaitingQueue
        public DataTable TotalWaitingQueue(int deptid)
        {
            DataTable TotalWaitingQueue = new DataTable();
            DataSet dstwq = new DataSet();
            try
            {
                con = new SqlConnection(ConnectionString);
                con.Open();
                string sql = "select c.visit_tnx_id as visit_tnx,c.visit_queue_no as queue_no from tbl_queue_tnx q,tbl_customervisit_tnx c where q.queue_department_id = @deptid and q.queu_visit_tnxid = c.visit_tnx_id and CONVERT(DATE, q.queue_datetime) = CONVERT(DATE, GETDATE())and q.queue_status_id = 1";

                cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@deptid", deptid);

                sqlad.SelectCommand = cmd;
                cmd.ExecuteNonQuery();

                sqlad.SelectCommand = cmd;
                sqlad.Fill(dstwq);
                TotalWaitingQueue = dstwq.Tables[0];

                con.Close();
                return TotalWaitingQueue;
            }
            catch
            {
                return TotalWaitingQueue;
            }
            finally
            {
                con.Close();
                //sqlrd.Close();
                cmd.Cancel();
            }
        }
        #endregion SMS - Total Waiting Queue

        // SMS - Missed Queue

        #region Queue Display - Missed Queue

        public DataTable DaoGetMissedQueue()
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            SqlCommand MySqlCommand = new SqlCommand("select v.visit_queue_no_show from tbl_queue_tnx q,tbl_customervisit_tnx v,tbl_terminal_mst t where q.queue_department_id=@department_id and q.queue_status_id=4 and cast(queue_serv_starttime as date) = cast(getdate() as date) and q.queu_visit_tnxid=v.visit_tnx_id and q.queue_terminal_id=t.terminal_id order by q.queue_serv_starttime desc", MySqlConnection);
            //select v.visit_queue_no_show from tbl_queue_tnx q,tbl_customervisit_tnx v,tbl_terminal_mst t where q.queue_department_id=2 and q.queue_status_id=4 and q.queu_visit_tnxid=v.visit_tnx_id and q.queue_terminal_id=t.terminal_id order by q.queue_serv_starttime desc
            //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

            MySqlConnection.Open();

            SqlDataReader dr = MySqlCommand.ExecuteReader();

            DataTable MyQueueNO = new DataTable();

            MyQueueNO.Load(dr);

            MySqlConnection.Close();

            return MyQueueNO;

        }
        #endregion Queue Display - MissedQueue

        // SMS - Auto Queue Sending SMS

        #region SMS - Auto Queue Sending SMS


        public DataTable GetDaoAutoQueueSendingSMS()
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("select q.queue_tnx_id,v.visit_queue_no_show,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(queue_datetime as date) = cast(getdate() as date) and queue_status_id=1 and sms_status_flag='A'", MySqlConnection);

                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Auto Queue Sending SMS

        // SMS - Missed Queue Sent SMS

        #region SMS - Missed Queue Sent SMS

        public DataTable GetDaoAutoQueueSentSMS(SMSView smsview)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string sql = "update tbl_queue_tnx set sms_status_flag=@smsstatusflag where queue_tnx_id=@queuetnxid";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@smsstatusflag", smsview.SMSStatusFlag);
            cmd.Parameters.AddWithValue("@queuetnxid", smsview.QueueTransaction);

            SqlDataReader dr = cmd.ExecuteReader();

            DataTable MyAutoSMS = new DataTable();

            MyAutoSMS.Load(dr);
            con.Close();
            return MyAutoSMS;

        }

        #endregion SMS - Missed Queue Sent SMS

        #region incoming SMS

        public DataTable searchIncomingSMS(SMSView smsview)
        {
            string dattime = System.DateTime.Now.ToString();
            //insert into tbl_sms_mst(phone_number,incomingsms,incoming_sms_datetime)values(9002476871,'Test Message','2015-04-17 00:00:00')
            // select incomingsms from tbl_sms_mst where CONVERT(DATE, incoming_sms_datetime) = CONVERT(DATE, GETDATE())
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("select sms_content,sms_phoneno from tbl_sms_tnx where CONVERT(DATE, sms_datetime) = CONVERT(DATE, GETDATE()) and sms_status_flag='I'", MySqlConnection);

                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MySms = new DataTable();

                MySms.Load(dr);

                MySqlConnection.Close();

                return MySms;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }

        }
        #endregion incoming SMS

        #region retrieve SMS status

        public DataTable serachQueueStatus(SMSView smsview)
        {
            string dattime = System.DateTime.Now.ToString();
            //insert into tbl_sms_mst(phone_number,incomingsms,incoming_sms_datetime)values(9002476871,'Test Message','2015-04-17 00:00:00')
            // select incomingsms from tbl_sms_mst where CONVERT(DATE, incoming_sms_datetime) = CONVERT(DATE, GETDATE())
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("select DISTINCT tq.queue_status_id from tbl_queue_tnx tq, tbl_sms_tnx stx,tbl_customervisit_tnx ctv where stx.sms_content=ctv.visit_queue_no_show COLLATE SQL_Latin1_General_CP1_CI_AI and CONVERT(DATE, tq.queue_datetime) = CONVERT(DATE, GETDATE()) and stx.sms_status_flag='I'", MySqlConnection);

                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQSMSstatus = new DataTable();

                MyQSMSstatus.Load(dr);

                MySqlConnection.Close();

                return MyQSMSstatus;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
        }

        #endregion retrieve SMS status

        #region incoming sms status update

        public DataTable updateincomingsms(SMSView smsview)
        {
            try
            {
                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

                SqlCommand MySqlCommand = new SqlCommand("update tbl_incomingsms_mst set status_flag=@IncomingsmsStatus where incomingsms=@MySms and CONVERT(DATE, incoming_sms_datetime) = CONVERT(DATE, GETDATE())", MySqlConnection);

                MySqlCommand.Parameters.AddWithValue("@IncomingsmsStatus", smsview.InsmsStatus);
                MySqlCommand.Parameters.AddWithValue("@MySms", smsview.MySms);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQSMSstatus = new DataTable();

                MyQSMSstatus.Load(dr);

                MySqlConnection.Close();

                return MyQSMSstatus;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
        }

        #endregion incoming sms status update

        #region queue position retrieve

        public DataTable selectQueuePosition(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            try
            {
                MySqlConnection.Open();
                // string sql = "select DISTINCT q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='A'";
                // string sql = "select DISTINCT v.visit_queue_no_show from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and sms_status_flag='N'";
                // string sql = "select DISTINCT q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.message_status_flag='A'";
                //string sql = "select q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c,tbl_appointment_tnx a where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and a.appointment_id=v.customer_appointment_id and q.queue_status_id=1 and q.message_status_flag='A' order by v.consulting_status ASC, v.customer_appointment_time ASC";
                string sql = "select c.visit_tnx_id,c.visit_queue_no,c.visit_queue_no_show  from tbl_queue_tnx q,tbl_customervisit_tnx c where q.queu_visit_tnxid = c.visit_tnx_id and CONVERT(DATE, q.queue_datetime) = CONVERT(DATE, GETDATE())and q.queue_status_id = 1 and message_status_flag='A' order by c.consulting_status ASC,c.customer_appointment_time ASC";
                SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
                // MySqlCommand.Parameters.AddWithValue("@Departmentid",smsview.DepartmentID);
                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQSMSstatus = new DataTable();

                MyQSMSstatus.Load(dr);

                MySqlConnection.Close();

                return MyQSMSstatus;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", ex);

            }
        }
        #endregion queue position retrieve

        #region queue position retrieve123

        public DataTable selectQueuePosition123(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            try
            {
                MySqlConnection.Open();
                string sql = "select DISTINCT td.department_desc, q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_department_mst td, tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.queue_department_id=@Departmentid and td.department_id=@Departmentid";
                // string sql = "select DISTINCT v.visit_queue_no_show from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and sms_status_flag='N'";

                SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
                MySqlCommand.Parameters.AddWithValue("@Departmentid", smsview.DepartmentID);
                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQSMSstatus = new DataTable();

                MyQSMSstatus.Load(dr);

                MySqlConnection.Close();

                return MyQSMSstatus;
            }
            catch (Exception ex)
            {
                MySqlConnection.Close();
                throw new Exception("Error Occured While Retrieving Data From DataBase", ex);

            }
            finally
            {
                MySqlConnection.Close();
            }
        }
        #endregion queue position retrieve

        #region position retrieve by using Queueno
        public DataTable positionretrievebyusingQueueno(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            try
            {
                MySqlConnection.Open();
                // string sql = "select q.queu_visit_tnxid,v.visit_queue_no_show,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N' and queue_department_id=@Departmentid";
                // string sql = "select v.visit_queue_no_show from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and queue_department_id=@Departmentid";
                string sql = "select tq.queue_department_id from tbl_customervisit_tnx cv,tbl_queue_tnx tq where cv.visit_tnx_id=tq.queu_visit_tnxid and cv.visit_queue_no_show=@MySms and CONVERT(DATE, tq.queue_datetime) = CONVERT(DATE, GETDATE())";
                SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
                MySqlCommand.Parameters.AddWithValue("@MySms", smsview.MySms);
                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQSMSstatus = new DataTable();

                MyQSMSstatus.Load(dr);

                MySqlConnection.Close();

                return MyQSMSstatus;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", ex);

            }
        }
        #endregion position retrieve by using Queueno

        #region retrieve name
        public DataTable retrieveNameByCustID(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select DISTINCT members_firstname,members_lastname,cd.members_mobile,cd.members_email from tbl_customer_dtl cd,tbl_customervisit_tnx tcv,tbl_customerreg_mst cr where cd.members_id=@memberid and cd.members_id=tcv.visit_member_id and members_customer_id=@CustID  and cr.customer_id=tcv.visit_customer_id";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@CustID", smsview.CustId);
            MySqlCommand.Parameters.AddWithValue("@memberid", smsview.MenberId);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion retrieve name

        #region insert new sms
        public string InsertNewSMS(SMSView smsview)
        {
            SqlConnection MysqlConnection = new SqlConnection(ConnectionString);
            try
            {

                string sql = "if not exists(select * from tbl_sms_tnx where sms_cust_id=@sms_cust_id and sms_phoneno=@sms_phoneno and sms_queueno=@QueueNo and sms_content=@sms_content and sms_status_flag=@sms_status_flag  )insert into tbl_sms_tnx(sms_visit_tnxid,sms_cust_id, sms_datetime,sms_phoneno,sms_status_flag,sms_content,sms_queueno)values(@sms_visit_tnxid,@sms_cust_id,@sms_datetime,@sms_phoneno,@sms_status_flag,@sms_content,@QueueNo)";
                cmd = new SqlCommand(sql, MysqlConnection);
                cmd.Parameters.AddWithValue("@sms_visit_tnxid", smsview.QueueTransaction);
                cmd.Parameters.AddWithValue("@sms_cust_id", smsview.CustId);
                cmd.Parameters.AddWithValue("@sms_datetime", smsview.SMSDateTime);
                cmd.Parameters.AddWithValue("@sms_phoneno", smsview.PhoneNo);
                cmd.Parameters.AddWithValue("@sms_status_flag", smsview.IncomingsmsFlag);
                cmd.Parameters.AddWithValue("@sms_content", smsview.MySms);
                cmd.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
                // cmd.Parameters.AddWithValue("@sms_delivery_status", smsview.DeliveryReport);
                // DataTable dt3 = new DataTable();
                MysqlConnection.Open();
                int i = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return Convert.ToString(i);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion insert new sms

        #region insert Alert sms
        public string InsertAlertSMS(SMSView smsview)
        {
            SqlConnection MysqlConnection = new SqlConnection(ConnectionString);
            try
            {

                string sql = "if not exists(select * from tbl_sms_tnx where sms_cust_id=@sms_cust_id and sms_phoneno=@sms_phoneno and sms_queueno=@QueueNo and sms_content=@sms_content and sms_status_flag=@sms_status_flag  )insert into tbl_sms_tnx(sms_visit_tnxid,sms_cust_id, sms_datetime,sms_phoneno,sms_status_flag,sms_content,sms_queueno)values(@sms_visit_tnxid,@sms_cust_id,@sms_datetime,@sms_phoneno,@sms_status_flag,@sms_content,@QueueNo)";
                cmd = new SqlCommand(sql, MysqlConnection);
                cmd.Parameters.AddWithValue("@sms_visit_tnxid", smsview.QueueTransaction);
                cmd.Parameters.AddWithValue("@sms_cust_id", smsview.CustId);
                cmd.Parameters.AddWithValue("@sms_datetime", smsview.SMSDateTime);
                cmd.Parameters.AddWithValue("@sms_phoneno", smsview.PhoneNo);
                cmd.Parameters.AddWithValue("@sms_status_flag", smsview.IncomingsmsFlag);
                cmd.Parameters.AddWithValue("@sms_content", smsview.MySms);
                cmd.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
                // cmd.Parameters.AddWithValue("@sms_delivery_status", smsview.DeliveryReport);
                // DataTable dt3 = new DataTable();
                MysqlConnection.Open();
                int i = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return Convert.ToString(i);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion insert Alert sms

        #region insert MissedQ sms
        public string InsertMissedQSMS(SMSView smsview)
        {
            SqlConnection MysqlConnection = new SqlConnection(ConnectionString);
            try
            {

                string sql = "if not exists(select * from tbl_sms_tnx where sms_cust_id=@sms_cust_id and sms_phoneno=@sms_phoneno and sms_queueno=@QueueNo and sms_content=@sms_content and sms_status_flag=@sms_status_flag  )insert into tbl_sms_tnx(sms_visit_tnxid,sms_cust_id, sms_datetime,sms_phoneno,sms_status_flag,sms_content,sms_queueno)values(@sms_visit_tnxid,@sms_cust_id,@sms_datetime,@sms_phoneno,@sms_status_flag,@sms_content,@QueueNo)";
                cmd = new SqlCommand(sql, MysqlConnection);
                cmd.Parameters.AddWithValue("@sms_visit_tnxid", smsview.QueueTransaction);
                cmd.Parameters.AddWithValue("@sms_cust_id", smsview.CustId);
                cmd.Parameters.AddWithValue("@sms_datetime", smsview.SMSDateTime);
                cmd.Parameters.AddWithValue("@sms_phoneno", smsview.PhoneNo);
                cmd.Parameters.AddWithValue("@sms_status_flag", smsview.IncomingsmsFlag);
                cmd.Parameters.AddWithValue("@sms_content", smsview.MySms);
                cmd.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
                // cmd.Parameters.AddWithValue("@sms_delivery_status", smsview.DeliveryReport);
                // DataTable dt3 = new DataTable();
                MysqlConnection.Open();
                int i = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return Convert.ToString(i);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion insert MissedQ sms

        #region retrieve visit tnxid by using replied sms
        public DataTable retrievevisittnxidbyusingrepliedsms(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select visit_tnx_id from tbl_customervisit_tnx where visit_queue_no_show=@InSMS and CONVERT(DATE, visit_datetime) = CONVERT(DATE, GETDATE())";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@InSMS", smsview.MySms);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion  retrieve visit tnxid by using replied sms

        #region Retrieve Department id
        public DataTable selectDeptId(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select queue_department_id from tbl_queue_tnx where queu_visit_tnxid=@TnxId";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@TnxId", smsview.QueueTransaction);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyDeptid = new DataTable();
            MyDeptid.Load(dr);
            MySqlConnection.Close();
            return MyDeptid;
        }
        #endregion Retrieve Department id

        #region SelectQueueStatus
        public DataTable SelectQueueStatus(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            // string sql = "select ctv.visit_queue_no_show from tbl_queue_tnx tq,tbl_customervisit_tnx ctv where tq.queu_visit_tnxid=ctv.visit_tnx_id and queue_department_id=@DeptId and queue_status_id=1 and CONVERT(DATE, queue_datetime) = CONVERT(DATE, GETDATE()) and message_status_flag='A'";
            //string sql = "select ctv.visit_queue_no_show from tbl_queue_tnx tq,tbl_customervisit_tnx ctv where tq.queu_visit_tnxid=ctv.visit_tnx_id and queue_department_id=@DeptId and queue_status_id=1 and CONVERT(DATE, queue_datetime) = CONVERT(DATE, GETDATE()) and message_status_flag='A' order by ctv.consulting_status ASC, ctv.customer_appointment_time asc";
           // string sql = "select c.visit_tnx_id,c.visit_queue_no,c.visit_queue_no_show from tbl_queue_tnx q,tbl_customervisit_tnx c where q.queue_department_id = @DeptId and message_status_flag='A' and q.queu_visit_tnxid = c.visit_tnx_id and CONVERT(DATE, q.queue_datetime) = CONVERT(DATE, GETDATE())and q.queue_status_id = 1 order by c.consulting_status ASC,c.customer_appointment_time ASC";
            string sql = "select c.visit_tnx_id,c.visit_queue_no,c.visit_queue_no_show  from tbl_queue_tnx q,tbl_customervisit_tnx c where q.queue_department_id = @DeptId and q.queu_visit_tnxid = c.visit_tnx_id and CONVERT(DATE, q.queue_datetime) = CONVERT(DATE, GETDATE())and q.queue_status_id = 1 order by c.consulting_status ASC,c.customer_appointment_time ASC";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@DeptId", smsview.DepartmentID);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyDeptid = new DataTable();
            MyDeptid.Load(dr);
            MySqlConnection.Close();
            return MyDeptid;
        }
        #endregion SelectQueueStatus

        #region SelectQueueStatus123
        public DataTable SelectQueueStatus123(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
           // string sql = "select ctv.visit_queue_no_show from tbl_queue_tnx tq,tbl_customervisit_tnx ctv where tq.queu_visit_tnxid=ctv.visit_tnx_id and queue_department_id=@DeptId and queue_status_id=1 and CONVERT(DATE, queue_datetime) = CONVERT(DATE, GETDATE()) order by ctv.consulting_status ASC, ctv.customer_appointment_time asc";
            string sql = "select c.visit_queue_no_show from tbl_queue_tnx q,tbl_customervisit_tnx c where q.queue_department_id = @DeptId and q.queu_visit_tnxid = c.visit_tnx_id and CONVERT(DATE, q.queue_datetime) = CONVERT(DATE, GETDATE())and q.queue_status_id = 1 order by c.consulting_status ASC,c.customer_appointment_time ASC";

            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@DeptId", smsview.DepartmentID);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyDeptid = new DataTable();
            MyDeptid.Load(dr);
            MySqlConnection.Close();
            return MyDeptid;
        }
        #endregion SelectQueueStatus123

        #region SelectCustIDByUsingQueueno

        public DataTable SelectCustIDByUsingQueueno(SMSView smsview)
        {
            SqlConnection MysqlConnection = new SqlConnection(ConnectionString);
            MysqlConnection.Open();
            string sql = "select tcv.visit_customer_id,cd.members_id from tbl_customervisit_tnx tcv, tbl_customer_dtl cd where tcv.visit_queue_no_show=@Mysms and CONVERT(DATE, tcv.visit_datetime) = CONVERT(DATE, GETDATE()) and tcv.visit_member_id=cd.members_id";
            SqlCommand MyCommand = new SqlCommand(sql, MysqlConnection);
            MyCommand.Parameters.AddWithValue("@Mysms", smsview.MySms);
            SqlDataReader dr = MyCommand.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            MysqlConnection.Close();
            return dt;
        }

        #endregion SelectCustIDByUsingQueueno

        #region insert Reply sms
        public string InsertReplySMS(SMSView smsview)
        {
            SqlConnection MysqlConnection = new SqlConnection(ConnectionString);
            try
            {
                MysqlConnection.Open();
                string sql = "insert into tbl_sms_tnx(sms_visit_tnxid,sms_cust_id, sms_datetime,sms_phoneno,sms_status_flag,sms_content,sms_queueno)values(@sms_visit_tnxid,@sms_cust_id,@sms_datetime,@sms_phoneno,@sms_status_flag,@sms_content,@QueueNo)";
                cmd = new SqlCommand(sql, MysqlConnection);
                cmd.Parameters.AddWithValue("@sms_visit_tnxid", smsview.QueueTransaction);
                cmd.Parameters.AddWithValue("@sms_cust_id", smsview.CustId);
                cmd.Parameters.AddWithValue("@sms_datetime", smsview.SMSDateTime);
                cmd.Parameters.AddWithValue("@sms_phoneno", smsview.PhoneNo);
                cmd.Parameters.AddWithValue("@sms_status_flag", smsview.SMSStatusFlag);
                cmd.Parameters.AddWithValue("@sms_content", smsview.MySms);
                cmd.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
                //  cmd.Parameters.AddWithValue("@SmsDeliveryStatus", smsview.DeliveryReport);
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                string insert = Convert.ToString(1);
                return insert;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion insert Reply sms

        #region retrieve name and mobileno
        public DataTable retrieveNamemobilenoByCustID(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select cr.customer_firstname,cr.customer_lastname,cd.members_mobile from tbl_customerreg_mst cr,tbl_customer_dtl cd where cr.customer_id=cd.members_customer_id and cr.customer_id=@CustID and cd.members_id=@members";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@CustID", smsview.CustId);
            MySqlCommand.Parameters.AddWithValue("@members", smsview.MenberId);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion retrieve name and mobileno

        #region get CustID
        public DataTable retCustID(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select DISTINCT tv.visit_customer_id,td.members_id from tbl_customer_dtl td, tbl_customervisit_tnx tv where cast(tv.visit_datetime as date) = cast(getdate() as date) and tv.visit_member_id=td.members_id and tv.visit_queue_no_show=@QueueNo";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion Get CustID

        #region New Message Confirmation
        public DataTable getMessageExistance(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select * from tbl_sms_tnx where sms_visit_tnxid=@QueueTransactionID and sms_status_flag='N' and sms_queueno=@qno";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@QueueTransactionID", smsview.QueueTransaction);
            MySqlCommand.Parameters.AddWithValue("@qno",smsview.MySms);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion New Message Confirmation

        #region Missed Message Confirmation
        public DataTable getMissedMessageExistance(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select * from tbl_sms_tnx where sms_visit_tnxid=@QueueTransactionID and sms_status_flag='M'";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@QueueTransactionID", smsview.QueueTransaction);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion Missed Message Confirmation

        #region Alert Message Confirmation
        public DataTable getAlertMessageExistance(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            string sql = "select * from tbl_sms_tnx where sms_visit_tnxid=@QueueTransactionID and sms_status_flag='A'";
            SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
            MySqlCommand.Parameters.AddWithValue("@QueueTransactionID", smsview.QueueTransaction);
            SqlDataReader dr = MySqlCommand.ExecuteReader();
            DataTable MyCustName = new DataTable();
            MyCustName.Load(dr);
            MySqlConnection.Close();
            return MyCustName;
        }
        #endregion Alert Message Confirmation

        #region get button event

        public DataTable SelectButtonEvent(SMSView smsview)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            try
            {
                MySqlConnection.Open();
                // string sql = "select DISTINCT q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='A'";
                // string sql = "select DISTINCT v.visit_queue_no_show from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and sms_status_flag='N'";
                // string sql = "select DISTINCT q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.message_status_flag='A'";
                //string sql = "select q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c,tbl_appointment_tnx a where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and a.appointment_id=v.customer_appointment_id and q.queue_status_id=1 and q.message_status_flag='A' order by v.consulting_status ASC, v.customer_appointment_time ASC";
                string sql = "select c.visit_tnx_id,c.visit_queue_no,c.visit_queue_no_show,q.queue_department_id  from tbl_queue_tnx q,tbl_customervisit_tnx c where q.queu_visit_tnxid = c.visit_tnx_id and CONVERT(DATE, q.queue_datetime) = CONVERT(DATE, GETDATE())and q.button_event_flag='E'";
                SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection);
                // MySqlCommand.Parameters.AddWithValue("@Departmentid",smsview.DepartmentID);
                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQSMSstatus = new DataTable();

                MyQSMSstatus.Load(dr);

                MySqlConnection.Close();

                return MyQSMSstatus;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", ex);

            }
        }
        #endregion 

        #region update button event flag

        public DataTable updateButtonFlag(SMSView smsview)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            string sql = "update tbl_queue_tnx set button_event_flag=@Buttonevent where queu_visit_tnxid=@buttonvisittnx";
            SqlCommand cmd = new SqlCommand(sql,con);
            cmd.Parameters.AddWithValue("@Buttonevent",smsview.ButtonEventFlag);
            cmd.Parameters.AddWithValue("@buttonvisittnx",smsview.ButtonVisitTnx);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dtt = new DataTable();
            dtt.Load(dr);
            con.Close();
            cmd.Dispose();
            return dtt;

        }

        #endregion

        #region insert Appointment Alert sms
        public string InsertAppointmentAlertSMS(SMSView smsview)
        {
            SqlConnection MysqlConnection = new SqlConnection(ConnectionString);
            try
            {

                string sql = "insert into tbl_sms_tnx(sms_visit_tnxid,sms_cust_id,sms_content,sms_datetime,sms_status_flag,sms_phoneno,sms_delivery_status,sms_queueno,sms_centre_id)values(@SmsVisittnxId,@CustId,@SmsDesc,@SMSDateTime,@SMSStatusFlag,@PhoneNo,@DeliveryReport,@QueueNo,@CentreId)";
                cmd = new SqlCommand(sql, MysqlConnection);
                cmd.Parameters.AddWithValue("@SmsVisittnxId", smsview.SmsVisittnxId);
                cmd.Parameters.AddWithValue("@CustId", smsview.CustId);
                cmd.Parameters.AddWithValue("@SmsDesc", smsview.SmsDesc);
                cmd.Parameters.AddWithValue("@SMSDateTime", smsview.SMSDateTime);
                cmd.Parameters.AddWithValue("@SMSStatusFlag", smsview.SMSStatusFlag);
                cmd.Parameters.AddWithValue("@PhoneNo", smsview.PhoneNo);
                cmd.Parameters.AddWithValue("@DeliveryReport", smsview.DeliveryReport);
                cmd.Parameters.AddWithValue("@QueueNo", smsview.QueueNo);
                cmd.Parameters.AddWithValue("@CentreId", smsview.CentreId);
                // DataTable dt3 = new DataTable();
                MysqlConnection.Open();
                int i = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return Convert.ToString(i);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region Update SMS_Alert statsus flag
        public void updatesmsalert(SMSView smsview)
        {
            SqlDataAdapter sqlad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConnectionString);
                con.Open();
                string sql = "update tbl_appointment_tnx set sms_alert=@sms_alert where appointment_id=@appointment_id";

                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@appointment_id", smsview.AppointmentID);
                cmd.Parameters.AddWithValue("@sms_alert", smsview.SMSalert);

                //cmd.Parameters.AddWithValue("@buttoneventflag", rtview.ButtonEventFlag);
                sqlad.InsertCommand = cmd;
                cmd.ExecuteNonQuery();


                //return "0";
            }
            catch
            {
                //return "1";
            }
            finally
            {
                con.Close();
                //sqlrd.Close();
                cmd.Cancel();
            }
        }
        #endregion Update SMS_Alert statsus flag

        #region SMS - Get Appointment Alert SMSexpire

        public DataTable GetDaoappointmentalertexpired()
        {
            //TemperaryParameterList = new List<OracleParameter>();
            try
            {
                //// Query For Fault Monitoring
                //query = string.Format("select device_alaram_id,device_id,device_fault_code from tmsplaza.device_fault_ln_tnx where device_alaram_id=(select max(device_alaram_id) from tmsplaza.device_fault_ln_tnx)");

                SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
                //string sql = "select a.appointment_mobileno, c.customer_firstname,a.appointment_time from tbl_appointment_tnx a, tbl_customerreg_mst c where c.customer_id=a.appointment_customer_id and CONVERT(date,a.appointment_time)=CONVERT(date,GETDATE())";
                // SqlCommand MySqlCommand = new SqlCommand("select q.queu_visit_tnxid,v.visit_queue_no_show,c.customer_mobile from tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                //SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                // SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queu_visit_tnxid,v.visit_queue_no_show,tc.members_mobile,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queu_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.message_status_flag='N'", MySqlConnection);
                //SqlCommand MySqlCommand = new SqlCommand(sql, MySqlConnection); 
                // SqlCommand MySqlCommand = new SqlCommand("select tc.members_id, q.queue_visit_tnxid,v.visit_queue_no_show,tc.members_customer_id,q.queue_department_id, v.visit_customer_id from tbl_customer_dtl tc, tbl_customervisit_tnx v,tbl_queue_tnx q,tbl_customerreg_mst c where v.visit_member_id=tc.members_id and v.visit_tnx_id=q.queue_visit_tnxid and c.customer_id=v.visit_customer_id and cast(q.queue_datetime as date) = cast(getdate() as date) and q.queue_status_id=1 and q.sms_status_flag='N'", MySqlConnection);
                //MySqlCommand.Parameters.AddWithValue("@department_id", queueview.DepartmentID);
                //By Beeta
                //SqlCommand MySqlCommand = new SqlCommand("select appointment_customer_id,customer_mobile, appointment_time,customer_firstname from tbl_appointment_tnx,tbl_customerreg_mst where customer_id=appointment_customer_id and CONVERT(date,appointment_time)=CONVERT(date,GETDATE())", MySqlConnection); 
                //SqlCommand MySqlCommand = new SqlCommand("select appointment_id,appointment_customer_id,customer_mobile,appointment_time,customer_firstname,sms_alert,appointment_centre_id from tbl_appointment_tnx,tbl_customerreg_mst and CONVERT(date,appointment_time)=CONVERT(date,GETDATE()) and sms_alert='A'", MySqlConnection);
                 SqlCommand MySqlCommand = new SqlCommand("select appointment_customer_id,appointment_time,sms_alert from tbl_appointment_tnx,tbl_customervisit_tnx where visit_customer_id=appointment_customer_id and CONVERT(date,appointment_time)=CONVERT(date,GETDATE()) and sms_alert='A'", MySqlConnection);

                MySqlConnection.Open();

                SqlDataReader dr = MySqlCommand.ExecuteReader();

                DataTable MyQueueNO = new DataTable();

                MyQueueNO.Load(dr);

                MySqlConnection.Close();

                return MyQueueNO;
            }
            catch (Exception exmsg)
            {
                throw new Exception("Error Occured While Retrieving Data From DataBase", exmsg);
            }
            // Executing Select Query 
            //return smsdbconnection.ExecuteSelectQuery(query, TemperaryParameterList);
        }

        #endregion SMS - Get Appointmentexpired

        #region get Appointment details
        public DataTable GetAppointmentDetails(SMSView smsview)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                //By Ravi- string sql = "select DISTINCT a.appointment_id,a.appointment_customer_id,a.appointment_mobileno,a.appointment_time,c.customer_firstname from tbl_appointment_tnx a,tbl_customerreg_mst c where c.customer_id=a.appointment_customer_id and CONVERT(date,appointment_time)=CONVERT(date,GETDATE())";
                string sql = "select DISTINCT a.appointment_id,a.appointment_customer_id,a.appointment_mobileno,a.appointment_time,c.customer_firstname from tbl_appointment_tnx a,tbl_customerreg_mst c where c.customer_id=a.appointment_customer_id and a.sms_alert='B' and CONVERT(date,appointment_time)=CONVERT(date,GETDATE())";
                SqlCommand cmd = new SqlCommand(sql,con);
                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dtappdetails = new DataTable();
                dtappdetails.Load(dr);
                con.Close();
                return dtappdetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion

        #region get Appointment transaction details
        public DataTable GetAppointmentTransactionDetails(SMSView smsview)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                string sql = "select * from tbl_customervisit_tnx where visit_customer_id=4350 and CONVERT(date,visit_datetime)=CONVERT(date,GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dtappdetails = new DataTable();
                dtappdetails.Load(dr);
                con.Close();
                return dtappdetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }
        #endregion
    }
}
