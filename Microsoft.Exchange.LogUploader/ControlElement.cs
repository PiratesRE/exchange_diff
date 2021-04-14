using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	public class ControlElement : ConfigurationElement
	{
		[ConfigurationProperty("SwitchOn", DefaultValue = "false", IsRequired = true)]
		public bool SwitchOn
		{
			get
			{
				return (bool)base["SwitchOn"];
			}
			set
			{
				base["SwitchOn"] = value;
			}
		}

		[ConfigurationProperty("CopiesCount", DefaultValue = 1, IsKey = false, IsRequired = false)]
		[IntegerValidator(MinValue = 1, MaxValue = 2)]
		public int CopiesCount
		{
			get
			{
				return (int)base["CopiesCount"];
			}
			set
			{
				base["CopiesCount"] = value;
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 24)]
		[ConfigurationProperty("PartitionsCount", DefaultValue = 4, IsKey = false, IsRequired = false)]
		public int PartitionsCount
		{
			get
			{
				return (int)base["PartitionsCount"];
			}
			set
			{
				base["PartitionsCount"] = value;
			}
		}
	}
}
