using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Proteger.ProtectiveObjects;
using Proteger.Logging;

using Plus.Database.Adapter;
using Plus.Database.Interfaces;

namespace Proteger.Managers
{
    public static class SettingsManager
    {

        #region GET

        public static ObservableCollection<RecloserSettings> GetRecloserSettings(int id)
        {
            ObservableCollection<RecloserSettings> settingsResult = new ObservableCollection<RecloserSettings>();
            DataTable linkTable = null;
            DataRow settingsRow = null;
            DateTime creationTime;

            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM link_ajustes_linha WHERE recloser_id = @thisid ORDER BY CASE link_ajustes_linha.`status` WHEN 0 THEN 1 ELSE 0 END,link_ajustes_linha.`status`;");
                dbClient.AddParameter("thisid", id);
                linkTable = dbClient.getTable();
                if (linkTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in linkTable.Rows)
                    {
                        try
                        {
                            dbClient.SetQuery("SELECT * FROM ajustes_linha WHERE id = @targetid LIMIT 1;");
                            dbClient.AddParameter("targetid", dr["settings_id"]);
                            settingsRow = dbClient.getRow();
                            if (settingsRow == null)
                            {
                                SystemLogging.SendErrorLog("Falha ao carregar ajuste para o religador: ID " + id + ". Entre em contato com o administrador do banco");
                                continue;
                            }
                            settingsResult.Add(new RecloserSettings(
                                Convert.ToInt32(settingsRow["id"]),
                                Convert.ToInt32(dr["status"]),
                                Convert.ToString(settingsRow["creation_service"]),
                                Convert.ToInt32(settingsRow["ftemp_pickup"]),
                                Convert.ToInt32(settingsRow["ftemp_curve"]),
                                Convert.ToDouble(settingsRow["ftemp_dial"]),
                                Convert.ToDouble(settingsRow["ftemp_mrt"]),
                                Convert.ToDouble(settingsRow["ftemp_timeadd"]),
                                Convert.ToInt32(settingsRow["ntemp_pickup"]),
                                Convert.ToInt32(settingsRow["ntemp_curve"]),
                                Convert.ToDouble(settingsRow["ntemp_dial"]),
                                Convert.ToDouble(settingsRow["ntemp_mrt"]),
                                Convert.ToDouble(settingsRow["ntemp_timeadd"]),
                                Convert.ToString(settingsRow["class"]),
                                Convert.ToInt32(settingsRow["num_op"]),
                                Convert.ToInt32(settingsRow["temp_op_1"]),
                                Convert.ToInt32(settingsRow["temp_op_2"]),
                                Convert.ToInt32(settingsRow["temp_op_3"]),
                                Convert.ToInt32(settingsRow["temp_rearm"]),
                                DateTime.Parse(dr["ts"].ToString())
                                ));
                            settingsRow = null;
                        }
                        catch (Exception ex)
                        {
                            SystemLogging.SendFatalErrorLog(ex.ToString());
                        }
                        //SystemLogging.SendProcessLog("Ajuste:" + dr["settings_id"] + " carregado");
                    }
                    return settingsResult;
                }
                else
                {
                    SystemLogging.SendWarningLog("Nenhum ajuste encontrado para o religador " + Core.GetRecloserManager().GetRecloserById(id).Number);
                    return null;
                }
            }
        }

        public static ObservableCollection<BaySettings> GetBaySettings(int id)
        {
            ObservableCollection<BaySettings> settingsResult = new ObservableCollection<BaySettings>();
            DataTable linkTable = null;
            DataRow settingsRow = null;
            bool enabled = true;  //TODO

            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM link_ajustes_bay WHERE bay_id = @thisid;");
                dbClient.AddParameter("thisid", id);
                linkTable = dbClient.getTable();
                if (linkTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in linkTable.Rows)
                    {
                        try
                        {
                            enabled = Convert.ToBoolean(dr["status"]);
                            dbClient.SetQuery("SELECT * FROM ajustes_bay WHERE id = @targetid LIMIT 1;");
                            dbClient.AddParameter("targetid", dr["settings_id"]);
                            settingsRow = dbClient.getRow();
                            if (settingsRow == null)
                            {
                                SystemLogging.SendErrorLog("Falha ao carregar ajuste para o religador: ID " + id + ". Entre em contato com o administrador do banco");
                                continue;
                            }
                            settingsResult.Add(new BaySettings(
                                Convert.ToInt32(settingsRow["id"]),
                                enabled,
                                Convert.ToString(settingsRow["creation_service"]),
                                Convert.ToInt32(settingsRow["ftemp_pickup"]),
                                Convert.ToDouble(settingsRow["ftemp_tap"]),
                                Convert.ToInt32(settingsRow["ftemp_curve"]),
                                Convert.ToDouble(settingsRow["ftemp_dial"]),
                                Convert.ToDouble(settingsRow["ftemp_mrt"]),
                                Convert.ToDouble(settingsRow["ftemp_timeadd"]),
                                Convert.ToInt32(settingsRow["ntemp_pickup"]),
                                Convert.ToInt32(settingsRow["ntemp_tap"]),
                                Convert.ToInt32(settingsRow["ntemp_curve"]),
                                Convert.ToDouble(settingsRow["ntemp_dial"]),
                                Convert.ToDouble(settingsRow["ntemp_mrt"]),
                                Convert.ToDouble(settingsRow["ntemp_timeadd"]),
                                Convert.ToInt32(settingsRow["num_op"]),
                                Convert.ToInt32(settingsRow["temp_op_1"]),
                                Convert.ToInt32(settingsRow["temp_op_2"]),
                                Convert.ToInt32(settingsRow["temp_op_3"]),
                                Convert.ToInt32(settingsRow["temp_rearm"]),
                                Convert.ToInt32(settingsRow["finst_pickup"]),
                                Convert.ToInt32(settingsRow["ninst_pickup"]),
                                Convert.ToDouble(settingsRow["finst_tap"]),
                                Convert.ToDouble(settingsRow["ninst_tap"])));

                            settingsRow = null;
                        }
                        catch (Exception ex)
                        {
                            SystemLogging.SendFatalErrorLog(ex.ToString());
                        }
                    }
                    return settingsResult;
                }
                else
                {
                    SystemLogging.SendWarningLog("Nenhum ajuste encontrado para o Bay " + Core.GetBayManager().GetBayById(id).CircuitCode);
                    return null;
                }
            }
        }

        #endregion

        #region LINK

        public static void LinkRecloserSettings(Recloser myRecloser, RecloserSettings mySettings)
        {
            if (mySettings.Status == 1)
            {
                List<RecloserSettings> disablabeList = myRecloser.Settings.Where(x => x.Status == 1).ToList();
                foreach (RecloserSettings rs in disablabeList)
                {
                    DisableSettings(myRecloser, rs);
                }
            }

            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                if (!(mySettings == null) && !(myRecloser == null))
                {
                    dbClient.SetQuery("INSERT INTO link_ajustes_linha (settings_id,recloser_id,status,ts) VALUES (@sid,@rid,@st,@dt);");
                    dbClient.AddParameter("sid", mySettings.Id);
                    dbClient.AddParameter("rid", myRecloser.Id);
                    dbClient.AddParameter("st", mySettings.Status);
                    dbClient.AddParameter("dt", mySettings.CreationDate);
                    dbClient.RunQuery();

                    Core.GetRecloserManager().AddNewSettings(myRecloser.Number, mySettings);
                }
            }

        }

        public static void DisableSettings(Recloser myRecloser, RecloserSettings mySettings)
        {
            if(mySettings.Status != 0)
            {
                using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE link_ajustes_linha SET status = 0 WHERE settings_id = @rsi;");
                    dbClient.AddParameter("rsi", mySettings.Id);
                    dbClient.RunQuery();
                }
                Core.GetRecloserManager().LoadedReclosers[Core.GetRecloserManager().GetRecloserIndex(myRecloser.Number)].DisableSettings(mySettings);
            }
        }

        public static void LinkBaySettings(Bay myBay, BaySettings mySettings)
        {
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                if (!(mySettings == null) && !(myBay == null))
                {
                    dbClient.SetQuery("INSERT INTO link_ajustes_bay (settings_id,bay_id,status) VALUES (@sid,@bid,@en);");
                    dbClient.AddParameter("sid", mySettings.Id);
                    dbClient.AddParameter("bid", myBay.Id);
                    dbClient.AddParameter("en", mySettings.Enabled);
                    dbClient.RunQuery();

                    //Core.GetRecloserManager().AddNewSettings(myBad.Number, mySettings);
                }
            }
        }

        #endregion

        #region CREATE

        #region Recloser

        public static RecloserSettings CreateNewRecloserSettings(int privateId, int status, string creationService, int phasePickUp, int phaseCurveId, double phaseDial,
            double phaseMRT, double phaseTimeAdd, int neutralPickUp, int neutralCurveId, double neutralDial, double neutralMRT, double neutralTimeAdd,
            string s_class, int numpOp, int opt1, int opt2, int opt3, int rearm, int instantaneousNeutralPickUp = 0, int instantaneousPhasePickUp = 0)
        {
            return new RecloserSettings(privateId, status, creationService, phasePickUp, phaseCurveId, phaseDial, phaseMRT, phaseTimeAdd, neutralPickUp, neutralCurveId,
                neutralDial, neutralMRT, neutralTimeAdd, s_class, numpOp, opt1, opt2, opt3, rearm, DateTime.Now);
        }

        public static RecloserSettings SaveRecloserSettings(RecloserSettings mySettings)
        {
            RecloserSettings realSettings = mySettings;
            int realId;

            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO ajustes_linha (creation_service, ftemp_pickup, ftemp_curve, ftemp_dial, ftemp_mrt, finst_pickup," +
                    "ntemp_pickup, ntemp_curve, ntemp_dial, ntemp_mrt, ninst_pickup, class, num_op, temp_op_1, temp_op_2, temp_op_3, temp_rearm) VALUES " +
                     "(@scres,@ftp,@ftc,@ftd,@ftm,@ftip,@ntp,@ntc,@ntd,@ntm,@ntip,@sclass,@snum,@top1,@top2,@top3,@tr);");
                dbClient.AddParameter("scres", mySettings.CreationService);
                dbClient.AddParameter("ftp", mySettings.PhasePickUp);
                dbClient.AddParameter("ftc", mySettings.PhaseCurveId);
                dbClient.AddParameter("ftd", mySettings.PhaseDial);
                dbClient.AddParameter("ftm", mySettings.PhaseMRT);
                dbClient.AddParameter("ftip", mySettings.InstantPhasePickUp);
                dbClient.AddParameter("ntp", mySettings.NeutralPickUp);
                dbClient.AddParameter("ntc", mySettings.NeutralCurveId);
                dbClient.AddParameter("ntd", mySettings.NeutralDial);
                dbClient.AddParameter("ntm", mySettings.NeutralMRT);
                dbClient.AddParameter("ntip", mySettings.InstantNeutralPickUp);
                dbClient.AddParameter("sclass", mySettings.Class);
                dbClient.AddParameter("snum", mySettings.NumberOfOperations);
                dbClient.AddParameter("top1", mySettings.Operation1Time);
                dbClient.AddParameter("top2", mySettings.Operation2Time);
                dbClient.AddParameter("top3", mySettings.Operation3Time);
                dbClient.AddParameter("tr", mySettings.Rearm);
                realId = Convert.ToInt32(dbClient.InsertQuery());
            }
            realSettings.Id = realId;
            return realSettings;
        }

        public static RecloserSettings CreateNewRecloserSettings(int status, string creationService, int phasePickUp, int phaseCurveId, double phaseDial,
            double phaseMRT, double phaseTimeAdd, int neutralPickUp, int neutralCurveId, double neutralDial, double neutralMRT, double neutralTimeAdd,
            string s_class, int numpOp, int opt1, int opt2, int opt3, int rearm, int instantaneousNeutralPickUp = 0, int instantaneousPhasePickUp = 0)
        {
            int id = 0;
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO ajustes_linha (creation_service, ftemp_pickup, ftemp_curve, ftemp_dial, ftemp_mrt, finst_pickup," +
                    "ntemp_pickup, ntemp_curve, ntemp_dial, ntemp_mrt, ninst_pickup, class, num_op, temp_op_1, temp_op_2, temp_op_3, temp_rearm) VALUES " +
                     "(@scres,@ftp,@ftc,@ftd,@ftm,@ftip,@ntp,@ntc,@ntd,@ntm,@ntip,@sclass,@snum,@top1,@top2,@top3,@tr);");
                dbClient.AddParameter("scres", creationService);
                dbClient.AddParameter("ftp", phasePickUp);
                dbClient.AddParameter("ftc", phaseCurveId);
                dbClient.AddParameter("ftd", phaseDial);
                dbClient.AddParameter("ftm", phaseMRT);
                dbClient.AddParameter("ftip", instantaneousPhasePickUp);
                dbClient.AddParameter("ntp", neutralPickUp);
                dbClient.AddParameter("ntc", neutralCurveId);
                dbClient.AddParameter("ntd", neutralDial);
                dbClient.AddParameter("ntm", neutralMRT);
                dbClient.AddParameter("ntip", instantaneousNeutralPickUp);
                dbClient.AddParameter("sclass", s_class);
                dbClient.AddParameter("snum", numpOp);
                dbClient.AddParameter("top1", opt1);
                dbClient.AddParameter("top2", opt2);
                dbClient.AddParameter("top3", opt3);
                dbClient.AddParameter("tr", rearm);
                id = Convert.ToInt32(dbClient.InsertQuery());
            }

            if (id == 0)
            {
                SystemLogging.SendFatalErrorLog("Falha ao criar ajuste");
                return null;
            }
            else
            {
                SystemLogging.SendProcessLog("Ajuste criado: " + id + " " + creationService);
                return new RecloserSettings(id, status, creationService, phasePickUp, phaseCurveId, phaseDial, phaseMRT, phaseTimeAdd, neutralPickUp, neutralCurveId,
                    neutralDial, neutralMRT, neutralTimeAdd, s_class, numpOp, opt1, opt2, opt3, rearm, DateTime.Now);
            }
        }
        #endregion

        #region Bay

        public static BaySettings CreateNewBaySettings(int privateId,bool enabled, string creationService, int phasePickUp, double phaseTAP, int phaseCurveId, double phaseDial,
           double phaseMRT, double phaseTimeAdd, int neutralPickUp, double neutralTAP, int neutralCurveId, double neutralDial, double neutralMRT, double neutralTimeAdd,
           int numpOp, int opt1, int opt2, int opt3, int rearm, int instantPhasePickUp, int instantNeutralPickUp,
           double instantPhaseTAP, double instantNeutralTAP)
        {
            return new BaySettings(privateId, enabled, creationService, phasePickUp, phaseTAP, phaseCurveId, phaseDial,
                phaseMRT, phaseTimeAdd, neutralPickUp, neutralTAP, neutralCurveId, neutralDial, neutralMRT,
                neutralTimeAdd, numpOp, opt1, opt2, opt3, rearm, instantPhasePickUp,
                instantNeutralPickUp, instantPhaseTAP, instantNeutralTAP);
        }

        public static BaySettings CreateNewBaySettings(bool enabled, string creationService, int phasePickUp, double phaseTAP, int phaseCurveId, double phaseDial,
            double phaseMRT, double phaseTimeAdd, int neutralPickUp, double neutralTAP, int neutralCurveId, double neutralDial, double neutralMRT, double neutralTimeAdd,
            int numpOp, int opt1, int opt2, int opt3, int rearm, int instantPhasePickUp, int instantNeutralPickUp,
            double instantPhaseTAP, double instantNeutralTAP)
        {
            int id = 0;
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO ajustes_bay (creation_service, ftemp_pickup, ftemp_tap, ftemp_curve, ftemp_dial, ftemp_mrt, ftemp_timeadd, finst_pickup," +
                    "finst_tap,ntemp_pickup,ntemp_tap, ntemp_curve, ntemp_dial, ntemp_mrt,ntemp_timeadd, ninst_pickup, ninst_tap, num_op, temp_op_1, temp_op_2, temp_op_3, temp_rearm) VALUES " +
                     "(@scres,@ftp,@ftt,@ftc,@ftd,@ftm,@fta,@fip,@fit,@ntp,@ntt,@ntc,@ntd,@ntm,@nta,@nip,@nit,@snum,@top1,@top2,@top3,@tr);");
                dbClient.AddParameter("scres", creationService);
                dbClient.AddParameter("ftp", phasePickUp);
                dbClient.AddParameter("ftt", phaseTAP);
                dbClient.AddParameter("ftc", phaseCurveId);
                dbClient.AddParameter("ftd", phaseDial);
                dbClient.AddParameter("ftm", phaseMRT);
                dbClient.AddParameter("fta", phaseTimeAdd);
                dbClient.AddParameter("fip", instantPhasePickUp);
                dbClient.AddParameter("fit", instantPhaseTAP);
                dbClient.AddParameter("ntp", neutralPickUp);
                dbClient.AddParameter("ntt", neutralTAP);
                dbClient.AddParameter("ntc", neutralCurveId);
                dbClient.AddParameter("ntd", neutralDial);
                dbClient.AddParameter("ntm", neutralMRT);
                dbClient.AddParameter("nta", neutralTimeAdd);
                dbClient.AddParameter("nip", instantNeutralPickUp);
                dbClient.AddParameter("nit", instantNeutralTAP);
                dbClient.AddParameter("snum", numpOp);
                dbClient.AddParameter("top1", opt1);
                dbClient.AddParameter("top2", opt2);
                dbClient.AddParameter("top3", opt3);
                dbClient.AddParameter("tr", rearm);
                id = Convert.ToInt32(dbClient.InsertQuery());
            }

            if (id == 0)
            {
                SystemLogging.SendFatalErrorLog("Falha ao criar ajuste de Bay");
                return null;
            }

            else
            {
                SystemLogging.SendProcessLog("Ajuste criado de Bay: " + id + " " + creationService);
                BaySettings myNewSettings = new BaySettings(id, enabled, creationService, phasePickUp, phaseTAP, phaseCurveId, phaseDial,
                    phaseMRT, phaseTimeAdd, neutralPickUp, neutralTAP, neutralCurveId, neutralDial, neutralMRT,
                    neutralTimeAdd, numpOp, opt1, opt2, opt3, rearm, instantPhasePickUp,
                    instantNeutralPickUp, instantPhaseTAP, instantNeutralTAP);
                return myNewSettings;
            }
        }

        #endregion
        #endregion

        public static void ValidateBayRTC(Bay b, string RTCProt)
        {
            using (IQueryAdapter dbClient = Core.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE alimentadores SET rtc_prot = @rtc, rtc_prot_checked = 1 WHERE id = @id LIMIT 1;");
                dbClient.AddParameter("rtc", RTCProt);
                dbClient.AddParameter("id", b.Id);
                dbClient.RunQuery();
            }
        }
    }
}
