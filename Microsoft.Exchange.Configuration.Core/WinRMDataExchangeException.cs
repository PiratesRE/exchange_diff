using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WinRMDataExchangeException : LocalizedException
	{
		public WinRMDataExchangeException(LocalizedString message) : base(message)
		{
		}

		public WinRMDataExchangeException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WinRMDataExchangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
