using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoUserOrOrganiztionProvidedException : LocalizedException
	{
		public NoUserOrOrganiztionProvidedException() : base(Strings.NoUserOrOrganiztionProvidedException)
		{
		}

		public NoUserOrOrganiztionProvidedException(Exception innerException) : base(Strings.NoUserOrOrganiztionProvidedException, innerException)
		{
		}

		protected NoUserOrOrganiztionProvidedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
