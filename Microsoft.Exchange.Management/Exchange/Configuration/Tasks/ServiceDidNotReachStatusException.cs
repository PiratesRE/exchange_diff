using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceDidNotReachStatusException : LocalizedException
	{
		public ServiceDidNotReachStatusException(string servicename, string status) : base(Strings.ServiceDidNotReachStatus(servicename, status))
		{
			this.servicename = servicename;
			this.status = status;
		}

		public ServiceDidNotReachStatusException(string servicename, string status, Exception innerException) : base(Strings.ServiceDidNotReachStatus(servicename, status), innerException)
		{
			this.servicename = servicename;
			this.status = status;
		}

		protected ServiceDidNotReachStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.servicename = (string)info.GetValue("servicename", typeof(string));
			this.status = (string)info.GetValue("status", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("servicename", this.servicename);
			info.AddValue("status", this.status);
		}

		public string Servicename
		{
			get
			{
				return this.servicename;
			}
		}

		public string Status
		{
			get
			{
				return this.status;
			}
		}

		private readonly string servicename;

		private readonly string status;
	}
}
