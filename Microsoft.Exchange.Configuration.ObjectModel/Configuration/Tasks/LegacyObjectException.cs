using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LegacyObjectException : LocalizedException
	{
		public LegacyObjectException(LocalizedString message) : base(message)
		{
		}

		public LegacyObjectException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected LegacyObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
