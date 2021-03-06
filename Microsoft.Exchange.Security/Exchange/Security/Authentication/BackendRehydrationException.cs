using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security.Authentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BackendRehydrationException : LocalizedException
	{
		public BackendRehydrationException(LocalizedString reason) : base(SecurityStrings.BackendRehydrationException(reason))
		{
			this.reason = reason;
		}

		public BackendRehydrationException(LocalizedString reason, Exception innerException) : base(SecurityStrings.BackendRehydrationException(reason), innerException)
		{
			this.reason = reason;
		}

		protected BackendRehydrationException(SerializationInfo info, StreamingContext context) : base(info, context)
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

		private readonly LocalizedString reason;
	}
}
