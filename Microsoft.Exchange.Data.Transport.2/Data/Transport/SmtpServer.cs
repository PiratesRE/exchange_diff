using System;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class SmtpServer : ICloneableInternal
	{
		internal SmtpServer()
		{
		}

		public abstract string Name { get; }

		public abstract Version Version { get; }

		public abstract IPPermission IPPermission { get; }

		public abstract AddressBook AddressBook { get; }

		public abstract AcceptedDomainCollection AcceptedDomains { get; }

		public abstract RemoteDomainCollection RemoteDomains { get; }

		internal Agent AssociatedAgent
		{
			get
			{
				return this.associatedAgent;
			}
			set
			{
				this.associatedAgent = value;
			}
		}

		object ICloneableInternal.Clone()
		{
			return base.MemberwiseClone();
		}

		public abstract void SubmitMessage(EmailMessage message);

		private Agent associatedAgent;
	}
}
