using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Cluster.Shared
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IActiveManagerSettings : ISettings
	{
		DxStoreMode DxStoreRunMode { get; }

		bool DxStoreIsUseHttpForInstanceCommunication { get; }

		bool DxStoreIsUseHttpForClientCommunication { get; }

		bool DxStoreIsEncryptionEnabled { get; }

		bool DxStoreIsPeriodicFixupEnabled { get; }

		bool DxStoreIsUseBinarySerializerForClientCommunication { get; }
	}
}
