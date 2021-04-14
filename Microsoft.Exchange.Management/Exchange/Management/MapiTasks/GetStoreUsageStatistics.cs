using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Get", "StoreUsageStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetStoreUsageStatistics : GetStatisticsBase<GeneralMailboxIdParameter, MailboxResourceMonitor, MailboxResourceMonitor>
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				MonadFilter monadFilter = new MonadFilter(value, this, ObjectSchema.GetInstance<MailboxResourceMonitorSchema>());
				this.inputFilter = monadFilter.InnerFilter;
				base.OptionalIdentityData.AdditionalFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MailboxResourceMonitor mailboxResourceMonitor = (MailboxResourceMonitor)dataObject;
			if (this.inputFilter != null)
			{
				if (OpathFilterEvaluator.FilterMatches(this.inputFilter, mailboxResourceMonitor))
				{
					base.WriteResult(dataObject);
					return;
				}
			}
			else
			{
				base.WriteResult(mailboxResourceMonitor);
			}
		}

		private QueryFilter inputFilter;
	}
}
