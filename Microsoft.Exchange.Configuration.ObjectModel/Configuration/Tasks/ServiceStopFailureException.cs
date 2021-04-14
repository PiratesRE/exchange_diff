using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceStopFailureException : LocalizedException
	{
		public ServiceStopFailureException(string name, string msg) : base(Strings.ServiceStopFailure(name, msg))
		{
			this.name = name;
			this.msg = msg;
		}

		public ServiceStopFailureException(string name, string msg, Exception innerException) : base(Strings.ServiceStopFailure(name, msg), innerException)
		{
			this.name = name;
			this.msg = msg;
		}

		protected ServiceStopFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("msg", this.msg);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string name;

		private readonly string msg;
	}
}
