using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Remove", "FailedMSOSyncObject", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveFailedMSOSyncObject : RemoveTaskBase<FailedMSOSyncObjectIdParameter, FailedMSOSyncObject>
	{
		protected override ObjectId RootId
		{
			get
			{
				return ForwardSyncDataAccessHelper.GetRootId(this.Identity);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return ForwardSyncDataAccessHelper.CreateSession(false);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveFailedMSOSyncObject(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.DataObject.IsTenantWideDivergence && !this.Force)
			{
				base.WriteError(new CannotRemoveTenantWideDivergenceException(base.DataObject.Identity.ToString()), ExchangeErrorCategory.Client, base.DataObject);
			}
		}

		protected override void InternalProcessRecord()
		{
			ForwardSyncDataAccessHelper.CleanUpDivergenceIds((IConfigurationSession)base.DataSession, base.DataObject);
			if (base.DataObject.IsTenantWideDivergence)
			{
				this.MarkDivergenceHaltingForContext(base.DataObject.ObjectId.ContextId);
			}
			base.InternalProcessRecord();
		}

		private void MarkDivergenceHaltingForContext(string contextId)
		{
			lock (this)
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, FailedMSOSyncObjectSchema.ContextId, contextId),
					new ComparisonFilter(ComparisonOperator.Equal, FailedMSOSyncObjectSchema.IsTenantWideDivergence, false),
					new ComparisonFilter(ComparisonOperator.NotEqual, FailedMSOSyncObjectSchema.ExternalDirectoryObjectClass, DirectoryObjectClass.Company)
				});
				this.MarkDivergencesHalting(filter);
			}
		}

		private void MarkDivergencesHalting(QueryFilter filter)
		{
			ForwardSyncDataAccessHelper forwardSyncDataAccessHelper = new ForwardSyncDataAccessHelper(base.DataObject.ServiceInstanceId);
			IEnumerable<FailedMSOSyncObject> enumerable = forwardSyncDataAccessHelper.FindDivergence(filter);
			IConfigurationSession configurationSession = ForwardSyncDataAccessHelper.CreateSession(false);
			foreach (FailedMSOSyncObject failedMSOSyncObject in enumerable)
			{
				failedMSOSyncObject.IsIgnoredInHaltCondition = false;
				configurationSession.Save(failedMSOSyncObject);
			}
		}
	}
}
