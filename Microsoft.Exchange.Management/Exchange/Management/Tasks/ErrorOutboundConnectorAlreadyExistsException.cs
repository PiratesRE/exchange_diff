using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorOutboundConnectorAlreadyExistsException : ManagementObjectAlreadyExistsException
	{
		public ErrorOutboundConnectorAlreadyExistsException(string name) : base(Strings.ErrorOutboundConnectorAlreadyExists(name))
		{
			this.name = name;
		}

		public ErrorOutboundConnectorAlreadyExistsException(string name, Exception innerException) : base(Strings.ErrorOutboundConnectorAlreadyExists(name), innerException)
		{
			this.name = name;
		}

		protected ErrorOutboundConnectorAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
