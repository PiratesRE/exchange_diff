using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ILanguageDetection_Implementation_ : ILanguageDetection, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_ILanguageDetection_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_ILanguageDetection_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_ILanguageDetection_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_ILanguageDetection_DataAccessor_>.Initialize(_DynamicStorageSelection_ILanguageDetection_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public bool EnableLanguageDetectionLogging
		{
			get
			{
				if (this.dataAccessor._EnableLanguageDetectionLogging_ValueProvider_ != null)
				{
					return this.dataAccessor._EnableLanguageDetectionLogging_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EnableLanguageDetectionLogging_MaterializedValue_;
			}
		}

		public int SamplingFrequency
		{
			get
			{
				if (this.dataAccessor._SamplingFrequency_ValueProvider_ != null)
				{
					return this.dataAccessor._SamplingFrequency_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._SamplingFrequency_MaterializedValue_;
			}
		}

		public bool EnableLanguageSelection
		{
			get
			{
				if (this.dataAccessor._EnableLanguageSelection_ValueProvider_ != null)
				{
					return this.dataAccessor._EnableLanguageSelection_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EnableLanguageSelection_MaterializedValue_;
			}
		}

		public string RegionDefaultLanguage
		{
			get
			{
				if (this.dataAccessor._RegionDefaultLanguage_ValueProvider_ != null)
				{
					return this.dataAccessor._RegionDefaultLanguage_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._RegionDefaultLanguage_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_ILanguageDetection_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
