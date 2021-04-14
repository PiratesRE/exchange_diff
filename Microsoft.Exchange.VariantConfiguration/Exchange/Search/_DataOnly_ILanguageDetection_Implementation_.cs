using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_ILanguageDetection_Implementation_ : ILanguageDetection, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public bool EnableLanguageDetectionLogging
		{
			get
			{
				return this._EnableLanguageDetectionLogging_MaterializedValue_;
			}
		}

		public int SamplingFrequency
		{
			get
			{
				return this._SamplingFrequency_MaterializedValue_;
			}
		}

		public bool EnableLanguageSelection
		{
			get
			{
				return this._EnableLanguageSelection_MaterializedValue_;
			}
		}

		public string RegionDefaultLanguage
		{
			get
			{
				return this._RegionDefaultLanguage_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _EnableLanguageDetectionLogging_MaterializedValue_;

		internal int _SamplingFrequency_MaterializedValue_;

		internal bool _EnableLanguageSelection_MaterializedValue_;

		internal string _RegionDefaultLanguage_MaterializedValue_;
	}
}
