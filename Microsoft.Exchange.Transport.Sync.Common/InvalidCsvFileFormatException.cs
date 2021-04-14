using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidCsvFileFormatException : ImportContactsException
	{
		public InvalidCsvFileFormatException() : base(Strings.InvalidCsvFileFormat)
		{
		}

		public InvalidCsvFileFormatException(Exception innerException) : base(Strings.InvalidCsvFileFormat, innerException)
		{
		}

		protected InvalidCsvFileFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
