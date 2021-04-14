using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Probes
{
	public abstract class OwaLocalLogonProbe : OwaLogonProbe
	{
		protected MailboxDatabaseInfo GetMailboxDatabaseInfo(ProbeDefinition definition, ICollection<MailboxDatabaseInfo> mailboxDatabases)
		{
			if (!string.IsNullOrEmpty(definition.TargetResource))
			{
				foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabases)
				{
					if (mailboxDatabaseInfo.MailboxDatabaseName.Equals(definition.TargetResource))
					{
						return mailboxDatabaseInfo;
					}
				}
				throw new ArgumentException(Strings.OwaMailboxDatabaseDoesntExist(definition.TargetResource));
			}
			int index = new Random().Next(mailboxDatabases.Count);
			return mailboxDatabases.ElementAt(index);
		}
	}
}
