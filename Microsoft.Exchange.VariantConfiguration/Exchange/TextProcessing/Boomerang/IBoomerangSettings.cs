using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IBoomerangSettings : ISettings
	{
		bool Enabled { get; }

		string MasterKeyRegistryPath { get; }

		string MasterKeyRegistryKeyName { get; }

		uint NumberOfValidIntervalsInDays { get; }
	}
}
