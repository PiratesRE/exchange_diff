using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDMemberNameInUseException : WLCDMemberException
	{
		public WLCDMemberNameInUseException(LocalizedString message) : base(message)
		{
		}

		public WLCDMemberNameInUseException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDMemberNameInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
