using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessageDepot
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IMessageDepotSettings : ISettings
	{
		bool Enabled { get; }

		IList<DayOfWeek> EnabledOnDaysOfWeek { get; }
	}
}
