using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutodiscoverServiceUnavailableException : LocalizedException
	{
		public AutodiscoverServiceUnavailableException() : base(Strings.messageAutodiscoverServiceUnavailableException)
		{
		}

		public AutodiscoverServiceUnavailableException(Exception innerException) : base(Strings.messageAutodiscoverServiceUnavailableException, innerException)
		{
		}

		protected AutodiscoverServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
