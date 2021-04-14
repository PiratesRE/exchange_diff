using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidMainStreamCookieException : LocalizedException
	{
		public InvalidMainStreamCookieException() : base(DirectoryStrings.InvalidMainStreamCookieException)
		{
		}

		public InvalidMainStreamCookieException(Exception innerException) : base(DirectoryStrings.InvalidMainStreamCookieException, innerException)
		{
		}

		protected InvalidMainStreamCookieException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
