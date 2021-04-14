using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Get", "LogonStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetLogonStatistics : GetStatisticsBase<LogonableObjectIdParameter, LogonStatistics, LogonStatistics>
	{
	}
}
