using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IWorkloadSettings_Implementation_ : IWorkloadSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public WorkloadClassification Classification
		{
			get
			{
				return this._Classification_MaterializedValue_;
			}
		}

		public int MaxConcurrency
		{
			get
			{
				return this._MaxConcurrency_MaterializedValue_;
			}
		}

		public bool Enabled
		{
			get
			{
				return this._Enabled_MaterializedValue_;
			}
		}

		public bool EnabledDuringBlackout
		{
			get
			{
				return this._EnabledDuringBlackout_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal WorkloadClassification _Classification_MaterializedValue_;

		internal int _MaxConcurrency_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal bool _EnabledDuringBlackout_MaterializedValue_;
	}
}
