using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsSharedIdentityInconsistentStateException : LocalizedException
	{
		public RmsSharedIdentityInconsistentStateException(LocalizedString details) : base(Strings.RmsSharedIdentityInconsistentState(details))
		{
			this.details = details;
		}

		public RmsSharedIdentityInconsistentStateException(LocalizedString details, Exception innerException) : base(Strings.RmsSharedIdentityInconsistentState(details), innerException)
		{
			this.details = details;
		}

		protected RmsSharedIdentityInconsistentStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.details = (LocalizedString)info.GetValue("details", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("details", this.details);
		}

		public LocalizedString Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly LocalizedString details;
	}
}
