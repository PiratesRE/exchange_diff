using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class AzureHubDefinition : Notification
	{
		public AzureHubDefinition(string hubName, string targetAppId) : base(PushNotificationCannedApp.AzureHubCreation.Name, hubName, null)
		{
			this.TargetAppId = targetAppId;
		}

		[DataMember(Name = "targetAppId", EmitDefaultValue = false)]
		public string TargetAppId { get; private set; }

		public string HubName
		{
			get
			{
				return base.RecipientId;
			}
		}

		public static AzureHubDefinition CreateMonitoringHubDefinition(string hubName, string targetAppId)
		{
			return new AzureHubDefinition(hubName, targetAppId)
			{
				IsMonitoring = true
			};
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("targetAppId:").Append(this.TargetAppId.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (string.IsNullOrWhiteSpace(this.TargetAppId))
			{
				errors.Add(Strings.InvalidTargetAppId(base.GetType().Name));
			}
		}
	}
}
