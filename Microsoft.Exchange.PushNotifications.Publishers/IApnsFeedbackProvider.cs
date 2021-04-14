using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IApnsFeedbackProvider
	{
		ApnsFeedbackValidationResult ValidateNotification(ApnsNotification notification);
	}
}
