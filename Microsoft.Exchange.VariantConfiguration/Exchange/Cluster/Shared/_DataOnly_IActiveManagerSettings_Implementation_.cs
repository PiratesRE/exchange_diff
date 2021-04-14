using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Cluster.Shared
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DataOnly_IActiveManagerSettings_Implementation_ : IActiveManagerSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
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

		public DxStoreMode DxStoreRunMode
		{
			get
			{
				return this._DxStoreRunMode_MaterializedValue_;
			}
		}

		public bool DxStoreIsUseHttpForInstanceCommunication
		{
			get
			{
				return this._DxStoreIsUseHttpForInstanceCommunication_MaterializedValue_;
			}
		}

		public bool DxStoreIsUseHttpForClientCommunication
		{
			get
			{
				return this._DxStoreIsUseHttpForClientCommunication_MaterializedValue_;
			}
		}

		public bool DxStoreIsEncryptionEnabled
		{
			get
			{
				return this._DxStoreIsEncryptionEnabled_MaterializedValue_;
			}
		}

		public bool DxStoreIsPeriodicFixupEnabled
		{
			get
			{
				return this._DxStoreIsPeriodicFixupEnabled_MaterializedValue_;
			}
		}

		public bool DxStoreIsUseBinarySerializerForClientCommunication
		{
			get
			{
				return this._DxStoreIsUseBinarySerializerForClientCommunication_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal DxStoreMode _DxStoreRunMode_MaterializedValue_;

		internal bool _DxStoreIsUseHttpForInstanceCommunication_MaterializedValue_;

		internal bool _DxStoreIsUseHttpForClientCommunication_MaterializedValue_;

		internal bool _DxStoreIsEncryptionEnabled_MaterializedValue_;

		internal bool _DxStoreIsPeriodicFixupEnabled_MaterializedValue_;

		internal bool _DxStoreIsUseBinarySerializerForClientCommunication_MaterializedValue_;
	}
}
