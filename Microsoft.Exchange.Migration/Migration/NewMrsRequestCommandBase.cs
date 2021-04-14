using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	internal class NewMrsRequestCommandBase : MrsAccessorCommand
	{
		protected NewMrsRequestCommandBase(string cmdletName, ICollection<Type> ignoredExceptions) : base(cmdletName, ignoredExceptions, null)
		{
		}

		public bool SuspendWhenReadyToComplete
		{
			set
			{
				base.AddParameter("SuspendWhenReadyToComplete", value);
			}
		}

		public TimeSpan IncrementalSyncInterval
		{
			set
			{
				base.AddParameter("IncrementalSyncInterval", value);
			}
		}

		public Unlimited<int> BadItemLimit
		{
			set
			{
				base.AddParameter("BadItemLimit", value);
				Unlimited<int> value2 = new Unlimited<int>(TestIntegration.Instance.LargeDataLossThreshold);
				if (value >= value2)
				{
					this.AcceptLargeDataLoss = true;
				}
			}
		}

		public Unlimited<int> LargeItemLimit
		{
			set
			{
				base.AddParameter("LargeItemLimit", value);
				Unlimited<int> value2 = new Unlimited<int>(TestIntegration.Instance.LargeDataLossThreshold);
				if (value >= value2)
				{
					this.AcceptLargeDataLoss = true;
				}
			}
		}

		public bool AcceptLargeDataLoss
		{
			set
			{
				if (value && !this.hasAcceptedLargeDataLoss)
				{
					base.AddParameter("AcceptLargeDataLoss");
					this.hasAcceptedLargeDataLoss = true;
				}
			}
		}

		public string BatchName
		{
			set
			{
				base.AddParameter("BatchName", value);
			}
		}

		internal const string AcceptLargeDataLossParameter = "AcceptLargeDataLoss";

		private bool hasAcceptedLargeDataLoss;
	}
}
