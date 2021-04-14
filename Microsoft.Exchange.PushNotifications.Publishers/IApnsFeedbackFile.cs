using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IApnsFeedbackFile : IApnsFeedbackProvider, IEquatable<IApnsFeedbackFile>
	{
		ApnsFeedbackFileId Identifier { get; }

		ApnsFeedbackFileIO FileIO { get; }

		ITracer Tracer { get; }

		bool IsLoaded { get; }

		void Load();

		void Remove();

		bool HasExpired(TimeSpan expirationThreshold);
	}
}
