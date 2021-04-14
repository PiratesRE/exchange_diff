using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PrincipalMapper : LookupTable<MappedPrincipal>
	{
		public PrincipalMapper(IMailbox mailbox)
		{
			this.mailbox = mailbox;
			this.byMailboxGuid = new PrincipalMapper.MailboxGuidIndex();
			this.bySid = new PrincipalMapper.SidIndex();
			this.byX500Proxy = new PrincipalMapper.X500ProxyIndex();
			base.RegisterIndex(this.byMailboxGuid);
			base.RegisterIndex(this.bySid);
			base.RegisterIndex(this.byX500Proxy);
		}

		public PrincipalMapper.X500ProxyIndex ByX500Proxy
		{
			get
			{
				return this.byX500Proxy;
			}
		}

		public PrincipalMapper.MailboxGuidIndex ByMailboxGuid
		{
			get
			{
				return this.byMailboxGuid;
			}
		}

		public PrincipalMapper.SidIndex BySid
		{
			get
			{
				return this.bySid;
			}
		}

		public void AddLegDN(string legDN)
		{
			this.ByX500Proxy.AddKey(legDN);
		}

		public string LookupLegDnByExProxy(string exProxy)
		{
			MappedPrincipal mappedPrincipal = this.ByX500Proxy[exProxy];
			if (mappedPrincipal == null)
			{
				return this.ScrambleLegDN(exProxy);
			}
			return mappedPrincipal.LegacyDN;
		}

		public void AddSid(SecurityIdentifier objectSid)
		{
			this.BySid.AddKey(objectSid);
		}

		public SecurityIdentifier LookupSidByMailboxGuid(Guid mailboxGuid)
		{
			MappedPrincipal mappedPrincipal = this.ByMailboxGuid[mailboxGuid];
			if (mappedPrincipal == null)
			{
				return null;
			}
			return mappedPrincipal.ObjectSid;
		}

		public SecurityIdentifier LookupSidByExProxy(string exProxy)
		{
			MappedPrincipal mappedPrincipal = this.ByX500Proxy[exProxy];
			if (mappedPrincipal == null)
			{
				return null;
			}
			return mappedPrincipal.ObjectSid;
		}

		private string ScrambleLegDN(string legDN)
		{
			return string.Format("{0}_unmapped{1}", legDN, Guid.NewGuid().ToString("N"));
		}

		private IMailbox mailbox;

		private PrincipalMapper.X500ProxyIndex byX500Proxy;

		private PrincipalMapper.MailboxGuidIndex byMailboxGuid;

		private PrincipalMapper.SidIndex bySid;

		public class X500ProxyIndex : LookupIndex<string, MappedPrincipal>
		{
			protected override ICollection<string> RetrieveKeys(MappedPrincipal data)
			{
				HashSet<string> hashSet = new HashSet<string>(this.GetEqualityComparer());
				if (!string.IsNullOrEmpty(data.LegacyDN))
				{
					hashSet.Add(data.LegacyDN);
				}
				if (data.ProxyAddresses != null)
				{
					foreach (string proxyAddressString in data.ProxyAddresses)
					{
						ProxyAddress proxyAddress = ProxyAddress.Parse(proxyAddressString);
						if (proxyAddress != null && proxyAddress.Prefix == ProxyAddressPrefix.X500 && !hashSet.Contains(proxyAddress.AddressString))
						{
							hashSet.Add(proxyAddress.AddressString);
						}
					}
				}
				return hashSet;
			}

			protected override MappedPrincipal[] LookupKeys(string[] keys)
			{
				MappedPrincipal[] array = new MappedPrincipal[keys.Length];
				for (int i = 0; i < keys.Length; i++)
				{
					array[i] = new MappedPrincipal();
					array[i].LegacyDN = keys[i];
				}
				return ((PrincipalMapper)base.Owner).mailbox.ResolvePrincipals(array);
			}

			protected override IEqualityComparer<string> GetEqualityComparer()
			{
				return StringComparer.OrdinalIgnoreCase;
			}
		}

		public class MailboxGuidIndex : LookupIndex<Guid, MappedPrincipal>
		{
			protected override ICollection<Guid> RetrieveKeys(MappedPrincipal data)
			{
				if (data.MailboxGuid != Guid.Empty)
				{
					return new Guid[]
					{
						data.MailboxGuid
					};
				}
				return null;
			}

			protected override MappedPrincipal[] LookupKeys(Guid[] keys)
			{
				MappedPrincipal[] array = new MappedPrincipal[keys.Length];
				for (int i = 0; i < keys.Length; i++)
				{
					array[i] = new MappedPrincipal();
					array[i].MailboxGuid = keys[i];
				}
				return ((PrincipalMapper)base.Owner).mailbox.ResolvePrincipals(array);
			}
		}

		public class SidIndex : LookupIndex<SecurityIdentifier, MappedPrincipal>
		{
			protected override ICollection<SecurityIdentifier> RetrieveKeys(MappedPrincipal data)
			{
				if (data.ObjectSid != null)
				{
					return new SecurityIdentifier[]
					{
						data.ObjectSid
					};
				}
				return null;
			}

			protected override MappedPrincipal[] LookupKeys(SecurityIdentifier[] keys)
			{
				MappedPrincipal[] array = new MappedPrincipal[keys.Length];
				for (int i = 0; i < keys.Length; i++)
				{
					array[i] = new MappedPrincipal();
					array[i].ObjectSid = keys[i];
				}
				return ((PrincipalMapper)base.Owner).mailbox.ResolvePrincipals(array);
			}
		}
	}
}
