using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorInboundConnectorAlreadyExistsException : ManagementObjectAlreadyExistsException
	{
		public ErrorInboundConnectorAlreadyExistsException(string name) : base(Strings.ErrorInboundConnectorAlreadyExists(name))
		{
			this.name = name;
		}

		public ErrorInboundConnectorAlreadyExistsException(string name, Exception innerException) : base(Strings.ErrorInboundConnectorAlreadyExists(name), innerException)
		{
			this.name = name;
		}

		protected ErrorInboundConnectorAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
