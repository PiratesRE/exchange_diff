using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	public class PartitionElement : ConfigurationElement
	{
		[ConfigurationProperty("Name", DefaultValue = "", IsKey = true, IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 1)]
		[ConfigurationProperty("CopyId", DefaultValue = 0, IsKey = false, IsRequired = true)]
		public int CopyId
		{
			get
			{
				return (int)base["CopyId"];
			}
			set
			{
				base["CopyId"] = value;
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 23)]
		[ConfigurationProperty("PartitionId", DefaultValue = 0, IsKey = false, IsRequired = true)]
		public int PartitionId
		{
			get
			{
				return (int)base["PartitionId"];
			}
			set
			{
				base["PartitionId"] = value;
			}
		}

		[ConfigurationProperty("DbWriteFailurePercent", DefaultValue = 0, IsKey = false, IsRequired = false)]
		[IntegerValidator(MinValue = 0, MaxValue = 100)]
		public int DbWriteFailurePercent
		{
			get
			{
				return (int)base["DbWriteFailurePercent"];
			}
			set
			{
				base["DbWriteFailurePercent"] = value;
			}
		}

		[ConfigurationProperty("WriteToRealDB", IsRequired = false, DefaultValue = "false")]
		public bool WriteToRealDB
		{
			get
			{
				return (bool)base["WriteToRealDB"];
			}
			set
			{
				base["WriteToRealDB"] = value;
			}
		}

		[ConfigurationProperty("DBWriteTime", IsRequired = false, DefaultValue = "00:00:04")]
		[TimeSpanValidator(MinValueString = "00:00:00.100")]
		public TimeSpan DBWriteTime
		{
			get
			{
				return (TimeSpan)base["DBWriteTime"];
			}
			set
			{
				base["DBWriteTime"] = value;
			}
		}

		[ConfigurationProperty("IsHealthy", IsRequired = false, DefaultValue = "true")]
		public bool IsHealthy
		{
			get
			{
				return (bool)base["IsHealthy"];
			}
			set
			{
				base["IsHealthy"] = value;
			}
		}

		[ConfigurationProperty("ExceptionString", DefaultValue = "", IsKey = false, IsRequired = false)]
		public string ExceptionString
		{
			get
			{
				return (string)base["ExceptionString"];
			}
			set
			{
				base["ExceptionString"] = value;
			}
		}
	}
}
