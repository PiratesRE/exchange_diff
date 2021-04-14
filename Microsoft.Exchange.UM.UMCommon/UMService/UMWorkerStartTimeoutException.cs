using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Exchange.UM.UMService.Exceptions;

namespace Microsoft.Exchange.UM.UMService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMWorkerStartTimeoutException : UMServiceBaseException
	{
		public UMWorkerStartTimeoutException(int seconds) : base(Strings.UMWorkerStartTimeoutException(seconds))
		{
			this.seconds = seconds;
		}

		public UMWorkerStartTimeoutException(int seconds, Exception innerException) : base(Strings.UMWorkerStartTimeoutException(seconds), innerException)
		{
			this.seconds = seconds;
		}

		protected UMWorkerStartTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.seconds = (int)info.GetValue("seconds", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("seconds", this.seconds);
		}

		public int Seconds
		{
			get
			{
				return this.seconds;
			}
		}

		private readonly int seconds;
	}
}
