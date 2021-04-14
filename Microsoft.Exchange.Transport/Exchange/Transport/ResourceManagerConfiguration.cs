using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport
{
	internal class ResourceManagerConfiguration
	{
		public bool EnableResourceMonitoring
		{
			get
			{
				return this.enableResourceMonitoring;
			}
			protected set
			{
				this.enableResourceMonitoring = value;
			}
		}

		public TimeSpan ResourceMonitoringInterval
		{
			get
			{
				return this.resourceMonitoringInterval;
			}
			protected set
			{
				this.resourceMonitoringInterval = value;
			}
		}

		public bool DehydrateMessagesUnderMemoryPressure
		{
			get
			{
				return this.dehydrateMessagesUnderMemoryPressure;
			}
			protected set
			{
				this.dehydrateMessagesUnderMemoryPressure = value;
			}
		}

		public ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration PrivateBytesResourceMonitor
		{
			get
			{
				return this.privateBytesResourceMonitor;
			}
			protected set
			{
				this.privateBytesResourceMonitor = value;
			}
		}

		public ResourceManagerConfiguration.ResourceMonitorConfiguration DatabaseDiskSpaceResourceMonitor
		{
			get
			{
				return this.databaseDiskSpaceResourceMonitor;
			}
			protected set
			{
				this.databaseDiskSpaceResourceMonitor = value;
			}
		}

		public ResourceManagerConfiguration.ResourceMonitorConfiguration DatabaseLoggingDiskSpaceResourceMonitor
		{
			get
			{
				return this.databaseLoggingDiskSpaceResourceMonitor;
			}
			protected set
			{
				this.databaseLoggingDiskSpaceResourceMonitor = value;
			}
		}

		public ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration VersionBucketsResourceMonitor
		{
			get
			{
				return this.versionBucketsResourceMonitor;
			}
			protected set
			{
				this.versionBucketsResourceMonitor = value;
			}
		}

		public ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration SubmissionQueueResourceMonitor
		{
			get
			{
				return this.submissionQueueResourceMonitor;
			}
			protected set
			{
				this.submissionQueueResourceMonitor = value;
			}
		}

		public ResourceManagerConfiguration.ResourceMonitorConfiguration MemoryTotalBytesResourceMonitor
		{
			get
			{
				return this.memoryTotalBytesResourceMonitor;
			}
			protected set
			{
				this.memoryTotalBytesResourceMonitor = value;
			}
		}

		public ResourceManagerConfiguration.ThrottlingControllerConfiguration ThrottlingControllerConfig
		{
			get
			{
				return this.throttlingControllerConfiguration;
			}
			protected set
			{
				this.throttlingControllerConfiguration = value;
			}
		}

		public void AddDiagnosticInfo(XElement parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			parent.Add(new object[]
			{
				new XElement("enableResourceMonitoring", this.enableResourceMonitoring),
				new XElement("resourceMonitoringInterval", this.resourceMonitoringInterval),
				new XElement("dehydrateMessagesUnderMemoryPressure", this.dehydrateMessagesUnderMemoryPressure)
			});
		}

		public virtual void Load()
		{
			this.enableResourceMonitoring = Components.TransportAppConfig.ResourceManager.EnableResourceMonitoring;
			this.resourceMonitoringInterval = Components.TransportAppConfig.ResourceManager.ResourceMonitoringInterval;
			this.dehydrateMessagesUnderMemoryPressure = Components.TransportAppConfig.ResourceManager.DehydrateMessagesUnderMemoryPressure;
			this.privateBytesResourceMonitor = new ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration(Components.TransportAppConfig.ResourceManager.PercentagePrivateBytesHighThreshold, Components.TransportAppConfig.ResourceManager.PercentagePrivateBytesMediumThreshold, Components.TransportAppConfig.ResourceManager.PercentagePrivateBytesNormalThreshold, Components.TransportAppConfig.ResourceManager.PrivateBytesHistoryDepth);
			this.databaseDiskSpaceResourceMonitor = new ResourceManagerConfiguration.ResourceMonitorConfiguration(Components.TransportAppConfig.ResourceManager.PercentageDatabaseDiskSpaceHighThreshold, Components.TransportAppConfig.ResourceManager.PercentageDatabaseDiskSpaceMediumThreshold, Components.TransportAppConfig.ResourceManager.PercentageDatabaseDiskSpaceNormalThreshold);
			this.databaseLoggingDiskSpaceResourceMonitor = new ResourceManagerConfiguration.ResourceMonitorConfiguration(Components.TransportAppConfig.ResourceManager.PercentageDatabaseLoggingDiskSpaceHighThreshold, Components.TransportAppConfig.ResourceManager.PercentageDatabaseLoggingDiskSpaceMediumThreshold, Components.TransportAppConfig.ResourceManager.PercentageDatabaseLoggingDiskSpaceNormalThreshold);
			this.versionBucketsResourceMonitor = new ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration(Components.TransportAppConfig.ResourceManager.VersionBucketsHighThreshold, Components.TransportAppConfig.ResourceManager.VersionBucketsMediumThreshold, Components.TransportAppConfig.ResourceManager.VersionBucketsNormalThreshold, Components.TransportAppConfig.ResourceManager.VersionBucketsHistoryDepth);
			this.submissionQueueResourceMonitor = new ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration(Components.TransportAppConfig.ResourceManager.SubmissionQueueHighThreshold, Components.TransportAppConfig.ResourceManager.SubmissionQueueMediumThreshold, Components.TransportAppConfig.ResourceManager.SubmissionQueueNormalThreshold, Components.TransportAppConfig.ResourceManager.SubmissionQueueHistoryDepth);
			this.memoryTotalBytesResourceMonitor = new ResourceManagerConfiguration.ResourceMonitorConfiguration(Components.TransportAppConfig.ResourceManager.PercentagePhysicalMemoryUsedLimit, Components.TransportAppConfig.ResourceManager.PercentagePhysicalMemoryUsedLimit - 5, Components.TransportAppConfig.ResourceManager.PercentagePhysicalMemoryUsedLimit - 10);
			this.throttlingControllerConfiguration = new ResourceManagerConfiguration.ThrottlingControllerConfiguration(Components.TransportAppConfig.ResourceManager.BaseThrottlingDelayInterval, Components.TransportAppConfig.ResourceManager.StartThrottlingDelayInterval, Components.TransportAppConfig.ResourceManager.StepThrottlingDelayInterval, Components.TransportAppConfig.ResourceManager.MaxThrottlingDelayInterval);
		}

		private bool enableResourceMonitoring;

		private TimeSpan resourceMonitoringInterval;

		private bool dehydrateMessagesUnderMemoryPressure;

		private ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration privateBytesResourceMonitor;

		private ResourceManagerConfiguration.ResourceMonitorConfiguration databaseDiskSpaceResourceMonitor;

		private ResourceManagerConfiguration.ResourceMonitorConfiguration databaseLoggingDiskSpaceResourceMonitor;

		private ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration versionBucketsResourceMonitor;

		private ResourceManagerConfiguration.StabilizedResourceMonitorConfiguration submissionQueueResourceMonitor;

		private ResourceManagerConfiguration.ResourceMonitorConfiguration memoryTotalBytesResourceMonitor;

		private ResourceManagerConfiguration.ThrottlingControllerConfiguration throttlingControllerConfiguration;

		internal class ResourceMonitorConfiguration
		{
			public ResourceMonitorConfiguration(int highThreshold, int mediumThreshold, int normalThreshold)
			{
				this.highThreshold = highThreshold;
				this.mediumThreshold = mediumThreshold;
				this.normalThreshold = normalThreshold;
			}

			public int HighThreshold
			{
				get
				{
					return this.highThreshold;
				}
			}

			public int MediumThreshold
			{
				get
				{
					return this.mediumThreshold;
				}
			}

			public int NormalThreshold
			{
				get
				{
					return this.normalThreshold;
				}
			}

			public virtual void AddDiagnosticInfo(XElement parent)
			{
				if (parent == null)
				{
					throw new ArgumentNullException("parent");
				}
				parent.Add(new object[]
				{
					new XElement("normalThreshold", this.normalThreshold),
					new XElement("mediumThreshold", this.mediumThreshold),
					new XElement("highThreshold", this.highThreshold)
				});
			}

			private int highThreshold;

			private int mediumThreshold;

			private int normalThreshold;
		}

		internal class StabilizedResourceMonitorConfiguration : ResourceManagerConfiguration.ResourceMonitorConfiguration
		{
			public StabilizedResourceMonitorConfiguration(int highThreshold, int mediumThreshold, int normalThreshold, int historyDepth) : base(highThreshold, mediumThreshold, normalThreshold)
			{
				this.historyDepth = historyDepth;
			}

			public int HistoryDepth
			{
				get
				{
					return this.historyDepth;
				}
			}

			public override void AddDiagnosticInfo(XElement parent)
			{
				base.AddDiagnosticInfo(parent);
				parent.Add(new XElement("historyDepth", this.historyDepth));
			}

			private int historyDepth;
		}

		internal class ThrottlingControllerConfiguration
		{
			public ThrottlingControllerConfiguration(TimeSpan baseThrottlingDelayInterval, TimeSpan startThrottlingDelayInterval, TimeSpan stepThrottlingDelayInterval, TimeSpan maxThrottlingDelayInterval)
			{
				this.baseThrottlingDelayInterval = baseThrottlingDelayInterval;
				this.maxThrottlingDelayInterval = maxThrottlingDelayInterval;
				this.stepThrottlingDelayInterval = stepThrottlingDelayInterval;
				this.startThrottlingDelayInterval = startThrottlingDelayInterval;
			}

			public TimeSpan BaseThrottlingDelayInterval
			{
				get
				{
					return this.baseThrottlingDelayInterval;
				}
			}

			public TimeSpan MaxThrottlingDelayInterval
			{
				get
				{
					return this.maxThrottlingDelayInterval;
				}
			}

			public TimeSpan StepThrottlingDelayInterval
			{
				get
				{
					return this.stepThrottlingDelayInterval;
				}
			}

			public TimeSpan StartThrottlingDelayInterval
			{
				get
				{
					return this.startThrottlingDelayInterval;
				}
			}

			public void AddDiagnosticInfo(XElement parent)
			{
				if (parent == null)
				{
					throw new ArgumentNullException("parent");
				}
				parent.Add(new object[]
				{
					new XElement("baseThrottlingDelayInterval", this.baseThrottlingDelayInterval),
					new XElement("startThrottlingDelayInterval", this.startThrottlingDelayInterval),
					new XElement("stepThrottlingDelayInterval", this.stepThrottlingDelayInterval),
					new XElement("maxThrottlingDelayInterval", this.maxThrottlingDelayInterval)
				});
			}

			private TimeSpan baseThrottlingDelayInterval;

			private TimeSpan maxThrottlingDelayInterval;

			private TimeSpan stepThrottlingDelayInterval;

			private TimeSpan startThrottlingDelayInterval;
		}
	}
}
