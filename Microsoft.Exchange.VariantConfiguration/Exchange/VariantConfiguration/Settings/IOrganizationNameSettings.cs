using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IOrganizationNameSettings : ISettings
	{
		IList<string> OrgNames { get; }

		IList<string> DogfoodOrgNames { get; }
	}
}
