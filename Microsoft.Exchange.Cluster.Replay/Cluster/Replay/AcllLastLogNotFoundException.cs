using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllLastLogNotFoundException : TransientException
	{
		public AcllLastLogNotFoundException(string dbCopy, long generation) : base(ReplayStrings.AcllLastLogNotFoundException(dbCopy, generation))
		{
			this.dbCopy = dbCopy;
			this.generation = generation;
		}

		public AcllLastLogNotFoundException(string dbCopy, long generation, Exception innerException) : base(ReplayStrings.AcllLastLogNotFoundException(dbCopy, generation), innerException)
		{
			this.dbCopy = dbCopy;
			this.generation = generation;
		}

		protected AcllLastLogNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.generation = (long)info.GetValue("generation", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("generation", this.generation);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public long Generation
		{
			get
			{
				return this.generation;
			}
		}

		private readonly string dbCopy;

		private readonly long generation;
	}
}
