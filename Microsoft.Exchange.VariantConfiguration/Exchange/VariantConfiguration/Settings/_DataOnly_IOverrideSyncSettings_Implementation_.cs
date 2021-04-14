using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.VariantConfiguration.Settings
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IOverrideSyncSettings_Implementation_ : IOverrideSyncSettings, IFeature, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public TimeSpan RefreshInterval
		{
			get
			{
				return this._RefreshInterval_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal TimeSpan _RefreshInterval_MaterializedValue_ = default(TimeSpan);
	}
}
