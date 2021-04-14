using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADInitializationException : TaskTransientException
	{
		public ADInitializationException(LocalizedString reason) : base(Strings.ADInitializationException(reason))
		{
			this.reason = reason;
		}

		public ADInitializationException(LocalizedString reason, Exception innerException) : base(Strings.ADInitializationException(reason), innerException)
		{
			this.reason = reason;
		}

		protected ADInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (LocalizedString)info.GetValue("reason", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public LocalizedString Reason
		{
			get
			{
				return this.reason;
			}
		}

		private LocalizedString reason;
	}
}
