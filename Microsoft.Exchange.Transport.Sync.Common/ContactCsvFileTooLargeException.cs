using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ContactCsvFileTooLargeException : ImportContactsException
	{
		public ContactCsvFileTooLargeException(int maxContacts) : base(Strings.ContactCsvFileTooLarge(maxContacts))
		{
			this.maxContacts = maxContacts;
		}

		public ContactCsvFileTooLargeException(int maxContacts, Exception innerException) : base(Strings.ContactCsvFileTooLarge(maxContacts), innerException)
		{
			this.maxContacts = maxContacts;
		}

		protected ContactCsvFileTooLargeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maxContacts = (int)info.GetValue("maxContacts", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maxContacts", this.maxContacts);
		}

		public int MaxContacts
		{
			get
			{
				return this.maxContacts;
			}
		}

		private readonly int maxContacts;
	}
}
