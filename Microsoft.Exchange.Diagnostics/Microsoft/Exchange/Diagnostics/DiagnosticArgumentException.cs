using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DiagnosticArgumentException : LocalizedException
	{
		public DiagnosticArgumentException(LocalizedString message) : base(message)
		{
		}

		public DiagnosticArgumentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DiagnosticArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
