using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooManyOustandingMwiRequestsException : MwiDeliveryException
	{
		public TooManyOustandingMwiRequestsException(string userName) : base(Strings.descTooManyOutstandingMwiRequestsError(userName))
		{
			this.userName = userName;
		}

		public TooManyOustandingMwiRequestsException(string userName, Exception innerException) : base(Strings.descTooManyOutstandingMwiRequestsError(userName), innerException)
		{
			this.userName = userName;
		}

		protected TooManyOustandingMwiRequestsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		private readonly string userName;
	}
}
