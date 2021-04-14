using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskNotSupportedOnVersionException : LocalizedException
	{
		public TaskNotSupportedOnVersionException(string cmdletName, int serverVersion) : base(Strings.TaskNotSupportedOnVersionException(cmdletName, serverVersion))
		{
			this.cmdletName = cmdletName;
			this.serverVersion = serverVersion;
		}

		public TaskNotSupportedOnVersionException(string cmdletName, int serverVersion, Exception innerException) : base(Strings.TaskNotSupportedOnVersionException(cmdletName, serverVersion), innerException)
		{
			this.cmdletName = cmdletName;
			this.serverVersion = serverVersion;
		}

		protected TaskNotSupportedOnVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cmdletName = (string)info.GetValue("cmdletName", typeof(string));
			this.serverVersion = (int)info.GetValue("serverVersion", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cmdletName", this.cmdletName);
			info.AddValue("serverVersion", this.serverVersion);
		}

		public string CmdletName
		{
			get
			{
				return this.cmdletName;
			}
		}

		public int ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		private readonly string cmdletName;

		private readonly int serverVersion;
	}
}
