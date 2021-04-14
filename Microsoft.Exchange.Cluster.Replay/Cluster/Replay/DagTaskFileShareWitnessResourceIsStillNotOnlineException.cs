using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskFileShareWitnessResourceIsStillNotOnlineException : DagTaskServerException
	{
		public DagTaskFileShareWitnessResourceIsStillNotOnlineException(string fswResource, string currentState) : base(ReplayStrings.DagTaskFileShareWitnessResourceIsStillNotOnlineException(fswResource, currentState))
		{
			this.fswResource = fswResource;
			this.currentState = currentState;
		}

		public DagTaskFileShareWitnessResourceIsStillNotOnlineException(string fswResource, string currentState, Exception innerException) : base(ReplayStrings.DagTaskFileShareWitnessResourceIsStillNotOnlineException(fswResource, currentState), innerException)
		{
			this.fswResource = fswResource;
			this.currentState = currentState;
		}

		protected DagTaskFileShareWitnessResourceIsStillNotOnlineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fswResource = (string)info.GetValue("fswResource", typeof(string));
			this.currentState = (string)info.GetValue("currentState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fswResource", this.fswResource);
			info.AddValue("currentState", this.currentState);
		}

		public string FswResource
		{
			get
			{
				return this.fswResource;
			}
		}

		public string CurrentState
		{
			get
			{
				return this.currentState;
			}
		}

		private readonly string fswResource;

		private readonly string currentState;
	}
}
