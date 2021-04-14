using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidCookieException : LocalizedException
	{
		public InvalidCookieException() : base(DirectoryStrings.InvalidCookieException)
		{
		}

		public InvalidCookieException(Exception innerException) : base(DirectoryStrings.InvalidCookieException, innerException)
		{
		}

		protected InvalidCookieException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
