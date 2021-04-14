using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExportMailboxChangesRow : BaseRow
	{
		public ExportMailboxChangesRow(MailboxAuditLogSearch searchResult)
		{
		}
	}
}
