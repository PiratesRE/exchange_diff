using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoSpeechDetectedException : LocalizedException
	{
		public NoSpeechDetectedException() : base(Strings.NoSpeechDetectedException)
		{
		}

		public NoSpeechDetectedException(Exception innerException) : base(Strings.NoSpeechDetectedException, innerException)
		{
		}

		protected NoSpeechDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
