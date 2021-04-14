using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics
{
	internal class MailboxStatisticsLogEntry : ConfigurableObject
	{
		public MailboxStatisticsLogEntry() : base(new SimpleProviderPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MailboxStatisticsLogEntrySchema>();
			}
		}
	}
}
