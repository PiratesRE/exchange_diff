using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedingAnotherServerException : TaskServerException
	{
		public SeedingAnotherServerException(string seedingServerName, string requestServerName) : base(ReplayStrings.SeedingAnotherServerException(seedingServerName, requestServerName))
		{
			this.seedingServerName = seedingServerName;
			this.requestServerName = requestServerName;
		}

		public SeedingAnotherServerException(string seedingServerName, string requestServerName, Exception innerException) : base(ReplayStrings.SeedingAnotherServerException(seedingServerName, requestServerName), innerException)
		{
			this.seedingServerName = seedingServerName;
			this.requestServerName = requestServerName;
		}

		protected SeedingAnotherServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.seedingServerName = (string)info.GetValue("seedingServerName", typeof(string));
			this.requestServerName = (string)info.GetValue("requestServerName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("seedingServerName", this.seedingServerName);
			info.AddValue("requestServerName", this.requestServerName);
		}

		public string SeedingServerName
		{
			get
			{
				return this.seedingServerName;
			}
		}

		public string RequestServerName
		{
			get
			{
				return this.requestServerName;
			}
		}

		private readonly string seedingServerName;

		private readonly string requestServerName;
	}
}
