using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidServiceInstanceMapXmlFormatException : LocalizedException
	{
		public InvalidServiceInstanceMapXmlFormatException(string reason) : base(Strings.InvalidServiceInstanceMapXmlFormat(reason))
		{
			this.reason = reason;
		}

		public InvalidServiceInstanceMapXmlFormatException(string reason, Exception innerException) : base(Strings.InvalidServiceInstanceMapXmlFormat(reason), innerException)
		{
			this.reason = reason;
		}

		protected InvalidServiceInstanceMapXmlFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
