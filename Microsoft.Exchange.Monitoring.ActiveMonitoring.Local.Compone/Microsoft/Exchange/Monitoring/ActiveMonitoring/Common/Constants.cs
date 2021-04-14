using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public sealed class Constants
	{
		public const string E14PowershellEndpointFormat = "https://{0}psh.outlook.com/PowerShell-LiveID";

		public const string ProdPowershellEndpoint = "https://outlook.office365.com/PowerShell-LiveID";

		public const string SDFPowershellEndpoint = "https://sdfpilot.outlook.com/PowerShell-LiveID";

		public const string GallatinPowershellEndpoint = "https://partner.outlook.cn/PowerShell-LiveID";

		public const string PRODEWSUrl = "https://outlook.office365.com/ews/exchange.asmx";

		public const string SDFEWSUrl = "https://sdfpilot.outlook.com/ews/exchange.asmx";

		public const string GallatinEWSUrl = "https://partner.outlook.cn/ews/exchange.asmx";

		public const string EWSBPOSDURLFormat = "{0}/ews/exchange.asmx";

		public const string E14EWSUrlFormat = "https://{0}.outlook.com/ews/exchange.asmx";

		public const string EWSVersionKey = "EWSVersion";

		public const string SourceEWSVersionKey = "SourceEWSVersion";

		public const string TargetEWSVersionKey = "TargetEWSVersion";

		public const string SourceEwsUrlKey = "SourceEwsUrl";

		public const string TargetEwsUrlKey = "TargetEwsUrl";

		public const string ExchangeVersionKey = "ExchangeVersion";

		public const string XDiagInfoKey = "X-DiagInfo";

		public const string XFEServerKey = "X-FEServer";

		public const string RequestIdKey = "request-id";
	}
}
