using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.GroupMailbox.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExceededMaxSubscribersException : DataSourceOperationException
	{
		public ExceededMaxSubscribersException(LocalizedString message) : base(message)
		{
		}

		public ExceededMaxSubscribersException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ExceededMaxSubscribersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
