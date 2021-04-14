using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HungDetectionGumIdChangedException : ClusterException
	{
		public HungDetectionGumIdChangedException(int localGumId, int remoteGumId, string lockOwnerName, long hungNodesMask) : base(ReplayStrings.HungDetectionGumIdChanged(localGumId, remoteGumId, lockOwnerName, hungNodesMask))
		{
			this.localGumId = localGumId;
			this.remoteGumId = remoteGumId;
			this.lockOwnerName = lockOwnerName;
			this.hungNodesMask = hungNodesMask;
		}

		public HungDetectionGumIdChangedException(int localGumId, int remoteGumId, string lockOwnerName, long hungNodesMask, Exception innerException) : base(ReplayStrings.HungDetectionGumIdChanged(localGumId, remoteGumId, lockOwnerName, hungNodesMask), innerException)
		{
			this.localGumId = localGumId;
			this.remoteGumId = remoteGumId;
			this.lockOwnerName = lockOwnerName;
			this.hungNodesMask = hungNodesMask;
		}

		protected HungDetectionGumIdChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.localGumId = (int)info.GetValue("localGumId", typeof(int));
			this.remoteGumId = (int)info.GetValue("remoteGumId", typeof(int));
			this.lockOwnerName = (string)info.GetValue("lockOwnerName", typeof(string));
			this.hungNodesMask = (long)info.GetValue("hungNodesMask", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("localGumId", this.localGumId);
			info.AddValue("remoteGumId", this.remoteGumId);
			info.AddValue("lockOwnerName", this.lockOwnerName);
			info.AddValue("hungNodesMask", this.hungNodesMask);
		}

		public int LocalGumId
		{
			get
			{
				return this.localGumId;
			}
		}

		public int RemoteGumId
		{
			get
			{
				return this.remoteGumId;
			}
		}

		public string LockOwnerName
		{
			get
			{
				return this.lockOwnerName;
			}
		}

		public long HungNodesMask
		{
			get
			{
				return this.hungNodesMask;
			}
		}

		private readonly int localGumId;

		private readonly int remoteGumId;

		private readonly string lockOwnerName;

		private readonly long hungNodesMask;
	}
}
