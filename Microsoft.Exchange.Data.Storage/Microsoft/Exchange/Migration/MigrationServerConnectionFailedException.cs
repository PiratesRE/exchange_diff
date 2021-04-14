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
	internal class MigrationServerConnectionFailedException : MigrationTransientException
	{
		public MigrationServerConnectionFailedException(string remoteHost) : base(Strings.ErrorConnectionFailed(remoteHost))
		{
			this.remoteHost = remoteHost;
		}

		public MigrationServerConnectionFailedException(string remoteHost, Exception innerException) : base(Strings.ErrorConnectionFailed(remoteHost), innerException)
		{
			this.remoteHost = remoteHost;
		}

		protected MigrationServerConnectionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.remoteHost = (string)info.GetValue("remoteHost", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("remoteHost", this.remoteHost);
		}

		public string RemoteHost
		{
			get
			{
				return this.remoteHost;
			}
		}

		private readonly string remoteHost;
	}
}
