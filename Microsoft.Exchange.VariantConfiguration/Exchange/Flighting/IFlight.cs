using System;
using System.CodeDom.Compiler;

namespace Microsoft.Exchange.Flighting
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IFlight
	{
		string Name { get; }

		bool Enabled { get; }

		string Rotate { get; }

		string Ramp { get; }

		string RampSeed { get; }
	}
}
