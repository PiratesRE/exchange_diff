using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IEacWebRequest_Implementation_ : IEacWebRequest, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public string Request
		{
			get
			{
				return this._Request_MaterializedValue_;
			}
		}

		public bool Enabled
		{
			get
			{
				return this._Enabled_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal string _Request_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;
	}
}
