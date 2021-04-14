using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CalendarConfigurationBase : BaseRow
	{
		public CalendarConfigurationBase(MailboxCalendarConfiguration mailboxCalendarConfiguration) : base(mailboxCalendarConfiguration)
		{
			this.MailboxCalendarConfiguration = mailboxCalendarConfiguration;
		}

		public MailboxCalendarConfiguration MailboxCalendarConfiguration { get; private set; }
	}
}
