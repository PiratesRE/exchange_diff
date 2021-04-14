using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ProcessCrashingException : LocalizedException
	{
		public ProcessCrashingException(string serviceName, string server) : base(Strings.ProcessCrashing(serviceName, server))
		{
			this.serviceName = serviceName;
			this.server = server;
		}

		public ProcessCrashingException(string serviceName, string server, Exception innerException) : base(Strings.ProcessCrashing(serviceName, server), innerException)
		{
			this.serviceName = serviceName;
			this.server = server;
		}

		protected ProcessCrashingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
			info.AddValue("server", this.server);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string serviceName;

		private readonly string server;
	}
}
