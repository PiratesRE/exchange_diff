using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PortIsBusyException : LocalizedException
	{
		public PortIsBusyException(int port) : base(Strings.PortIsBusy(port))
		{
			this.port = port;
		}

		public PortIsBusyException(int port, Exception innerException) : base(Strings.PortIsBusy(port), innerException)
		{
			this.port = port;
		}

		protected PortIsBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.port = (int)info.GetValue("port", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("port", this.port);
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		private readonly int port;
	}
}
