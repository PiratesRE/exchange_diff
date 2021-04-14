using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxJunkEmailConfigurationDataProvider : XsoMailboxDataProviderBase
	{
		private IRecipientSession Session { get; set; }

		public MailboxJunkEmailConfigurationDataProvider(ExchangePrincipal mailboxOwner, IRecipientSession session, string action) : base(mailboxOwner, action)
		{
			this.Session = session;
		}

		public MailboxJunkEmailConfigurationDataProvider(ExchangePrincipal mailboxOwner, string action) : base(mailboxOwner, action)
		{
		}

		public MailboxJunkEmailConfigurationDataProvider(MailboxSession session) : base(session)
		{
		}

		internal MailboxJunkEmailConfigurationDataProvider()
		{
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			this.TestArguments<T>(filter, rootId);
			JunkEmailRule rule = base.MailboxSession.JunkEmailRule;
			MailboxJunkEmailConfiguration configuration = (MailboxJunkEmailConfiguration)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			configuration.MailboxOwnerId = base.MailboxSession.MailboxOwner.ObjectId;
			configuration.Enabled = rule.IsEnabled;
			configuration.ContactsTrusted = rule.IsContactsFolderTrusted;
			configuration.TrustedListsOnly = rule.TrustedListsOnly;
			configuration.TrustedSendersAndDomains = this.CompileTrusted(rule);
			configuration.BlockedSendersAndDomains = this.CompileBlocked(rule);
			yield return (T)((object)configuration);
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			if (this.Session == null)
			{
				throw new DataSourceOperationException(ServerStrings.JunkEmailInvalidConstructionException);
			}
			this.TestForJunkEmailFolder();
			MailboxJunkEmailConfiguration o = (MailboxJunkEmailConfiguration)instance;
			JunkEmailRule junkEmailRule = base.MailboxSession.JunkEmailRule;
			this.PrepareJunkEmailRule(o, junkEmailRule);
			this.SaveRule(junkEmailRule);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxJunkEmailConfigurationDataProvider>(this);
		}

		private void TestForJunkEmailFolder()
		{
			try
			{
				if (base.MailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail) == null)
				{
					throw new DataSourceOperationException(ServerStrings.JunkEmailFolderNotFoundException);
				}
			}
			catch (ObjectNotFoundException)
			{
				throw new DataSourceOperationException(ServerStrings.JunkEmailFolderNotFoundException);
			}
		}

		private void TestArguments<T>(QueryFilter filter, ObjectId rootId)
		{
			if (filter != null && !(filter is FalseFilter))
			{
				throw new NotSupportedException("filter");
			}
			if (rootId != null && rootId is ADObjectId && !ADObjectId.Equals((ADObjectId)rootId, base.MailboxSession.MailboxOwner.ObjectId))
			{
				throw new NotSupportedException("rootId");
			}
			if (!typeof(MailboxJunkEmailConfiguration).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
		}

		private string[] CompileTrusted(JunkEmailRule rule)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			hashSet.UnionWith(rule.TrustedSenderEmailCollection);
			hashSet.UnionWith(rule.TrustedSenderDomainCollection);
			hashSet.UnionWith(rule.TrustedRecipientEmailCollection);
			hashSet.UnionWith(rule.TrustedRecipientDomainCollection);
			return Array.ConvertAll<string, string>(hashSet.ToArray<string>(), (string s) => s.TrimStart(new char[]
			{
				'@'
			}));
		}

		private string[] CompileBlocked(JunkEmailRule rule)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			hashSet.UnionWith(rule.BlockedSenderEmailCollection);
			hashSet.UnionWith(rule.BlockedSenderDomainCollection);
			return Array.ConvertAll<string, string>(hashSet.ToArray<string>(), (string s) => s.TrimStart(new char[]
			{
				'@'
			}));
		}

		private void SaveRule(JunkEmailRule rule)
		{
			try
			{
				rule.Save();
			}
			catch (ObjectDisposedException)
			{
				throw new DataSourceTransientException(ServerStrings.JunkEmailObjectDisposedException);
			}
			catch (InvalidOperationException)
			{
				throw new DataSourceTransientException(ServerStrings.JunkEmailInvalidOperationException);
			}
		}

		private void PrepareJunkEmailRule(MailboxJunkEmailConfiguration o, JunkEmailRule rule)
		{
			rule.IsEnabled = o.Enabled;
			rule.TrustedListsOnly = o.TrustedListsOnly;
			if (o.ContactsTrusted)
			{
				rule.SynchronizeContactsCache();
			}
			else
			{
				rule.ClearContactsCache();
			}
			this.SynchronizeTrustedLists(rule);
			this.SetBlockedList(o, rule);
			this.SetTrustedList(o, rule);
		}

		private void SynchronizeTrustedLists(JunkEmailRule rule)
		{
			HashSet<string> hashSet = new HashSet<string>(rule.TrustedRecipientEmailCollection, StringComparer.OrdinalIgnoreCase);
			hashSet.UnionWith(rule.TrustedSenderEmailCollection);
			this.SetDelta(hashSet.ToArray<string>(), rule.TrustedRecipientEmailCollection, new MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy(this.SetListWithoutValidation));
			this.SetDelta(hashSet.ToArray<string>(), rule.TrustedSenderEmailCollection, new MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy(this.SetListWithoutValidation));
			HashSet<string> hashSet2 = new HashSet<string>(rule.TrustedRecipientDomainCollection, StringComparer.OrdinalIgnoreCase);
			hashSet2.UnionWith(rule.TrustedSenderDomainCollection);
			this.SetDelta(hashSet2.ToArray<string>(), rule.TrustedRecipientDomainCollection, new MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy(this.SetListWithoutValidation));
			this.SetDelta(hashSet2.ToArray<string>(), rule.TrustedSenderDomainCollection, new MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy(this.SetListWithoutValidation));
		}

		private void SetBlockedList(MailboxJunkEmailConfiguration o, JunkEmailRule rule)
		{
			MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple junkEmailValidationTuple = this.SetList(o.BlockedSendersAndDomains, rule.BlockedSenderEmailCollection, rule.BlockedSenderDomainCollection);
			LocalizedString localizedString = LocalizedString.Empty;
			switch (junkEmailValidationTuple.Problem)
			{
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsUsersEmailOrDomain:
				localizedString = ServerStrings.JunkEmailBlockedListOwnersEmailAddressException(junkEmailValidationTuple.Address);
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsInternalToOrganization:
				localizedString = ServerStrings.JunkEmailBlockedListInternalToOrganizationException(junkEmailValidationTuple.Address);
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsDuplicate:
				localizedString = ServerStrings.JunkEmailBlockedListXsoDuplicateException(junkEmailValidationTuple.Address);
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsEmpty:
				localizedString = ServerStrings.JunkEmailBlockedListXsoEmptyException;
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsMalformatted:
				localizedString = ServerStrings.JunkEmailBlockedListXsoFormatException(junkEmailValidationTuple.Address);
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsNull:
				localizedString = ServerStrings.JunkEmailBlockedListXsoNullException;
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsTooBig:
				localizedString = ServerStrings.JunkEmailBlockedListXsoTooBigException(junkEmailValidationTuple.Address);
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsFull:
				localizedString = ServerStrings.JunkEmailBlockedListXsoTooManyException;
				goto IL_C7;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood:
				goto IL_C7;
			}
			localizedString = ServerStrings.JunkEmailBlockedListXsoGenericException(junkEmailValidationTuple.Address);
			IL_C7:
			if (localizedString != LocalizedString.Empty)
			{
				PropertyValidationError propertyValidationError = new PropertyValidationError(localizedString, MailboxJunkEmailConfigurationSchema.BlockedSendersAndDomains, o.BlockedSendersAndDomains);
				throw new PropertyValidationException(localizedString.ToString(), propertyValidationError.PropertyDefinition, new PropertyValidationError[]
				{
					propertyValidationError
				});
			}
		}

		private void SetTrustedList(MailboxJunkEmailConfiguration o, JunkEmailRule rule)
		{
			MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple junkEmailValidationTuple = this.SetList(o.TrustedSendersAndDomains, rule.TrustedSenderEmailCollection, rule.TrustedSenderDomainCollection);
			if (junkEmailValidationTuple.Problem == MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood)
			{
				junkEmailValidationTuple = this.SetList(o.TrustedSendersAndDomains, rule.TrustedRecipientEmailCollection, rule.TrustedRecipientDomainCollection);
			}
			LocalizedString localizedString = LocalizedString.Empty;
			switch (junkEmailValidationTuple.Problem)
			{
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsUsersEmailOrDomain:
				localizedString = ServerStrings.JunkEmailTrustedListOwnersEmailAddressException(junkEmailValidationTuple.Address);
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsInternalToOrganization:
				localizedString = ServerStrings.JunkEmailTrustedListInternalToOrganizationException(junkEmailValidationTuple.Address);
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsDuplicate:
				localizedString = ServerStrings.JunkEmailTrustedListXsoDuplicateException(junkEmailValidationTuple.Address);
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsEmpty:
				localizedString = ServerStrings.JunkEmailTrustedListXsoEmptyException;
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsMalformatted:
				localizedString = ServerStrings.JunkEmailTrustedListXsoFormatException(junkEmailValidationTuple.Address);
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsNull:
				localizedString = ServerStrings.JunkEmailTrustedListXsoNullException;
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsTooBig:
				localizedString = ServerStrings.JunkEmailTrustedListXsoTooBigException(junkEmailValidationTuple.Address);
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsFull:
				localizedString = ServerStrings.JunkEmailTrustedListXsoTooManyException;
				goto IL_EB;
			case MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood:
				goto IL_EB;
			}
			localizedString = ServerStrings.JunkEmailTrustedListXsoGenericException(junkEmailValidationTuple.Address);
			IL_EB:
			if (localizedString != LocalizedString.Empty)
			{
				PropertyValidationError propertyValidationError = new PropertyValidationError(localizedString, MailboxJunkEmailConfigurationSchema.TrustedSendersAndDomains, o.TrustedSendersAndDomains);
				throw new PropertyValidationException(localizedString.ToString(), propertyValidationError.PropertyDefinition, new PropertyValidationError[]
				{
					propertyValidationError
				});
			}
		}

		private MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple SetList(MultiValuedProperty<string> addresses, JunkEmailCollection junkEmails, JunkEmailCollection junkDomains)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string[] array = this.CullEmailsAndDomains(addresses, list, list2);
			foreach (string text in array)
			{
				if (junkEmails.Contains(text))
				{
					list.Add(text);
				}
				else
				{
					if (!junkDomains.Contains(text))
					{
						return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsMalformatted, text);
					}
					list2.Add(text);
				}
			}
			MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple result = this.SetDelta(list.ToArray(), junkEmails, new MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy(this.SetEmailsList));
			if (result.Problem != MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood)
			{
				return result;
			}
			return this.SetDelta(list2.ToArray(), junkDomains, new MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy(this.SetDomainsList));
		}

		private MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple SetDelta(string[] addresses, JunkEmailCollection junk, MailboxJunkEmailConfigurationDataProvider.JunkEmailAdditionStrategy addToRule)
		{
			HashSet<string> hashSet = new HashSet<string>(addresses, StringComparer.OrdinalIgnoreCase);
			hashSet.UnionWith(junk);
			ICollection<string> collection = this.Subtract(hashSet, addresses);
			ICollection<string> collection2 = this.Subtract(hashSet, junk);
			foreach (string item in collection)
			{
				junk.Remove(item);
			}
			if (collection2.Count == 0)
			{
				return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood, string.Empty);
			}
			return addToRule(collection2, junk);
		}

		private MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple SetEmailsList(ICollection<string> emails, JunkEmailCollection junk)
		{
			new List<MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple>();
			foreach (string text in emails)
			{
				if (this.IsUsersEmailOrDomain(text, false))
				{
					return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsUsersEmailOrDomain, text);
				}
				if (this.IsInternalToOrganization(text))
				{
					return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsInternalToOrganization, text);
				}
			}
			try
			{
				junk.AddRange(emails.ToArray<string>());
			}
			catch (JunkEmailValidationException ex)
			{
				return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(ex.Problem, (string)ex.StringFormatParameters[0]);
			}
			return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood, string.Empty);
		}

		private MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple SetDomainsList(ICollection<string> domains, JunkEmailCollection junk)
		{
			foreach (string text in domains)
			{
				if (this.IsUsersEmailOrDomain(text, true))
				{
					return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsUsersEmailOrDomain, text);
				}
			}
			try
			{
				junk.AddRange(domains.ToArray<string>());
			}
			catch (JunkEmailValidationException ex)
			{
				return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(ex.Problem, (string)ex.StringFormatParameters[0]);
			}
			return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood, string.Empty);
		}

		private MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple SetListWithoutValidation(ICollection<string> addresses, JunkEmailCollection junk)
		{
			bool validating = junk.Validating;
			junk.Validating = false;
			junk.AddRange(addresses.ToArray<string>());
			junk.Validating = validating;
			return new MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood, string.Empty);
		}

		private string[] CullEmailsAndDomains(ICollection<string> addresses, List<string> emails, List<string> domains)
		{
			List<string> list = new List<string>();
			foreach (string text in addresses)
			{
				string item = null;
				if (this.TryParseEmail(text, out item))
				{
					emails.Add(item);
				}
				else if (this.TryParseDomain(text, out item))
				{
					domains.Add(item);
				}
				else
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		private bool TryParseEmail(string address, out string result)
		{
			if (string.IsNullOrEmpty(address))
			{
				result = null;
				return false;
			}
			if (!RoutingAddress.IsValidAddress(address))
			{
				result = null;
				return false;
			}
			result = RoutingAddress.Parse(address).ToString();
			return true;
		}

		private bool TryParseDomain(string address, out string result)
		{
			if (string.IsNullOrEmpty(address))
			{
				result = null;
				return false;
			}
			switch (address.IndexOf('@'))
			{
			case -1:
				if (SmtpAddress.IsValidDomain(address))
				{
					result = "@" + address;
					return true;
				}
				result = null;
				return false;
			case 0:
				if (SmtpAddress.IsValidDomain(address.Substring(1)))
				{
					result = address;
					return true;
				}
				result = null;
				return false;
			default:
				result = null;
				return false;
			}
		}

		private bool IsUsersEmailOrDomain(string email, bool isDomain)
		{
			ADRecipient adrecipient = this.Session.Read(base.MailboxSession.MailboxOwner.ObjectId);
			if (adrecipient == null)
			{
				return false;
			}
			foreach (ProxyAddress proxyAddress in adrecipient.EmailAddresses)
			{
				if (proxyAddress != null && proxyAddress.Prefix == ProxyAddressPrefix.Smtp && SmtpAddress.IsValidSmtpAddress(proxyAddress.AddressString))
				{
					SmtpAddress smtpAddress = (SmtpAddress)((SmtpProxyAddress)proxyAddress);
					string b = isDomain ? smtpAddress.Domain.ToString() : smtpAddress.ToString();
					string a = isDomain ? email.Substring(1) : email;
					if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsInternalToOrganization(string email)
		{
			return this.Session.IsRecipientInOrg(ProxyAddress.Parse(email));
		}

		private ICollection<string> Subtract(HashSet<string> subject, ICollection<string> target)
		{
			HashSet<string> hashSet = new HashSet<string>(subject, StringComparer.OrdinalIgnoreCase);
			foreach (string item in target)
			{
				hashSet.Remove(item);
			}
			return hashSet;
		}

		private enum JunkEmailValidationProblem
		{
			IsUsersEmailOrDomain,
			IsInternalToOrganization,
			IsDuplicate,
			IsEmpty,
			IsMalformatted,
			IsNull,
			IsTooBig,
			IsFull,
			IsInvalid,
			IsGood
		}

		private delegate MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationTuple JunkEmailAdditionStrategy(ICollection<string> addresses, JunkEmailCollection junk);

		private struct JunkEmailValidationTuple
		{
			public JunkEmailValidationTuple(MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem problem, string address)
			{
				this.Problem = problem;
				this.Address = address;
			}

			public JunkEmailValidationTuple(JunkEmailCollection.ValidationProblem problem, string address)
			{
				this.Address = address;
				switch (problem)
				{
				case JunkEmailCollection.ValidationProblem.Null:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsNull;
					return;
				case JunkEmailCollection.ValidationProblem.Duplicate:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsDuplicate;
					return;
				case JunkEmailCollection.ValidationProblem.FormatError:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsMalformatted;
					return;
				case JunkEmailCollection.ValidationProblem.Empty:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsEmpty;
					return;
				case JunkEmailCollection.ValidationProblem.TooBig:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsTooBig;
					return;
				case JunkEmailCollection.ValidationProblem.TooManyEntries:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsFull;
					return;
				case JunkEmailCollection.ValidationProblem.EntryInInvalidEntriesList:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsInvalid;
					return;
				default:
					this.Problem = MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem.IsGood;
					return;
				}
			}

			public readonly MailboxJunkEmailConfigurationDataProvider.JunkEmailValidationProblem Problem;

			public readonly string Address;
		}
	}
}
