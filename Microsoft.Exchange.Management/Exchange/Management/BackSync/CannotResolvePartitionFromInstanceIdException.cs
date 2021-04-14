using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.BackSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotResolvePartitionFromInstanceIdException : LocalizedException
	{
		public CannotResolvePartitionFromInstanceIdException(string serviceInstance) : base(Strings.CannotResolvePartitionFromInstanceId(serviceInstance))
		{
			this.serviceInstance = serviceInstance;
		}

		public CannotResolvePartitionFromInstanceIdException(string serviceInstance, Exception innerException) : base(Strings.CannotResolvePartitionFromInstanceId(serviceInstance), innerException)
		{
			this.serviceInstance = serviceInstance;
		}

		protected CannotResolvePartitionFromInstanceIdException(SerializationInfo info, StreamingContext context) : base(info, context)
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
