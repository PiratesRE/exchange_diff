using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceDisabledException : LocalizedException
	{
		public ServiceDisabledException(string servicename) : base(Strings.ServiceDisabled(servicename))
		{
			this.servicename = servicename;
		}

		public ServiceDisabledException(string servicename, Exception innerException) : base(Strings.ServiceDisabled(servicename), innerException)
		{
			this.servicename = servicename;
		}

		protected ServiceDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.servicename = (string)info.GetValue("servicename", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("servicename", this.servicename);
		}

		public string Servicename
		{
			get
			{
				return this.servicename;
			}
		}

		private readonly string servicename;
	}
}
