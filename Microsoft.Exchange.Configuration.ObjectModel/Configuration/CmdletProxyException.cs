using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CmdletProxyException : LocalizedException
	{
		public CmdletProxyException(string command, string serverFqn, string serverVersion, string proxyMethod, string errorMessage) : base(Strings.ErrorCmdletProxy(command, serverFqn, serverVersion, proxyMethod, errorMessage))
		{
			this.command = command;
			this.serverFqn = serverFqn;
			this.serverVersion = serverVersion;
			this.proxyMethod = proxyMethod;
			this.errorMessage = errorMessage;
		}

		public CmdletProxyException(string command, string serverFqn, string serverVersion, string proxyMethod, string errorMessage, Exception innerException) : base(Strings.ErrorCmdletProxy(command, serverFqn, serverVersion, proxyMethod, errorMessage), innerException)
		{
			this.command = command;
			this.serverFqn = serverFqn;
			this.serverVersion = serverVersion;
			this.proxyMethod = proxyMethod;
			this.errorMessage = errorMessage;
		}

		protected CmdletProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.command = (string)info.GetValue("command", typeof(string));
			this.serverFqn = (string)info.GetValue("serverFqn", typeof(string));
			this.serverVersion = (string)info.GetValue("serverVersion", typeof(string));
			this.proxyMethod = (string)info.GetValue("proxyMethod", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("command", this.command);
			info.AddValue("serverFqn", this.serverFqn);
			info.AddValue("serverVersion", this.serverVersion);
			info.AddValue("proxyMethod", this.proxyMethod);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string Command
		{
			get
			{
				return this.command;
			}
		}

		public string ServerFqn
		{
			get
			{
				return this.serverFqn;
			}
		}

		public string ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public string ProxyMethod
		{
			get
			{
				return this.proxyMethod;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string command;

		private readonly string serverFqn;

		private readonly string serverVersion;

		private readonly string proxyMethod;

		private readonly string errorMessage;
	}
}
