using System;
using System.Configuration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSProxyConfiguration : ConfigurationSection
	{
		private MRSProxyConfiguration()
		{
		}

		public static MRSProxyConfiguration Instance
		{
			get
			{
				MRSProxyConfiguration mrsproxyConfiguration = null;
				try
				{
					mrsproxyConfiguration = (MRSProxyConfiguration)ConfigurationManager.GetSection("MRSProxyConfiguration");
				}
				catch (ConfigurationErrorsException ex)
				{
					MrsTracer.ProxyService.Error("Failed to load MrsProxyConfiguration: {0}", new object[]
					{
						ex.ToString()
					});
				}
				if (mrsproxyConfiguration == null)
				{
					mrsproxyConfiguration = new MRSProxyConfiguration();
				}
				return mrsproxyConfiguration;
			}
		}

		[ConfigurationProperty("DataImportTimeout", DefaultValue = "00:05:00")]
		[TimeSpanValidator(MinValueString = "00:00:10", MaxValueString = "00:30:00", ExcludeRange = false)]
		internal TimeSpan DataImportTimeout
		{
			get
			{
				return (TimeSpan)base["DataImportTimeout"];
			}
			set
			{
				base["DataImportTimeout"] = value;
			}
		}

		[ConfigurationProperty("MaxMRSConnections", DefaultValue = "100")]
		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		internal int MaxMRSConnections
		{
			get
			{
				return (int)base["MaxMRSConnections"];
			}
			set
			{
				base["MaxMRSConnections"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			MrsTracer.ProxyService.Warning("Ignoring unrecognized configuration attribute {0}={1}", new object[]
			{
				name,
				value
			});
			return true;
		}

		private static class MrsProxyConfigSchema
		{
			public const string SectionName = "MRSProxyConfiguration";

			public const string DataImportTimeout = "DataImportTimeout";

			public const string MaxMRSConnections = "MaxMRSConnections";
		}
	}
}
