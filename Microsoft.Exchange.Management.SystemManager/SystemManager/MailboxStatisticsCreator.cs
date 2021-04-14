using System;
using System.Data;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MailboxStatisticsCreator : IDataObjectCreator
	{
		public object Create(DataTable table)
		{
			MailboxStatistics mailboxStatistics = new MailboxStatistics();
			mailboxStatistics.Dispose();
			return mailboxStatistics;
		}
	}
}
