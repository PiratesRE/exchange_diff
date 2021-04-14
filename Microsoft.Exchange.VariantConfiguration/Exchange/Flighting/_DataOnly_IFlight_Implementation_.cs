using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IFlight_Implementation_ : IFlight, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public bool Enabled
		{
			get
			{
				return this._Enabled_MaterializedValue_;
			}
		}

		public string Rotate
		{
			get
			{
				return this._Rotate_MaterializedValue_;
			}
		}

		public string Ramp
		{
			get
			{
				return this._Ramp_MaterializedValue_;
			}
		}

		public string RampSeed
		{
			get
			{
				return this._RampSeed_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal string _Rotate_MaterializedValue_;

		internal string _Ramp_MaterializedValue_;

		internal string _RampSeed_MaterializedValue_;
	}
}
