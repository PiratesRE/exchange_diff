using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Deployment.Probes
{
	public class CertificateExpirationProbe : ProbeWorkItem
	{
		private int NumberOfDaysBeforeExpirationFrom { get; set; }

		private int NumberOfDaysBeforeExpirationTo { get; set; }

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.NumberOfDaysBeforeExpirationFrom = attributeHelper.GetInt("NumberOfDaysBeforeExpirationFrom", true, 60, null, null);
			this.NumberOfDaysBeforeExpirationTo = attributeHelper.GetInt("NumberOfDaysBeforeExpirationTo", true, 30, null, null);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("CertificateExpirationProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			this.InitializeAttributes(null);
			X509Store x509Store = null;
			DateTime dateTime = DateTime.UtcNow.AddDays((double)this.NumberOfDaysBeforeExpirationFrom);
			DateTime dateTime2 = DateTime.UtcNow.AddDays((double)this.NumberOfDaysBeforeExpirationTo);
			try
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("Opening LocalComputer\\My certificate store. Check for certificate which will expire between {0} and {1}{2}", dateTime, dateTime2, Environment.NewLine);
				x509Store = new X509Store("My", StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection certificates = x509Store.Certificates;
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				foreach (X509Certificate2 x509Certificate in certificates)
				{
					DateTime dateTime3 = x509Certificate.NotAfter.ToUniversalTime();
					if (dateTime3 >= dateTime && dateTime3 < dateTime2)
					{
						TimeSpan timeSpan = DateTime.UtcNow - dateTime3;
						flag = true;
						stringBuilder.AppendFormat("Certificate with subject name '{0}' and thumbprint '{1}' is going to expire in {2} days ({3} (UTC timezone)).", new object[]
						{
							x509Certificate.SubjectName.Name,
							x509Certificate.Thumbprint,
							(int)timeSpan.TotalDays,
							dateTime3
						});
						stringBuilder.AppendLine();
					}
				}
				if (flag)
				{
					base.Result.FailureContext = stringBuilder.ToString();
					base.Result.Error = CertificateExpirationProbe.ProbeErrorMessage;
					throw new Exception(CertificateExpirationProbe.ProbeErrorMessage);
				}
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "No certificates near expiry were found.\r\n";
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += string.Format("CertificateExpirationProbe finished at {0}.", DateTime.UtcNow);
			}
		}

		public static readonly string ProbeErrorMessage = "One or more certificates are near expiry.";
	}
}
