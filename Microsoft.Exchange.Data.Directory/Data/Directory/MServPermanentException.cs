using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MServPermanentException : DataSourceOperationException
	{
		public MServPermanentException(LocalizedString message) : base(message)
		{
		}

		public MServPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MServPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
