using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceNotInstalledException : LocalizedException
	{
		public ServiceNotInstalledException(string servicename) : base(Strings.ServiceNotInstalled(servicename))
		{
			this.servicename = servicename;
		}

		public ServiceNotInstalledException(string servicename, Exception innerException) : base(Strings.ServiceNotInstalled(servicename), innerException)
		{
			this.servicename = servicename;
		}

		protected ServiceNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
