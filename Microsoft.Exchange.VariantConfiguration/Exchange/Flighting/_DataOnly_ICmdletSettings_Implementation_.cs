using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_ICmdletSettings_Implementation_ : ICmdletSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public IList<string> AllFlightingParams
		{
			get
			{
				return this._AllFlightingParams_MaterializedValue_;
			}
		}

		public IList<string> Params0
		{
			get
			{
				return this._Params0_MaterializedValue_;
			}
		}

		public IList<string> Params1
		{
			get
			{
				return this._Params1_MaterializedValue_;
			}
		}

		public IList<string> Params2
		{
			get
			{
				return this._Params2_MaterializedValue_;
			}
		}

		public IList<string> Params3
		{
			get
			{
				return this._Params3_MaterializedValue_;
			}
		}

		public IList<string> Params4
		{
			get
			{
				return this._Params4_MaterializedValue_;
			}
		}

		public IList<string> Params5
		{
			get
			{
				return this._Params5_MaterializedValue_;
			}
		}

		public IList<string> Params6
		{
			get
			{
				return this._Params6_MaterializedValue_;
			}
		}

		public IList<string> Params7
		{
			get
			{
				return this._Params7_MaterializedValue_;
			}
		}

		public IList<string> Params8
		{
			get
			{
				return this._Params8_MaterializedValue_;
			}
		}

		public IList<string> Params9
		{
			get
			{
				return this._Params9_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal IList<string> _AllFlightingParams_MaterializedValue_;

		internal IList<string> _Params0_MaterializedValue_;

		internal IList<string> _Params1_MaterializedValue_;

		internal IList<string> _Params2_MaterializedValue_;

		internal IList<string> _Params3_MaterializedValue_;

		internal IList<string> _Params4_MaterializedValue_;

		internal IList<string> _Params5_MaterializedValue_;

		internal IList<string> _Params6_MaterializedValue_;

		internal IList<string> _Params7_MaterializedValue_;

		internal IList<string> _Params8_MaterializedValue_;

		internal IList<string> _Params9_MaterializedValue_;
	}
}
