using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllCouldNotControlReplicaInstanceException : TransientException
	{
		public AcllCouldNotControlReplicaInstanceException(string dbCopy) : base(ReplayStrings.AcllCouldNotControlReplicaInstanceException(dbCopy))
		{
			this.dbCopy = dbCopy;
		}

		public AcllCouldNotControlReplicaInstanceException(string dbCopy, Exception innerException) : base(ReplayStrings.AcllCouldNotControlReplicaInstanceException(dbCopy), innerException)
		{
			this.dbCopy = dbCopy;
		}

		protected AcllCouldNotControlReplicaInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		private readonly string dbCopy;
	}
}
