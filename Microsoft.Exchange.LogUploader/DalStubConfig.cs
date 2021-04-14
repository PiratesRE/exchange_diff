using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	public class DalStubConfig : ConfigurationSection
	{
		[ConfigurationProperty("Control")]
		public ControlElement Control
		{
			get
			{
				return (ControlElement)base["Control"];
			}
			set
			{
				base["Control"] = value;
			}
		}

		[ConfigurationProperty("PartitionSettings", IsRequired = true)]
		public PartitionsCollection Partitions
		{
			get
			{
				return (PartitionsCollection)base["PartitionSettings"];
			}
		}
	}
}
