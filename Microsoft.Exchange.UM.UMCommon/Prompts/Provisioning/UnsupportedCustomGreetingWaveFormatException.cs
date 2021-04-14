using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedCustomGreetingWaveFormatException : PublishingException
	{
		public UnsupportedCustomGreetingWaveFormatException() : base(Strings.UnsupportedCustomGreetingWaveFormat)
		{
		}

		public UnsupportedCustomGreetingWaveFormatException(Exception innerException) : base(Strings.UnsupportedCustomGreetingWaveFormat, innerException)
		{
		}

		protected UnsupportedCustomGreetingWaveFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
