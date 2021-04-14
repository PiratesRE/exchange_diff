using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsOnlineUrlsNotPresentException : LocalizedException
	{
		public RmsOnlineUrlsNotPresentException() : base(Strings.RmsOnlineUrlsNotPresent)
		{
		}

		public RmsOnlineUrlsNotPresentException(Exception innerException) : base(Strings.RmsOnlineUrlsNotPresent, innerException)
		{
		}

		protected RmsOnlineUrlsNotPresentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
