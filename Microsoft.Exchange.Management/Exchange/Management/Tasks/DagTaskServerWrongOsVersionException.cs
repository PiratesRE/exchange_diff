using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerWrongOsVersionException : LocalizedException
	{
		public DagTaskServerWrongOsVersionException(string serverName) : base(Strings.DagTaskServerWrongOsVersionException(serverName))
		{
			this.serverName = serverName;
		}

		public DagTaskServerWrongOsVersionException(string serverName, Exception innerException) : base(Strings.DagTaskServerWrongOsVersionException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected DagTaskServerWrongOsVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
