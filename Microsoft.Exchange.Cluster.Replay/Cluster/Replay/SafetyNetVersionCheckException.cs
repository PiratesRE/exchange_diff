using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SafetyNetVersionCheckException : DumpsterRedeliveryException
	{
		public SafetyNetVersionCheckException(string error) : base(ReplayStrings.SafetyNetVersionCheckException(error))
		{
			this.error = error;
		}

		public SafetyNetVersionCheckException(string error, Exception innerException) : base(ReplayStrings.SafetyNetVersionCheckException(error), innerException)
		{
			this.error = error;
		}

		protected SafetyNetVersionCheckException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
