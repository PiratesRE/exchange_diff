using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedCustomGreetingWmaFormatException : PublishingException
	{
		public UnsupportedCustomGreetingWmaFormatException() : base(Strings.UnsupportedCustomGreetingWmaFormat)
		{
		}

		public UnsupportedCustomGreetingWmaFormatException(Exception innerException) : base(Strings.UnsupportedCustomGreetingWmaFormat, innerException)
		{
		}

		protected UnsupportedCustomGreetingWmaFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
