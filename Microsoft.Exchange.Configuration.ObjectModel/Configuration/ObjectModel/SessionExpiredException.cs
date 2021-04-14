using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SessionExpiredException : TransientException
	{
		public SessionExpiredException() : base(Strings.SessionExpiredException)
		{
		}

		public SessionExpiredException(Exception innerException) : base(Strings.SessionExpiredException, innerException)
		{
		}

		protected SessionExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
