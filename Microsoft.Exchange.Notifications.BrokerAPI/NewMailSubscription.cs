using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class NewMailSubscription : BaseSubscription
	{
		public NewMailSubscription() : base(NotificationType.NewMail)
		{
		}

		protected override bool Validate()
		{
			return base.Validate() && base.CultureInfo.Equals(CultureInfo.InvariantCulture);
		}
	}
}
