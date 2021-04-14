using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdamUninstallProcessFailureException : LocalizedException
	{
		public AdamUninstallProcessFailureException(string processName, int exitCode) : base(Strings.AdamUninstallProcessFailure(processName, exitCode))
		{
			this.processName = processName;
			this.exitCode = exitCode;
		}

		public AdamUninstallProcessFailureException(string processName, int exitCode, Exception innerException) : base(Strings.AdamUninstallProcessFailure(processName, exitCode), innerException)
		{
			this.processName = processName;
			this.exitCode = exitCode;
		}

		protected AdamUninstallProcessFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.processName = (string)info.GetValue("processName", typeof(string));
			this.exitCode = (int)info.GetValue("exitCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("processName", this.processName);
			info.AddValue("exitCode", this.exitCode);
		}

		public string ProcessName
		{
			get
			{
				return this.processName;
			}
		}

		public int ExitCode
		{
			get
			{
				return this.exitCode;
			}
		}

		private readonly string processName;

		private readonly int exitCode;
	}
}
