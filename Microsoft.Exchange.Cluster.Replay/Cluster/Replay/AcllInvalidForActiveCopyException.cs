using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllInvalidForActiveCopyException : TransientException
	{
		public AcllInvalidForActiveCopyException(string dbCopy) : base(ReplayStrings.AcllInvalidForActiveCopyException(dbCopy))
		{
			this.dbCopy = dbCopy;
		}

		public AcllInvalidForActiveCopyException(string dbCopy, Exception innerException) : base(ReplayStrings.AcllInvalidForActiveCopyException(dbCopy), innerException)
		{
			this.dbCopy = dbCopy;
		}

		protected AcllInvalidForActiveCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
