using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidServiceInstanceIdException : LocalizedException
	{
		public InvalidServiceInstanceIdException(string serviceInstanceId) : base(DirectoryStrings.InvalidServiceInstanceIdException(serviceInstanceId))
		{
			this.serviceInstanceId = serviceInstanceId;
		}

		public InvalidServiceInstanceIdException(string serviceInstanceId, Exception innerException) : base(DirectoryStrings.InvalidServiceInstanceIdException(serviceInstanceId), innerException)
		{
			this.serviceInstanceId = serviceInstanceId;
		}

		protected InvalidServiceInstanceIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceInstanceId = (string)info.GetValue("serviceInstanceId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceInstanceId", this.serviceInstanceId);
		}

		public string ServiceInstanceId
		{
			get
			{
				return this.serviceInstanceId;
			}
		}

		private readonly string serviceInstanceId;
	}
}
