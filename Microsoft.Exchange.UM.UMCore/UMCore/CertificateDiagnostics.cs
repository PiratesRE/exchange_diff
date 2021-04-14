using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CertificateDiagnostics : DisposableBase
	{
		protected abstract ExEventLog.EventTuple CertificateDetailsEventTuple { get; }

		protected abstract UMNotificationEvent CertificateNearExpiry { get; }

		protected abstract Component UMExchangeComponent { get; }

		protected abstract ExEventLog.EventTuple CertificateAboutToExpireEventTuple { get; }

		protected abstract ExEventLog.EventTuple CertificateExpirationOkEventTuple { get; }

		public CertificateDiagnostics(X509Certificate2 certificate)
		{
			this.certificate = certificate;
			UmGlobals.ExEvent.LogEvent(this.CertificateDetailsEventTuple, null, new object[]
			{
				CommonUtil.ToEventLogString(this.certificate.Issuer),
				CommonUtil.ToEventLogString(this.certificate.GetSerialNumberString()),
				CommonUtil.ToEventLogString(this.certificate.Thumbprint),
				CommonUtil.ToEventLogString(CertificateUtils.IsSelfSignedCertificate(this.certificate)),
				CommonUtil.ToEventLogString(this.certificate.NotAfter)
			});
			this.AnalyzeCertificateExpirationDate();
			TimeSpan timeSpan = new TimeSpan(UMRecyclerConfig.SubsequentAlertIntervalAfterFirstAlert, 0, 0, 0);
			this.certTimer = new Timer(new TimerCallback(this.TimerCallback), null, timeSpan, timeSpan);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					if (this.certTimer != null)
					{
						this.certTimer.Dispose();
						this.certTimer = null;
					}
				}
			}
		}

		protected virtual TimeSpan ExpirationTimeLeft
		{
			get
			{
				return CertificateUtils.TimeToExpire(this.certificate);
			}
		}

		protected virtual int DaysBeforeExpirationAlert
		{
			get
			{
				return UMRecyclerConfig.DaysBeforeCertExpiryForAlert;
			}
		}

		private void AnalyzeCertificateExpirationDate()
		{
			TimeSpan expirationTimeLeft = this.ExpirationTimeLeft;
			if (expirationTimeLeft.Days <= this.DaysBeforeExpirationAlert)
			{
				StatefulEventLog.Instance.LogRedEvent(base.GetType().Name, this.CertificateAboutToExpireEventTuple, null, true, new object[]
				{
					expirationTimeLeft.Days
				});
				UMEventNotificationHelper.PublishUMFailureEventNotificationItem(this.UMExchangeComponent, this.CertificateNearExpiry.ToString());
				return;
			}
			StatefulEventLog.Instance.LogGreenEvent(base.GetType().Name, this.CertificateExpirationOkEventTuple, null, false, new object[]
			{
				expirationTimeLeft.Days,
				this.DaysBeforeExpirationAlert
			});
			UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(this.UMExchangeComponent, this.CertificateNearExpiry.ToString());
		}

		private void TimerCallback(object state)
		{
			lock (this)
			{
				if (this.certTimer != null)
				{
					this.AnalyzeCertificateExpirationDate();
				}
			}
		}

		private Timer certTimer;

		private X509Certificate2 certificate;
	}
}
