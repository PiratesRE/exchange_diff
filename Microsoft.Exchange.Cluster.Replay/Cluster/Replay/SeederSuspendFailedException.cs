using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederSuspendFailedException : SeedInProgressException
	{
		public SeederSuspendFailedException(string specificError) : base(ReplayStrings.SeederSuspendFailedException(specificError))
		{
			this.specificError = specificError;
		}

		public SeederSuspendFailedException(string specificError, Exception innerException) : base(ReplayStrings.SeederSuspendFailedException(specificError), innerException)
		{
			this.specificError = specificError;
		}

		protected SeederSuspendFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.specificError = (string)info.GetValue("specificError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("specificError", this.specificError);
		}

		public string SpecificError
		{
			get
			{
				return this.specificError;
			}
		}

		private readonly string specificError;
	}
}
