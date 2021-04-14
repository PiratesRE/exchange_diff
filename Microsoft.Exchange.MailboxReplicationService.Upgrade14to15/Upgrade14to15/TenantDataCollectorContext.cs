using System;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TenantDataCollectorContext : AnchorContext
	{
		internal TenantDataCollectorContext() : base("TenantDataCollector", OrganizationCapability.TenantUpgrade, TenantDataCollectorContext.AnchorConfig)
		{
		}

		public static AnchorConfig AnchorConfig { get; private set; } = new TenantDataCollectorConfig();

		public override CacheProcessorBase[] CreateCacheComponents(WaitHandle stopEvent)
		{
			SymphonyProxy symphonyProxyInstance = new SymphonyProxy();
			OrgOperationProxy orgOperationProxyInstance = new OrgOperationProxy();
			return new CacheProcessorBase[]
			{
				new FirstOrgCacheScanner(this, stopEvent),
				new TenantDataCollectorScheduler(this, orgOperationProxyInstance, symphonyProxyInstance, stopEvent)
			};
		}

		public const string WebServiceUri = "WebServiceUri";

		public const string CertificateSubject = "CertificateSubject";

		public const string WorkLoadServiceName = "WorkloadService.svc";

		public const string E14DataDirectory = "E14DataDirectory";

		public const string E15DataDirectory = "E15DataDirectory";

		public const string UpgradeUnitsConversionFactor = "UpgradeUnitsConversionFactor";

		public const string CheckAllAccountPartitions = "CheckAllAccountPartitions";

		public const string UploadToSymphony = "UploadToSymphony";

		public const string ValidateMailboxVersions = "ValidateMailboxVersions";

		internal const string TenantDataCollectorApplicationName = "TenantDataCollector";
	}
}
