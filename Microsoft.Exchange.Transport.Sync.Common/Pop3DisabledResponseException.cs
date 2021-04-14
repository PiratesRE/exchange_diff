using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3DisabledResponseException : LocalizedException
	{
		public Pop3DisabledResponseException() : base(Strings.Pop3DisabledResponseException)
		{
		}

		public Pop3DisabledResponseException(Exception innerException) : base(Strings.Pop3DisabledResponseException, innerException)
		{
		}

		protected Pop3DisabledResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
