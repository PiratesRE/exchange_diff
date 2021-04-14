using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class WrongPropertyValueCombinationException : LocalizedException
	{
		public WrongPropertyValueCombinationException(LocalizedString message) : base(message)
		{
		}

		public WrongPropertyValueCombinationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WrongPropertyValueCombinationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
