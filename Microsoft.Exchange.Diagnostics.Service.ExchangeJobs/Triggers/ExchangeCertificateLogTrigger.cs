using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.LogAnalyzer.Analyzers.CertificateLog;
using Microsoft.Exchange.LogAnalyzer.Extensions.CertificateLog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public sealed class ExchangeCertificateLogTrigger : CertificateLogAnalyzer
	{
		public ExchangeCertificateLogTrigger(IJob job) : base(job)
		{
		}

		protected override bool ShouldValidateCertificate(CertificateInformation info)
		{
			return info.StoreName.Equals(StoreName.My.ToString());
		}

		protected override void ValidateExpiration(CertificateInformation info)
		{
			TimeSpan t = info.ValidTo.ToUniversalTime().Subtract(DateTime.UtcNow);
			string text = string.Format("The following certificate is expiring within {0} day(s).", Math.Ceiling(t.TotalDays));
			if (t <= this.ErrorDaysBeforeExpiry)
			{
				TriggerHandler.Trigger("SSLCertificateErrorEvent", new object[]
				{
					text,
					info.ToString(),
					info.ComponentOwner,
					base.GetType().Name
				});
				return;
			}
			if (t <= this.WarningDaysBeforeExpiry)
			{
				TriggerHandler.Trigger("SSLCertificateWarningEvent", new object[]
				{
					text,
					info.ToString(),
					info.ComponentOwner,
					base.GetType().Name
				});
			}
		}
	}
}
