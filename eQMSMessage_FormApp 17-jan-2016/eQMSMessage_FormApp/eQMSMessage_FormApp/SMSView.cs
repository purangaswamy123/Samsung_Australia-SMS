using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eQMSMessage_FormApp
{
    public class SMSView
    {
        public SMSView()
        {

        }

        // SMS View - Initiallizing Variables

        #region SMS View - Initiallizing Variables

        private string _buttoneventflag;
        private string _deliveryReport;
        private string _smsstatusflag;
        private int _queuetnxid;
        private int _smstnxid;
        private int _smsvisittnxid;
        private string _queueno;
        private string _centreid;
        private string _smsqueueno;
        private int _departmentid;
        private int _queuestatusid;
        private int _smscentreid;
        private string _mysms;
        private string _insmsstatus;
        private string _incomingsmsflag;
        private long _custid;
        private string _msg;
        private DateTime _smsdatetime;
        private string _phoneNo;
        private string _smsphoneno;
        private int _member_id;
        private int _buttonvisittnx;
        private int _smsvisittnx;
        private int _smscontent;
        private int _smscontenttypeid;
        private string _smsdesc;
        private int _smsalert;
        private char _smsactive;
        private DateTime _smsupdateddatetime;
        private string  _smsupdatedby;

        private int _appointment_id;
        private char _SMSAlert;
        #endregion SMS View - Initiallizing Variables

        // SMS View - Properties

        #region SMS View - Properties
        public int SMSContentTypeId
        {
            get
            {
                return _smscontenttypeid;
            }
            set
            {
                _smscontenttypeid = value;
            }
        }
        public int SMScontent
        {
            get
            {
                return _smscontent;
            }
            set
            {
                _smscontent = value;
            }
        }
        public string SmsDesc
        {
            get
            {
                return _smsdesc;
            }
            set
            {
                _smsdesc = value;
            }
        }
        public int SmsAlert
        {
            get
            {
                return _smsalert;
            }
            set
            {
                _smsalert = value;
            }
        }
        public char SmsActive
        {
            get
            {
                return _smsactive;
            }
            set
            {
                _smsactive = value;
            }
        }
        public DateTime SmsUpdatedDateTime
        {
            get
            {
                return _smsupdateddatetime;
            }
            set
            {
                _smsupdateddatetime = value;
            }
        }
        public string SmsUpdatedBy
        {
            get
            {
                return _smsupdatedby;
            }
            set
            {
                _smsupdatedby = value;
            }
        }
        public int ButtonVisitTnx
        {
            get
            {
                return _buttonvisittnx;
            }
            set
            {
                _buttonvisittnx = value;
            }
        }

        public string ButtonEventFlag
        {
            get
            {
                return _buttoneventflag;
            }
            set
            {
                _buttoneventflag = value;
            }
        }
        public int MenberId
        {
            get
            {
                return _member_id;
            }
            set
            {
                _member_id = value;
            }
        }
        public int SmstnxId
        {
            get
            {
                return _smstnxid;
            }
            set
            {
                _smstnxid = value;
            }
        }
        public int SmsVisittnxId
        {
            get
            {
                return _smsvisittnxid;
            }
            set
            {
                _smsvisittnxid = value;
            }
        }
        public string DeliveryReport
        {
            get
            {
                return _deliveryReport;
            }
            set
            {
                _deliveryReport = value;
            }
        }

        public string PhoneNo
        {
            get
            {
                return _phoneNo;
            }
            set
            {
                _phoneNo = value;
            }
        }

        public DateTime SMSDateTime
        {
            get
            {
                return _smsdatetime;
            }
            set
            {
                _smsdatetime = value;
            }
        }
        public string Message
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }
        public long CustId
        {
            get
            {
                return _custid;
            }
            set
            {
                _custid = value;
            }
        }
        public string IncomingsmsFlag
        {
            get
            {
                return _incomingsmsflag;
            }
            set
            {
                _incomingsmsflag = value;
            }
        }
        public string InsmsStatus
        {
            get
            {
                return _insmsstatus;
            }
            set
            {
                _insmsstatus = value;
            }
        }

        public string MySms
        {
            get { return _mysms; }
            set { _mysms = value; }
        }

        public string SMSStatusFlag
        {
            get { return _smsstatusflag; }
            set { _smsstatusflag = value; }
        }

        public int QueueTransaction
        {
            get { return _queuetnxid; }
            set { _queuetnxid = value; }
        }

        public string QueueNo
        {
            get { return _queueno; }
            set { _queueno = value; }
        }
        public string CentreId
        {
            get { return _centreid; }
            set { _centreid = value; }
        }

        public int DepartmentID
        {
            get { return _departmentid; }
            set { _departmentid = value; }
        }

        public int QueueStatusID
        {
            get { return _queuestatusid; }
            set { _queuestatusid = value; }
        }

        public char SMSalert
        {
            get
            {
                return _SMSAlert;
            }
            set
            {
                _SMSAlert = value;
            }
        }

        public int AppointmentID
        {
            get { return _appointment_id; }
            set { _appointment_id = value; }
        }

        #endregion SMS View - Properties

    }
}
