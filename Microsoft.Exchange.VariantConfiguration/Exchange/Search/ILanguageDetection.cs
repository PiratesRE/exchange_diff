using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface ILanguageDetection : ISettings
	{
		bool EnableLanguageDetectionLogging { get; }

		int SamplingFrequency { get; }

		bool EnableLanguageSelection { get; }

		string RegionDefaultLanguage { get; }
	}
}
