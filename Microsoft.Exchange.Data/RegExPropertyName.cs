using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract]
	public enum RegExPropertyName
	{
		[EnumMember]
		Subject = 1,
		[EnumMember]
		BodyAsPlaintext,
		[EnumMember]
		BodyAsHTML,
		[EnumMember]
		SenderSMTPAddress
	}
}
