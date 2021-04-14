using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationServerConnectionTimeoutException : MigrationTransientException
	{
		public MigrationServerConnectionTimeoutException(string remoteHost, TimeSpan timeout) : base(Strings.ErrorConnectionTimeout(remoteHost, timeout))
		{
			this.remoteHost = remoteHost;
			this.timeout = timeout;
		}

		public MigrationServerConnectionTimeoutException(string remoteHost, TimeSpan timeout, Exception innerException) : base(Strings.ErrorConnectionTimeout(remoteHost, timeout), innerException)
		{
			this.remoteHost = remoteHost;
			this.timeout = timeout;
		}

		protected MigrationServerConnectionTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.remoteHost = (string)info.GetValue("remoteHost", typeof(string));
			this.timeout = (TimeSpan)info.GetValue("timeout", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("remoteHost", this.remoteHost);
			info.AddValue("timeout", this.timeout);
		}

		public string RemoteHost
		{
			get
			{
				return this.remoteHost;
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		private readonly string remoteHost;

		private readonly TimeSpan timeout;
	}
}
