using System;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole.Advanced;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public abstract class SyncQueueSharedDataCommand : ExchangeSharedDataCommand
	{
		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		public sealed override void PrimaryExecute(string key, CommunicationChannelCollection channels, NodeStructureSettingsStore store, SyncQueue syncQueue)
		{
			channels[key].Channel.SetData(WinformsHelper.Serialize(this));
		}

		public sealed override void ExtensionExecute(IExchangeExtensionSnapIn snapIn)
		{
			this.Execute(snapIn);
			try
			{
				snapIn.SharedDataItem.RequestDataUpdate(WinformsHelper.Serialize(new AcKnowledgeCommand
				{
					AcknowledgedId = this.Id
				}));
			}
			catch (PrimarySnapInDataException)
			{
			}
		}

		public abstract void Execute(IExchangeExtensionSnapIn snapIn);

		private Guid id = default(Guid);
	}
}
