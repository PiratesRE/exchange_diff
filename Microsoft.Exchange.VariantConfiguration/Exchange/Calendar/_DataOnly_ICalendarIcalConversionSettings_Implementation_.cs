using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Calendar
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_ICalendarIcalConversionSettings_Implementation_ : ICalendarIcalConversionSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public bool LocalTimeZoneReferenceForRecurrenceNeeded
		{
			get
			{
				return this._LocalTimeZoneReferenceForRecurrenceNeeded_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _LocalTimeZoneReferenceForRecurrenceNeeded_MaterializedValue_;
	}
}
