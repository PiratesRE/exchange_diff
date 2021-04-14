using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMExtension
	{
		public UMExtension()
		{
		}

		public UMExtension(string extension, string phoneContext, string dpName)
		{
			this.Extension = extension;
			this.PhoneContext = phoneContext;
			this.DialPlanName = dpName;
		}

		[DataMember]
		public string PhoneContext { get; set; }

		[DataMember]
		public string DialPlanName { get; set; }

		[DataMember]
		public string Extension { get; set; }
	}
}
