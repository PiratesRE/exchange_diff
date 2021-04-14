using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public abstract class RusPublishingPipelineBase : ProbeWorkItem
	{
		public RusPublishingPipelineBase()
		{
			string forefrontBackgroundInstallPath = RusPublishingPipelineBase.GetForefrontBackgroundInstallPath("MsiInstallPath");
			ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
			{
				ExeConfigFilename = Path.Combine(forefrontBackgroundInstallPath, "Microsoft.Forefront.Hygiene.Rus.Pipeline.dll.config")
			};
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
			KeyValueConfigurationElement keyValueConfigurationElement = configuration.AppSettings.Settings["PrimaryActiveQdsPath"];
			this.RusPrimaryFileShareRootPath = ((keyValueConfigurationElement != null) ? keyValueConfigurationElement.Value : null);
			keyValueConfigurationElement = configuration.AppSettings.Settings["SecondaryActiveQdsPath"];
			this.RusAlternateFileShareRootPath = ((keyValueConfigurationElement != null) ? keyValueConfigurationElement.Value : null);
			if (string.IsNullOrEmpty(this.RusPrimaryFileShareRootPath) && string.IsNullOrEmpty(this.RusAlternateFileShareRootPath))
			{
				this.LogTraceErrorAndThrowApplicationException("Both RUS Primary and Alternate share root paths are empty");
			}
		}

		public static string ForeFrontdlUniversalManifestCabUrl
		{
			get
			{
				return Path.Combine(RusEngine.RusEngineDownloadUrl, "metadata", "UniversalManifest.cab").Replace("\\", "/");
			}
		}

		protected string RusPrimaryFileShareRootPath { get; set; }

		protected string RusAlternateFileShareRootPath { get; set; }

		protected string[] Platforms { get; set; }

		protected string RusEngineName { get; set; }

		protected RusEngine RusEngine { get; set; }

		protected bool AreManifestFilesOutOfSync(DateTime expectedDateTime, DateTime actualDateTime, TimeSpan allowedThresholdTimeSpan)
		{
			return expectedDateTime - actualDateTime > allowedThresholdTimeSpan;
		}

		protected int GetIntegerExtensionAttributeFromXml(string workContextXml, string xmlNodeName, string extensionAttributeName, int defaultIntegerValue, int acceptedMinimum, int acceptedMaximum)
		{
			if (acceptedMaximum < acceptedMinimum)
			{
				throw new ArgumentException(string.Format("acceptedMaximum: {0} is less than acceptedMinimum: {1}", acceptedMaximum, acceptedMinimum));
			}
			string extensionAttributeStringFromXml = this.GetExtensionAttributeStringFromXml(workContextXml, xmlNodeName, extensionAttributeName, false);
			int num = defaultIntegerValue;
			if (!string.IsNullOrWhiteSpace(extensionAttributeStringFromXml) && (!int.TryParse(extensionAttributeStringFromXml, out num) || num < acceptedMinimum || num > acceptedMaximum))
			{
				num = defaultIntegerValue;
			}
			return num;
		}

		protected TimeSpan GetTimeSpanExtensionAttributeFromXml(string workContextXml, string xmlNodeName, string extensionAttributeName, TimeSpan defaultTimeSpanValue, TimeSpan minTimeSpan, TimeSpan maxTimeSpan)
		{
			if (maxTimeSpan < minTimeSpan)
			{
				throw new ArgumentException(string.Format("maxTimeSpan: {0} is less than minTimeSpan: {1}", maxTimeSpan, minTimeSpan));
			}
			string extensionAttributeStringFromXml = this.GetExtensionAttributeStringFromXml(workContextXml, xmlNodeName, extensionAttributeName, false);
			TimeSpan timeSpan = defaultTimeSpanValue;
			if (!string.IsNullOrWhiteSpace(extensionAttributeStringFromXml) && (!TimeSpan.TryParse(extensionAttributeStringFromXml, out timeSpan) || timeSpan < minTimeSpan || timeSpan > maxTimeSpan))
			{
				timeSpan = defaultTimeSpanValue;
			}
			return timeSpan;
		}

		protected string GetExtensionAttributeStringFromXml(string workContextXml, string xmlNodeName, string extensionAttributeName, bool throwOnNullOrEmpty = true)
		{
			if (string.IsNullOrWhiteSpace(workContextXml))
			{
				throw new ArgumentNullException("WorkContext definition Xml is null or empty");
			}
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(workContextXml);
			XmlElement xmlElement = Utils.CheckXmlElement(xmlDocument.SelectSingleNode(xmlNodeName), xmlNodeName);
			string attribute = xmlElement.GetAttribute(extensionAttributeName);
			if (throwOnNullOrEmpty && string.IsNullOrWhiteSpace(attribute))
			{
				throw new ArgumentNullException("Extension Attribute value in WorkContext xml is found to be null or empty", extensionAttributeName);
			}
			return attribute;
		}

		protected Collection<PSObject> ExecuteForeFrontManagementShellScript(string psScript, bool throwOnErrors = false)
		{
			if (string.IsNullOrWhiteSpace(psScript))
			{
				throw new ArgumentNullException("psScript", "PowerShell script cannot be null or empty.");
			}
			this.TraceDebug(string.Format("[PowerShellScript: {0}]", psScript));
			Collection<PSObject> result = null;
			try
			{
				RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
				PSSnapInException ex = null;
				runspaceConfiguration.AddPSSnapIn("Microsoft.Forefront.Management.PowerShell", out ex);
				if (ex != null)
				{
					this.TraceDebug(string.Format("Non-fatal error occurred while adding the powerShell snap-in - {0}. Warning: {1}", "Microsoft.Forefront.Management.PowerShell", ex.Message));
				}
				using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
				{
					runspace.Open();
					Pipeline pipeline = runspace.CreatePipeline();
					Command item = new Command(psScript, true);
					pipeline.Commands.Add(item);
					result = pipeline.Invoke();
					if (pipeline.HadErrors)
					{
						StringBuilder stringBuilder = new StringBuilder(Environment.NewLine);
						foreach (object obj in pipeline.Error.ReadToEnd())
						{
							stringBuilder.AppendLine(obj.ToString());
						}
						string text = string.Format("Errors occurred while executing pipeline.Invoke(). Command: {0} Errors: {1}", psScript, stringBuilder.ToString());
						if (throwOnErrors)
						{
							this.LogTraceErrorAndThrowApplicationException(text);
						}
						this.TraceDebug(text);
					}
				}
			}
			catch (Exception ex2)
			{
				this.LogTraceErrorAndThrowApplicationException(string.Format("Failed to execute ForeFrontManagementShell script {0}. Exception: {1}", psScript, ex2.ToString()));
			}
			return result;
		}

		protected string FormatBackgroundJobTaskResultsToString(Collection<PSObject> bgdTaskResults)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PSObject psobject in bgdTaskResults)
			{
				string value = string.Format("TaskId: {0}, StartTime: {1}, EndTime: {2}, ExecutableState: {3}, TaskCompletionStatus: {4}, BgdMachineName: {5}, RetryAttempts: {6} \n", new object[]
				{
					Convert.ToString(psobject.Properties["TaskId"].Value),
					Convert.ToString(psobject.Properties["StartTime"].Value),
					Convert.ToString(psobject.Properties["EndTime"].Value),
					Convert.ToString(psobject.Properties["ExecutableState"].Value),
					Convert.ToString(psobject.Properties["TaskCompletionStatus"].Value),
					Convert.ToString(psobject.Properties["BackgroundJobManagerMachineName"].Value),
					Convert.ToString(psobject.Properties["RetryAttempts"].Value)
				});
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		protected void LogTraceErrorAndThrowApplicationException(string errorMsg)
		{
			base.Result.Error = errorMsg;
			this.TraceError(errorMsg);
			throw new ApplicationException(errorMsg);
		}

		protected void TraceDebug(string debugMsg)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + debugMsg + " ";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.BackgroundTracer, base.TraceContext, debugMsg, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\RusPublishingPipelineBase.cs", 431);
		}

		protected void TraceError(string errorMsg)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext = result.ExecutionContext + errorMsg + " ";
			WTFDiagnostics.TraceError(ExTraceGlobals.BackgroundTracer, base.TraceContext, errorMsg, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Background\\Probes\\RusPublishingPipelineBase.cs", 441);
		}

		protected string GetRegionTag()
		{
			string exchangeLabsRegKeyValue = RusEngine.GetExchangeLabsRegKeyValue("Region");
			if (string.IsNullOrWhiteSpace(exchangeLabsRegKeyValue))
			{
				this.LogTraceErrorAndThrowApplicationException(string.Format("Registry string [{0}] value is found to be empty", exchangeLabsRegKeyValue));
			}
			string exchangeLabsRegKeyValue2 = RusEngine.GetExchangeLabsRegKeyValue("RegionServiceInstance");
			if (string.IsNullOrWhiteSpace(exchangeLabsRegKeyValue2))
			{
				this.LogTraceErrorAndThrowApplicationException(string.Format("Registry string [{0}] value is found to be empty", exchangeLabsRegKeyValue2));
			}
			return exchangeLabsRegKeyValue + exchangeLabsRegKeyValue2;
		}

		private static string GetForefrontBackgroundInstallPath(string regStringName)
		{
			string result = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\FfoBackground\\Setup"))
				{
					if (registryKey == null)
					{
						throw new ApplicationException(string.Format("HKLM forefrontKey registry key [{0}] is found to be null", "SOFTWARE\\Microsoft\\FfoBackground\\Setup"));
					}
					if (registryKey.GetValue(regStringName) == null)
					{
						throw new ApplicationException(string.Format("RegistryKey.GetValue() returned null for the reg string [{0}]", regStringName));
					}
					result = (registryKey.GetValue(regStringName) as string);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException(string.Format("An error occured while loading registry key value [{0}]. Exception: {1}", regStringName, ex.ToString()));
			}
			return result;
		}

		protected const string EngineNameXmlAttribute = "EngineName";

		protected const string PlatformsXmlAttribute = "Platforms";

		protected const string RegionKey = "Region";

		protected const string RegionServiceInstanceKey = "RegionServiceInstance";

		protected const string ForeFrontManagementShellSnapIn = "Microsoft.Forefront.Management.PowerShell";

		protected const string RusSignaturePollingBgdJobFileName = "Microsoft.Forefront.Hygiene.Rus.SignaturePollingJob.exe";

		protected const string RusEngineUpdatePublisherBgdJobFileName = "Microsoft.Forefront.Hygiene.Rus.EngineUpdatePublisher.exe";

		protected const string RusPipelineBinaryConfigFileName = "Microsoft.Forefront.Hygiene.Rus.Pipeline.dll.config";

		protected const string GetBgdDefinitionCmdlet = "Get-BackgroundJobDefinition";

		protected const string GetBgdScheduleCmdlet = "Get-BackgroundJobSchedule";

		protected const string GetBgdTaskCmdlet = "Get-BackgroundJobTask";

		protected const string GetAsyncQueueRequestCmdlet = "Get-AsyncQueueRequest";

		protected const string GetAsyncQueueLogsCmdlet = "Get-AsyncQueueLog";

		protected const string RusPipelineJobOwnerName = "RusPipelineJob";

		protected const string BackgroundApplicationInstallRegPath = "SOFTWARE\\Microsoft\\FfoBackground\\Setup";

		protected const string BackgroundApplicationInstallRegKey = "MsiInstallPath";

		protected const string OpenBrace = "{";

		protected const string ClosedBrace = "}";
	}
}
