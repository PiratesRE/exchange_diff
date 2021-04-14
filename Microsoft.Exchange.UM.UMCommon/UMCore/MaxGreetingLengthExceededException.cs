using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MaxGreetingLengthExceededException : LocalizedException
	{
		public MaxGreetingLengthExceededException(int greetingLength) : base(Strings.MaxGreetingLengthExceededException(greetingLength))
		{
			this.greetingLength = greetingLength;
		}

		public MaxGreetingLengthExceededException(int greetingLength, Exception innerException) : base(Strings.MaxGreetingLengthExceededException(greetingLength), innerException)
		{
			this.greetingLength = greetingLength;
		}

		protected MaxGreetingLengthExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.greetingLength = (int)info.GetValue("greetingLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("greetingLength", this.greetingLength);
		}

		public int GreetingLength
		{
			get
			{
				return this.greetingLength;
			}
		}

		private readonly int greetingLength;
	}
}
