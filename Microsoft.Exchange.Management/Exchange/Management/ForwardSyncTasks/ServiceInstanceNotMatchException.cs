using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceInstanceNotMatchException : LocalizedException
	{
		public ServiceInstanceNotMatchException(string objectId, string requestServiceInstance, string objectServiceInstance) : base(Strings.ServiceInstanceNotMatchMessage(objectId, requestServiceInstance, objectServiceInstance))
		{
			this.objectId = objectId;
			this.requestServiceInstance = requestServiceInstance;
			this.objectServiceInstance = objectServiceInstance;
		}

		public ServiceInstanceNotMatchException(string objectId, string requestServiceInstance, string objectServiceInstance, Exception innerException) : base(Strings.ServiceInstanceNotMatchMessage(objectId, requestServiceInstance, objectServiceInstance), innerException)
		{
			this.objectId = objectId;
			this.requestServiceInstance = requestServiceInstance;
			this.objectServiceInstance = objectServiceInstance;
		}

		protected ServiceInstanceNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectId = (string)info.GetValue("objectId", typeof(string));
			this.requestServiceInstance = (string)info.GetValue("requestServiceInstance", typeof(string));
			this.objectServiceInstance = (string)info.GetValue("objectServiceInstance", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectId", this.objectId);
			info.AddValue("requestServiceInstance", this.requestServiceInstance);
			info.AddValue("objectServiceInstance", this.objectServiceInstance);
		}

		public string ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		public string RequestServiceInstance
		{
			get
			{
				return this.requestServiceInstance;
			}
		}

		public string ObjectServiceInstance
		{
			get
			{
				return this.objectServiceInstance;
			}
		}

		private readonly string objectId;

		private readonly string requestServiceInstance;

		private readonly string objectServiceInstance;
	}
}
