using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Set", "FailedMSOSyncObject", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetFailedMSOSyncObject : SetObjectWithIdentityTaskBase<FailedMSOSyncObjectIdParameter, FailedMSOSyncObjectPresentationObject, FailedMSOSyncObject>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetFailedMSOSyncObject(this.Identity.ToString());
			}
		}

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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.IsValidationDivergence && !this.DataObject.IsIgnoredInHaltCondition)
			{
				base.WriteError(new ValidationDivergenceMustBeNonHaltingException(this.DataObject.Identity.ToString()), ExchangeErrorCategory.Client, this.DataObject);
			}
			TaskLogger.LogExit();
		}
	}
}
