using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotBeginSeedingInstanceNotInStateException : TaskServerException
	{
		public CannotBeginSeedingInstanceNotInStateException(string dbName, string state) : base(ReplayStrings.CannotBeginSeedingInstanceNotInStateException(dbName, state))
		{
			this.dbName = dbName;
			this.state = state;
		}

		public CannotBeginSeedingInstanceNotInStateException(string dbName, string state, Exception innerException) : base(ReplayStrings.CannotBeginSeedingInstanceNotInStateException(dbName, state), innerException)
		{
			this.dbName = dbName;
			this.state = state;
		}

		protected CannotBeginSeedingInstanceNotInStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.state = (string)info.GetValue("state", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("state", this.state);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string State
		{
			get
			{
				return this.state;
			}
		}

		private readonly string dbName;

		private readonly string state;
	}
}
