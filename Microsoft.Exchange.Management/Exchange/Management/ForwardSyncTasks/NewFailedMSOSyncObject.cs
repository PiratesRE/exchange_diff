using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("New", "FailedMSOSyncObject", SupportsShouldProcess = true)]
	public sealed class NewFailedMSOSyncObject : Task
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public SyncObjectId ObjectId { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public ServiceInstanceId ServiceInstanceId { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsTemporary { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsIncrementalOnly { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsLinkRelated { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsTenantWideDivergence { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsValidationDivergence { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsRetriable { get; set; }

		public ObjectId Identity
		{
			get
			{
				return new CompoundSyncObjectId(this.ObjectId, this.ServiceInstanceId);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.forwardSyncDataAccessHelper == null)
			{
				this.forwardSyncDataAccessHelper = new ForwardSyncDataAccessHelper(this.ServiceInstanceId.InstanceId);
			}
			FailedMSOSyncObject existingDivergence = this.forwardSyncDataAccessHelper.GetExistingDivergence(this.ObjectId);
			if (existingDivergence != null)
			{
				this.forwardSyncDataAccessHelper.UpdateExistingDivergence(existingDivergence, 1, false, false, false, true, new string[]
				{
					"TenantWideDivergenceConvertedFromCompanyDivergence"
				}, 1, false, true, null);
			}
			else
			{
				this.forwardSyncDataAccessHelper.PersistNewDivergence(this.ObjectId, DateTime.UtcNow, this.IsIncrementalOnly, this.IsLinkRelated, this.IsTemporary, this.IsTenantWideDivergence, new string[]
				{
					"DivergenceInjectedFromOutside"
				}, this.IsValidationDivergence, this.IsRetriable, null);
			}
			if (this.IsTenantWideDivergence)
			{
				this.MarkDivergenceUnhaltingForContext(this.ObjectId.ContextId);
			}
		}

		protected override void InternalValidate()
		{
			Exception innerException;
			if (!NewFailedMSOSyncObject.IsValidGuid(this.ObjectId.ContextId, out innerException) || !NewFailedMSOSyncObject.IsValidGuid(this.ObjectId.ObjectId, out innerException))
			{
				base.WriteError(new LocalizedException(DirectoryStrings.ExArgumentException("ObjectId", this.ObjectId), innerException), ExchangeErrorCategory.Client, null);
			}
			if (this.IsTenantWideDivergence)
			{
				if (this.ObjectId.ObjectClass != DirectoryObjectClass.Company || string.CompareOrdinal(this.ObjectId.ContextId, this.ObjectId.ObjectId) != 0)
				{
					base.WriteError(new InvalidObjectIdForTenantWideDivergenceException(this.ObjectId.ToString()), ExchangeErrorCategory.Client, null);
				}
			}
			else if ((this.ObjectId.ObjectClass != DirectoryObjectClass.Company && string.CompareOrdinal(this.ObjectId.ContextId, this.ObjectId.ObjectId) == 0) || (this.ObjectId.ObjectClass == DirectoryObjectClass.Company && string.CompareOrdinal(this.ObjectId.ContextId, this.ObjectId.ObjectId) != 0))
			{
				base.WriteError(new LocalizedException(DirectoryStrings.ExArgumentException("ObjectId", this.ObjectId), innerException), ExchangeErrorCategory.Client, null);
			}
			if (this.forwardSyncDataAccessHelper == null)
			{
				this.forwardSyncDataAccessHelper = new ForwardSyncDataAccessHelper(this.ServiceInstanceId.InstanceId);
			}
			FailedMSOSyncObject existingDivergence = this.forwardSyncDataAccessHelper.GetExistingDivergence(this.ObjectId);
			if (existingDivergence != null && (!this.IsTenantWideDivergence || existingDivergence.IsTenantWideDivergence))
			{
				base.WriteError(new DivergenceAlreadyExistsException(this.ObjectId.ToString()), ExchangeErrorCategory.Client, existingDivergence);
			}
			if (this.IsValidationDivergence && this.IsTenantWideDivergence)
			{
				base.WriteError(new CannotBeBothValidationDivergenceAndTenantWideDivergenceException(this.ObjectId.ToString()), ExchangeErrorCategory.Client, null);
			}
			if (!this.IsValidationDivergence && !this.IsRetriable)
			{
				base.WriteError(new NoneValidationDivergenceMustBeRetriableException(this.ObjectId.ToString()), ExchangeErrorCategory.Client, null);
			}
			base.InternalValidate();
		}

		private static bool IsValidGuid(string guidString, out Exception parseException)
		{
			parseException = null;
			try
			{
				new Guid(guidString);
			}
			catch (FormatException ex)
			{
				parseException = ex;
				return false;
			}
			catch (OverflowException ex2)
			{
				parseException = ex2;
				return false;
			}
			return true;
		}

		private void MarkDivergenceUnhaltingForContext(string contextId)
		{
			lock (this)
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, FailedMSOSyncObjectSchema.ContextId, contextId),
					new ComparisonFilter(ComparisonOperator.Equal, FailedMSOSyncObjectSchema.IsTenantWideDivergence, false),
					new ComparisonFilter(ComparisonOperator.NotEqual, FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass, DirectoryObjectClass.Company)
				});
				this.MarkDivergencesUnhalting(filter);
			}
		}

		private void MarkDivergencesUnhalting(QueryFilter filter)
		{
			IEnumerable<FailedMSOSyncObject> enumerable = this.forwardSyncDataAccessHelper.FindDivergence(filter);
			IConfigurationSession configurationSession = ForwardSyncDataAccessHelper.CreateSession(false);
			foreach (FailedMSOSyncObject failedMSOSyncObject in enumerable)
			{
				failedMSOSyncObject.IsIgnoredInHaltCondition = true;
				configurationSession.Save(failedMSOSyncObject);
			}
		}

		private ForwardSyncDataAccessHelper forwardSyncDataAccessHelper;
	}
}
