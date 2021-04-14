using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MissingServerResponseException : TransientException
	{
		public MissingServerResponseException() : base(Strings.MissingServerResponseException)
		{
		}

		public MissingServerResponseException(Exception innerException) : base(Strings.MissingServerResponseException, innerException)
		{
		}

		protected MissingServerResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
