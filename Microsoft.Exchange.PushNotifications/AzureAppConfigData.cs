using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class AzureAppConfigData : BasicDataContract
	{
		public AzureAppConfigData(string appId, string serializedToken, string partition)
		{
			this.AppId = appId;
			this.SerializedToken = serializedToken;
			this.Partition = partition;
		}

		[DataMember(Name = "appId", EmitDefaultValue = false)]
		public string AppId { get; private set; }

		[DataMember(Name = "serializedToken", EmitDefaultValue = false)]
		public string SerializedToken { get; private set; }

		[DataMember(Name = "partition", EmitDefaultValue = false)]
		public string Partition { get; private set; }

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("appId:").Append(this.AppId.ToNullableString()).Append("; ");
			sb.Append("serializedToken:").Append(this.SerializedToken.ToNullableString()).Append("; ");
			sb.Append("partition:").Append(this.Partition.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (string.IsNullOrWhiteSpace(this.AppId))
			{
				errors.Add(Strings.InvalidAppId);
			}
			if (string.IsNullOrWhiteSpace(this.SerializedToken))
			{
				errors.Add(Strings.InvalidSerializedToken);
			}
		}

		public const int DefaultTokenExpirationTimeInSeconds = 93600;
	}
}
