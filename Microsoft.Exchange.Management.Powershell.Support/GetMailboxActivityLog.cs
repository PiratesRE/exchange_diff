using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "MailboxActivityLog", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxActivityLog : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, Position = 0)]
		public new MailboxIdParameter Identity
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

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			try
			{
				if (this.Identity != null)
				{
					LocalizedString? localizedString;
					IEnumerable<ADUser> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
					this.WriteResult<ADUser>(dataObjects);
				}
				else
				{
					base.InternalProcessRecord();
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			try
			{
				if (dataObject != null)
				{
					ADUser user = (ADUser)dataObject;
					ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(user, null);
					using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management;Action=Get-MailboxActivityLog"))
					{
						IActivityLog activityLog = ActivityLogFactory.Current.Bind(mailboxSession);
						foreach (Activity activity in activityLog.Query())
						{
							base.WriteResult(new ActivityLogEntryPresentationObject(activity));
						}
					}
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}
	}
}
