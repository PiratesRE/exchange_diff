using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Providers;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("New", "MailMessage", SupportsShouldProcess = true)]
	public sealed class NewMailMessage : NewTenantADTaskBase<MailMessage>
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Subject
		{
			get
			{
				return this.DataObject.Subject;
			}
			set
			{
				this.DataObject.Subject = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public string Body
		{
			get
			{
				return this.DataObject.Body;
			}
			set
			{
				this.DataObject.Body = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailBodyFormat BodyFormat
		{
			get
			{
				return this.DataObject.BodyFormat;
			}
			set
			{
				this.DataObject.BodyFormat = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is AdUserNotFoundException;
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADObjectId identity;
			if (!base.TryGetExecutingUserId(out identity))
			{
				throw new ExecutingUserPropertyNotFoundException("executingUserid");
			}
			this.mailboxUser = (ADUser)base.TenantGlobalCatalogSession.Read<ADUser>(identity);
			return new MailMessageConfigDataProvider(base.TenantGlobalCatalogSession, this.mailboxUser);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			if (this.mailboxUser != null && this.mailboxUser.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new TaskException(Strings.ErrorCannotSendMailToPublicFolderMailbox(this.mailboxUser.Name)), ErrorCategory.InvalidArgument, this);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					if (string.IsNullOrEmpty(this.Subject) && string.IsNullOrEmpty(this.Body))
					{
						this.WriteWarning(Strings.EmptyMesssageWillBeCreated);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailMessage(this.mailboxUser.Name);
			}
		}

		private ADUser mailboxUser;
	}
}
