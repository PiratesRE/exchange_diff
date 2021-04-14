using System;
using System.CodeDom.Compiler;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IOverrideSyncSettings : IFeature, ISettings
	{
		TimeSpan RefreshInterval { get; }
	}
}
