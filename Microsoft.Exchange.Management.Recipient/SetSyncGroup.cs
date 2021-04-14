using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "SyncGroup", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSyncGroup : SetRecipientObjectTask<NonMailEnabledGroupIdParameter, SyncGroup, ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetGroup(this.Identity.ToString());
			}
		}

		[Parameter]
		public GroupType Type
		{
			get
			{
				return (GroupType)(base.Fields[ADGroupSchema.GroupType] ?? GroupType.Distribution);
			}
			set
			{
				base.Fields[ADGroupSchema.GroupType] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADGroup adgroup = (ADGroup)base.PrepareDataObject();
			if (base.Fields.IsModified(ADGroupSchema.GroupType))
			{
				if (this.Type != GroupType.Distribution && this.Type != GroupType.Security)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorGroupTypeInvalid), ExchangeErrorCategory.Client, null);
				}
				bool flag = (adgroup.GroupType & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled;
				if (this.Type == GroupType.Distribution && flag)
				{
					adgroup.GroupType &= (GroupTypeFlags)2147483647;
				}
				else if (this.Type == GroupType.Security && !flag)
				{
					adgroup.GroupType |= GroupTypeFlags.SecurityEnabled;
				}
			}
			return adgroup;
		}
	}
}
