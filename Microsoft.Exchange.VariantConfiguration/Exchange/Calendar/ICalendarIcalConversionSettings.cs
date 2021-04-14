using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Calendar
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface ICalendarIcalConversionSettings : ISettings
	{
		bool LocalTimeZoneReferenceForRecurrenceNeeded { get; }
	}
}
