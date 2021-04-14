using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceUninstallFailureException : LocalizedException
	{
		public ServiceUninstallFailureException(string servicename, string error) : base(Strings.ServiceUninstallFailure(servicename, error))
		{
			this.servicename = servicename;
			this.error = error;
		}

		public ServiceUninstallFailureException(string servicename, string error, Exception innerException) : base(Strings.ServiceUninstallFailure(servicename, error), innerException)
		{
			this.servicename = servicename;
			this.error = error;
		}

		protected ServiceUninstallFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.servicename = (string)info.GetValue("servicename", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("servicename", this.servicename);
			info.AddValue("error", this.error);
		}

		public string Servicename
		{
			get
			{
				return this.servicename;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string servicename;

		private readonly string error;
	}
}
