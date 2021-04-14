using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IClutterModelConfigurationSettings_Implementation_ : IClutterModelConfigurationSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
		{
			this.dataAccessor = dataAccessor;
			this.context = context;
		}

		public string Name
		{
			get
			{
				return this.dataAccessor._Name_MaterializedValue_;
			}
		}

		public int MaxModelVersion
		{
			get
			{
				if (this.dataAccessor._MaxModelVersion_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxModelVersion_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxModelVersion_MaterializedValue_;
			}
		}

		public int MinModelVersion
		{
			get
			{
				if (this.dataAccessor._MinModelVersion_ValueProvider_ != null)
				{
					return this.dataAccessor._MinModelVersion_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinModelVersion_MaterializedValue_;
			}
		}

		public int NumberOfVersionCrumbsToRecord
		{
			get
			{
				if (this.dataAccessor._NumberOfVersionCrumbsToRecord_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfVersionCrumbsToRecord_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfVersionCrumbsToRecord_MaterializedValue_;
			}
		}

		public bool AllowTrainingOnMutipleModelVersions
		{
			get
			{
				if (this.dataAccessor._AllowTrainingOnMutipleModelVersions_ValueProvider_ != null)
				{
					return this.dataAccessor._AllowTrainingOnMutipleModelVersions_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._AllowTrainingOnMutipleModelVersions_MaterializedValue_;
			}
		}

		public int NumberOfModelVersionToTrain
		{
			get
			{
				if (this.dataAccessor._NumberOfModelVersionToTrain_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfModelVersionToTrain_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfModelVersionToTrain_MaterializedValue_;
			}
		}

		public IList<int> BlockedModelVersions
		{
			get
			{
				if (this.dataAccessor._BlockedModelVersions_ValueProvider_ != null)
				{
					return this.dataAccessor._BlockedModelVersions_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._BlockedModelVersions_MaterializedValue_;
			}
		}

		public IList<int> ClassificationModelVersions
		{
			get
			{
				if (this.dataAccessor._ClassificationModelVersions_ValueProvider_ != null)
				{
					return this.dataAccessor._ClassificationModelVersions_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ClassificationModelVersions_MaterializedValue_;
			}
		}

		public IList<int> DeprecatedModelVersions
		{
			get
			{
				if (this.dataAccessor._DeprecatedModelVersions_ValueProvider_ != null)
				{
					return this.dataAccessor._DeprecatedModelVersions_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DeprecatedModelVersions_MaterializedValue_;
			}
		}

		public double ProbabilityBehaviourSwitchPerWeek
		{
			get
			{
				if (this.dataAccessor._ProbabilityBehaviourSwitchPerWeek_ValueProvider_ != null)
				{
					return this.dataAccessor._ProbabilityBehaviourSwitchPerWeek_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ProbabilityBehaviourSwitchPerWeek_MaterializedValue_;
			}
		}

		public double SymmetricNoise
		{
			get
			{
				if (this.dataAccessor._SymmetricNoise_ValueProvider_ != null)
				{
					return this.dataAccessor._SymmetricNoise_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SymmetricNoise_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
