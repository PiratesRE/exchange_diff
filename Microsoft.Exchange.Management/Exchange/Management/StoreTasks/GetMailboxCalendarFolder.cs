using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "MailboxCalendarFolder", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxCalendarFolder : GetTenantXsoObjectWithFolderIdentityTaskBase<MailboxCalendarFolder>
	{
		protected override ObjectId RootId
		{
			get
			{
				return null;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return false;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			ADUser mailboxOwner = this.PrepareMailboxUser();
			base.InnerMailboxFolderDataProvider = new MailboxCalendarFolderDataProvider(base.SessionSettings, mailboxOwner, "Get-MailboxCalendarFolder");
			return base.InnerMailboxFolderDataProvider;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			LocalizedString? localizedString;
			IEnumerable<MailboxCalendarFolder> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
			this.WriteResult<MailboxCalendarFolder>(dataObjects);
			TaskLogger.LogExit();
		}
	}
}
