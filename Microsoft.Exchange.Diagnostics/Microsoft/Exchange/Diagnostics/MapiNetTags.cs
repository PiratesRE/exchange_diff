using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MapiNetTags
	{
		public const int MapiAttach = 10;

		public const int MapiCollector = 11;

		public const int MapiContainer = 12;

		public const int MapiErrorNotification = 13;

		public const int MapiExtendedNotification = 14;

		public const int MapiFolder = 15;

		public const int MapiIStream = 16;

		public const int MapiMessage = 17;

		public const int MapiModifyTable = 18;

		public const int MapiNewMailNotification = 19;

		public const int MapiNotification = 20;

		public const int MapiObjectNotification = 21;

		public const int MapiProp = 22;

		public const int MapiStatusObjectNotification = 23;

		public const int MapiStore = 24;

		public const int MapiStream = 25;

		public const int MapiSynchroniser = 26;

		public const int MapiTable = 27;

		public const int MapiTableNotification = 28;

		public const int MapiUnk = 29;

		public const int NamedProp = 30;

		public const int NotificationHelper = 31;

		public const int ManifestCallbackHelper = 32;

		public const int DisposableRef = 33;

		public const int HierarchyManifestCallbackHelper = 34;

		public const int FaultInjection = 35;

		public const int MapiFxCollector = 36;

		public const int FxProxyHelper = 37;

		public const int ModuleInitDeinit = 60;

		public const int RPCManagedCallstacks = 70;

		public const int TraceCrossServerCalls = 80;

		public const int tagInformation = 400;

		public const int tagError = 401;

		public const int tagMostError = 402;

		public const int tagHcotTable = 410;

		public const int tagMspCsCheck = 420;

		public const int tagMspCsTrace = 421;

		public const int tagMspCsBlock = 422;

		public const int tagMspCsAssert = 423;

		public const int tagThreadErrorContext = 430;

		public const int tagSmallBuff = 440;

		public const int tagRpcCalls = 441;

		public const int tagRops = 442;

		public const int tagRpcRawBuffer = 443;

		public const int tagRpcExceptions = 444;

		public const int tagTableMethods = 460;

		public const int tagForceGhosted = 500;

		public const int tagDontReuseRpc = 501;

		public const int tagCnctConnect = 520;

		public const int tagDelegatedAuth = 521;

		public const int tagCxhPool = 522;

		public const int tagDiagnosticContext = 538;

		public const int tagLocation = 539;

		public const int tagUnkRelease = 540;

		public const int tagMoveMailbox = 541;

		public const int tagLocalDirectory = 565;

		public const int tagXTCFailure = 580;

		public const int tagXTCPolling = 581;

		public const int tagEseBack = 700;

		public const int tagFaultInjection = 906;

		public static Guid guid = new Guid("82914ab6-016b-442c-8e49-2562a4333be0");
	}
}
