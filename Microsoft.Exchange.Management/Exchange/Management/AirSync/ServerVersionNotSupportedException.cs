using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerVersionNotSupportedException : LocalizedException
	{
		public ServerVersionNotSupportedException(string cmdletName, int cmdletVersion, int serverVersion) : base(Strings.ServerVersionNotSupportedException(cmdletName, cmdletVersion, serverVersion))
		{
			this.cmdletName = cmdletName;
			this.cmdletVersion = cmdletVersion;
			this.serverVersion = serverVersion;
		}

		public ServerVersionNotSupportedException(string cmdletName, int cmdletVersion, int serverVersion, Exception innerException) : base(Strings.ServerVersionNotSupportedException(cmdletName, cmdletVersion, serverVersion), innerException)
		{
			this.cmdletName = cmdletName;
			this.cmdletVersion = cmdletVersion;
			this.serverVersion = serverVersion;
		}

		protected ServerVersionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cmdletName = (string)info.GetValue("cmdletName", typeof(string));
			this.cmdletVersion = (int)info.GetValue("cmdletVersion", typeof(int));
			this.serverVersion = (int)info.GetValue("serverVersion", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cmdletName", this.cmdletName);
			info.AddValue("cmdletVersion", this.cmdletVersion);
			info.AddValue("serverVersion", this.serverVersion);
		}

		public string CmdletName
		{
			get
			{
				return this.cmdletName;
			}
		}

		public int CmdletVersion
		{
			get
			{
				return this.cmdletVersion;
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

		private readonly int cmdletVersion;

		private readonly int serverVersion;
	}
}
