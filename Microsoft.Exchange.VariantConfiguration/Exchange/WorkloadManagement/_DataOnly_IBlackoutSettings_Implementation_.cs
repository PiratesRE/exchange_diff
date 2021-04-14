using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.WorkloadManagement
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IBlackoutSettings_Implementation_ : IBlackoutSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public TimeSpan StartTime
		{
			get
			{
				return this._StartTime_MaterializedValue_;
			}
		}

		public TimeSpan EndTime
		{
			get
			{
				return this._EndTime_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal TimeSpan _StartTime_MaterializedValue_ = default(TimeSpan);

		internal TimeSpan _EndTime_MaterializedValue_ = default(TimeSpan);
	}
}
