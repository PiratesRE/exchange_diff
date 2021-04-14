using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "CalendarNotification", DefaultParameterSetName = "Identity")]
	public sealed class GetCalendarNotification : GetXsoObjectWithIdentityTaskBase<CalendarNotification, ADUser>
	{
		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new VersionedXmlDataProvider(principal, "Get-CalendarNotification");
		}
	}
}
