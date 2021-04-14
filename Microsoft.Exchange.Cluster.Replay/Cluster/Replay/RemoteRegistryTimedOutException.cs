using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoteRegistryTimedOutException : TransientException
	{
		public RemoteRegistryTimedOutException(string machineName, int secondsTimeout) : base(ReplayStrings.RemoteRegistryTimedOutException(machineName, secondsTimeout))
		{
			this.machineName = machineName;
			this.secondsTimeout = secondsTimeout;
		}

		public RemoteRegistryTimedOutException(string machineName, int secondsTimeout, Exception innerException) : base(ReplayStrings.RemoteRegistryTimedOutException(machineName, secondsTimeout), innerException)
		{
			this.machineName = machineName;
			this.secondsTimeout = secondsTimeout;
		}

		protected RemoteRegistryTimedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.machineName = (string)info.GetValue("machineName", typeof(string));
			this.secondsTimeout = (int)info.GetValue("secondsTimeout", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("machineName", this.machineName);
			info.AddValue("secondsTimeout", this.secondsTimeout);
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public int SecondsTimeout
		{
			get
			{
				return this.secondsTimeout;
			}
		}

		private readonly string machineName;

		private readonly int secondsTimeout;
	}
}
