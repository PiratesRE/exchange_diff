using System;

namespace Microsoft.Exchange.Data.Transport
{
	internal abstract class SnapshotWriter
	{
		internal static string SnapshotBaseFolder
		{
			get
			{
				return SnapshotWriter.snapshotBaseFolder;
			}
			set
			{
				SnapshotWriter.snapshotBaseFolder = value;
			}
		}

		public abstract void WriteOriginalData(int agentId, string id, string topic, string address, MailItem mailItem);

		public abstract void WritePreProcessedData(int agentId, string prefix, string id, string topic, MailItem mailItem);

		public abstract void WriteProcessedData(string prefix, string id, string topic, string agent, MailItem mailItem);

		private static string snapshotBaseFolder;
	}
}
