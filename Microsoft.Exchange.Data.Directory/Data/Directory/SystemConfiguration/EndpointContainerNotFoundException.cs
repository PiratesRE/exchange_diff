using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EndpointContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public EndpointContainerNotFoundException(string endpointName) : base(DirectoryStrings.EndpointContainerNotFoundException(endpointName))
		{
			this.endpointName = endpointName;
		}

		public EndpointContainerNotFoundException(string endpointName, Exception innerException) : base(DirectoryStrings.EndpointContainerNotFoundException(endpointName), innerException)
		{
			this.endpointName = endpointName;
		}

		protected EndpointContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointName = (string)info.GetValue("endpointName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointName", this.endpointName);
		}

		public string EndpointName
		{
			get
			{
				return this.endpointName;
			}
		}

		private readonly string endpointName;
	}
}
