using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceDatabaseNotFoundException : TransientException
	{
		public SourceDatabaseNotFoundException(Guid g, string sourceServer) : base(ReplayStrings.SourceDatabaseNotFound(g, sourceServer))
		{
			this.g = g;
			this.sourceServer = sourceServer;
		}

		public SourceDatabaseNotFoundException(Guid g, string sourceServer, Exception innerException) : base(ReplayStrings.SourceDatabaseNotFound(g, sourceServer), innerException)
		{
			this.g = g;
			this.sourceServer = sourceServer;
		}

		protected SourceDatabaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.g = (Guid)info.GetValue("g", typeof(Guid));
			this.sourceServer = (string)info.GetValue("sourceServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("g", this.g);
			info.AddValue("sourceServer", this.sourceServer);
		}

		public Guid G
		{
			get
			{
				return this.g;
			}
		}

		public string SourceServer
		{
			get
			{
				return this.sourceServer;
			}
		}

		private readonly Guid g;

		private readonly string sourceServer;
	}
}
