using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerFswServerNotAccessibleWithWmiException : LocalizedException
	{
		public DagTaskServerFswServerNotAccessibleWithWmiException(string fswServer) : base(Strings.DagTaskServerFswServerNotAccessibleWithWmi(fswServer))
		{
			this.fswServer = fswServer;
		}

		public DagTaskServerFswServerNotAccessibleWithWmiException(string fswServer, Exception innerException) : base(Strings.DagTaskServerFswServerNotAccessibleWithWmi(fswServer), innerException)
		{
			this.fswServer = fswServer;
		}

		protected DagTaskServerFswServerNotAccessibleWithWmiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fswServer = (string)info.GetValue("fswServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fswServer", this.fswServer);
		}

		public string FswServer
		{
			get
			{
				return this.fswServer;
			}
		}

		private readonly string fswServer;
	}
}
