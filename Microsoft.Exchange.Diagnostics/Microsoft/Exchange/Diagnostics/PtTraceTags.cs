using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct PtTraceTags
	{
		public const int DS_DsAccess = 10;

		public const int DS_Mprw = 11;

		public const int DS_DlkHash = 12;

		public const int DS_MdCache = 13;

		public const int DS_Tokenm = 14;

		public const int DS_IevntLog = 15;

		public const int DS_EvntWrap = 16;

		public const int DS_ADTopoSvc = 17;

		public const int DS_CacheAttrs = 21;

		public const int DS_Callback = 22;

		public const int DS_Conn = 23;

		public const int DS_ContextCache = 24;

		public const int DS_DscEvent = 25;

		public const int DS_Group = 26;

		public const int DS_Incar = 27;

		public const int DS_IncarCache = 28;

		public const int DS_LdapApi = 29;

		public const int DS_MprwLock = 30;

		public const int DS_MsgCache = 31;

		public const int DS_Other = 32;

		public const int DS_Perf = 33;

		public const int DS_Reachability = 34;

		public const int DS_ReqCache = 35;

		public const int DS_ShmToPvFailure = 36;

		public const int DS_ShmToPv_NULL_handle = 37;

		public const int DS_Topo = 38;

		public const int DS_TopoDNS = 39;

		public const int DS_TopoServerLists = 40;

		public const int DS_TopoTiming = 41;

		public const int common_epoxy = 50;

		public const int common_smtpaddr = 51;

		public const int common_parse821 = 52;

		public const int common_excache2 = 53;

		public const int common_requery = 54;

		public const int common_exrw = 55;

		public const int admin_admin = 60;

		public const int admin_ds2mb = 61;

		public const int cluster_getcomp = 63;

		public const int deployment_main = 64;

		public const int dsa_oabgen = 70;

		public const int cal_Debug = 100;

		public const int cal_XProcCache = 101;

		public const int cal_DsaMgr = 102;

		public const int cal_Url = 103;

		public const int cal_Xml = 104;

		public const int cal_Rfp = 105;

		public const int cal_exfcache = 106;

		public const int cal_ExOleDb = 110;

		public const int cal_ExOleDb_Errors = 111;

		public const int cal_ExOleDb_Events = 112;

		public const int cal_ExOleDb_ThreadPool = 113;

		public const int cal_ExOleDb_Transactions = 114;

		public const int cal_ExOleDb_SystemEvents = 115;

		public const int cal_ExOleDb_ClientControl = 116;

		public const int cal_ExOleDb_EntryExit = 117;

		public const int cal_ExOleDb_Impersonation = 118;

		public const int cal_ExOleDb_Hsots = 119;

		public const int cal_Prx = 120;

		public const int cal_PrxConn = 121;

		public const int cal_PrxParser = 122;

		public const int cal_PrxReplMgr = 123;

		public const int cal_PrxRequest = 124;

		public const int cal_PrxSrv = 125;

		public const int cal_PrxSec = 126;

		public const int cal_IdleThrd = 130;

		public const int cal_LinkFix = 131;

		public const int cal_Nmspc = 132;

		public const int cal_StringBlock = 133;

		public const int cal_Schema = 134;

		public const int cal_SchemaPop = 135;

		public const int cal_DbCommandTree = 136;

		public const int cal_Sql = 137;

		public const int cal_Exoledbesh_Errors = 138;

		public const int cal_CalcProps = 140;

		public const int cal_MdbInst = 141;

		public const int cal_LogCallback = 142;

		public const int cal_AdminLogon = 143;

		public const int cal_StorextErr = 144;

		public const int cal_Davex = 150;

		public const int cal_DavexDbgHeaders = 151;

		public const int cal_Epoxy = 152;

		public const int cal_Repl = 153;

		public const int cal_Ifs = 154;

		public const int cal_IfsCache = 155;

		public const int cal_WebClient = 156;

		public const int cal_FileStream = 157;

		public const int cal_PackedResponse = 158;

		public const int cal_Exdav = 160;

		public const int cal_Notif = 161;

		public const int cal_Props = 162;

		public const int cal_Search = 163;

		public const int cal_SessMgr = 164;

		public const int cal_Locks = 165;

		public const int cal_EnumAtts = 166;

		public const int cal_PropFind = 167;

		public const int cal_OWASMimeAlgorithm = 170;

		public const int cal_OWASMimeData = 171;

		public const int cal_OWASMimeCall = 172;

		public const int cal_OWAFilteringAlgorithm = 173;

		public const int cal_OWAFilteringCall = 174;

		public const int cal_OWAFilteringAction = 175;

		public const int cal_OWAJunkEmail = 176;

		public const int cal_Actv = 180;

		public const int cal_BodyStream = 181;

		public const int cal_Content = 182;

		public const int cal_Ecb = 183;

		public const int cal_EcbLogging = 184;

		public const int cal_EcbStream = 185;

		public const int cal_Event = 186;

		public const int cal_Lock = 187;

		public const int cal_Method = 188;

		public const int cal_Persist = 189;

		public const int cal_Request = 190;

		public const int cal_Response = 191;

		public const int cal_ScriptMap = 192;

		public const int cal_Transmit = 193;

		public const int cal_DavprsDbgHeaders = 194;

		public const int cal_Metabase = 195;

		public const int cal_Unpack = 199;

		public const int OWAAuth_General = 200;

		public const int OWAAuth_Algorithm = 201;

		public const int OWAAuth_ExtensionError = 202;

		public const int OWAAuth_FilterError = 203;

		public const int OWAAuth_CryptoError = 204;

		public const int OWAAuth_MetabaseInfo = 205;

		public const int OWAAuth_MetabaseError = 206;

		public const int OWAAuth_Debug = 209;

		public const int OWAAuth_RPCInfo = 210;

		public const int OWAAuth_RPCError = 211;

		public const int EASFilter_Requests = 250;

		public const int ExFba_General = 270;

		public const int ExFba_ServiceInfo = 271;

		public const int ExFba_ServiceError = 272;

		public const int ExFba_CryptoError = 273;

		public const int ExFba_TombstoneInfo = 274;

		public const int ExFba_TombstoneError = 275;

		public const int ExFba_Algorithm = 276;

		public const int ExFba_RPCInfo = 277;

		public const int ExFba_RPCError = 278;

		public const int ExFba_Debug = 279;

		public const int ExFba_CASInfo = 281;

		public const int ExFba_CASError = 282;

		public const int AuthModuleFilter_Requests = 280;

		public const int codeinject_main = 600;

		public const int dsaccess_test = 601;

		public static Guid guid = new Guid("1b88b5f7-be69-4d19-b065-9c30b6df8185");
	}
}
