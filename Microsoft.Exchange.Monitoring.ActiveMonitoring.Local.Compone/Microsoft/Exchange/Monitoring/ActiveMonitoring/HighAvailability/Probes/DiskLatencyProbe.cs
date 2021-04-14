using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	internal class DiskLatencyProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, double thresholdInHours, int recurrenceInterval, int timeout, int maxRetry, double latencyThreshold = 0.01)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = typeof(DiskLatencyProbe).FullName;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.Attributes[DiskLatencyProbe.ThresholdAttrName] = thresholdInHours.ToString();
			probeDefinition.Attributes[DiskLatencyProbe.LatencyThresholdAttrName] = latencyThreshold.ToString();
			return probeDefinition;
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, double thresholdInHours, int recurrenceInterval, double latencyThreshold = 0.01)
		{
			return DiskLatencyProbe.CreateDefinition(name, serviceName, thresholdInHours, recurrenceInterval, recurrenceInterval / 2, 3, latencyThreshold);
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (!propertyBag.ContainsKey(DiskLatencyProbe.ThresholdAttrName))
			{
				throw new ArgumentException("Please specify value for" + DiskLatencyProbe.ThresholdAttrName);
			}
			pDef.Attributes[DiskLatencyProbe.ThresholdAttrName] = propertyBag[DiskLatencyProbe.ThresholdAttrName].ToString().Trim();
			if (propertyBag.ContainsKey(DiskLatencyProbe.LatencyThresholdAttrName))
			{
				pDef.Attributes[DiskLatencyProbe.LatencyThresholdAttrName] = propertyBag[DiskLatencyProbe.LatencyThresholdAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for" + DiskLatencyProbe.LatencyThresholdAttrName);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			double num = 0.0;
			double num2 = 0.0;
			if (string.IsNullOrWhiteSpace(base.Definition.Attributes["threshold"]) || !double.TryParse(base.Definition.Attributes["threshold"], out num))
			{
				throw new HighAvailabilityMAProbeException("Required attribute 'threshold' is either in wrong format or is empty");
			}
			if (string.IsNullOrWhiteSpace(base.Definition.Attributes["latencyThreshold"]) || !double.TryParse(base.Definition.Attributes["latencyThreshold"], out num2))
			{
				throw new HighAvailabilityMAProbeException("Required attribute 'latencyThreshold' is either in wrong format or is empty");
			}
			DateTime utcNow = DateTime.UtcNow;
			base.Result.StateAttribute4 = string.Format("LatencyThreshold={0:0.##} Threshold={1:0.##} hours. CurrentTime={2}", num2, num, utcNow.ToString());
			double num3 = 0.0;
			using (PerformanceCounter performanceCounter = new PerformanceCounter("LogicalDisk", "Avg. Disk sec/Write", "_Total"))
			{
				num3 = (double)performanceCounter.NextValue();
				Thread.Sleep(1000);
				for (int i = 0; i < 5; i++)
				{
					double num4 = (double)performanceCounter.NextValue();
					num3 = (num3 * (double)i + num4) / (double)(i + 1);
					Thread.Sleep(1000);
				}
			}
			bool flag = num3 >= num2;
			base.Result.StateAttribute2 = string.Format("AverageLatency={0:0.######}. Threshold over={1}", num3, flag);
			bool flag2 = false;
			double num5 = 0.0;
			DateTime minValue = DateTime.MinValue;
			if (this.TryDeserializeRegWatermark(out flag2, out num5, out minValue))
			{
				base.Result.StateAttribute3 = string.Format("RegOverThreshold={0}, RegLastValue={1:0.#####}, RegLastChange={2}", flag2, num5, minValue);
				if (flag2 == flag)
				{
					if (flag)
					{
						if (DateTime.UtcNow - minValue >= TimeSpan.FromHours(num))
						{
							string text = string.Format("Probe failure result. Disk latency is still over threshold ({0}, threshold={1}), and it has been this way since {2}", num3, num2, minValue);
							base.Result.StateAttribute5 = text;
							this.WriteSerializedRegWatermark(flag, num3, minValue);
							throw new HighAvailabilityMAProbeRedResultException(text);
						}
					}
					else
					{
						this.WriteSerializedRegWatermark(flag, num3, minValue);
					}
				}
				else
				{
					this.WriteSerializedRegWatermark(flag, num3, DateTime.UtcNow);
				}
			}
			else
			{
				base.Result.StateAttribute3 = "RegValue not found or deserialization failed.";
				this.WriteSerializedRegWatermark(flag, num3, DateTime.UtcNow);
			}
			base.Result.StateAttribute5 = string.Format("Pass", new object[0]);
		}

		private bool TryDeserializeRegWatermark(out bool overThreshold, out double lastValue, out DateTime lastStateChange)
		{
			string value = HighAvailabilityUtility.NonCachedRegReader.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States", "DiskLatencyWatermark", string.Empty);
			if (string.IsNullOrEmpty(value))
			{
				overThreshold = false;
				lastValue = 0.0;
				lastStateChange = DateTime.MinValue;
				return false;
			}
			string[] array = value.Split(new char[]
			{
				';'
			});
			if (array.Length != 3)
			{
				overThreshold = false;
				lastValue = 0.0;
				lastStateChange = DateTime.MinValue;
				return false;
			}
			try
			{
				overThreshold = bool.Parse(array[0]);
				lastValue = double.Parse(array[1]);
				lastStateChange = DateTime.Parse(array[2]);
			}
			catch (FormatException)
			{
				overThreshold = false;
				lastValue = 0.0;
				lastStateChange = DateTime.MinValue;
				return false;
			}
			return true;
		}

		private void WriteSerializedRegWatermark(bool overThreshold, double lastValue, DateTime lastStateChange)
		{
			string text = string.Join(";", new string[]
			{
				overThreshold.ToString(),
				lastValue.ToString(),
				lastStateChange.ToString()
			});
			HighAvailabilityUtility.RegWriter.CreateSubKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States");
			HighAvailabilityUtility.RegWriter.SetValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States", "DiskLatencyWatermark", text, RegistryValueKind.String);
			base.Result.StateAttribute1 = "RegKey updated!";
		}

		public static readonly string ThresholdAttrName = "threshold";

		public static readonly string LatencyThresholdAttrName = "latencyThreshold";
	}
}
