using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MailboxAssociationReplicationState", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxAssociationReplicationState : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public override MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser aduser = (ADUser)dataObject;
			MailboxAssociationReplicationStatePresentationObject mailboxAssociationReplicationStatePresentationObject = new MailboxAssociationReplicationStatePresentationObject
			{
				Identity = aduser.Identity
			};
			if (CmdletProxy.TryToProxyOutputObject(mailboxAssociationReplicationStatePresentationObject, base.CurrentTaskContext, aduser, this.Identity == null, this.ConfirmationMessage, CmdletProxy.AppendIdentityToProxyCmdlet(aduser)))
			{
				return mailboxAssociationReplicationStatePresentationObject;
			}
			try
			{
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(aduser, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management;Action=Get-MailboxAssociationReplicationState"))
				{
					mailboxSession.Mailbox.Load(GetMailboxAssociationReplicationState.MailboxProperties);
					return new MailboxAssociationReplicationStatePresentationObject
					{
						Identity = aduser.Identity,
						NextReplicationTime = new ExDateTime?(mailboxSession.Mailbox.GetValueOrDefault<ExDateTime>(MailboxSchema.MailboxAssociationNextReplicationTime, ExDateTime.MinValue))
					};
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.Identity);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, this.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StorageTransientException || exception is StoragePermanentException || base.IsKnownException(exception);
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;

		private static readonly PropertyDefinition[] MailboxProperties = new PropertyDefinition[]
		{
			MailboxSchema.MailboxAssociationNextReplicationTime
		};
	}
}
