using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WLCDPasswordIncludesMemberNameException : WLCDMemberException
	{
		public WLCDPasswordIncludesMemberNameException(LocalizedString message) : base(message)
		{
		}

		public WLCDPasswordIncludesMemberNameException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WLCDPasswordIncludesMemberNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
