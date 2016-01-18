using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace eQMSMessage_FormApp
{
    class SMSController
    {
        SMSView smsview = new SMSView();

        public SMSController()
        {

        }

        public DataTable SetSMSSearchRow(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoSetSMSSearchRow(smsview);
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable SetSMSStatusR(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoSetSMSStatusR(smsview);
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetTotalWaitingQueue(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoTotalWaitingQueue(smsview);
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetTotalWaitingMissedQueue(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoTotalWaitingMissedQueue(smsview);
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetGeneratedQueue()
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoQueueGenerationSMS();
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetQueueTokenGenerationSentSMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                DataTable dt1 = new DataTable();
                // Getting Data From Dao
                dt1 = smsdao.GetDaoQueueTokenGenerationSentSMS(smsview);
                return dt1;
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetRetrieveSMSstatusFlag(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                DataTable dt = new DataTable();
                dt = smsdao.RetrieveSMSstatusFlag(smsview);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        public DataTable appointmentalert()
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoappointmentalert();
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }


        public DataTable GetMissedQueueSendingSMS()
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoMissedQueueSendingSMS();
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetMissedQueueSentSMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoMissedQueueSentSMS(smsview);
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetMissedQueue()
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.DaoGetMissedQueue();
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }

        }

        public DataTable GetAutoQueueSendingSMS()
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoAutoQueueSendingSMS();
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }



        public DataTable AppRemender()
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.AppRemender();
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetAutoQueueSentSMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                // Getting Data From Dao
                return smsdao.GetDaoAutoQueueSentSMS(smsview);
            }
            catch
            {
                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetIncomingSMS(SMSView smsview)
        {
            SMSDAO smsdao=new SMSDAO();
            try
            {
                return smsdao.searchIncomingSMS(smsview);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetQueueStatus(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                return smsdao.serachQueueStatus(smsview);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetsmsStatusFlag(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                return smsdao.updateincomingsms(smsview);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetQueuePosition(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            DataTable dt = new DataTable();
            try
            {
                dt = smsdao.selectQueuePosition(smsview);
                return dt;
            }
            catch (Exception)
            {

                return dt;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetQueuePosition123(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            DataTable dt = new DataTable();
            try
            {
                dt = smsdao.selectQueuePosition123(smsview);
                return dt;
            }
            catch (Exception)
            {

                return dt;
            }
            finally
            {
                smsdao = null;
            }
        }

        public DataTable GetpositionbyusingQueueno(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            DataTable dt = new DataTable();
            try
            {
                dt = smsdao.positionretrievebyusingQueueno(smsview);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }

        #region retrieve CustomerName
        public DataTable GetCustomerName(SMSView smsview)
        {
            DataTable dta = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                return dta = smsdao.retrieveNameByCustID(smsview);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion retrieve CustomerName

        #region GetInsertNewSMS
        public string GetInsertNewSMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                string dt2;
                dt2= smsdao.InsertNewSMS(smsview);
                return dt2;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetInsertNewSMS

        #region GetInsertAlertSMS
        public string GetInsertAlertSMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                string dt2;
                dt2 = smsdao.InsertAlertSMS(smsview);
                return dt2;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetInsertAlertSMS


        #region GetInsertMissedQSMS
        public string GetInsertMissedQSMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                string dt2;
                dt2 = smsdao.InsertMissedQSMS(smsview);
                return dt2;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetInsertMissedQSMS

        #region retrievevisittnxidbyusingrepliedsms
        public DataTable Getvisittnxidbyusingrepliedsms(SMSView smsview)
        {
            DataTable dt = new DataTable();
            SMSDAO smsdao=new SMSDAO();
            try
            {
                dt = smsdao.retrievevisittnxidbyusingrepliedsms(smsview);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion retrievevisittnxidbyusingrepliedsms

        #region GetDeptid
        public DataTable GetDeptid(SMSView smsview)
        {
            DataTable dt4 = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                dt4 = smsdao.selectDeptId(smsview);
                return dt4;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetDeptid

        #region GetStatusCount
        public DataTable GetStatusCount(SMSView smsview)
        {
            DataTable dt7 = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                dt7 = smsdao.SelectQueueStatus(smsview);
                return dt7;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetStatusCount

        #region GetStatusCount123
        public DataTable GetStatusCount123(SMSView smsview)
        {
            DataTable dt7 = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                dt7 = smsdao.SelectQueueStatus123(smsview);
                return dt7;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetStatusCount

        #region GetCustIDByUsingQueueno
        public DataTable GetCustIDByUsingQueueno(SMSView smsview)
        {
            DataTable dt1 = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                dt1 = smsdao.SelectCustIDByUsingQueueno(smsview);
                return dt1;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }

        }
        #endregion GetCustIDByUsingQueueno

        #region GetInsertReplySMS
        public string GetInsertReplySMS(SMSView smsview)
        {
            SMSDAO smsdao = new SMSDAO();
            try
            {
                return smsdao.InsertReplySMS(smsview);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion GetInsertReplySMS

        #region retrieve CustomerNameAndMobileNo
        public DataTable GetCustomerNameMobile(SMSView smsview)
        {
            DataTable dta = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                return dta = smsdao.retrieveNamemobilenoByCustID(smsview);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion retrieve CustomerNameAndMobileNo

        #region GetretCustID

       public DataTable GetCustId(SMSView smsview)
        {
            DataTable dta = new DataTable();
            SMSDAO smsdao = new SMSDAO();
            try
            {
                return dta = smsdao.retCustID(smsview);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                smsdao = null;
            }
        }
        #endregion GetretCustID

       #region New Message existance
       public DataTable GetNewMessageExistance(SMSView smsview)
       {
           DataTable dta = new DataTable();
           SMSDAO smsdao = new SMSDAO();
           try
           {
               return dta = smsdao.getMessageExistance(smsview);
           }
           catch (Exception)
           {

               throw;
           }
           finally
           {
               smsdao = null;
           }
       }
       #endregion New Message Existance

       #region Missed Message existance
       public DataTable GetMessedQMessageExistance(SMSView smsview)
       {
           DataTable dta = new DataTable();
           SMSDAO smsdao = new SMSDAO();
           try
           {
               return dta = smsdao.getMissedMessageExistance(smsview);
           }
           catch (Exception)
           {

               throw;
           }
           finally
           {
               smsdao = null;
           }
       }
       #endregion Missed Message Existance

       #region Alert Message existance
       public DataTable GetAlertQMessageExistance(SMSView smsview)
       {
           DataTable dta = new DataTable();
           SMSDAO smsdao = new SMSDAO();
           try
           {
               return dta = smsdao.getAlertMessageExistance(smsview);
           }
           catch (Exception)
           {

               throw;
           }
           finally
           {
               smsdao = null;
           }
       }
       #endregion Alert Message Existance

       public DataTable GetButtonEvent(SMSView smsview)
       {
           SMSDAO smsdao = new SMSDAO();
           DataTable dt = new DataTable();
           try
           {
               dt = smsdao.SelectButtonEvent(smsview);
               return dt;
           }
           catch (Exception)
           {

               return dt;
           }
           finally
           {
               smsdao = null;
           }
       }
       public DataTable getUpdatebuttoneventflag(SMSView smsview)
       {
           SMSDAO smsdao = new SMSDAO();
           DataTable dt1 = new DataTable();
           try
           {
               dt1 = smsdao.updateButtonFlag(smsview);
               return dt1;
           }
           catch (Exception)
           {

               return dt1;
           }
           finally
           {
               smsdao = null;
           }
       }

       public string getInsertAppointmentAlertSms(SMSView smsview)
       {
           SMSDAO smsdao = new SMSDAO();
           DataTable dt = new DataTable();
           string i;
           try
           {
               i = smsdao.InsertAppointmentAlertSMS(smsview);
               return i;
           }
           catch (Exception)
           {
               
               throw;
           }
       }


       #region Update SMS_Alert statsus flag

       public void updatesmsalert(SMSView smsview)
       {
           DataTable dta = new DataTable();
           SMSDAO smsdao = new SMSDAO();
           try
           {
               smsdao.updatesmsalert(smsview);
           }
           catch (Exception)
           {

               throw;
           }
           finally
           {
               smsdao = null;
           }
       }
       #endregion Update SMS_Alert statsus flag

       #region get appointment details
       public DataTable appointmentexpired(SMSView smsview)
       {
           SMSDAO smsdao = new SMSDAO();
           try
           {
               // Getting Data From Dao
               return smsdao.GetAppointmentDetails(smsview);
           }
           catch
           {
               throw;
           }
           finally
           {
               smsdao = null;
           }
       }
       #endregion get appointment details

       #region get appointment details
       public DataTable appointmenTranstexpired(SMSView smsview)
       {
           SMSDAO smsdao = new SMSDAO();
           try
           {
               // Getting Data From Dao
               return smsdao.GetAppointmentTransactionDetails(smsview);
           }
           catch
           {
               throw;
           }
           finally
           {
               smsdao = null;
           }
       }
       #endregion get appointment details
    }
}
