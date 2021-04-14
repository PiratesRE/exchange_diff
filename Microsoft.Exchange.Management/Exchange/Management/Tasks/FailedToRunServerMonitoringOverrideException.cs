using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToRunServerMonitoringOverrideException : LocalizedException
	{
		public FailedToRunServerMonitoringOverrideException(string server, string failure) : base(Strings.FailedToRunServerMonitoringOverride(server, failure))
		{
			this.server = server;
			this.failure = failure;
		}

		public FailedToRunServerMonitoringOverrideException(string server, string failure, Exception innerException) : base(Strings.FailedToRunServerMonitoringOverride(server, failure), innerException)
		{
			this.server = server;
			this.failure = failure;
		}

		protected FailedToRunServerMonitoringOverrideException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("failure", this.failure);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string server;

		private readonly string failure;
	}
}
