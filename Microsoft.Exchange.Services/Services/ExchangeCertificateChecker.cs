using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services
{
	internal sealed class ExchangeCertificateChecker : IDisposable
	{
		internal static void Initialize(int checkerIntervalInMinutes)
		{
			ExchangeCertificateChecker.Singleton = new ExchangeCertificateChecker(checkerIntervalInMinutes);
		}

		internal static void Terminate()
		{
			if (ExchangeCertificateChecker.Singleton != null)
			{
				ExchangeCertificateChecker.Singleton.Dispose();
				ExchangeCertificateChecker.Singleton = null;
			}
		}

		internal static ExchangeCertificateChecker Singleton { get; private set; }

		private ExchangeCertificateChecker(int checkerIntervalInMinutes)
		{
			int period = 60000 * checkerIntervalInMinutes;
			this.certificateCheckerTimer = new Timer(new TimerCallback(this.CheckForCertificateExpiration), this, 0, period);
		}

		private void CheckForCertificateExpiration(object state)
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime t = utcNow.AddDays(7.0);
			DateTime t2 = utcNow.AddMonths(2);
			List<X509Certificate2> localCertificates = this.GetLocalCertificates();
			List<X509Certificate2> list = new List<X509Certificate2>();
			List<X509Certificate2> list2 = new List<X509Certificate2>();
			List<X509Certificate2> list3 = new List<X509Certificate2>();
			foreach (X509Certificate2 x509Certificate in localCertificates)
			{
				if (DateTime.Compare(x509Certificate.NotAfter, utcNow) <= 0)
				{
					list3.Add(x509Certificate);
				}
				else if (DateTime.Compare(x509Certificate.NotAfter, t) <= 0)
				{
					list2.Add(x509Certificate);
				}
				else if (DateTime.Compare(x509Certificate.NotAfter, t2) <= 0)
				{
					list.Add(x509Certificate);
				}
			}
			if (list3.Count<X509Certificate2>() > 0)
			{
				this.LogCertificateExpirationEvents(ServicesEventLogConstants.Tuple_CertificateExpired, list3, true);
			}
			if (list2.Count<X509Certificate2>() > 0)
			{
				this.LogCertificateExpirationEvents(ServicesEventLogConstants.Tuple_CertificateExpiresVerySoon, list2, false);
			}
			if (list.Count<X509Certificate2>() > 0)
			{
				this.LogCertificateExpirationEvents(ServicesEventLogConstants.Tuple_CertificateExpiresSoon, list, false);
			}
		}

		private List<X509Certificate2> GetLocalCertificates()
		{
			List<X509Certificate2> list = new List<X509Certificate2>();
			Dictionary<string, X509Certificate2> dictionary = new Dictionary<string, X509Certificate2>();
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store = null;
			}
			X509Store x509Store2 = new X509Store("REQUEST", StoreLocation.LocalMachine);
			try
			{
				x509Store2.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store2 = null;
			}
			try
			{
				if (x509Store != null)
				{
					foreach (X509Certificate2 certificate in x509Store.Certificates)
					{
						this.AddLatestCertificate(dictionary, certificate);
					}
				}
				if (x509Store2 != null)
				{
					foreach (X509Certificate2 x509Certificate in x509Store2.Certificates)
					{
						string value = CertificateEnroller.ReadPkcs10Request(x509Certificate);
						if (!string.IsNullOrEmpty(value))
						{
							list.Add(x509Certificate);
						}
					}
				}
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<LocalizedException>(0L, "[ExchangeCertificateHelper.GetLocalCertificates -- Error while reading certificates: {0}.", arg);
			}
			catch (CryptographicException arg2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<CryptographicException>(0L, "[ExchangeCertificateHelper.GetLocalCertificates -- Error while reading certificates: {0}.", arg2);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
				if (x509Store2 != null)
				{
					x509Store2.Close();
				}
			}
			list.AddRange(dictionary.Values.ToList<X509Certificate2>());
			return list;
		}

		private void LogCertificateExpirationEvents(ExEventLog.EventTuple tuple, IEnumerable<X509Certificate2> certificates, bool expired)
		{
			string traceFormat = "Certificate {0} expire" + (expired ? "d" : "s") + " on {1}.";
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				ServiceDiagnostics.LogEventWithTrace(tuple, null, ExTraceGlobals.CommonAlgorithmTracer, null, traceFormat, new object[]
				{
					x509Certificate.ToString(false),
					x509Certificate.NotAfter
				});
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "ExchangeCertificateChecker.Dispose called. IsDisposed: {0} IsDisposing: {1}", this.isDisposed, isDisposing);
			lock (this.lockObject)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<bool>((long)this.GetHashCode(), "ExchangeCertificateChecker.Dispose. After lock.  IsDisposed: {0}", this.isDisposed);
				if (!this.isDisposed)
				{
					if (isDisposing && this.certificateCheckerTimer != null)
					{
						this.certificateCheckerTimer.Dispose();
					}
					this.isDisposed = true;
				}
			}
		}

		private void AddLatestCertificate(Dictionary<string, X509Certificate2> dictionary, X509Certificate2 certificate)
		{
			if (dictionary == null || certificate == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(certificate.Subject))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[ExchangeCertificateHelper.AddLatestCertificate -- certificate.Subject is null. Thumbprint: {0}.", certificate.Thumbprint ?? "<NULL>");
				return;
			}
			if (dictionary.Keys.Contains(certificate.Subject))
			{
				if (dictionary[certificate.Subject].NotAfter < certificate.NotAfter)
				{
					dictionary[certificate.Subject] = certificate;
					return;
				}
			}
			else
			{
				dictionary.Add(certificate.Subject, certificate);
			}
		}

		private const string RequestStoreName = "REQUEST";

		private Timer certificateCheckerTimer;

		private bool isDisposed;

		private object lockObject = new object();
	}
}
