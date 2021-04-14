using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class ExProgressRecord : ProgressRecord
	{
		public ExProgressRecord(int activityId, LocalizedString activity, LocalizedString statusDescription) : base(activityId, activity.ToString(), statusDescription.ToString())
		{
		}
	}
}
