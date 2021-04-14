using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetResponseMessageConfiguration : SetResourceConfigurationBase
	{
		[DataMember]
		public bool AddAdditionalResponse
		{
			get
			{
				return (bool)(base["AddAdditionalResponse"] ?? false);
			}
			set
			{
				base["AddAdditionalResponse"] = value;
			}
		}

		[DataMember]
		public string AdditionalResponse
		{
			get
			{
				return (string)base["AdditionalResponse"];
			}
			set
			{
				base["AdditionalResponse"] = value;
			}
		}
	}
}
