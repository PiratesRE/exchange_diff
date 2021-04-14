using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class SeederInstanceAlreadyFailedException : SeedPrepareException
	{
		internal SeederInstanceAlreadyFailedException(RpcSeederStatus seederStatus, string sourceMachine) : base(ReplayStrings.SeederInstanceAlreadyFailedException)
		{
			this.seederStatus = seederStatus;
		}

		internal SeederInstanceAlreadyFailedException(RpcSeederStatus seederStatus, string sourceMachine, Exception innerException) : base(ReplayStrings.SeederInstanceAlreadyFailedException, innerException)
		{
			this.seederStatus = seederStatus;
			this.sourceMachine = sourceMachine;
		}

		protected SeederInstanceAlreadyFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.seederStatus = (RpcSeederStatus)info.GetValue("seederStatus", typeof(RpcSeederStatus));
			this.sourceMachine = (string)info.GetValue("sourceMachine", typeof(string));
		}

		public override string Message
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_message))
				{
					if (this.seederStatus == null || this.seederStatus.ErrorInfo == null || !this.seederStatus.ErrorInfo.IsFailed())
					{
						return base.Message;
					}
					this.m_message = HaRpcExceptionHelper.AppendLastErrorString(base.Message, this.seederStatus.ErrorInfo.ErrorMessage);
				}
				return this.m_message;
			}
		}

		internal RpcSeederStatus SeederStatus
		{
			get
			{
				return this.seederStatus;
			}
		}

		internal string SourceMachine
		{
			get
			{
				return this.sourceMachine;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("seederStatus", this.seederStatus, typeof(RpcSeederStatus));
			info.AddValue("sourceMachine", this.sourceMachine, typeof(string));
		}

		private string m_message;

		private RpcSeederStatus seederStatus;

		private string sourceMachine;
	}
}
