using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tools
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SupportedToolsDataException : LocalizedException
	{
		public SupportedToolsDataException(LocalizedString message) : base(message)
		{
		}

		public SupportedToolsDataException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SupportedToolsDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
