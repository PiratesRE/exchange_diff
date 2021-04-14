using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.ImportContacts;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Import", "ContactList", DefaultParameterSetName = "Data", SupportsShouldProcess = true)]
	public sealed class ImportContactList : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Data", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Stream", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
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

		[Parameter(Mandatory = true, ParameterSetName = "Data")]
		[Parameter(Mandatory = true, ParameterSetName = "Stream")]
		public SwitchParameter CSV
		{
			get
			{
				return (SwitchParameter)(base.Fields["CSV"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CSV"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Stream")]
		public Stream CSVStream
		{
			get
			{
				return this.csvStream;
			}
			set
			{
				this.csvStream = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Data")]
		public byte[] CSVData
		{
			get
			{
				return (byte[])base.Fields["CSVData"];
			}
			set
			{
				base.Fields["CSVData"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ImportContactListConfirmation(this.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ADSessionSettings adSettings = ADSessionSettings.RescopeToOrganization(base.SessionSettings, this.DataObject.OrganizationId, true);
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(adSettings, this.DataObject, RemotingOptions.AllowCrossSite);
				if (this.CSV == true)
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1260UL, "Importing contacts from CSV.", new object[0]);
					if (this.CSVStream != null)
					{
						this.ImportContactsFromStream(this.CSVStream, exchangePrincipal);
					}
					else
					{
						using (Stream stream = new MemoryStream(this.CSVData))
						{
							this.ImportContactsFromStream(stream, exchangePrincipal);
						}
					}
				}
				base.InternalProcessRecord();
			}
			catch (ObjectNotFoundException innerException)
			{
				this.WriteDebugInfoAndError(new MailboxFailureException(innerException), (ErrorCategory)1002, this.DataObject);
			}
			finally
			{
				this.WriteDebugInfo();
			}
			TaskLogger.LogExit();
		}

		private void ImportContactsFromStream(Stream csvStream, ExchangePrincipal exchangePrincipal)
		{
			int num = 0;
			LocalizedException ex = null;
			try
			{
				CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1261UL, "Opening Mailbox Session to mailbox {0}.", new object[]
				{
					this.DataObject.Identity
				});
				using (MailboxSession mailboxSession = SubscriptionManager.OpenMailbox(exchangePrincipal, ExchangeMailboxOpenType.AsAdministrator, ImportContactList.ClientInfoString))
				{
					CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1262UL, "Start Contacts Import to mailbox {0}.", new object[]
					{
						this.DataObject.Identity
					});
					OutlookCsvImportContact outlookCsvImportContact = new OutlookCsvImportContact(mailboxSession);
					num = outlookCsvImportContact.ImportContactsFromOutlookCSV(csvStream);
				}
			}
			catch (ImportContactsException ex2)
			{
				ex = ex2;
			}
			catch (LocalizedException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				this.WriteDebugInfoAndError(ex, (ErrorCategory)1003, this.DataObject);
			}
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)1263UL, "Imported {0} contacts to mailbox {1}.", new object[]
			{
				num,
				this.DataObject.Identity
			});
			ImportContactListResult importContactListResult = new ImportContactListResult(this.DataObject.Identity);
			importContactListResult.ContactsImported = num;
			importContactListResult.ResetChangeTracking();
			base.WriteObject(importContactListResult);
		}

		private void WriteDebugInfoAndError(Exception exception, ErrorCategory category, object target)
		{
			this.WriteDebugInfo();
			base.WriteError(exception, category, target);
		}

		private void WriteDebugInfo()
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(CommonLoggingHelper.SyncLogSession.GetBlackBoxText());
			}
			CommonLoggingHelper.SyncLogSession.ClearBlackBox();
		}

		private const string ParameterSetNameStream = "Stream";

		private const string ParameterSetNameData = "Data";

		private static readonly string ClientInfoString = "Client=TransportSync;Action=ImportContacts";

		private Stream csvStream;
	}
}
