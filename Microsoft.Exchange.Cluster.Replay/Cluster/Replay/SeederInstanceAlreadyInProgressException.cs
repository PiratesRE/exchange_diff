using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederInstanceAlreadyInProgressException : SeedPrepareException
	{
		public SeederInstanceAlreadyInProgressException(string sourceMachine) : base(ReplayStrings.SeederInstanceAlreadyInProgressException(sourceMachine))
		{
			this.sourceMachine = sourceMachine;
		}

		public SeederInstanceAlreadyInProgressException(string sourceMachine, Exception innerException) : base(ReplayStrings.SeederInstanceAlreadyInProgressException(sourceMachine), innerException)
		{
			this.sourceMachine = sourceMachine;
		}

		protected SeederInstanceAlreadyInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sourceMachine = (string)info.GetValue("sourceMachine", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sourceMachine", this.sourceMachine);
		}

		public string SourceMachine
		{
			get
			{
				return this.sourceMachine;
			}
		}

		private readonly string sourceMachine;
	}
}
