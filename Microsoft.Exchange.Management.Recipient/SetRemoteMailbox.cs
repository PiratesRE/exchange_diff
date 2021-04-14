using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("set", "RemoteMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRemoteMailbox : SetMailUserBase<RemoteMailboxIdParameter, RemoteMailbox>
	{
		[Parameter(Mandatory = false)]
		public ConvertibleRemoteMailboxSubType Type
		{
			get
			{
				return (ConvertibleRemoteMailboxSubType)base.Fields["Type"];
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ACLableSyncedObjectEnabled
		{
			get
			{
				return (SwitchParameter)(base.Fields["ACLableSyncedObjectEnabled"] ?? false);
			}
			set
			{
				base.Fields["ACLableSyncedObjectEnabled"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRemoteMailbox(this.Identity.ToString());
			}
		}

		private new SwitchParameter BypassLiveId
		{
			get
			{
				return base.BypassLiveId;
			}
			set
			{
				base.BypassLiveId = value;
			}
		}

		private new string FederatedIdentity
		{
			get
			{
				return base.FederatedIdentity;
			}
			set
			{
				base.FederatedIdentity = value;
			}
		}

		private new NetID NetID
		{
			get
			{
				return base.NetID;
			}
			set
			{
				base.NetID = value;
			}
		}

		private new Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
			set
			{
				base.SKUCapability = value;
			}
		}

		private new MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
			set
			{
				base.AddOnSKUCapability = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			if (base.Fields.IsModified("Type"))
			{
				RemoteMailboxType type = (RemoteMailboxType)this.Type;
				aduser.UpdateRemoteMailboxType(type, this.ACLableSyncedObjectEnabled);
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return RemoteMailbox.FromDataObject((ADUser)dataObject);
		}

		public const string ParameterACLableSyncedEnabled = "ACLableSyncedObjectEnabled";
	}
}
