using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.FederatedDirectory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeAdaptorException : LocalizedException
	{
		public ExchangeAdaptorException(LocalizedString message) : base(message)
		{
		}

		public ExchangeAdaptorException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ExchangeAdaptorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
