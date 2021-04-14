using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IClutterDataSelectionSettings_Implementation_ : IClutterDataSelectionSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public int MaxFolderCount
		{
			get
			{
				if (this.dataAccessor._MaxFolderCount_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxFolderCount_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxFolderCount_MaterializedValue_;
			}
		}

		public int BatchSizeForTrainedModel
		{
			get
			{
				if (this.dataAccessor._BatchSizeForTrainedModel_ValueProvider_ != null)
				{
					return this.dataAccessor._BatchSizeForTrainedModel_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._BatchSizeForTrainedModel_MaterializedValue_;
			}
		}

		public int BatchSizeForDefaultModel
		{
			get
			{
				if (this.dataAccessor._BatchSizeForDefaultModel_ValueProvider_ != null)
				{
					return this.dataAccessor._BatchSizeForDefaultModel_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._BatchSizeForDefaultModel_MaterializedValue_;
			}
		}

		public int MaxInboxFolderProportion
		{
			get
			{
				if (this.dataAccessor._MaxInboxFolderProportion_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxInboxFolderProportion_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxInboxFolderProportion_MaterializedValue_;
			}
		}

		public int MaxDeletedFolderProportion
		{
			get
			{
				if (this.dataAccessor._MaxDeletedFolderProportion_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxDeletedFolderProportion_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxDeletedFolderProportion_MaterializedValue_;
			}
		}

		public int MaxOtherFolderProportion
		{
			get
			{
				if (this.dataAccessor._MaxOtherFolderProportion_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxOtherFolderProportion_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxOtherFolderProportion_MaterializedValue_;
			}
		}

		public int MinRespondActionShare
		{
			get
			{
				if (this.dataAccessor._MinRespondActionShare_ValueProvider_ != null)
				{
					return this.dataAccessor._MinRespondActionShare_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinRespondActionShare_MaterializedValue_;
			}
		}

		public int MinIgnoreActionShare
		{
			get
			{
				if (this.dataAccessor._MinIgnoreActionShare_ValueProvider_ != null)
				{
					return this.dataAccessor._MinIgnoreActionShare_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinIgnoreActionShare_MaterializedValue_;
			}
		}

		public int MaxIgnoreActionShare
		{
			get
			{
				if (this.dataAccessor._MaxIgnoreActionShare_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxIgnoreActionShare_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxIgnoreActionShare_MaterializedValue_;
			}
		}

		public int NumberOfMonthsToIncludeInRetrospectiveTraining
		{
			get
			{
				if (this.dataAccessor._NumberOfMonthsToIncludeInRetrospectiveTraining_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfMonthsToIncludeInRetrospectiveTraining_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfMonthsToIncludeInRetrospectiveTraining_MaterializedValue_;
			}
		}

		public int NumberOfDaysToSkipFromCurrentForTraining
		{
			get
			{
				if (this.dataAccessor._NumberOfDaysToSkipFromCurrentForTraining_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfDaysToSkipFromCurrentForTraining_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfDaysToSkipFromCurrentForTraining_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
