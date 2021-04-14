using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPortNumberException : LocalizedException
	{
		public InvalidPortNumberException(int port) : base(Strings.InvalidPortNumber(port))
		{
			this.port = port;
		}

		public InvalidPortNumberException(int port, Exception innerException) : base(Strings.InvalidPortNumber(port), innerException)
		{
			this.port = port;
		}

		protected InvalidPortNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
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
