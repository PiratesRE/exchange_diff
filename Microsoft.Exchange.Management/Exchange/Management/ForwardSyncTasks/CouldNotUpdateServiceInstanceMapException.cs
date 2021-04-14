using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotUpdateServiceInstanceMapException : LocalizedException
	{
		public CouldNotUpdateServiceInstanceMapException(string reason) : base(Strings.CouldNotUpdateServiceInstanceMap(reason))
		{
			this.reason = reason;
		}

		public CouldNotUpdateServiceInstanceMapException(string reason, Exception innerException) : base(Strings.CouldNotUpdateServiceInstanceMap(reason), innerException)
		{
			this.reason = reason;
		}

		protected CouldNotUpdateServiceInstanceMapException(SerializationInfo info, StreamingContext context) : base(info, context)
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
