using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.MessageDepot
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IMessageDepotSettings_Implementation_ : IMessageDepotSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IMessageDepotSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IMessageDepotSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IMessageDepotSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IMessageDepotSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IMessageDepotSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public bool Enabled
		{
			get
			{
				if (this.dataAccessor._Enabled_ValueProvider_ != null)
				{
					return this.dataAccessor._Enabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._Enabled_MaterializedValue_;
			}
		}

		public IList<DayOfWeek> EnabledOnDaysOfWeek
		{
			get
			{
				if (this.dataAccessor._EnabledOnDaysOfWeek_ValueProvider_ != null)
				{
					return this.dataAccessor._EnabledOnDaysOfWeek_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._EnabledOnDaysOfWeek_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IMessageDepotSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
