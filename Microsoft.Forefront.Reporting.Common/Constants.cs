using System;

namespace Microsoft.Forefront.Reporting.Common
{
	public static class Constants
	{
		public const int CosmosRequestIDPadding = 1000000;

		public const string DateReplacement = "{DATE}";

		public const string HourReplacement = "{HOUR}";

		public const string SequenceReplacement = "{SN}";

		public const string RegionReplacement = "{REGION}";

		public const string BatchIDReplacement = "{BATCHID}";

		public const string OnDemandRequestIDReplacement = "{REQUEST_ID}";

		public const string ExchangeVCReplacement = "{EXO_VC}";

		public const string SharePointVCReplacement = "{SPO_VC}";

		public const string WorkDirctoryReplacement = "{WORK_DIR}";

		public const string DataMiningCosmosDll = "Microsoft.Datacenter.Datamining.Cosmos.dll";

		public const string EOPDataMiningDll = "Microsoft.Forefront.Reporting.Cosmos.dll";

		public const string ScopeLibDll = "Relevance.ScopeLib.dll";

		public const string EmptyStream = "Empty.log";

		public const string EmptyStream1 = "Empty1.log";

		public const bool CosmosCompression = true;

		public const char ParameterDelimiter = '\t';

		public const string StatusDelimiter = "__";

		public const char MsgStatusRecipientDelimiter = ';';

		public const char QueryDefinitionFieldSeperator = ',';

		public const string MsgStatusValueDemilter = "##";

		public static readonly Guid TenantIDForSystemCount = new Guid("ae3384e1-e9f9-4479-845b-a4c3ff006a66");
	}
}
