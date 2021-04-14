using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionWrapperException : AmDbActionException
	{
		public AmDbActionWrapperException(string dbActionError) : base(ReplayStrings.AmDbActionWrapperException(dbActionError))
		{
			this.dbActionError = dbActionError;
		}

		public AmDbActionWrapperException(string dbActionError, Exception innerException) : base(ReplayStrings.AmDbActionWrapperException(dbActionError), innerException)
		{
			this.dbActionError = dbActionError;
		}

		protected AmDbActionWrapperException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbActionError = (string)info.GetValue("dbActionError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbActionError", this.dbActionError);
		}

		public string DbActionError
		{
			get
			{
				return this.dbActionError;
			}
		}

		private readonly string dbActionError;
	}
}
