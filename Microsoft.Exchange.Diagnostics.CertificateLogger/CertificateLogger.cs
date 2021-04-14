using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.Exchange.LogAnalyzer.Extensions.CertificateLog;

namespace Microsoft.Exchange.Diagnostics.CertificateLogger
{
	public sealed class CertificateLogger : IDisposable
	{
		public CertificateLogger(string logDirectory, TimeSpan monitorInterval, long maxDirectorySize, long maxFileSize, int maxBufferSize, TimeSpan flushInterval)
		{
			if (monitorInterval.TotalMilliseconds < 0.0 || monitorInterval.TotalMilliseconds > 2147483647.0)
			{
				throw new ArgumentOutOfRangeException("monitorInterval");
			}
			this.log = new CertificateLog(logDirectory, maxDirectorySize, maxFileSize, maxBufferSize, flushInterval);
			this.refreshInterval = monitorInterval;
			if (monitorInterval.TotalMilliseconds > 0.0)
			{
				this.refreshTimer = new Timer(300000.0);
				this.refreshTimer.Elapsed += this.TimerEvent;
				this.refreshTimer.Start();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void PollCertificateStores()
		{
			IEnumerable<X509Store> certificateStores = this.GetCertificateStores();
			foreach (X509Store store in certificateStores)
			{
				this.LogCertificatesInStore(store);
			}
		}

		internal void LogCertificatesInStore(X509Store store)
		{
			IList<CertificateInformation> certificatesFromStore = this.GetCertificatesFromStore(store);
			foreach (CertificateInformation info in certificatesFromStore)
			{
				this.log.LogCertificateInformation(info);
			}
		}

		internal void TimerEvent(object sender, ElapsedEventArgs e)
		{
			if (this.refreshTimer.Interval != this.refreshInterval.TotalMilliseconds)
			{
				this.refreshTimer.Interval = this.refreshInterval.TotalMilliseconds;
			}
			this.PollCertificateStores();
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				if (this.refreshTimer != null)
				{
					this.refreshTimer.Stop();
					this.refreshTimer.Enabled = false;
					this.refreshTimer.Dispose();
				}
				if (this.log != null)
				{
					this.log.Dispose();
				}
				this.disposed = true;
			}
		}

		private IEnumerable<X509Store> GetCertificateStores()
		{
			List<X509Store> list = new List<X509Store>();
			foreach (object obj in Enum.GetValues(typeof(StoreName)))
			{
				StoreName storeName = (StoreName)obj;
				list.Add(new X509Store(storeName, StoreLocation.LocalMachine));
			}
			return list;
		}

		private IList<CertificateInformation> GetCertificatesFromStore(X509Store store)
		{
			if (store == null)
			{
				throw new ArgumentNullException("store");
			}
			List<CertificateInformation> list = new List<CertificateInformation>();
			try
			{
				store.Open(OpenFlags.OpenExistingOnly);
				list.AddRange(from X509Certificate2 certificate in store.Certificates
				select new CertificateInformation(certificate, store));
			}
			catch (CryptographicException ex)
			{
				Logger.LogErrorMessage("The following exception occured: " + ex.Message, new object[0]);
			}
			finally
			{
				store.Close();
			}
			return list;
		}

		private readonly TimeSpan refreshInterval;

		private readonly Timer refreshTimer;

		private readonly CertificateLog log;

		private bool disposed;
	}
}
