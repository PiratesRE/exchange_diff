using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CsvValidationException : LocalizedException
	{
		public CsvValidationException(LocalizedString message) : base(message)
		{
		}

		public CsvValidationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CsvValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
