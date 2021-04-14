using System;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.PushNotifications.Server.Wcf
{
	internal class PublishUserNotificationBehaviorExtension : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get
			{
				return typeof(PublishUserNotificationBehavior);
			}
		}

		protected override object CreateBehavior()
		{
			return new PublishUserNotificationBehavior();
		}
	}
}
