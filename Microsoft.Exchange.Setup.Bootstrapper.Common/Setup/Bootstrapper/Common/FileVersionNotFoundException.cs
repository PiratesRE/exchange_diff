using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileVersionNotFoundException : LocalizedException
	{
		public FileVersionNotFoundException(LocalizedString message) : base(message)
		{
		}

		public FileVersionNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FileVersionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
