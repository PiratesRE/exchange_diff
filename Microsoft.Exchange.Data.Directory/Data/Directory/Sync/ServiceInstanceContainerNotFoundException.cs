using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceInstanceContainerNotFoundException : MandatoryContainerNotFoundException
	{
		public ServiceInstanceContainerNotFoundException(string serviceInstance) : base(DirectoryStrings.ServiceInstanceContainerNotFoundException(serviceInstance))
		{
			this.serviceInstance = serviceInstance;
		}

		public ServiceInstanceContainerNotFoundException(string serviceInstance, Exception innerException) : base(DirectoryStrings.ServiceInstanceContainerNotFoundException(serviceInstance), innerException)
		{
			this.serviceInstance = serviceInstance;
		}

		protected ServiceInstanceContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceInstance = (string)info.GetValue("serviceInstance", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceInstance", this.serviceInstance);
		}

		public string ServiceInstance
		{
			get
			{
				return this.serviceInstance;
			}
		}

		private readonly string serviceInstance;
	}
}
