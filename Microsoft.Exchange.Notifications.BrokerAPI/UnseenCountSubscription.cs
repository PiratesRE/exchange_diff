using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class UnseenCountSubscription : BaseSubscription
	{
		public UnseenCountSubscription() : base(NotificationType.UnseenCount)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public string UserExternalDirectoryObjectId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string UserLegacyDN { get; set; }

		protected override bool Validate()
		{
			return base.Validate() && (!string.IsNullOrEmpty(this.UserExternalDirectoryObjectId) || !string.IsNullOrEmpty(this.UserLegacyDN)) && base.CultureInfo.Equals(CultureInfo.InvariantCulture);
		}
	}
}
