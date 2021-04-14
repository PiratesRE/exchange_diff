using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IClutterDataSelectionSettings_Implementation_ : IClutterDataSelectionSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public int MaxFolderCount
		{
			get
			{
				return this._MaxFolderCount_MaterializedValue_;
			}
		}

		public int BatchSizeForTrainedModel
		{
			get
			{
				return this._BatchSizeForTrainedModel_MaterializedValue_;
			}
		}

		public int BatchSizeForDefaultModel
		{
			get
			{
				return this._BatchSizeForDefaultModel_MaterializedValue_;
			}
		}

		public int MaxInboxFolderProportion
		{
			get
			{
				return this._MaxInboxFolderProportion_MaterializedValue_;
			}
		}

		public int MaxDeletedFolderProportion
		{
			get
			{
				return this._MaxDeletedFolderProportion_MaterializedValue_;
			}
		}

		public int MaxOtherFolderProportion
		{
			get
			{
				return this._MaxOtherFolderProportion_MaterializedValue_;
			}
		}

		public int MinRespondActionShare
		{
			get
			{
				return this._MinRespondActionShare_MaterializedValue_;
			}
		}

		public int MinIgnoreActionShare
		{
			get
			{
				return this._MinIgnoreActionShare_MaterializedValue_;
			}
		}

		public int MaxIgnoreActionShare
		{
			get
			{
				return this._MaxIgnoreActionShare_MaterializedValue_;
			}
		}

		public int NumberOfMonthsToIncludeInRetrospectiveTraining
		{
			get
			{
				return this._NumberOfMonthsToIncludeInRetrospectiveTraining_MaterializedValue_;
			}
		}

		public int NumberOfDaysToSkipFromCurrentForTraining
		{
			get
			{
				return this._NumberOfDaysToSkipFromCurrentForTraining_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal int _MaxFolderCount_MaterializedValue_;

		internal int _BatchSizeForTrainedModel_MaterializedValue_;

		internal int _BatchSizeForDefaultModel_MaterializedValue_;

		internal int _MaxInboxFolderProportion_MaterializedValue_;

		internal int _MaxDeletedFolderProportion_MaterializedValue_;

		internal int _MaxOtherFolderProportion_MaterializedValue_;

		internal int _MinRespondActionShare_MaterializedValue_;

		internal int _MinIgnoreActionShare_MaterializedValue_;

		internal int _MaxIgnoreActionShare_MaterializedValue_;

		internal int _NumberOfMonthsToIncludeInRetrospectiveTraining_MaterializedValue_;

		internal int _NumberOfDaysToSkipFromCurrentForTraining_MaterializedValue_;
	}
}
