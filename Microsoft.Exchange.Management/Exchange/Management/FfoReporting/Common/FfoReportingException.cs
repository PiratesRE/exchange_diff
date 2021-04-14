using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FfoReportingException : LocalizedException
	{
		public FfoReportingException() : base(Strings.FfoReportingMessage)
		{
		}

		public FfoReportingException(Exception innerException) : base(Strings.FfoReportingMessage, innerException)
		{
		}

		protected FfoReportingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
