using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Datacenter.Management.ActiveMonitoring;
using Microsoft.Exchange.Management.Powershell.CentralAdmin;
using Microsoft.Office.Management.Azure;

namespace Microsoft.Forefront.Monitoring.Management.OutsideIn
{
	[Cmdlet("Get", "AzureDeploymentVips")]
	public class GetAzureDeploymentVipsCommand : PSCmdlet
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Filter { get; set; }

		[Parameter(Mandatory = false)]
		public int MaxDegreeOfParallelism { get; set; }

		[Parameter(Mandatory = false)]
		public string Cloud { get; set; }

		public new void WriteVerbose(string text)
		{
			base.WriteVerbose(string.Format("[{0}] {1}", DateTime.UtcNow.ToLongTimeString(), text));
		}

		protected override void ProcessRecord()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			X509Certificate2 azureManagementCertificate = CommonHelper.GetCertificate(CommonHelper.GetSecureValue("CertificateSubject"));
			if (azureManagementCertificate == null)
			{
				throw new Exception(string.Format("We were unable to find the certificate with the following subject: {0}. Without this certificate, we can't get the Azure Deployment Vip.", CommonHelper.GetSecureValue("CertificateSubject")));
			}
			IEnumerable<AzureInstance> azureInstances = AzureResourceHelper.GetAzureInstances(this.Filter);
			this.WriteVerbose(string.Format("Found {0} Azure instances.", azureInstances.Count<AzureInstance>()));
			foreach (AzureInstance azureInstance2 in azureInstances)
			{
				if (azureInstance2 == null)
				{
					this.WriteVerbose("Found one AzureInstance object null.");
				}
				else
				{
					if (string.IsNullOrWhiteSpace(azureInstance2.Name))
					{
						this.WriteVerbose("Found one AzureInstance without a name.");
					}
					if (string.IsNullOrWhiteSpace(azureInstance2.AzureSubscription))
					{
						this.WriteVerbose("Found one AzureInstance without a subscription.");
					}
				}
			}
			HashSet<IPAddress> vipIps = new HashSet<IPAddress>();
			ConcurrentQueue<GetAzureDeploymentVipsCommand.GetAzureDeploymentVipsException> exceptions = new ConcurrentQueue<GetAzureDeploymentVipsCommand.GetAzureDeploymentVipsException>();
			Parallel.ForEach<AzureInstance>(azureInstances, new ParallelOptions
			{
				MaxDegreeOfParallelism = ((this.MaxDegreeOfParallelism != 0) ? this.MaxDegreeOfParallelism : 10)
			}, delegate(AzureInstance azureInstance)
			{
				try
				{
					XElement xelement = CommonHelper.RetryAction<XElement>(new Func<XElement>(delegate()
					{
						DeploymentManagement deploymentManagement = new DeploymentManagement();
						string text = string.IsNullOrWhiteSpace(this.Cloud) ? "windows.net" : this.Cloud;
						return deploymentManagement.GetDeploymentInfo(azureInstance.Name, azureInstance.AzureSubscription, text, azureManagementCertificate, "Production", "2012-03-01");
					}), new object[0]);
					HashSet<IPAddress> vipIps;
					lock (vipIps)
					{
						foreach (IPAddress item in IPAddressManagement.GetUniqueVips(xelement))
						{
							vipIps.Add(item);
						}
					}
				}
				catch (Exception inner)
				{
					exceptions.Enqueue(new GetAzureDeploymentVipsCommand.GetAzureDeploymentVipsException(azureInstance, inner));
				}
			});
			if (exceptions.Count > 0)
			{
				this.WriteVerbose(string.Format("Found {0} exceptions. Unable to retrieve the VIP for the following instances: {1}.", exceptions.Count, string.Join(", ", from e in exceptions
				select e.AzureInstance.Name)));
				foreach (GetAzureDeploymentVipsCommand.GetAzureDeploymentVipsException ex in exceptions)
				{
					base.WriteWarning(ex.ToString());
				}
			}
			this.WriteVerbose(string.Format("Completed in {0} seconds. We found {1} IPs ({2}).", stopwatch.Elapsed.TotalSeconds, vipIps.Count, string.Join(", ", from ip in vipIps
			select ip.ToString())));
			foreach (IPAddress ipaddress in vipIps)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					base.WriteObject(ipaddress);
				}
				else
				{
					this.WriteVerbose(string.Format("Skipping IP {0} because it is not an IPv4 address", ipaddress.ToString()));
				}
			}
		}

		[Serializable]
		public class GetAzureDeploymentVipsException : Exception
		{
			public GetAzureDeploymentVipsException()
			{
			}

			public GetAzureDeploymentVipsException(AzureInstance azureInstance, Exception inner) : base(string.Empty, inner)
			{
				this.AzureInstance = azureInstance;
			}

			public AzureInstance AzureInstance { get; private set; }

			public override string ToString()
			{
				return string.Format("AzureInstanceName = {0}; Exception = {1}", this.AzureInstance.Name, base.ToString());
			}
		}
	}
}
