using System;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeHandlerContext : AnchorContext
	{
		internal UpgradeHandlerContext() : base("UpgradeHandler", OrganizationCapability.TenantUpgrade, UpgradeHandlerContext.AnchorConfig)
		{
		}

		public static AnchorConfig AnchorConfig { get; private set; } = new UpgradeHandlerConfig();

		public override CacheProcessorBase[] CreateCacheComponents(WaitHandle stopEvent)
		{
			SymphonyProxy symphonyProxyInstance = new SymphonyProxy();
			OrgOperationProxy orgOperationProxyInstance = new OrgOperationProxy();
			return new CacheProcessorBase[]
			{
				new FirstOrgCacheScanner(this, stopEvent),
				new UpgradeHandlerScheduler(this, orgOperationProxyInstance, symphonyProxyInstance, stopEvent)
			};
		}

		public const string WebServiceUri = "WebServiceUri";

		public const string CertificateSubject = "CertificateSubject";

		public const string WorkLoadServiceName = "WorkloadService.svc";

		public const string NumberOfSetMailboxAttempts = "NumberOfSetMailboxAttempts";

		public const string SetMailboxAttemptIntervalSeconds = "SetMailboxAttemptIntervalSeconds";

		internal const string UpgradeApplicationName = "UpgradeHandler";
	}
}
