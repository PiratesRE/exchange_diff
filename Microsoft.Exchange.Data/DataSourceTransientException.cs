using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataSourceTransientException : TransientException
	{
		public DataSourceTransientException(LocalizedString message) : base(message)
		{
		}

		public DataSourceTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DataSourceTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
