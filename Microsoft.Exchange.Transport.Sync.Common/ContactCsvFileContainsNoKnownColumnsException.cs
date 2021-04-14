using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ContactCsvFileContainsNoKnownColumnsException : ImportContactsException
	{
		public ContactCsvFileContainsNoKnownColumnsException() : base(Strings.ContactCsvFileContainsNoKnownColumns)
		{
		}

		public ContactCsvFileContainsNoKnownColumnsException(Exception innerException) : base(Strings.ContactCsvFileContainsNoKnownColumns, innerException)
		{
		}

		protected ContactCsvFileContainsNoKnownColumnsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
