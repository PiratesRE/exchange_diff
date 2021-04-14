using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceFailedToStopException : LocalizedException
	{
		public ServiceFailedToStopException(string service) : base(Strings.ServiceFailedToStop(service))
		{
			this.service = service;
		}

		public ServiceFailedToStopException(string service, Exception innerException) : base(Strings.ServiceFailedToStop(service), innerException)
		{
			this.service = service;
		}

		protected ServiceFailedToStopException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.service = (string)info.GetValue("service", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("service", this.service);
		}

		public string Service
		{
			get
			{
				return this.service;
			}
		}

		private readonly string service;
	}
}
