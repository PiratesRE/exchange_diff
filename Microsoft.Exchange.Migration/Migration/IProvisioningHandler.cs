using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal interface IProvisioningHandler
	{
		void RegisterJob(Guid jobId, CultureInfo cultureInfo, Guid ownerExchangeObjectId, ADObjectId ownerId, DelegatedPrincipal delegatedAdminOwner, SubmittedByUserAdminType migrationRequesterRole, string tenantOrganization, OrganizationId organizationId = null);

		void UnregisterJob(Guid jobId);

		bool IsJobRegistered(Guid jobId);

		bool CanUnregisterJob(Guid jobId);

		bool HasCapacity(Guid jobId);

		bool QueueItem(Guid jobId, ObjectId itemId, IProvisioningData provisioningData);

		bool IsItemQueued(ObjectId itemId);

		bool IsItemCompleted(ObjectId itemId);

		void CancelItem(ObjectId itemId);

		ProvisionedObject DequeueItem(ObjectId itemId);
	}
}
