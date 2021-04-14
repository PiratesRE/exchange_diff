using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmGetServiceProcessException : AmCommonException
	{
		public AmGetServiceProcessException(string serviceName, int state, int pid) : base(ReplayStrings.AmGetServiceProcessFailed(serviceName, state, pid))
		{
			this.serviceName = serviceName;
			this.state = state;
			this.pid = pid;
		}

		public AmGetServiceProcessException(string serviceName, int state, int pid, Exception innerException) : base(ReplayStrings.AmGetServiceProcessFailed(serviceName, state, pid), innerException)
		{
			this.serviceName = serviceName;
			this.state = state;
			this.pid = pid;
		}

		protected AmGetServiceProcessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
			this.state = (int)info.GetValue("state", typeof(int));
			this.pid = (int)info.GetValue("pid", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
			info.AddValue("state", this.state);
			info.AddValue("pid", this.pid);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public int State
		{
			get
			{
				return this.state;
			}
		}

		public int Pid
		{
			get
			{
				return this.pid;
			}
		}

		private readonly string serviceName;

		private readonly int state;

		private readonly int pid;
	}
}
