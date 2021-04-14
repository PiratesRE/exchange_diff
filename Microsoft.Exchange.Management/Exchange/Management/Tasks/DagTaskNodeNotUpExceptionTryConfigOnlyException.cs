using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskNodeNotUpExceptionTryConfigOnlyException : LocalizedException
	{
		public DagTaskNodeNotUpExceptionTryConfigOnlyException(string machineName, string clusterName, string machineState) : base(Strings.DagTaskNodeNotUpExceptionTryConfigOnlyException(machineName, clusterName, machineState))
		{
			this.machineName = machineName;
			this.clusterName = clusterName;
			this.machineState = machineState;
		}

		public DagTaskNodeNotUpExceptionTryConfigOnlyException(string machineName, string clusterName, string machineState, Exception innerException) : base(Strings.DagTaskNodeNotUpExceptionTryConfigOnlyException(machineName, clusterName, machineState), innerException)
		{
			this.machineName = machineName;
			this.clusterName = clusterName;
			this.machineState = machineState;
		}

		protected DagTaskNodeNotUpExceptionTryConfigOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.machineName = (string)info.GetValue("machineName", typeof(string));
			this.clusterName = (string)info.GetValue("clusterName", typeof(string));
			this.machineState = (string)info.GetValue("machineState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("machineName", this.machineName);
			info.AddValue("clusterName", this.clusterName);
			info.AddValue("machineState", this.machineState);
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public string ClusterName
		{
			get
			{
				return this.clusterName;
			}
		}

		public string MachineState
		{
			get
			{
				return this.machineState;
			}
		}

		private readonly string machineName;

		private readonly string clusterName;

		private readonly string machineState;
	}
}
