using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DNSEntryNotFound : LocalizedException
	{
		public DNSEntryNotFound() : base(Strings.DNSEntryNotFound)
		{
		}

		public DNSEntryNotFound(Exception innerException) : base(Strings.DNSEntryNotFound, innerException)
		{
		}

		protected DNSEntryNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
