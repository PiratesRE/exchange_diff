using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RemoteServerTooSlowException : LocalizedException
	{
		public RemoteServerTooSlowException(string remoteServer, int port, TimeSpan actualLatency, TimeSpan expectedLatency) : base(Strings.RemoteServerTooSlowException(remoteServer, port, actualLatency, expectedLatency))
		{
			this.remoteServer = remoteServer;
			this.port = port;
			this.actualLatency = actualLatency;
			this.expectedLatency = expectedLatency;
		}

		public RemoteServerTooSlowException(string remoteServer, int port, TimeSpan actualLatency, TimeSpan expectedLatency, Exception innerException) : base(Strings.RemoteServerTooSlowException(remoteServer, port, actualLatency, expectedLatency), innerException)
		{
			this.remoteServer = remoteServer;
			this.port = port;
			this.actualLatency = actualLatency;
			this.expectedLatency = expectedLatency;
		}

		protected RemoteServerTooSlowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.remoteServer = (string)info.GetValue("remoteServer", typeof(string));
			this.port = (int)info.GetValue("port", typeof(int));
			this.actualLatency = (TimeSpan)info.GetValue("actualLatency", typeof(TimeSpan));
			this.expectedLatency = (TimeSpan)info.GetValue("expectedLatency", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("remoteServer", this.remoteServer);
			info.AddValue("port", this.port);
			info.AddValue("actualLatency", this.actualLatency);
			info.AddValue("expectedLatency", this.expectedLatency);
		}

		public string RemoteServer
		{
			get
			{
				return this.remoteServer;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public TimeSpan ActualLatency
		{
			get
			{
				return this.actualLatency;
			}
		}

		public TimeSpan ExpectedLatency
		{
			get
			{
				return this.expectedLatency;
			}
		}

		private readonly string remoteServer;

		private readonly int port;

		private readonly TimeSpan actualLatency;

		private readonly TimeSpan expectedLatency;
	}
}
