using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IClutterModelConfigurationSettings_Implementation_ : IClutterModelConfigurationSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public int MaxModelVersion
		{
			get
			{
				return this._MaxModelVersion_MaterializedValue_;
			}
		}

		public int MinModelVersion
		{
			get
			{
				return this._MinModelVersion_MaterializedValue_;
			}
		}

		public int NumberOfVersionCrumbsToRecord
		{
			get
			{
				return this._NumberOfVersionCrumbsToRecord_MaterializedValue_;
			}
		}

		public bool AllowTrainingOnMutipleModelVersions
		{
			get
			{
				return this._AllowTrainingOnMutipleModelVersions_MaterializedValue_;
			}
		}

		public int NumberOfModelVersionToTrain
		{
			get
			{
				return this._NumberOfModelVersionToTrain_MaterializedValue_;
			}
		}

		public IList<int> BlockedModelVersions
		{
			get
			{
				return this._BlockedModelVersions_MaterializedValue_;
			}
		}

		public IList<int> ClassificationModelVersions
		{
			get
			{
				return this._ClassificationModelVersions_MaterializedValue_;
			}
		}

		public IList<int> DeprecatedModelVersions
		{
			get
			{
				return this._DeprecatedModelVersions_MaterializedValue_;
			}
		}

		public double ProbabilityBehaviourSwitchPerWeek
		{
			get
			{
				return this._ProbabilityBehaviourSwitchPerWeek_MaterializedValue_;
			}
		}

		public double SymmetricNoise
		{
			get
			{
				return this._SymmetricNoise_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal int _MaxModelVersion_MaterializedValue_;

		internal int _MinModelVersion_MaterializedValue_;

		internal int _NumberOfVersionCrumbsToRecord_MaterializedValue_;

		internal bool _AllowTrainingOnMutipleModelVersions_MaterializedValue_;

		internal int _NumberOfModelVersionToTrain_MaterializedValue_;

		internal IList<int> _BlockedModelVersions_MaterializedValue_;

		internal IList<int> _ClassificationModelVersions_MaterializedValue_;

		internal IList<int> _DeprecatedModelVersions_MaterializedValue_;

		internal double _ProbabilityBehaviourSwitchPerWeek_MaterializedValue_;

		internal double _SymmetricNoise_MaterializedValue_;
	}
}
