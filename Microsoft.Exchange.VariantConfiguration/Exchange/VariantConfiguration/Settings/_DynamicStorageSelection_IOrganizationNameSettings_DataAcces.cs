using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_ : VariantObjectDataAccessorBase<IOrganizationNameSettings, _DynamicStorageSelection_IOrganizationNameSettings_Implementation_, _DynamicStorageSelection_IOrganizationNameSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal IList<string> _OrgNames_MaterializedValue_;

		internal ValueProvider<IList<string>> _OrgNames_ValueProvider_;

		internal IList<string> _DogfoodOrgNames_MaterializedValue_;

		internal ValueProvider<IList<string>> _DogfoodOrgNames_ValueProvider_;
	}
}
