using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal class ResourceMeteringConfig : TransportAppConfig
	{
		public ResourceMeteringConfig(int maxVersionBuckets, NameValueCollection appSettings = null) : base(appSettings)
		{
			ArgumentValidator.ThrowIfInvalidValue<int>("maxVersionBuckets", maxVersionBuckets, (int versionBuckets) => versionBuckets > 100);
			this.maxPressureTransitions.Add("UsedVersionBuckets", new PressureTransitions((long)(maxVersionBuckets - 97), (long)(maxVersionBuckets - 98), (long)(maxVersionBuckets - 99), (long)(maxVersionBuckets - 100)));
			this.isResourceTrackingEnabled = base.GetConfigBool("ResourceTrackingEnabled", false);
			if (this.isResourceTrackingEnabled)
			{
				this.statusPublishInterval = base.GetConfigTimeSpan("StatusPublishInterval", TimeSpan.FromSeconds(10.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(1.0));
				this.disabledResources = this.GetDisabledResources();
				this.resourceMeteringInterval = base.GetConfigTimeSpan("ResourceMeteringInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromSeconds(2.0));
				this.resourceLoggingInterval = base.GetConfigTimeSpan("ResourceLoggingInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromSeconds(2.0));
				this.resourceMeterTimeout = base.GetConfigTimeSpan("ResourceMeterTimeout", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(5.0));
				this.maxTransientExceptionsAllowed = base.GetConfigInt("MaxTransientExceptionsAllowed", 1, 100, 5);
				this.versionBucketsStabilizationSamples = base.GetConfigInt("VersionBucketsStabilizationSamples", 2, 1000, 10);
				this.privateBytesStabilizationSamples = base.GetConfigInt("PrivateBytesStabilizationSamples", 2, 1000, 30);
				this.submissionQueueStabilizationSamples = base.GetConfigInt("SubmissionQueueStabilizationSamples", 2, 1000, 300);
				this.sustainedDuration = base.GetConfigTimeSpan("SustainedDuration", TimeSpan.FromMinutes(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
			}
			this.resourceLogBackgroundWriteInterval = base.GetConfigTimeSpan("ResourceLogBackgroundWriteInterval", TimeSpan.FromMilliseconds(100.0), TimeSpan.FromSeconds(20.0), TimeSpan.FromSeconds(15.0));
			this.resourceLogBufferSize = base.GetConfigInt("ResourceLogBufferSize", 0, (int)ByteQuantifiedSize.FromMB(10UL).ToBytes(), (int)ByteQuantifiedSize.FromKB(64UL).ToBytes());
			this.resourceLogFlushInterval = base.GetConfigTimeSpan("ResourceLogFlushInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromSeconds(60.0));
		}

		public TimeSpan SustainedDuration
		{
			get
			{
				return this.sustainedDuration;
			}
		}

		public TimeSpan StatusPublishInterval
		{
			get
			{
				return this.statusPublishInterval;
			}
		}

		public Dictionary<ResourceIdentifier, PressureTransitions> GetPressureTransitionsForResources(IEnumerable<ResourceIdentifier> meteredResources)
		{
			return this.FetchPressureTransitions(meteredResources);
		}

		public IEnumerable<ResourceIdentifier> DisabledResources
		{
			get
			{
				return this.disabledResources;
			}
		}

		public TimeSpan ResourceMeteringInterval
		{
			get
			{
				return this.resourceMeteringInterval;
			}
		}

		public TimeSpan ResourceLoggingInterval
		{
			get
			{
				return this.resourceLoggingInterval;
			}
		}

		public TimeSpan ResourceLogFlushInterval
		{
			get
			{
				return this.resourceLogFlushInterval;
			}
		}

		public TimeSpan ResourceLogBackgroundWriteInterval
		{
			get
			{
				return this.resourceLogBackgroundWriteInterval;
			}
		}

		public int ResourceLogBufferSize
		{
			get
			{
				return this.resourceLogBufferSize;
			}
		}

		public TimeSpan ResourceMeterTimeout
		{
			get
			{
				return this.resourceMeterTimeout;
			}
		}

		public int MaxTransientExceptionsAllowed
		{
			get
			{
				return this.maxTransientExceptionsAllowed;
			}
		}

		public int VersionBucketsStabilizationSamples
		{
			get
			{
				return this.versionBucketsStabilizationSamples;
			}
		}

		public int PrivateBytesStabilizationSamples
		{
			get
			{
				return this.privateBytesStabilizationSamples;
			}
		}

		public int SubmissionQueueStabilizationSamples
		{
			get
			{
				return this.submissionQueueStabilizationSamples;
			}
		}

		public bool IsResourceTrackingEnabled
		{
			get
			{
				return this.isResourceTrackingEnabled;
			}
		}

		private static PressureTransitions GetDefaultPrivateBytesPressureTransitions()
		{
			ulong val;
			if (Environment.Is64BitProcess)
			{
				val = 1099511627776UL;
			}
			else if (ResourceMeteringConfig.GetTotalVirtualMemory() > ByteQuantifiedSize.FromGB(2UL).ToBytes())
			{
				val = 1887436800UL;
			}
			else
			{
				val = 838860800UL;
			}
			ulong num = Math.Min(ResourceMeteringConfig.TotalPhysicalMemory * 75UL / 100UL, val);
			int num2 = (int)(num * 100UL / ResourceMeteringConfig.TotalPhysicalMemory);
			return new PressureTransitions((long)num2, (long)(num2 - 2), (long)(num2 - 3), (long)(num2 - 4));
		}

		private static ulong GetTotalVirtualMemory()
		{
			INativeMethodsWrapper nativeMethodsWrapper = NativeMethodsWrapperFactory.CreateNativeMethodsWrapper();
			ulong result;
			if (!nativeMethodsWrapper.GetTotalVirtualMemory(out result))
			{
				throw new Win32Exception();
			}
			return result;
		}

		private static ulong GetTotalPhysicalMemory()
		{
			INativeMethodsWrapper nativeMethodsWrapper = NativeMethodsWrapperFactory.CreateNativeMethodsWrapper();
			ulong result;
			if (!nativeMethodsWrapper.GetTotalSystemMemory(out result))
			{
				throw new Win32Exception();
			}
			return result;
		}

		private Dictionary<ResourceIdentifier, PressureTransitions> FetchPressureTransitions(IEnumerable<ResourceIdentifier> meteredResources)
		{
			Dictionary<ResourceIdentifier, PressureTransitions> dictionary = new Dictionary<ResourceIdentifier, PressureTransitions>();
			IEnumerable<ResourceIdentifier> source = this.GetDisabledResources();
			using (IEnumerator<ResourceIdentifier> enumerator = meteredResources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ResourceMeteringConfig.<>c__DisplayClass8 CS$<>8__locals1 = new ResourceMeteringConfig.<>c__DisplayClass8();
					CS$<>8__locals1.resource = enumerator.Current;
					ResourceIdentifier instanceAgnosticResource = new ResourceIdentifier(CS$<>8__locals1.resource.Name, "");
					if (!source.Any((ResourceIdentifier disabled) => CS$<>8__locals1.resource == disabled) && !source.Any((ResourceIdentifier disabled) => instanceAgnosticResource == disabled) && !dictionary.ContainsKey(CS$<>8__locals1.resource))
					{
						PressureTransitions value = this.FetchPressureTransitionForResource(CS$<>8__locals1.resource);
						dictionary.Add(CS$<>8__locals1.resource, value);
					}
				}
			}
			return dictionary;
		}

		private PressureTransitions FetchPressureTransitionForResource(ResourceIdentifier resource)
		{
			PressureTransitions pressureTransitions = this.minPressureTransitions[resource.Name];
			PressureTransitions pressureTransitions2 = this.maxPressureTransitions[resource.Name];
			PressureTransitions pressureTransitions3 = this.defaultPressureTransitions[resource.Name];
			long pressureTransitionValue = this.GetPressureTransitionValue(resource, "MediumToHigh", pressureTransitions.MediumToHigh, pressureTransitions2.MediumToHigh, pressureTransitions3.MediumToHigh);
			long pressureTransitionValue2 = this.GetPressureTransitionValue(resource, "HighToMedium", pressureTransitions.HighToMedium, pressureTransitions2.HighToMedium, pressureTransitions3.HighToMedium);
			long pressureTransitionValue3 = this.GetPressureTransitionValue(resource, "LowToMedium", pressureTransitions.LowToMedium, pressureTransitions2.LowToMedium, pressureTransitions3.LowToMedium);
			long pressureTransitionValue4 = this.GetPressureTransitionValue(resource, "MediumToLow", pressureTransitions.MediumToLow, pressureTransitions2.MediumToLow, pressureTransitions3.MediumToLow);
			return new PressureTransitions(pressureTransitionValue, pressureTransitionValue2, pressureTransitionValue3, pressureTransitionValue4);
		}

		private long GetPressureTransitionValue(ResourceIdentifier resource, string transitionName, long minValue, long maxValue, long defaultValue)
		{
			string configKeyForResourceTransition = this.GetConfigKeyForResourceTransition(resource, transitionName);
			if (string.IsNullOrEmpty(this.AppSettings[configKeyForResourceTransition]))
			{
				ResourceIdentifier resource2 = new ResourceIdentifier(resource.Name, "");
				configKeyForResourceTransition = this.GetConfigKeyForResourceTransition(resource2, transitionName);
			}
			return base.GetConfigLong(configKeyForResourceTransition, minValue, maxValue, defaultValue);
		}

		private IEnumerable<ResourceIdentifier> GetDisabledResources()
		{
			return base.GetConfigList<ResourceIdentifier>("DisabledResourceMeters", ';', new AppConfig.TryParse<ResourceIdentifier>(ResourceIdentifier.TryParse));
		}

		private string GetConfigKeyForResourceTransition(ResourceIdentifier resource, string transition)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
			{
				resource.ToString(),
				transition
			});
		}

		private const int MaxStabilizationSampleCount = 1000;

		private const ulong PrivateBytesLimit2GB = 838860800UL;

		private const ulong PrivateBytesLimit3GB = 1887436800UL;

		private const ulong PrivateBytesLimit64Bit = 1099511627776UL;

		private readonly TimeSpan statusPublishInterval;

		public static readonly ulong TotalPhysicalMemory = ResourceMeteringConfig.GetTotalPhysicalMemory();

		private readonly Dictionary<string, PressureTransitions> minPressureTransitions = new Dictionary<string, PressureTransitions>
		{
			{
				"PrivateBytes",
				new PressureTransitions(4L, 3L, 2L, 1L)
			},
			{
				"QueueLength",
				new PressureTransitions(4L, 3L, 2L, 1L)
			},
			{
				"UsedVersionBuckets",
				new PressureTransitions(4L, 3L, 2L, 1L)
			},
			{
				"DatabaseUsedSpace",
				new PressureTransitions(10L, 5L, 4L, 3L)
			},
			{
				"UsedDiskSpace",
				new PressureTransitions(10L, 5L, 4L, 3L)
			},
			{
				"SystemMemory",
				new PressureTransitions(85L, 80L, 75L, 70L)
			}
		};

		private readonly Dictionary<string, PressureTransitions> defaultPressureTransitions = new Dictionary<string, PressureTransitions>
		{
			{
				"PrivateBytes",
				ResourceMeteringConfig.GetDefaultPrivateBytesPressureTransitions()
			},
			{
				"QueueLength",
				new PressureTransitions(15000L, 10000L, 9999L, 2000L)
			},
			{
				"UsedVersionBuckets",
				new PressureTransitions(2500L, 2000L, 1999L, 1750L)
			},
			{
				"DatabaseUsedSpace",
				new PressureTransitions(100L, 98L, 97L, 96L)
			},
			{
				"UsedDiskSpace",
				new PressureTransitions(99L, 90L, 89L, 80L)
			},
			{
				"SystemMemory",
				new PressureTransitions(94L, 89L, 88L, 84L)
			}
		};

		private readonly Dictionary<string, PressureTransitions> maxPressureTransitions = new Dictionary<string, PressureTransitions>
		{
			{
				"PrivateBytes",
				new PressureTransitions(100L, 99L, 98L, 97L)
			},
			{
				"QueueLength",
				new PressureTransitions(50000L, 45000L, 40000L, 35000L)
			},
			{
				"DatabaseUsedSpace",
				new PressureTransitions(100L, 98L, 97L, 96L)
			},
			{
				"UsedDiskSpace",
				new PressureTransitions(100L, 98L, 97L, 96L)
			},
			{
				"SystemMemory",
				new PressureTransitions(94L, 90L, 85L, 80L)
			}
		};

		private readonly IEnumerable<ResourceIdentifier> disabledResources;

		private readonly TimeSpan resourceMeteringInterval;

		private readonly TimeSpan resourceLoggingInterval;

		private readonly TimeSpan resourceMeterTimeout;

		private readonly int maxTransientExceptionsAllowed;

		private readonly int versionBucketsStabilizationSamples;

		private readonly int privateBytesStabilizationSamples;

		private readonly int submissionQueueStabilizationSamples;

		private readonly bool isResourceTrackingEnabled;

		private readonly TimeSpan resourceLogFlushInterval;

		private readonly TimeSpan resourceLogBackgroundWriteInterval;

		private readonly int resourceLogBufferSize;

		private readonly TimeSpan sustainedDuration;
	}
}
