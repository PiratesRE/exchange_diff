using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "GroupMailbox", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveGroupMailbox : RemoveMailboxBase<MailboxIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNullOrEmpty]
		public RecipientIdParameter ExecutingUser
		{
			get
			{
				return (RecipientIdParameter)base.Fields["ExecutingUser"];
			}
			set
			{
				base.Fields["ExecutingUser"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public RecipientIdType RecipientIdType
		{
			get
			{
				return (RecipientIdType)base.Fields["RecipientIdType"];
			}
			set
			{
				base.Fields["RecipientIdType"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter FromSyncClient
		{
			get
			{
				return (SwitchParameter)(base.Fields["FromSyncClient"] ?? false);
			}
			set
			{
				base.Fields["FromSyncClient"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new SwitchParameter ForReconciliation
		{
			get
			{
				return base.ForReconciliation;
			}
			set
			{
				base.ForReconciliation = value;
			}
		}

		private new SwitchParameter Arbitration { get; set; }

		private new DatabaseIdParameter Database { get; set; }

		private new SwitchParameter Disconnect { get; set; }

		private new SwitchParameter IgnoreDefaultScope { get; set; }

		private new SwitchParameter IgnoreLegalHold { get; set; }

		private new SwitchParameter KeepWindowsLiveID { get; set; }

		private new SwitchParameter PublicFolder { get; set; }

		private new SwitchParameter RemoveLastArbitrationMailboxAllowed { get; set; }

		private new StoreMailboxIdParameter StoreMailboxIdentity { get; set; }

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (adrecipient.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ExchangeErrorCategory.Client, this.Identity);
			}
			return adrecipient;
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new RemoveGroupMailboxTaskModuleFactory();
		}

		private const string ParameterExecutingUser = "ExecutingUser";

		private const string ParameterRecipientIdType = "RecipientIdType";

		private const string ParameterFromSyncClient = "FromSyncClient";
	}
}
