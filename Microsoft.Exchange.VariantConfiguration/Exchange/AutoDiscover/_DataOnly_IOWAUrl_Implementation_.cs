using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.AutoDiscover
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IOWAUrl_Implementation_ : IOWAUrl, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public string OwaInternalAuthMethods
		{
			get
			{
				return this._OwaInternalAuthMethods_MaterializedValue_;
			}
		}

		public string OwaExternalAuthMethods
		{
			get
			{
				return this._OwaExternalAuthMethods_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal string _OwaInternalAuthMethods_MaterializedValue_;

		internal string _OwaExternalAuthMethods_MaterializedValue_;
	}
}
