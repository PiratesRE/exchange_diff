using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.MessageDepot
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IMessageDepotSettings_DataAccessor_ : VariantObjectDataAccessorBase<IMessageDepotSettings, _DynamicStorageSelection_IMessageDepotSettings_Implementation_, _DynamicStorageSelection_IMessageDepotSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal IList<DayOfWeek> _EnabledOnDaysOfWeek_MaterializedValue_;

		internal ValueProvider<IList<DayOfWeek>> _EnabledOnDaysOfWeek_ValueProvider_;
	}
}
