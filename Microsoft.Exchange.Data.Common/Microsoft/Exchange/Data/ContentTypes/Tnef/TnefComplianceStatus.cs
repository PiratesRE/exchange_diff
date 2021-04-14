using System;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	[Flags]
	public enum TnefComplianceStatus
	{
		Compliant = 0,
		InvalidAttribute = 1,
		InvalidAttributeLevel = 2,
		InvalidAttributeLength = 16,
		StreamTruncated = 32,
		InvalidTnefSignature = 64,
		InvalidTnefVersion = 256,
		InvalidMessageClass = 512,
		InvalidRowCount = 1024,
		InvalidAttributeValue = 2048,
		AttributeOverflow = 4096,
		InvalidAttributeChecksum = 8192,
		InvalidMessageCodepage = 16384,
		UnsupportedPropertyType = 32768,
		InvalidPropertyLength = 65536,
		InvalidDate = 131072,
		NestingTooDeep = 262144
	}
}
