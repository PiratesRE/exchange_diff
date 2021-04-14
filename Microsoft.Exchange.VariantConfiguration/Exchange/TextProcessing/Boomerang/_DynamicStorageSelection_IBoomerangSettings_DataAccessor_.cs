using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IBoomerangSettings_DataAccessor_ : VariantObjectDataAccessorBase<IBoomerangSettings, _DynamicStorageSelection_IBoomerangSettings_Implementation_, _DynamicStorageSelection_IBoomerangSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal string _MasterKeyRegistryPath_MaterializedValue_;

		internal ValueProvider<string> _MasterKeyRegistryPath_ValueProvider_;

		internal string _MasterKeyRegistryKeyName_MaterializedValue_;

		internal ValueProvider<string> _MasterKeyRegistryKeyName_ValueProvider_;

		internal uint _NumberOfValidIntervalsInDays_MaterializedValue_;

		internal ValueProvider<uint> _NumberOfValidIntervalsInDays_ValueProvider_;
	}
}
