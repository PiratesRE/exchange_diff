using System;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsCertProvider
	{
		public ApnsCertProvider(ApnsCertStore store, string appId) : this(store, false, appId)
		{
		}

		public ApnsCertProvider(ApnsCertStore store, ITracer tracer, string appId) : this(store, false, tracer, appId)
		{
		}

		internal ApnsCertProvider(ApnsCertStore store, bool ignoreCertificateErrors, string appId) : this(store, ignoreCertificateErrors, ExTraceGlobals.ApnsPublisherTracer, appId)
		{
		}

		internal ApnsCertProvider(ApnsCertStore store, bool ignoreCertificateErrors, ITracer tracer, string appId)
		{
			this.Store = store;
			this.IgnoreCertificateErrors = ignoreCertificateErrors;
			this.Tracer = tracer;
			this.AppId = appId;
		}

		private string AppId { get; set; }

		private ApnsCertStore Store { get; set; }

		private ITracer Tracer { get; set; }

		private bool IgnoreCertificateErrors { get; set; }

		public virtual X509Certificate2 LoadCertificate(string thumbprint, string altThumbprint = null)
		{
			X509Certificate2 x509Certificate = this.LoadCertificate(thumbprint);
			if (x509Certificate == null && !string.IsNullOrEmpty(altThumbprint))
			{
				x509Certificate = this.LoadCertificate(altThumbprint);
			}
			if (x509Certificate == null)
			{
				PushNotificationsCrimsonEvents.ApnsCertNotFound.Log<string, string, string>(string.Empty, thumbprint, string.Empty);
				throw this.HandleLoadException(thumbprint, "ApnsCertPresent", new ApnsCertificateException(Strings.ApnsCertificateNotFound(thumbprint)));
			}
			return x509Certificate;
		}

		public bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				PushNotificationsMonitoring.PublishSuccessNotification("ApnsCertValidation", this.AppId);
				return true;
			}
			string text = "<null>";
			string text2 = "<null>";
			string text3 = sslPolicyErrors.ToString();
			if (certificate != null && !string.IsNullOrEmpty(certificate.Subject))
			{
				text = certificate.Subject;
			}
			X509Certificate2 x509Certificate = certificate as X509Certificate2;
			if (x509Certificate != null)
			{
				text2 = x509Certificate.Thumbprint;
			}
			if (!this.IgnoreCertificateErrors)
			{
				this.Tracer.TraceError((long)this.GetHashCode(), string.Format("[ValidateCertificate] Validation {0} for certificate '{1}', thumbprint:'{2}', error:'{3}'", new object[]
				{
					this.IgnoreCertificateErrors ? "ignored" : "failed",
					text,
					text2,
					text3
				}));
				PushNotificationsCrimsonEvents.ApnsCertValidationFailed.Log<X509Certificate, string, string>(certificate, text2, text3);
				PushNotificationsMonitoring.PublishFailureNotification("ApnsCertValidation", this.AppId, "");
			}
			return this.IgnoreCertificateErrors;
		}

		private X509Certificate2 LoadCertificate(string thumbprint)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("thumbprint", thumbprint);
			X509Certificate2 result;
			try
			{
				this.Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection x509Certificate2Collection = this.Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection == null || x509Certificate2Collection.Count <= 0)
				{
					result = null;
				}
				else
				{
					PushNotificationsMonitoring.PublishSuccessNotification("ApnsCertPresent", this.AppId);
					X509Certificate2 x509Certificate = x509Certificate2Collection[0];
					try
					{
						x509Certificate.PrivateKey.GetHashCode();
					}
					catch (NullReferenceException)
					{
						PushNotificationsCrimsonEvents.ApnsCertPrivateKeyError.Log<string, string, string>(x509Certificate.FriendlyName, thumbprint, string.Empty);
						throw this.HandleLoadException(thumbprint, "ApnsCertPrivateKey", new ApnsCertificateException(Strings.ApnsCertificatePrivateKeyError(x509Certificate.FriendlyName, thumbprint)));
					}
					PushNotificationsMonitoring.PublishSuccessNotification("ApnsCertPrivateKey", this.AppId);
					this.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "[LoadCertificate] Certificate '{0}' found for thumbprint '{1}'", x509Certificate.FriendlyName, thumbprint);
					PushNotificationsMonitoring.PublishSuccessNotification("ApnsCertLoaded", this.AppId);
					result = x509Certificate;
				}
			}
			catch (CryptographicException exception)
			{
				PushNotificationsCrimsonEvents.ApnsCertException.Log<string, string>(thumbprint, exception.ToTraceString());
				throw this.HandleLoadException(thumbprint, exception);
			}
			catch (SecurityException exception2)
			{
				PushNotificationsCrimsonEvents.ApnsCertException.Log<string, string>(thumbprint, exception2.ToTraceString());
				throw this.HandleLoadException(thumbprint, exception2);
			}
			finally
			{
				this.Store.Close();
			}
			return result;
		}

		private Exception HandleLoadException(string thumbprint, Exception exception)
		{
			PushNotificationsCrimsonEvents.ApnsCertException.Log<string, string>(thumbprint, exception.ToTraceString());
			return this.HandleLoadException(thumbprint, "ApnsCertLoaded", new ApnsCertificateException(Strings.ApnsCertificateExternalException(thumbprint, exception.Message), exception));
		}

		private Exception HandleLoadException(string thumbprint, string monitoringKey, ApnsCertificateException exception)
		{
			this.Tracer.TraceError<string, string>((long)this.GetHashCode(), "[LoadCertificate] An error occurred loading certificate for thumbprint '{0}': {1}", thumbprint, exception.ToTraceString());
			PushNotificationsMonitoring.PublishFailureNotification(monitoringKey, this.AppId, "");
			return exception;
		}
	}
}
