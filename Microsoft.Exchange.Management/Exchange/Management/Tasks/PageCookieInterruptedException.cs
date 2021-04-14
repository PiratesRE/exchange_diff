using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PageCookieInterruptedException : LocalizedException
	{
		public PageCookieInterruptedException() : base(Strings.PageCookieInterruptedException)
		{
		}

		public PageCookieInterruptedException(Exception innerException) : base(Strings.PageCookieInterruptedException, innerException)
		{
		}

		protected PageCookieInterruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
