using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "CalendarNotification", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetCalendarNotification : SetXsoObjectWithIdentityTaskBase<CalendarNotification>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetCalendarNotification(this.Identity.ToString());
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			return new VersionedXmlDataProvider(principal, userToken, "Set-CalendarNotification");
		}

		protected override void SaveXsoObject(IConfigDataProvider provider, IConfigurable dataObject)
		{
			base.SaveXsoObject(provider, dataObject);
			ADUser dataObject2 = this.DataObject;
			SmsSqmDataPointHelper.AddNotificationConfigDataPoint(SmsSqmSession.Instance, dataObject2.Id, dataObject2.LegacyExchangeDN, (CalendarNotification)dataObject);
		}
	}
}
