using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[Serializable]
	public sealed class ApnsFeedbackManagerSettings : RegistryObject
	{
		public int ExpirationThresholdInMilliseconds
		{
			get
			{
				return (int)this[ApnsFeedbackManagerSettings.Schema.ExpirationThresholdInMilliseconds];
			}
			set
			{
				this[ApnsFeedbackManagerSettings.Schema.ExpirationThresholdInMilliseconds] = value;
			}
		}

		public int UpdateIntervalInMilliseconds
		{
			get
			{
				return (int)this[ApnsFeedbackManagerSettings.Schema.UpdateIntervalInMilliseconds];
			}
			set
			{
				this[ApnsFeedbackManagerSettings.Schema.UpdateIntervalInMilliseconds] = value;
			}
		}

		internal override RegistryObjectSchema RegistrySchema
		{
			get
			{
				return ApnsFeedbackManagerSettings.schema;
			}
		}

		private static readonly ApnsFeedbackManagerSettings.Schema schema = ObjectSchema.GetInstance<ApnsFeedbackManagerSettings.Schema>();

		internal class Schema : RegistryObjectSchema
		{
			public override string DefaultRegistryKeyPath
			{
				get
				{
					return "SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications";
				}
			}

			public override string DefaultName
			{
				get
				{
					return "Feedback";
				}
			}

			public const string RegistryRootName = "Feedback";

			public static readonly RegistryObjectId RootObjectId = new RegistryObjectId("SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications", "Feedback");

			public static readonly RegistryPropertyDefinition ExpirationThresholdInMilliseconds = new RegistryPropertyDefinition("ExpirationThresholdInMilliseconds", typeof(int), 259200000);

			public static readonly RegistryPropertyDefinition UpdateIntervalInMilliseconds = new RegistryPropertyDefinition("UpdateIntervalInMilliseconds", typeof(int), 43200000);
		}
	}
}
