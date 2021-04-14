using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsChannelSettings : PushNotificationSettingsBase
	{
		public ApnsChannelSettings(string appId, string certificateThumbprint, ApnsEndPoint host, bool ignoreCertificateErrors = false) : this(appId, certificateThumbprint, null, host, 500, 300000, 3, 1500, 2, 5000, 5000, 600, ignoreCertificateErrors)
		{
		}

		public ApnsChannelSettings(string appId, string certificateThumbprint, string certificateThumbprintFallback, ApnsEndPoint host, int connectStepTimeout, int connectTotalTimeout, int connectRetryMax, int connectRetryDelay, int authenticateRetryMax, int readTimeout, int writeTimeout, int backOffTime, bool ignoreCertificateErrors) : base(appId)
		{
			this.CertificateThumbprint = certificateThumbprint;
			this.CertificateThumbprintFallback = certificateThumbprintFallback;
			this.ApnsEndPoint = host;
			this.ConnectStepTimeout = connectStepTimeout;
			this.ConnectTotalTimeout = connectTotalTimeout;
			this.ConnectRetryMax = connectRetryMax;
			this.ConnectRetryDelay = connectRetryDelay;
			this.AuthenticateRetryMax = authenticateRetryMax;
			this.ReadTimeout = readTimeout;
			this.WriteTimeout = writeTimeout;
			this.BackOffTimeInSeconds = backOffTime;
			this.IgnoreCertificateErrors = ignoreCertificateErrors;
		}

		public ApnsEndPoint ApnsEndPoint { get; private set; }

		public string Host
		{
			get
			{
				return this.ApnsEndPoint.Host;
			}
		}

		public string FeedbackHost
		{
			get
			{
				return this.ApnsEndPoint.FeedbackHost;
			}
		}

		public int Port
		{
			get
			{
				return this.ApnsEndPoint.Port;
			}
		}

		public int FeedbackPort
		{
			get
			{
				return this.ApnsEndPoint.FeedbackPort;
			}
		}

		public string CertificateThumbprint { get; private set; }

		public string CertificateThumbprintFallback { get; private set; }

		public int ConnectStepTimeout { get; private set; }

		public int ConnectTotalTimeout { get; private set; }

		public int ConnectRetryMax { get; private set; }

		public int ConnectRetryDelay { get; private set; }

		public int AuthenticateRetryMax { get; private set; }

		public int ReadTimeout { get; private set; }

		public int WriteTimeout { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		public bool IgnoreCertificateErrors { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			if (string.IsNullOrWhiteSpace(this.CertificateThumbprint))
			{
				errors.Add(Strings.ValidationErrorEmptyString("CertificateThumbprint"));
			}
			if (string.IsNullOrWhiteSpace(this.Host) || Uri.CheckHostName(this.Host) == UriHostNameType.Unknown)
			{
				errors.Add(Strings.ValidationErrorInvalidUri("Host", this.Host ?? string.Empty, string.Empty));
			}
			if (string.IsNullOrWhiteSpace(this.FeedbackHost) || Uri.CheckHostName(this.FeedbackHost) == UriHostNameType.Unknown)
			{
				errors.Add(Strings.ValidationErrorInvalidUri("FeedbackHost", this.FeedbackHost ?? string.Empty, string.Empty));
			}
			if (this.Port < 0 || this.Port > 65535)
			{
				errors.Add(Strings.ValidationErrorRangeInteger("Port", 0, 65535, this.Port));
			}
			if (this.FeedbackPort < 0 || this.FeedbackPort > 65535)
			{
				errors.Add(Strings.ValidationErrorRangeInteger("FeedbackPort", 0, 65535, this.FeedbackPort));
			}
			if (this.ConnectTotalTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("ConnectTotalTimeout", this.ConnectTotalTimeout));
			}
			if (this.ConnectStepTimeout < 0 || this.ConnectStepTimeout > this.ConnectTotalTimeout)
			{
				errors.Add(Strings.ValidationErrorRangeInteger("ConnectStepTimeout", 0, this.ConnectTotalTimeout, this.ConnectStepTimeout));
			}
			if (this.ConnectRetryMax < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("ConnectRetryMax", this.ConnectRetryMax));
			}
			if (this.ConnectRetryDelay < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("ConnectRetryDelay", this.ConnectRetryDelay));
			}
			if (this.AuthenticateRetryMax < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("AuthenticateRetryMax", this.AuthenticateRetryMax));
			}
			if (this.ReadTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("ReadTimeout", this.ReadTimeout));
			}
			if (this.WriteTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("WriteTimeout", this.WriteTimeout));
			}
		}

		public const string DefaultCertificateThumbprintFallback = null;

		public const int DefaultConnectStepTimeout = 500;

		public const int DefaultConnectTotalTimeout = 300000;

		public const int DefaultConnectRetryMax = 3;

		public const int DefaultConnectRetryDelay = 1500;

		public const int DefaultAuthenticateRetryMax = 2;

		public const int DefaultReadTimeout = 5000;

		public const int DefaultWriteTimeout = 5000;

		public const int DefaultBackOffTimeInSeconds = 600;

		public const bool DefaultIgnoreCertificateErrors = false;
	}
}
