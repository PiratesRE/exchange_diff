using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMountNotAllowedDueToAcllErrorException : AmServerException
	{
		public AmDbMountNotAllowedDueToAcllErrorException(string errMessage, long numLogsLost) : base(ServerStrings.AmDbMountNotAllowedDueToAcllErrorException(errMessage, numLogsLost))
		{
			this.errMessage = errMessage;
			this.numLogsLost = numLogsLost;
		}

		public AmDbMountNotAllowedDueToAcllErrorException(string errMessage, long numLogsLost, Exception innerException) : base(ServerStrings.AmDbMountNotAllowedDueToAcllErrorException(errMessage, numLogsLost), innerException)
		{
			this.errMessage = errMessage;
			this.numLogsLost = numLogsLost;
		}

		protected AmDbMountNotAllowedDueToAcllErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMessage = (string)info.GetValue("errMessage", typeof(string));
			this.numLogsLost = (long)info.GetValue("numLogsLost", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMessage", this.errMessage);
			info.AddValue("numLogsLost", this.numLogsLost);
		}

		public string ErrMessage
		{
			get
			{
				return this.errMessage;
			}
		}

		public long NumLogsLost
		{
			get
			{
				return this.numLogsLost;
			}
		}

		private readonly string errMessage;

		private readonly long numLogsLost;
	}
}
