using System;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public class AcKnowledgeCommand : NotificationSharedDataCommand
	{
		public Guid AcknowledgedId { get; set; }

		public override void PrimaryExecute(string key, CommunicationChannelCollection channels, NodeStructureSettingsStore store, SyncQueue syncQueue)
		{
			syncQueue.AcKnowledge(this.AcknowledgedId);
		}
	}
}
