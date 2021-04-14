using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Calendar
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ICalendarUpgradeSettings_DataAccessor_ : VariantObjectDataAccessorBase<ICalendarUpgradeSettings, _DynamicStorageSelection_ICalendarUpgradeSettings_Implementation_, _DynamicStorageSelection_ICalendarUpgradeSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal int _MinCalendarItemsForUpgrade_MaterializedValue_;

		internal ValueProvider<int> _MinCalendarItemsForUpgrade_ValueProvider_;
	}
}
