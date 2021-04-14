using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum BodyFormat
	{
		[EnumMember]
		PlainText = 1,
		[EnumMember]
		Html,
		[EnumMember]
		Rtf
	}
}
