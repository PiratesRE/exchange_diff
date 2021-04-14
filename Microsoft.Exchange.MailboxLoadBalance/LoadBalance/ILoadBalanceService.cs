using System;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[ServiceContract(SessionMode = SessionMode.Allowed)]
	internal interface ILoadBalanceService : IVersionedService, IDisposeTrackable, IDisposable
	{
		[OperationContract]
		void BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric);

		[OperationContract]
		SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data);

		[OperationContract]
		void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity identity, ByteQuantifiedSize targetSize);

		[OperationContract]
		Band[] GetActiveBands();

		[OperationContract]
		HeatMapCapacityData GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData);

		[OperationContract]
		BatchCapacityDatum GetConsumerBatchCapacity(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize);

		[OperationContract]
		MailboxProvisioningResult GetDatabaseForProvisioning(MailboxProvisioningData provisioningData);

		[OperationContract]
		DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryDatabase database);

		[OperationContract(Name = "GetDatabaseSizeInformation2")]
		DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryIdentity database);

		[OperationContract]
		MailboxProvisioningResult GetLocalDatabaseForProvisioning(MailboxProvisioningData provisioningData);

		[OperationContract(Name = "GetLocalServerData2")]
		LoadContainer GetLocalServerData(Band[] bands);
	}
}
