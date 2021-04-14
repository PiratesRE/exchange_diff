using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.HolidayCalendars
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IHostSettings_DataAccessor_ : VariantObjectDataAccessorBase<IHostSettings, _DynamicStorageSelection_IHostSettings_Implementation_, _DynamicStorageSelection_IHostSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal string _Endpoint_MaterializedValue_;

		internal ValueProvider<string> _Endpoint_ValueProvider_;

		internal int _Timeout_MaterializedValue_;

		internal ValueProvider<int> _Timeout_ValueProvider_;
	}
}
