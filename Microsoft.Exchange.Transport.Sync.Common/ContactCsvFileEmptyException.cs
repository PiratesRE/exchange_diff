using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ContactCsvFileEmptyException : ImportContactsException
	{
		public ContactCsvFileEmptyException() : base(Strings.ContactCsvFileEmpty)
		{
		}

		public ContactCsvFileEmptyException(Exception innerException) : base(Strings.ContactCsvFileEmpty, innerException)
		{
		}

		protected ContactCsvFileEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
