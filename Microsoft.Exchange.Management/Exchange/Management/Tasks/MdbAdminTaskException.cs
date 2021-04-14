using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MdbAdminTaskException : LocalizedException
	{
		public MdbAdminTaskException(LocalizedString message) : base(message)
		{
		}

		public MdbAdminTaskException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MdbAdminTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
