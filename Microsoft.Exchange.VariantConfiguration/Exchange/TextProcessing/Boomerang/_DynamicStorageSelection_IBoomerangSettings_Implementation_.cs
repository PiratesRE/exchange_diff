using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IBoomerangSettings_Implementation_ : IBoomerangSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IBoomerangSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IBoomerangSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IBoomerangSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IBoomerangSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IBoomerangSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public string MasterKeyRegistryPath
		{
			get
			{
				if (this.dataAccessor._MasterKeyRegistryPath_ValueProvider_ != null)
				{
					return this.dataAccessor._MasterKeyRegistryPath_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MasterKeyRegistryPath_MaterializedValue_;
			}
		}

		public string MasterKeyRegistryKeyName
		{
			get
			{
				if (this.dataAccessor._MasterKeyRegistryKeyName_ValueProvider_ != null)
				{
					return this.dataAccessor._MasterKeyRegistryKeyName_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MasterKeyRegistryKeyName_MaterializedValue_;
			}
		}

		public uint NumberOfValidIntervalsInDays
		{
			get
			{
				if (this.dataAccessor._NumberOfValidIntervalsInDays_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfValidIntervalsInDays_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfValidIntervalsInDays_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IBoomerangSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
