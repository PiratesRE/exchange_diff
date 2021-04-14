using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederInstanceAlreadyCompletedException : SeedPrepareException
	{
		public SeederInstanceAlreadyCompletedException(string sourceMachine) : base(ReplayStrings.SeederInstanceAlreadyCompletedException(sourceMachine))
		{
			this.sourceMachine = sourceMachine;
		}

		public SeederInstanceAlreadyCompletedException(string sourceMachine, Exception innerException) : base(ReplayStrings.SeederInstanceAlreadyCompletedException(sourceMachine), innerException)
		{
			this.sourceMachine = sourceMachine;
		}

		protected SeederInstanceAlreadyCompletedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
