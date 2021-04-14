using System;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public abstract class ExchangeSharedDataCommand
	{
		public abstract void PrimaryExecute(string key, CommunicationChannelCollection channels, NodeStructureSettingsStore store, SyncQueue syncQueue);

		public abstract void ExtensionExecute(IExchangeExtensionSnapIn snapIn);

		public static ExchangeSharedDataCommand Parse(byte[] bytes)
		{
			return (ExchangeSharedDataCommand)WinformsHelper.DeSerialize(bytes);
		}
	}
}
