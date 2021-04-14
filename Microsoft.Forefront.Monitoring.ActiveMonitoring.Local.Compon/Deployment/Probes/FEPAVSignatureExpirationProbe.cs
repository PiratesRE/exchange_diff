using System;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Deployment.Probes
{
	public class FEPAVSignatureExpirationProbe : ProbeWorkItem
	{
		private int ExpirationThresholdInHours { get; set; }

		public static ProbeDefinition CreateDefinition(int expirationThresholdInHours)
		{
			return new ProbeDefinition
			{
				AssemblyPath = typeof(FEPAVSignatureExpirationProbe).Assembly.Location,
				TypeName = typeof(FEPAVSignatureExpirationProbe).FullName,
				Name = typeof(FEPAVSignatureExpirationProbe).Name,
				ServiceName = ExchangeComponent.FfoDeployment.Name,
				RecurrenceIntervalSeconds = 1800,
				TimeoutSeconds = 30,
				MaxRetryAttempts = 2,
				TargetResource = Environment.MachineName
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime dateTime = DateTime.MinValue;
			DateTime dateTime2 = DateTime.MinValue;
			DateTime dateTime3 = DateTime.MinValue;
			base.Result.ExecutionContext = string.Format("FEPAVSignatureExpirationProbe started at {0}.\r\n", DateTime.UtcNow);
			this.InitializeAttributes(null);
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("Opening FEP AV Signature registry. ExpirationThresholdInHours:{0}\r\n", this.ExpirationThresholdInHours);
			this.CheckAntimalwareService();
			dateTime2 = FEPAVSignatureExpirationProbe.GetDateTimeFromRegistry("AVSignatureApplied");
			dateTime = FEPAVSignatureExpirationProbe.GetDateTimeFromRegistry("SignaturesLastChecked");
			dateTime3 = FEPAVSignatureExpirationProbe.GetDateTimeFromRegistry("LastFallbackTime");
			if ((dateTime.Equals(DateTime.MinValue) && dateTime3.Equals(DateTime.MinValue)) || dateTime2.Equals(DateTime.MinValue))
			{
				base.Result.FailureContext = "FEP Signature Updates registry setting not available.  Check FEP installation, GPO and/or local policies and operations.";
				base.Result.Error = base.Result.FailureContext;
				throw new Exception(base.Result.FailureContext);
			}
			if (dateTime.Equals(DateTime.MinValue))
			{
				dateTime = dateTime3;
			}
			DateTime value = dateTime2.AddHours((double)this.ExpirationThresholdInHours);
			string antimalwareVersion = FEPAVSignatureExpirationProbe.GetAntimalwareVersion();
			if (DateTime.UtcNow.CompareTo(value) > 0)
			{
				base.Result.FailureContext = string.Format("Antimalware signature with version {0} is either near expiry or out of date, signature last applied time: '{1}' with last update check since '{2}'", antimalwareVersion, dateTime2.ToUniversalTime().ToString("r"), dateTime.ToUniversalTime().ToString("r"));
				base.Result.Error = base.Result.FailureContext;
				throw new Exception(base.Result.FailureContext);
			}
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += string.Format("FEP AV signature applied at time'{0}' with version '{1}' and is up-to-date.\r\n", dateTime2.ToUniversalTime().ToString("r"), antimalwareVersion);
			ProbeResult result3 = base.Result;
			result3.ExecutionContext += string.Format("FEPAVSignatureExpirationProbe finished at {0}.", DateTime.UtcNow);
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.ExpirationThresholdInHours = 48;
			RegistryKey registryKey2;
			RegistryKey registryKey = registryKey2 = Registry.LocalMachine.OpenSubKey(FEPAVSignatureExpirationProbe.FEPAVGpoPolicy);
			try
			{
				if (registryKey == null)
				{
					base.Result.FailureContext = "FEP Signature Updates Group policy not replicated to local server.  Check FEP installation, GPO and/or local policies and operations.";
					base.Result.Error = base.Result.FailureContext;
					throw new Exception(base.Result.FailureContext);
				}
				this.ExpirationThresholdInHours = 24 * (int)registryKey.GetValue("AVSignatureDue", 7);
				registryKey.Close();
				if (this.ExpirationThresholdInHours == 0)
				{
					base.Result.FailureContext = "FEP Signature Updates AVSignatureDue not defined in the Group policy.  Check FEP installation, GPO and/or local policies and operations.";
					base.Result.Error = base.Result.FailureContext;
					throw new Exception(base.Result.FailureContext);
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
		}

		private static DateTime GetDateTimeFromRegistry(string regName)
		{
			DateTime result = DateTime.MinValue;
			byte[] array = new byte[8];
			byte[] array2 = array;
			RegistryKey registryKey2;
			RegistryKey registryKey = registryKey2 = Registry.LocalMachine.OpenSubKey(FEPAVSignatureExpirationProbe.FEPAVRegistryKey);
			try
			{
				if (registryKey != null)
				{
					byte[] array3 = (byte[])registryKey.GetValue(regName, array2);
					registryKey.Close();
					if (!array3.Equals(array2))
					{
						long fileTime = BitConverter.ToInt64(array3, 0);
						result = DateTime.FromFileTimeUtc(fileTime);
					}
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			return result;
		}

		private static string GetAntimalwareVersion()
		{
			string result = string.Empty;
			RegistryKey registryKey2;
			RegistryKey registryKey = registryKey2 = Registry.LocalMachine.OpenSubKey(FEPAVSignatureExpirationProbe.FEPAVRegistryKey);
			try
			{
				if (registryKey != null)
				{
					result = (string)registryKey.GetValue("AVSignatureVersion", string.Empty);
					registryKey.Close();
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			return result;
		}

		private void CheckAntimalwareService()
		{
			using (ServiceController serviceController = new ServiceController("MsMpSvc"))
			{
				if (!serviceController.Status.Equals(ServiceControllerStatus.Running))
				{
					base.Result.FailureContext = "FEP Antimalware service (MsMpSvc) is not running.";
					base.Result.Error = TimeServiceProbe.ProbeErrorMessage;
					throw new Exception(TimeServiceProbe.ProbeErrorMessage);
				}
				ProbeResult result = base.Result;
				result.ExecutionContext += "MsMpsvc service is running.\r\n";
			}
		}

		private static readonly string FEPAVRegistryKey = "SOFTWARE\\Microsoft\\Microsoft Antimalware\\Signature Updates";

		private static readonly string FEPAVGpoPolicy = "SOFTWARE\\Policies\\Microsoft\\Microsoft Antimalware\\Signature Updates";
	}
}
