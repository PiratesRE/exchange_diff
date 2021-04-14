using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceResumeBlockedException : TaskServerException
	{
		public ReplayServiceResumeBlockedException(string previousError) : base(ReplayStrings.ReplayServiceResumeBlockedException(previousError))
		{
			this.previousError = previousError;
		}

		public ReplayServiceResumeBlockedException(string previousError, Exception innerException) : base(ReplayStrings.ReplayServiceResumeBlockedException(previousError), innerException)
		{
			this.previousError = previousError;
		}

		protected ReplayServiceResumeBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.previousError = (string)info.GetValue("previousError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("previousError", this.previousError);
		}

		public string PreviousError
		{
			get
			{
				return this.previousError;
			}
		}

		private readonly string previousError;
	}
}
