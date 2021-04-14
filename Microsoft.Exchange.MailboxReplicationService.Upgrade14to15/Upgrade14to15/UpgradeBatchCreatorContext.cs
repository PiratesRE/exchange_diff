using System;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpgradeBatchCreatorContext : AnchorContext
	{
		public UpgradeBatchCreatorContext() : base("UpgradeBatchCreator", OrganizationCapability.TenantUpgrade, new UpgradeBatchCreatorConfig())
		{
		}

		public override CacheProcessorBase[] CreateCacheComponents(WaitHandle stopEvent)
		{
			OrgOperationProxy orgOperationProxyInstance = new OrgOperationProxy();
			return new CacheProcessorBase[]
			{
				new CacheScanner(this, stopEvent),
				new UpgradeBatchCreatorScheduler(this, orgOperationProxyInstance, stopEvent)
			};
		}

		public const string BatchFileDirectoryPath = "BatchFileDirectoryPath";

		public const string MaxBatchSize = "MaxBatchSize";

		public const string UpgradeBatchFilenamePrefix = "UpgradeBatchFilenamePrefix";

		public const string DryRunBatchFilenamePrefix = "DryRunBatchFilenamePrefix";

		public const string E14CountUpdateIntervalName = "E14CountUpdateInterval";

		public const string RemoveNonUpgradeMoveRequests = "RemoveNonUpgradeMoveRequests";

		public const string ConfigOnly = "ConfigOnly";

		public const string DelayUntilCreateNewBatches = "DelayUntilCreateNewBatches";

		internal const string BatchFileDirectoryName = "UpgradeBatches";

		internal const string UpgradeApplicationName = "UpgradeBatchCreator";
	}
}
