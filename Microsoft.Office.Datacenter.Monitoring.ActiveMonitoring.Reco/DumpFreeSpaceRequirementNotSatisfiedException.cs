using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DumpFreeSpaceRequirementNotSatisfiedException : RecoveryActionExceptionCommon
	{
		public DumpFreeSpaceRequirementNotSatisfiedException(string directory, double available, double required) : base(StringsRecovery.DumpFreeSpaceRequirementNotSatisfied(directory, available, required))
		{
			this.directory = directory;
			this.available = available;
			this.required = required;
		}

		public DumpFreeSpaceRequirementNotSatisfiedException(string directory, double available, double required, Exception innerException) : base(StringsRecovery.DumpFreeSpaceRequirementNotSatisfied(directory, available, required), innerException)
		{
			this.directory = directory;
			this.available = available;
			this.required = required;
		}

		protected DumpFreeSpaceRequirementNotSatisfiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.directory = (string)info.GetValue("directory", typeof(string));
			this.available = (double)info.GetValue("available", typeof(double));
			this.required = (double)info.GetValue("required", typeof(double));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("directory", this.directory);
			info.AddValue("available", this.available);
			info.AddValue("required", this.required);
		}

		public string Directory
		{
			get
			{
				return this.directory;
			}
		}

		public double Available
		{
			get
			{
				return this.available;
			}
		}

		public double Required
		{
			get
			{
				return this.required;
			}
		}

		private readonly string directory;

		private readonly double available;

		private readonly double required;
	}
}
