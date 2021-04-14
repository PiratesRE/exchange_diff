using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RFROperationException : LocalizedException
	{
		public RFROperationException(string operation, int returnValue, string serverId, string userName) : base(Strings.messageRFROperationException(operation, returnValue, serverId, userName))
		{
			this.operation = operation;
			this.returnValue = returnValue;
			this.serverId = serverId;
			this.userName = userName;
		}

		public RFROperationException(string operation, int returnValue, string serverId, string userName, Exception innerException) : base(Strings.messageRFROperationException(operation, returnValue, serverId, userName), innerException)
		{
			this.operation = operation;
			this.returnValue = returnValue;
			this.serverId = serverId;
			this.userName = userName;
		}

		protected RFROperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operation = (string)info.GetValue("operation", typeof(string));
			this.returnValue = (int)info.GetValue("returnValue", typeof(int));
			this.serverId = (string)info.GetValue("serverId", typeof(string));
			this.userName = (string)info.GetValue("userName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operation", this.operation);
			info.AddValue("returnValue", this.returnValue);
			info.AddValue("serverId", this.serverId);
			info.AddValue("userName", this.userName);
		}

		public string Operation
		{
			get
			{
				return this.operation;
			}
		}

		public int ReturnValue
		{
			get
			{
				return this.returnValue;
			}
		}

		public string ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		private readonly string operation;

		private readonly int returnValue;

		private readonly string serverId;

		private readonly string userName;
	}
}
