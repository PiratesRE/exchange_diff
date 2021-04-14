using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayTestStoreConnectivityTimedoutException : LocalizedException
	{
		public ReplayTestStoreConnectivityTimedoutException(string operationName, string errorMsg) : base(ReplayStrings.ReplayTestStoreConnectivityTimedoutException(operationName, errorMsg))
		{
			this.operationName = operationName;
			this.errorMsg = errorMsg;
		}

		public ReplayTestStoreConnectivityTimedoutException(string operationName, string errorMsg, Exception innerException) : base(ReplayStrings.ReplayTestStoreConnectivityTimedoutException(operationName, errorMsg), innerException)
		{
			this.operationName = operationName;
			this.errorMsg = errorMsg;
		}

		protected ReplayTestStoreConnectivityTimedoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationName = (string)info.GetValue("operationName", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationName", this.operationName);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string operationName;

		private readonly string errorMsg;
	}
}
