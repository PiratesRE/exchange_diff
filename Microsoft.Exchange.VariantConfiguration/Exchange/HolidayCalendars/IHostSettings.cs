using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HolidayCalendars
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IHostSettings : ISettings
	{
		string Endpoint { get; }

		int Timeout { get; }
	}
}
