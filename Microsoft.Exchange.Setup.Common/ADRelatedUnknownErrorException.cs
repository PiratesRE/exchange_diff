using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADRelatedUnknownErrorException : LocalizedException
	{
		public ADRelatedUnknownErrorException() : base(Strings.ADRelatedUnknownError)
		{
		}

		public ADRelatedUnknownErrorException(Exception innerException) : base(Strings.ADRelatedUnknownError, innerException)
		{
		}

		protected ADRelatedUnknownErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
