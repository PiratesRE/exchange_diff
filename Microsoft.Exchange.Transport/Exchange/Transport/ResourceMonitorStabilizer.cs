using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport
{
	internal class ResourceMonitorStabilizer : ResourceMonitor
	{
		public ResourceMonitorStabilizer(ResourceMonitor resourceMonitor, ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration config) : base("ResourceMonitorStabilizer", config)
		{
			this.resourceMonitor = resourceMonitor;
			this.historyState = new ResourceMonitorStabilizer.HistoryState(config.HistoryDepth);
			base.DisplayName = this.resourceMonitor.DisplayName;
		}

		public override ResourceUses ResourceUses
		{
			get
			{
				return this.historyState.ResourceUses;
			}
		}

		public override int CurrentPressureRaw
		{
			get
			{
				return this.historyState.CurrentPressureRaw;
			}
		}

		public override ResourceUses CurrentResourceUsesRaw
		{
			get
			{
				return this.historyState.CurrentResourceUsesRaw;
			}
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			this.resourceMonitor.UpdateReading();
			this.historyState.AddReading(this.resourceMonitor.ResourceUses, this.resourceMonitor.CurrentPressure);
			currentReading = this.historyState.CurrentPressure;
			return true;
		}

		public override void DoCleanup()
		{
			this.resourceMonitor.DoCleanup();
		}

		public override XElement GetDiagnosticInfo(bool verbose)
		{
			XElement diagnosticInfo = this.resourceMonitor.GetDiagnosticInfo(verbose);
			diagnosticInfo.Add(this.historyState.GetDiagnosticInfo(verbose));
			return diagnosticInfo;
		}

		public override string ToString()
		{
			return this.resourceMonitor.ToString(this.ResourceUses, base.CurrentPressure);
		}

		public override void UpdateConfig()
		{
			int historyDepth = this.historyState.HistoryDepth;
			this.resourceMonitor.UpdateConfig();
			base.HighPressureLimit = this.resourceMonitor.HighPressureLimit;
			base.MediumPressureLimit = this.resourceMonitor.MediumPressureLimit;
			base.LowPressureLimit = this.resourceMonitor.LowPressureLimit;
			this.historyState = new ResourceMonitorStabilizer.HistoryState(historyDepth);
		}

		private ResourceMonitor resourceMonitor;

		private ResourceMonitorStabilizer.HistoryState historyState;

		private sealed class HistoryState
		{
			public HistoryState(int historyDepth)
			{
				if (historyDepth < 1)
				{
					throw new ArgumentException("historyDepth can't be smaller than 1.");
				}
				this.measurementHistory = new ResourceMonitorStabilizer.HistoryState.Measurement[historyDepth];
				for (int i = 0; i < historyDepth; i++)
				{
					this.measurementHistory[i] = new ResourceMonitorStabilizer.HistoryState.Measurement();
				}
				this.measurement = this.measurementHistory[0];
				this.currentResourceUsesRaw = ResourceUses.Normal;
			}

			public int HistoryDepth
			{
				get
				{
					return this.measurementHistory.Length;
				}
			}

			public ResourceUses ResourceUses
			{
				get
				{
					return this.measurement.ResourceUses;
				}
			}

			public ResourceUses CurrentResourceUsesRaw
			{
				get
				{
					return this.currentResourceUsesRaw;
				}
			}

			public int CurrentPressure
			{
				get
				{
					return this.measurement.Pressure;
				}
			}

			public int CurrentPressureRaw
			{
				get
				{
					return this.currentPressureRaw;
				}
			}

			public void AddReading(ResourceUses resourceUses, int pressure)
			{
				this.currentPressureRaw = pressure;
				this.currentResourceUsesRaw = resourceUses;
				this.measurementHistory[this.currentIndex] = new ResourceMonitorStabilizer.HistoryState.Measurement(resourceUses, pressure);
				this.currentIndex = (this.currentIndex + 1) % this.HistoryDepth;
				this.CalculateStabilizedState();
			}

			public XElement GetDiagnosticInfo(bool verbose)
			{
				XElement xelement = new XElement("HistoricalMeasurementsCollection", new XElement("count", this.HistoryDepth));
				if (verbose)
				{
					for (int i = 0; i < this.HistoryDepth; i++)
					{
						ResourceMonitorStabilizer.HistoryState.Measurement measurement = this.measurementHistory[this.MapLogicalToPhysicalIndex(i)];
						xelement.Add(measurement.GetDiagnosticInfo());
					}
				}
				else
				{
					xelement.Add(new XElement("help", "Use 'verbose' to get the measurements."));
				}
				return xelement;
			}

			private int MapLogicalToPhysicalIndex(int index)
			{
				int historyDepth = this.HistoryDepth;
				if (index < 0 || index >= historyDepth)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (index + this.currentIndex) % historyDepth;
			}

			private void CalculateStabilizedState()
			{
				ResourceMonitorStabilizer.HistoryState.Measurement measurement = new ResourceMonitorStabilizer.HistoryState.Measurement(ResourceUses.High, int.MaxValue);
				for (int i = 0; i < this.HistoryDepth; i++)
				{
					ResourceMonitorStabilizer.HistoryState.Measurement measurement2 = this.measurementHistory[this.MapLogicalToPhysicalIndex(i)];
					if (measurement2.ResourceUses < measurement.ResourceUses || measurement2.Pressure < measurement.Pressure)
					{
						measurement = measurement2;
					}
				}
				this.measurement = measurement;
			}

			private ResourceMonitorStabilizer.HistoryState.Measurement[] measurementHistory;

			private int currentIndex;

			private ResourceMonitorStabilizer.HistoryState.Measurement measurement;

			private int currentPressureRaw;

			private ResourceUses currentResourceUsesRaw;

			private sealed class Measurement
			{
				public Measurement() : this(ResourceUses.Normal, 0)
				{
				}

				public Measurement(ResourceUses resourceUses, int pressure)
				{
					this.resourceUses = resourceUses;
					this.pressure = pressure;
				}

				public ResourceUses ResourceUses
				{
					get
					{
						return this.resourceUses;
					}
				}

				public int Pressure
				{
					get
					{
						return this.pressure;
					}
				}

				public XElement GetDiagnosticInfo()
				{
					return new XElement("Measurement", new object[]
					{
						new XElement("pressureRaw", this.pressure),
						new XElement("resourceUsesRaw", this.resourceUses)
					});
				}

				private readonly ResourceUses resourceUses;

				private readonly int pressure;
			}
		}
	}
}
