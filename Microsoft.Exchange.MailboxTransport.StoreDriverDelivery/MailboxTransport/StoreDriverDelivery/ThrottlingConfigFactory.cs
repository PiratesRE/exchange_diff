using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class ThrottlingConfigFactory
	{
		public static IThrottlingConfig Create()
		{
			if (ThrottlingConfigFactory.InstanceBuilder != null)
			{
				return ThrottlingConfigFactory.InstanceBuilder();
			}
			return new ThrottlingConfig();
		}

		internal static ThrottlingConfigFactory.ThrottlingConfigBuilder InstanceBuilder;

		public delegate IThrottlingConfig ThrottlingConfigBuilder();
	}
}
