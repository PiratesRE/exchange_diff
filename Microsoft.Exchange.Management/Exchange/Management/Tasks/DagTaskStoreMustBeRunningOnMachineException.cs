using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskStoreMustBeRunningOnMachineException : LocalizedException
	{
		public DagTaskStoreMustBeRunningOnMachineException(string machineName) : base(Strings.DagTaskStoreMustBeRunningOnMachineException(machineName))
		{
			this.machineName = machineName;
		}

		public DagTaskStoreMustBeRunningOnMachineException(string machineName, Exception innerException) : base(Strings.DagTaskStoreMustBeRunningOnMachineException(machineName), innerException)
		{
			this.machineName = machineName;
		}

		protected DagTaskStoreMustBeRunningOnMachineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.machineName = (string)info.GetValue("machineName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("machineName", this.machineName);
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		private readonly string machineName;
	}
}
