using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Cluster;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmServerException : HaRpcServerBaseException
	{
		public AmServerException(string errorMessage) : base(ServerStrings.AmServerException(errorMessage))
		{
			this.m_exceptionInfo.ErrorMessage = errorMessage;
		}

		public AmServerException(string errorMessage, Exception innerException) : base(ServerStrings.AmServerException(errorMessage), innerException)
		{
			this.m_exceptionInfo.ErrorMessage = errorMessage;
		}

		protected AmServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string ErrorMessage
		{
			get
			{
				return this.m_exceptionInfo.ErrorMessage;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
