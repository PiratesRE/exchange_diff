using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum ApnsResponseStatus
	{
		None,
		ProcessingError,
		MissingDeviceToken,
		MissingTopic,
		MissingPayload,
		InvalidTokenSize,
		InvalidTopicSize,
		InvalidPayloadSize,
		InvalidToken,
		Unknown = 255
	}
}
