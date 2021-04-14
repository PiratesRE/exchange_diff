using System;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Flags]
	public enum CalendarComplianceStatus
	{
		Compliant = 0,
		StreamTruncated = 1,
		PropertyTruncated = 2,
		InvalidCharacterInPropertyName = 4,
		InvalidCharacterInParameterName = 8,
		InvalidCharacterInParameterText = 16,
		InvalidCharacterInQuotedString = 32,
		InvalidCharacterInPropertyValue = 64,
		NotAllComponentsClosed = 128,
		ParametersOnComponentTag = 256,
		EndTagWithoutBegin = 512,
		ComponentNameMismatch = 1024,
		InvalidValueFormat = 2048,
		EmptyParameterName = 4096,
		EmptyPropertyName = 8192,
		EmptyComponentName = 16384,
		PropertyOutsideOfComponent = 32768,
		InvalidParameterValue = 65536,
		ParameterNameMissing = 131072
	}
}
