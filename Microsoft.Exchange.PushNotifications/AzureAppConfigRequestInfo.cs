using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class AzureAppConfigRequestInfo : BasicNotification
	{
		public AzureAppConfigRequestInfo(string[] appIds) : base(null)
		{
			this.AppIds = appIds;
		}

		[DataMember(Name = "appIds", EmitDefaultValue = false)]
		public string[] AppIds { get; private set; }

		public override string ToJson()
		{
			return JsonConverter.Serialize<AzureAppConfigRequestInfo>(this, null);
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("appIds:").Append(this.AppIds.ToNullableString(null)).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (this.AppIds == null || this.AppIds.Length == 0)
			{
				errors.Add(Strings.InvalidListOfAppIds);
			}
		}
	}
}
