using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthTaskNotRunOnExchangeServerException : LocalizedException
	{
		public CasHealthTaskNotRunOnExchangeServerException() : base(Strings.CasHealthTaskNotRunOnExchangeServer)
		{
		}

		public CasHealthTaskNotRunOnExchangeServerException(Exception innerException) : base(Strings.CasHealthTaskNotRunOnExchangeServer, innerException)
		{
		}

		protected CasHealthTaskNotRunOnExchangeServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
