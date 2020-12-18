using APIBase.Answer;
using APIBase.Structure;
using APIBase.Utils;
using IV.SW.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading;


namespace LegacySyncUtils
{
    public class Main
    {

        public static void SyncUser(string local, int userid, bool actionupdate = true)
        {

            var sql = "exec sync.PROC_SYNC_ACTION_EXECUTE{COND} 'Mn-User-Update', " + userid.ToString();
            var constring = ConfigurationManager.AppSettings["SYNC_USER_CONNSTRING"];

            try
            {

                if (constring != null)
                {
                    var parms = new Dictionary<string, object>();
                    parms["UserId"] = userid;
                    parms["actionupdate"] = actionupdate;

                    SqlThread.ThreadProcessSQL(constring, 10, sql.Replace("{COND}", "") + ", 1 ", sql.Replace("{COND}", "_TRIGGER"), "SyncUser", local, parms);

                }

            }
            catch (Exception ex)
            {

                Log.ApiException(null, "SyncUser", ex, "local".AndValue(local), "userid".AndValue(userid));

            }
        }

        public static void SyncCompany(string local, int companyid, bool actionupdate = true)
        {

            try
            {
                var constring = ConfigurationManager.AppSettings["SYNC_COMPANY_CONNSTRING"];
                if (constring != null)
                {
                    var sql = "exec sync.PROC_SYNC_ACTION_EXECUTE{COND} 'Mn-Company-Update', " + companyid.ToString();

                    var parms = new Dictionary<string, object>();
                    parms["companyid"] = companyid;
                    parms["actionupdate"] = actionupdate;

                    SqlThread.ThreadProcessSQL(constring, 10, sql.Replace("{COND}", ""), sql.Replace("{COND}", "_TRIGGER"), "SyncCompany", local, parms);

                }

            }
            catch (Exception ex)
            {

                Log.ApiException(null, "SyncCompany", ex, "local".AndValue(local), "companyid".AndValue(companyid));
            }
        }

        public static void ForceGfMnSync(string local, string userkey)
        {
            try
            {
                var constring = ConfigurationManager.AppSettings["SYNC_FORCE_GFMN_CONNSTRING"];
                if (constring != null)
                {
                    var parms = new Dictionary<string, object>();
                    parms["userkey"] = userkey;

                    SqlThread.ThreadProcessSQL(constring, 10, "exec sync.PROC_SYNC_FORCE_ACTION_EXECUTE '" + userkey + "'", null, "ForceGfMnSync", local, parms);

                }

            }
            catch (Exception ex)
            {

                Log.ApiException(null, "ForceGfMnSync", ex, "local".AndValue(local), "userkey".AndValue(userkey));
            }
        }

        public static void AdjustEmailConsolidation(string local, int olduserid, int oldemailid, int userid, int emailid)
        {
            try
            {
                var constring = ConfigurationManager.AppSettings["SYNC_CONSOLID_CONNSTRING"];
                if (constring != null)
                {
                    var parms = new Dictionary<string, object>();
                    parms["olduserid"] = olduserid;
                    parms["oldemailid"] = oldemailid;
                    parms["userid"] = userid;
                    parms["emailid"] = emailid;

                    SqlThread.ThreadProcessSQL(constring, 30, "exec sync.PROC_SYNC_CONSOLIDATION_MNGF_EXECUTE " +
                                            olduserid.ToString() + "," + oldemailid.ToString() + "," + userid.ToString() + "," + emailid.ToString(), null, "AdjustEmailConsolidation", local, parms);

                }

            }
            catch (Exception ex)
            {

                Log.ApiException(null, "AdjustEmailConsolidation", ex, "local".AndValue(local),
                    "olduserid".AndValue(olduserid), "oldemailid".AndValue(oldemailid),
                    "userid".AndValue(userid), "emailid".AndValue(emailid));
            }
        }

        //-------------------------- THREAD CONTROL
        public class SqlThread
        {
            string ConnString;
            int timeout;
            string Command;
            string CommandForTimeout;
            string operation;
            string source;
            Dictionary<string, object> parms;

            Thread t;

            public static List<SqlThread> Threads = new List<SqlThread>();

            public static void ThreadProcessSQL(string pConnString, int ptimeout, string pCommand, string pCommandForTimeout, string poperation, string psource, Dictionary<string, object> pparms)
            {
                var st = new SqlThread()
                {
                    ConnString = pConnString,
                    Command = pCommand,
                    CommandForTimeout = pCommandForTimeout,
                    operation = poperation,
                    parms = pparms,
                    source = psource,
                    timeout = ptimeout
                };

                Threads.Add(st);

                st.t = new Thread(st.ProcessSQL);
                st.t.Start();
            }

            private void ProcessSQL()
            {
                if (parms == null)
                    parms = new Dictionary<string, object>();

                parms["source"] = source;
                try
                {
                    var con = new GenConnection(GenConnection.CONNECTION_TYPE.MSSQL);
                    con.Config(ConnString);
                    con.ExecTimeout = timeout;
                    con.ThrowError = true;

                    con.ExecCommandNamedParameters(Command, parms.ToHybridDictionary(), true);
                    Log.ApiTrace(null, operation, parms.ToArray());
                }
                catch (Exception ex)
                {
                    if ((CommandForTimeout ?? "") != "" && ex.ToString().ToLower().Contains("timeout") || ex.ToString().ToLower().Contains("limite"))
                    {

                        try
                        {
                            var con = new GenConnection(GenConnection.CONNECTION_TYPE.MSSQL);
                            con.Config(ConnString);
                            con.ExecTimeout = timeout;
                            con.ThrowError = true;
                            con.ExecCommandNamedParameters(CommandForTimeout, parms.ToHybridDictionary(), true);
                            Log.ApiTrace(null, operation, parms.ToArray());
                        }
                        catch (Exception ex1)
                        {
                            parms["trigger"] = true;
                            parms["trigger_exception"] = ex1.ToString();
                            Log.ApiException(null, operation, ex, parms.ToArray());
                            throw;
                        }
                    }
                    else
                    {
                        Log.ApiException(null, operation, ex, parms.ToArray());
                    }
                }

                if (Threads.Contains(this))
                    try
                    {
                        Threads.Remove(this);
                    }
                    catch (Exception)
                    {
                    }

            }

        }

        public static APIAnswer ExecGenericStandardAnswer(Database db, string proc, params KeyValuePair<string, object>[] parms)
        {
            if (!proc.HasContent())
                return APIAnswer.Error(-2000, "proc invalida");

            if (parms != null && parms.Length > 0)
                proc = proc.FormatWith(parms.ToDictionaryStringObject());

            try
            {
                var retlist = db.SqlQuery<APIAnswer>(proc).ToArray();

                if (retlist.Length == 0)
                    return APIAnswer.Error(-2001, proc + ":retorno zerado");

                if (retlist.Length > 1)
                    return APIAnswer.Error(-2002, proc + ":retorno de mais de um");

                return retlist[0];
            }
            catch (Exception ex)
            {
                return APIAnswer.Error(-2001, proc + ":" + ex.ToString());
            }
        }


    }
}
