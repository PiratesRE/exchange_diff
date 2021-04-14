using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "MailboxCalendarConfiguration")]
	public sealed class GetMailboxCalendarConfiguration : GetMailboxConfigurationTaskBase<MailboxCalendarConfiguration>
	{
	}
}
