using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ContactLinking
{
	public abstract class ContactLinkingBaseCmdLet : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		internal ContactLinkingPerformanceTracker PerformanceTracker { get; private set; }

		protected abstract string UserAgent { get; }

		protected virtual bool OwnsPerformanceTrackerLifeCycle
		{
			get
			{
				return true;
			}
		}

		internal abstract void ContactLinkingOperation(MailboxSession mailboxSession);

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, this.DataObject, false, this.ConfirmationMessage, null);
			try
			{
				using (MailboxSession mailboxSession = ContactLinkingBaseCmdLet.OpenMailboxSessionAsAdmin(this.DataObject, this.UserAgent))
				{
					this.PerformanceTracker = new ContactLinkingPerformanceTracker(mailboxSession);
					if (this.OwnsPerformanceTrackerLifeCycle)
					{
						this.PerformanceTracker.Start();
					}
					this.ContactLinkingOperation(mailboxSession);
				}
			}
			catch (Exception ex)
			{
				TaskLogger.LogError(ex);
				this.WriteError(ex, ErrorCategory.NotSpecified, this.Identity, true);
			}
			finally
			{
				if (this.OwnsPerformanceTrackerLifeCycle && this.PerformanceTracker != null)
				{
					this.PerformanceTracker.Stop();
				}
				this.WritePerformanceData();
				TaskLogger.LogExit();
			}
		}

		private static MailboxSession OpenMailboxSessionAsAdmin(ADUser user, string userAgent)
		{
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(user, null);
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, userAgent);
		}

		private static PSObject CreateResultObject(MailboxIdParameter mailbox, ILogEvent logEvent)
		{
			PSObject psobject = new PSObject();
			psobject.Properties.Add(new PSNoteProperty("Mailbox", mailbox));
			foreach (KeyValuePair<string, object> keyValuePair in logEvent.GetEventData())
			{
				psobject.Properties.Add(new PSNoteProperty(keyValuePair.Key, keyValuePair.Value));
			}
			return psobject;
		}

		private void WritePerformanceData()
		{
			if (this.PerformanceTracker != null)
			{
				ILogEvent logEvent = this.PerformanceTracker.GetLogEvent();
				base.WriteObject(ContactLinkingBaseCmdLet.CreateResultObject(this.Identity, logEvent));
			}
		}
	}
}
