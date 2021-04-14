using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcmException : AudioConversionException
	{
		public AcmException(string failureMessage) : base(Strings.AcmFailure(failureMessage))
		{
			this.failureMessage = failureMessage;
		}

		public AcmException(string failureMessage, Exception innerException) : base(Strings.AcmFailure(failureMessage), innerException)
		{
			this.failureMessage = failureMessage;
		}

		protected AcmException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failureMessage = (string)info.GetValue("failureMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failureMessage", this.failureMessage);
		}

		public string FailureMessage
		{
			get
			{
				return this.failureMessage;
			}
		}

		private readonly string failureMessage;
	}
}
