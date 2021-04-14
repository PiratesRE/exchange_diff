using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class SendAddressIdParameter : IIdentityParameter
	{
		public SendAddressIdParameter()
		{
			this.sendAddressIdentity = new SendAddressIdentity();
		}

		public SendAddressIdParameter(SendAddress sendAddress)
		{
			this.Initialize(sendAddress.Identity);
		}

		public SendAddressIdParameter(string stringIdentity)
		{
			this.ThrowIfArgumentNullOrEmpty("stringIdentity", stringIdentity);
			this.Parse(stringIdentity);
			this.mailboxIdParameter = this.FromSendAddressIdentity();
		}

		internal SendAddressIdParameter(SendAddressIdentity sendAddressIdentity)
		{
			this.Initialize(sendAddressIdentity);
		}

		public string RawIdentity
		{
			get
			{
				return this.ToString();
			}
		}

		public bool IsUniqueIdentity
		{
			get
			{
				return this.sendAddressIdentity.IsUniqueIdentity;
			}
		}

		public MailboxIdParameter MailboxIdParameter
		{
			get
			{
				return this.mailboxIdParameter;
			}
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			if (this.IsUniqueIdentity)
			{
				return new List<T>(1)
				{
					(T)((object)session.Read<T>(this.sendAddressIdentity))
				};
			}
			return session.FindPaged<T>(null, rootId, false, null, 0);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public void Initialize(ObjectId objectId)
		{
			this.ThrowIfArgumentNull("objectId", objectId);
			SendAddressIdentity sendAddressIdentity = objectId as SendAddressIdentity;
			if (sendAddressIdentity == null)
			{
				string message = string.Format("objectId is the wrong type: {0} expected: {1}", objectId.GetType().Name, typeof(SendAddressIdentity).Name, CultureInfo.InvariantCulture);
				throw new ArgumentException(message, "objectId");
			}
			this.sendAddressIdentity = sendAddressIdentity;
			this.mailboxIdParameter = this.FromSendAddressIdentity();
		}

		public override string ToString()
		{
			if (this.sendAddressIdentity != null)
			{
				return this.sendAddressIdentity.ToString();
			}
			return string.Empty;
		}

		private void Parse(string stringIdentity)
		{
			string text = stringIdentity.Trim();
			if (text.Length == 0)
			{
				throw new ArgumentException("stringIdentity contained only spaces", "stringIdentity");
			}
			this.sendAddressIdentity = new SendAddressIdentity(text);
		}

		private MailboxIdParameter FromSendAddressIdentity()
		{
			return new MailboxIdParameter(this.sendAddressIdentity.MailboxIdParameterString);
		}

		private void ThrowIfArgumentNull(string name, object argument)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		private void ThrowIfArgumentNullOrEmpty(string name, string argument)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);
			}
			if (argument.Length == 0)
			{
				throw new ArgumentException("The value is set to empty", name);
			}
		}

		private SendAddressIdentity sendAddressIdentity;

		private MailboxIdParameter mailboxIdParameter;
	}
}
