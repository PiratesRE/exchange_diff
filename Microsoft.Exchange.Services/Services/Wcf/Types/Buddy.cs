using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class Buddy
	{
		internal Buddy(string displayName, string imAddress)
		{
			this.DisplayName = displayName;
			this.IMAddress = imAddress;
		}

		[DataMember]
		public string DisplayName { get; internal set; }

		[DataMember]
		public string IMAddress { get; internal set; }

		[DataMember]
		public string GroupId { get; internal set; }
	}
}
