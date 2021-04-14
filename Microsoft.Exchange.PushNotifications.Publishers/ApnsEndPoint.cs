using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsEndPoint
	{
		public ApnsEndPoint(string host, int port, string feedbackHost, int feedbackPort)
		{
			this.Host = host;
			this.Port = port;
			this.FeedbackHost = feedbackHost;
			this.FeedbackPort = feedbackPort;
		}

		public string Host { get; private set; }

		public string FeedbackHost { get; private set; }

		public int Port { get; private set; }

		public int FeedbackPort { get; private set; }

		public const string DefaultSandboxHost = "gateway.sandbox.push.apple.com";

		public const string DefaultSandboxFeedbackHost = "feedback.sandbox.push.apple.com";

		public const string DefaultProductionHost = "gateway.push.apple.com";

		public const string DefaultProductionFeedbackHost = "feedback.push.apple.com";

		public const int DefaultPort = 2195;

		public const int DefaultFeedbackPort = 2196;

		internal static readonly ApnsEndPoint Sandbox = new ApnsEndPoint("gateway.sandbox.push.apple.com", 2195, "feedback.sandbox.push.apple.com", 2196);

		internal static readonly ApnsEndPoint Production = new ApnsEndPoint("gateway.push.apple.com", 2195, "feedback.push.apple.com", 2196);
	}
}
