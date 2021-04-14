using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Search
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_ILanguageDetection_DataAccessor_ : VariantObjectDataAccessorBase<ILanguageDetection, _DynamicStorageSelection_ILanguageDetection_Implementation_, _DynamicStorageSelection_ILanguageDetection_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _EnableLanguageDetectionLogging_MaterializedValue_;

		internal ValueProvider<bool> _EnableLanguageDetectionLogging_ValueProvider_;

		internal int _SamplingFrequency_MaterializedValue_;

		internal ValueProvider<int> _SamplingFrequency_ValueProvider_;

		internal bool _EnableLanguageSelection_MaterializedValue_;

		internal ValueProvider<bool> _EnableLanguageSelection_ValueProvider_;

		internal string _RegionDefaultLanguage_MaterializedValue_;

		internal ValueProvider<string> _RegionDefaultLanguage_ValueProvider_;
	}
}
