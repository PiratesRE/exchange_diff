using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedingSourceReplicaInstanceNotFoundException : LocalizedException
	{
		public SeedingSourceReplicaInstanceNotFoundException(Guid guid, string sourceServer) : base(ReplayStrings.SeedingSourceReplicaInstanceNotFoundException(guid, sourceServer))
		{
			this.guid = guid;
			this.sourceServer = sourceServer;
		}

		public SeedingSourceReplicaInstanceNotFoundException(Guid guid, string sourceServer, Exception innerException) : base(ReplayStrings.SeedingSourceReplicaInstanceNotFoundException(guid, sourceServer), innerException)
		{
			this.guid = guid;
			this.sourceServer = sourceServer;
		}

		protected SeedingSourceReplicaInstanceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (Guid)info.GetValue("guid", typeof(Guid));
			this.sourceServer = (string)info.GetValue("sourceServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
			info.AddValue("sourceServer", this.sourceServer);
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string SourceServer
		{
			get
			{
				return this.sourceServer;
			}
		}

		private readonly Guid guid;

		private readonly string sourceServer;
	}
}
