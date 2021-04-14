using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataSourceOperationException : LocalizedException
	{
		public DataSourceOperationException(LocalizedString message) : base(message)
		{
		}

		public DataSourceOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DataSourceOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
