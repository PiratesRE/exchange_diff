using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdamSetAclsProcessFailureException : LocalizedException
	{
		public AdamSetAclsProcessFailureException(string processName, int exitCode, string dn) : base(Strings.AdamSetAclsProcessFailure(processName, exitCode, dn))
		{
			this.processName = processName;
			this.exitCode = exitCode;
			this.dn = dn;
		}

		public AdamSetAclsProcessFailureException(string processName, int exitCode, string dn, Exception innerException) : base(Strings.AdamSetAclsProcessFailure(processName, exitCode, dn), innerException)
		{
			this.processName = processName;
			this.exitCode = exitCode;
			this.dn = dn;
		}

		protected AdamSetAclsProcessFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.processName = (string)info.GetValue("processName", typeof(string));
			this.exitCode = (int)info.GetValue("exitCode", typeof(int));
			this.dn = (string)info.GetValue("dn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("processName", this.processName);
			info.AddValue("exitCode", this.exitCode);
			info.AddValue("dn", this.dn);
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

		public string Dn
		{
			get
			{
				return this.dn;
			}
		}

		private readonly string processName;

		private readonly int exitCode;

		private readonly string dn;
	}
}
