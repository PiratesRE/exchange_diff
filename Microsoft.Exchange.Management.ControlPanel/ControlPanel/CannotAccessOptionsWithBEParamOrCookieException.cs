using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotAccessOptionsWithBEParamOrCookieException : LocalizedException
	{
		public CannotAccessOptionsWithBEParamOrCookieException() : base(Strings.CannotAccessOptionsWithBEParamOrCookieMessage)
		{
		}

		public CannotAccessOptionsWithBEParamOrCookieException(Exception innerException) : base(Strings.CannotAccessOptionsWithBEParamOrCookieMessage, innerException)
		{
		}

		protected CannotAccessOptionsWithBEParamOrCookieException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
