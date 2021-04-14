using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Flighting
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IFlight_DataAccessor_ : VariantObjectDataAccessorBase<IFlight, _DynamicStorageSelection_IFlight_Implementation_, _DynamicStorageSelection_IFlight_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal string _Rotate_MaterializedValue_;

		internal ValueProvider<string> _Rotate_ValueProvider_;

		internal string _Ramp_MaterializedValue_;

		internal ValueProvider<string> _Ramp_ValueProvider_;

		internal string _RampSeed_MaterializedValue_;

		internal ValueProvider<string> _RampSeed_ValueProvider_;
	}
}
