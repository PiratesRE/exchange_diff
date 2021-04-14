using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceNotStarted : LocalizedException
	{
		public ServiceNotStarted(string serviceName) : base(Strings.ServiceNotStarted(serviceName))
		{
			this.serviceName = serviceName;
		}

		public ServiceNotStarted(string serviceName, Exception innerException) : base(Strings.ServiceNotStarted(serviceName), innerException)
		{
			this.serviceName = serviceName;
		}

		protected ServiceNotStarted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		private readonly string serviceName;
	}
}
