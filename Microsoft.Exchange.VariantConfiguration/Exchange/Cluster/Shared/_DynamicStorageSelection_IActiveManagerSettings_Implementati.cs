using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Cluster.Shared
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IActiveManagerSettings_Implementation_ : IActiveManagerSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IActiveManagerSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IActiveManagerSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IActiveManagerSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IActiveManagerSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IActiveManagerSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
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

		public DxStoreMode DxStoreRunMode
		{
			get
			{
				if (this.dataAccessor._DxStoreRunMode_ValueProvider_ != null)
				{
					return this.dataAccessor._DxStoreRunMode_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DxStoreRunMode_MaterializedValue_;
			}
		}

		public bool DxStoreIsUseHttpForInstanceCommunication
		{
			get
			{
				if (this.dataAccessor._DxStoreIsUseHttpForInstanceCommunication_ValueProvider_ != null)
				{
					return this.dataAccessor._DxStoreIsUseHttpForInstanceCommunication_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DxStoreIsUseHttpForInstanceCommunication_MaterializedValue_;
			}
		}

		public bool DxStoreIsUseHttpForClientCommunication
		{
			get
			{
				if (this.dataAccessor._DxStoreIsUseHttpForClientCommunication_ValueProvider_ != null)
				{
					return this.dataAccessor._DxStoreIsUseHttpForClientCommunication_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DxStoreIsUseHttpForClientCommunication_MaterializedValue_;
			}
		}

		public bool DxStoreIsEncryptionEnabled
		{
			get
			{
				if (this.dataAccessor._DxStoreIsEncryptionEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._DxStoreIsEncryptionEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DxStoreIsEncryptionEnabled_MaterializedValue_;
			}
		}

		public bool DxStoreIsPeriodicFixupEnabled
		{
			get
			{
				if (this.dataAccessor._DxStoreIsPeriodicFixupEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._DxStoreIsPeriodicFixupEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DxStoreIsPeriodicFixupEnabled_MaterializedValue_;
			}
		}

		public bool DxStoreIsUseBinarySerializerForClientCommunication
		{
			get
			{
				if (this.dataAccessor._DxStoreIsUseBinarySerializerForClientCommunication_ValueProvider_ != null)
				{
					return this.dataAccessor._DxStoreIsUseBinarySerializerForClientCommunication_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._DxStoreIsUseBinarySerializerForClientCommunication_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IActiveManagerSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
