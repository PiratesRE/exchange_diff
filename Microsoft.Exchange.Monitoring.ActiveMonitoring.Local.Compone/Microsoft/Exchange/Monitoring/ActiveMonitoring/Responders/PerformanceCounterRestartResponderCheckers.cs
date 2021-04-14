using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	internal class PerformanceCounterRestartResponderCheckers : RestartResponderChecker
	{
		internal PerformanceCounterRestartResponderCheckers(ResponderDefinition definition) : base(definition)
		{
		}

		internal override string SkipReasonOrException
		{
			get
			{
				return this.skipReasonOrException;
			}
		}

		protected override bool IsWithinThreshold()
		{
			this.skipReasonOrException = null;
			try
			{
				if (this.performanceCounterRestartResponderCheckers != null)
				{
					foreach (PerformanceCounterRestartResponderChecker performanceCounterRestartResponderChecker in this.performanceCounterRestartResponderCheckers)
					{
						if (performanceCounterRestartResponderChecker != null && !performanceCounterRestartResponderChecker.IsRestartAllowed)
						{
							this.skipReasonOrException += performanceCounterRestartResponderChecker.SkipReasonOrException;
							return false;
						}
						if (performanceCounterRestartResponderChecker != null)
						{
							this.skipReasonOrException += performanceCounterRestartResponderChecker.SkipReasonOrException;
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.skipReasonOrException = ex.ToString();
			}
			return true;
		}

		internal override string KeyOfEnabled
		{
			get
			{
				return "MoMTPerformanceCounterSettingEnabled";
			}
		}

		internal override string KeyOfSetting
		{
			get
			{
				return "MoMTPerformanceCounterSettings";
			}
		}

		internal override string DefaultSetting
		{
			get
			{
				if (PerformanceCounterRestartResponderCheckers.defaultSettings == null)
				{
					using (Stream manifestResourceStream = typeof(PerformanceCounterCheckSetting).Assembly.GetManifestResourceStream("MapiMTPerformanceCounterResponderCheckConfig.xml"))
					{
						if (manifestResourceStream != null)
						{
							using (StreamReader streamReader = new StreamReader(manifestResourceStream))
							{
								PerformanceCounterRestartResponderCheckers.defaultSettings = streamReader.ReadToEnd();
							}
						}
					}
				}
				return PerformanceCounterRestartResponderCheckers.defaultSettings;
			}
		}

		protected override bool OnSettingChange(string newSetting)
		{
			try
			{
				this.performanceCounterCheckSettings = null;
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PerformanceCounterCheckSetting[]));
				using (TextReader textReader = new StringReader(newSetting))
				{
					this.performanceCounterCheckSettings = (PerformanceCounterCheckSetting[])xmlSerializer.Deserialize(textReader);
					if (this.performanceCounterCheckSettings != null && this.performanceCounterCheckSettings.Length > 0)
					{
						this.performanceCounterRestartResponderCheckers = new PerformanceCounterRestartResponderChecker[this.performanceCounterCheckSettings.Length];
						int num = 0;
						foreach (PerformanceCounterCheckSetting performanceCounterCheckSetting in this.performanceCounterCheckSettings)
						{
							this.performanceCounterRestartResponderCheckers[num++] = new PerformanceCounterRestartResponderChecker(null, performanceCounterCheckSetting.CategoryName, performanceCounterCheckSetting.CounterName, performanceCounterCheckSetting.InstanceName, performanceCounterCheckSetting.MinThreshold, performanceCounterCheckSetting.MaxThreshold, performanceCounterCheckSetting.ReasonToSkip);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.skipReasonOrException = ex.ToString();
				return false;
			}
			return true;
		}

		private const string MoMTPerformanceCounterSettingEnabled = "MoMTPerformanceCounterSettingEnabled";

		private const string MapiMTPerformanceCounterResponderCheckConfig = "MapiMTPerformanceCounterResponderCheckConfig.xml";

		private const string MoMTPerformanceCounterSettings = "MoMTPerformanceCounterSettings";

		private string skipReasonOrException;

		private PerformanceCounterCheckSetting[] performanceCounterCheckSettings;

		private PerformanceCounterRestartResponderChecker[] performanceCounterRestartResponderCheckers;

		private static string defaultSettings;
	}
}
