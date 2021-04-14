using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Add", "Mailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class AddMailbox : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter AuxArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuxArchive"] ?? false);
			}
			set
			{
				base.Fields["AuxArchive"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AuxPrimary
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuxPrimary"] ?? false);
			}
			set
			{
				base.Fields["AuxPrimary"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			IMailboxLocationCollection mailboxLocations = this.DataObject.MailboxLocations;
			if (this.AuxArchive)
			{
				if (mailboxLocations.GetMailboxLocation(MailboxLocationType.MainArchive) == null)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxHasNoArchive(this.Identity.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			else if (this.AuxPrimary && this.DataObject.RecipientTypeDetails != RecipientTypeDetails.AuditLogMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMailboxIsNotAudit(this.Identity.ToString())), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADUser dataObject = this.DataObject;
			IMailboxLocationCollection mailboxLocations = dataObject.MailboxLocations;
			if (this.AuxArchive)
			{
				IMailboxLocationInfo mailboxLocation = mailboxLocations.GetMailboxLocation(MailboxLocationType.MainArchive);
				mailboxLocations.AddMailboxLocation(Guid.NewGuid(), mailboxLocation.DatabaseLocation, MailboxLocationType.AuxArchive);
			}
			else if (this.AuxPrimary)
			{
				mailboxLocations.AddMailboxLocation(Guid.NewGuid(), dataObject.Database, MailboxLocationType.AuxPrimary);
			}
			base.InternalProcessRecord();
			this.WriteResult();
			TaskLogger.LogExit();
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			Mailbox sendToPipeline = new Mailbox(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}
	}
}
