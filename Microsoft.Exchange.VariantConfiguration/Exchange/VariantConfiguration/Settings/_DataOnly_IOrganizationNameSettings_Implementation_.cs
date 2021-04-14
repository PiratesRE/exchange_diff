using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IOrganizationNameSettings_Implementation_ : IOrganizationNameSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return null;
			}
		}

		IVariantObjectInstance IVariantObjectInstanceProvider.GetVariantObjectInstance(VariantContextSnapshot context)
		{
			return this;
		}

		public string Name
		{
			get
			{
				return this._Name_MaterializedValue_;
			}
		}

		public IList<string> OrgNames
		{
			get
			{
				return this._OrgNames_MaterializedValue_;
			}
		}

		public IList<string> DogfoodOrgNames
		{
			get
			{
				return this._DogfoodOrgNames_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal IList<string> _OrgNames_MaterializedValue_;

		internal IList<string> _DogfoodOrgNames_MaterializedValue_;
	}
}
