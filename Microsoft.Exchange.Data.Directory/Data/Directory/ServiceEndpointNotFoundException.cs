using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceEndpointNotFoundException : ADOperationException
	{
		public ServiceEndpointNotFoundException(string serviceEndpointName) : base(DirectoryStrings.ErrorServiceEndpointNotFound(serviceEndpointName))
		{
			this.serviceEndpointName = serviceEndpointName;
		}

		public ServiceEndpointNotFoundException(string serviceEndpointName, Exception innerException) : base(DirectoryStrings.ErrorServiceEndpointNotFound(serviceEndpointName), innerException)
		{
			this.serviceEndpointName = serviceEndpointName;
		}

		protected ServiceEndpointNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceEndpointName = (string)info.GetValue("serviceEndpointName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceEndpointName", this.serviceEndpointName);
		}

		public string ServiceEndpointName
		{
			get
			{
				return this.serviceEndpointName;
			}
		}

		private readonly string serviceEndpointName;
	}
}
