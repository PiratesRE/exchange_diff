using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Cluster.Shared
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IActiveManagerSettings_DataAccessor_ : VariantObjectDataAccessorBase<IActiveManagerSettings, _DynamicStorageSelection_IActiveManagerSettings_Implementation_, _DynamicStorageSelection_IActiveManagerSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal DxStoreMode _DxStoreRunMode_MaterializedValue_;

		internal ValueProvider<DxStoreMode> _DxStoreRunMode_ValueProvider_;

		internal bool _DxStoreIsUseHttpForInstanceCommunication_MaterializedValue_;

		internal ValueProvider<bool> _DxStoreIsUseHttpForInstanceCommunication_ValueProvider_;

		internal bool _DxStoreIsUseHttpForClientCommunication_MaterializedValue_;

		internal ValueProvider<bool> _DxStoreIsUseHttpForClientCommunication_ValueProvider_;

		internal bool _DxStoreIsEncryptionEnabled_MaterializedValue_;

		internal ValueProvider<bool> _DxStoreIsEncryptionEnabled_ValueProvider_;

		internal bool _DxStoreIsPeriodicFixupEnabled_MaterializedValue_;

		internal ValueProvider<bool> _DxStoreIsPeriodicFixupEnabled_ValueProvider_;

		internal bool _DxStoreIsUseBinarySerializerForClientCommunication_MaterializedValue_;

		internal ValueProvider<bool> _DxStoreIsUseBinarySerializerForClientCommunication_ValueProvider_;
	}
}
