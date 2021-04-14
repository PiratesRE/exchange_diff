using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataMartTimeoutException : ReportingException
	{
		public DataMartTimeoutException() : base(Strings.DataMartTimeoutException)
		{
		}

		public DataMartTimeoutException(Exception innerException) : base(Strings.DataMartTimeoutException, innerException)
		{
		}

		protected DataMartTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
