using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RedirectionLogicException : LocalizedException
	{
		public RedirectionLogicException(LocalizedString message) : base(message)
		{
		}

		public RedirectionLogicException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RedirectionLogicException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
