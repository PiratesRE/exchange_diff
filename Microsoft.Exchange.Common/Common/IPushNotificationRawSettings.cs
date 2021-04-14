using System;

namespace Microsoft.Exchange.Common
{
	public interface IPushNotificationRawSettings : IEquatable<IPushNotificationRawSettings>
	{
		string Name { get; }

		PushNotificationPlatform Platform { get; }

		bool? Enabled { get; }

		Version ExchangeMinimumVersion { get; }

		Version ExchangeMaximumVersion { get; }

		int? QueueSize { get; }

		int? NumberOfChannels { get; }

		int? AddTimeout { get; }

		string AuthenticationId { get; }

		string AuthenticationKey { get; }

		string AuthenticationKeyFallback { get; }

		bool? IsAuthenticationKeyEncrypted { get; }

		string Url { get; }

		int? Port { get; }

		string SecondaryUrl { get; }

		int? SecondaryPort { get; }

		int? ConnectStepTimeout { get; }

		int? ConnectTotalTimeout { get; }

		int? ConnectRetryDelay { get; }

		int? ConnectRetryMax { get; }

		int? AuthenticateRetryMax { get; }

		int? ReadTimeout { get; }

		int? WriteTimeout { get; }

		bool? IgnoreCertificateErrors { get; }

		int? BackOffTimeInSeconds { get; }

		string UriTemplate { get; }

		int? MaximumCacheSize { get; }

		string RegistrationTemplate { get; }

		bool? RegistrationEnabled { get; }

		bool? MultifactorRegistrationEnabled { get; }

		string PartitionName { get; }

		bool? IsDefaultPartitionName { get; }

		string ToFullString();
	}
}
