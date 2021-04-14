using System;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Common
{
	[Serializable]
	public class Status
	{
		public Status(string name, SyncTreeType type)
		{
			this.Name = name;
			this.Type = type;
		}

		public Status()
		{
		}

		public static Status Deserialize(byte[] bytes)
		{
			Status result;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				result = (Status)Status.serializer.Deserialize(memoryStream);
			}
			return result;
		}

		public void Starting()
		{
			this.Result = StatusResult.InProgress;
			this.Scanned = 0;
			this.Added = 0;
			this.Deleted = 0;
			this.Updated = 0;
			this.TargetScanned = 0;
			this.FailureDetails = string.Empty;
			this.StartUTC = DateTime.UtcNow;
		}

		public void ObjectAdded()
		{
			this.Added++;
		}

		public void ObjectDeleted()
		{
			this.Deleted++;
		}

		public void ObjectUpdated()
		{
			this.Updated++;
		}

		public void ObjectScanned()
		{
			this.Scanned++;
		}

		public void TargetObjectScanned()
		{
			this.TargetScanned++;
		}

		public void Finished(StatusResult result)
		{
			if (this.Result == StatusResult.InProgress)
			{
				this.Result = result;
				this.EndUTC = DateTime.UtcNow;
				this.Dump();
			}
		}

		public byte[] Serialize()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Status.serializer.Serialize(memoryStream, this);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public void Dump()
		{
			ExTraceGlobals.SynchronizationJobTracer.TraceDebug((long)this.GetHashCode(), "Sync Status name: {0} type: {1} result: {2} start: {3} end: {4} scanned: {5} added: {6} deleted: {7} updated: {8} targetScanned: {9} failureDetails: {10}", new object[]
			{
				this.Name,
				this.Type,
				this.Result,
				this.StartUTC,
				this.EndUTC,
				this.Scanned,
				this.Added,
				this.Deleted,
				this.Updated,
				this.TargetScanned,
				this.FailureDetails
			});
		}

		private static StatusSerializer serializer = new StatusSerializer();

		public StatusResult Result;

		public SyncTreeType Type;

		public string Name = string.Empty;

		public string FailureDetails = string.Empty;

		public DateTime StartUTC;

		public DateTime EndUTC;

		public int Added;

		public int Deleted;

		public int Updated;

		public int Scanned;

		public int TargetScanned;
	}
}
