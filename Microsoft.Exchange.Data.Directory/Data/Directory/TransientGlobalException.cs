using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TransientGlobalException : DataSourceTransientException
	{
		public TransientGlobalException(LocalizedString message) : base(message)
		{
		}

		public TransientGlobalException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TransientGlobalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
