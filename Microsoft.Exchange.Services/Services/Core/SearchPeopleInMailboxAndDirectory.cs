using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SearchPeopleInMailboxAndDirectory : FindPeopleImplementation
	{
		public SearchPeopleInMailboxAndDirectory(FindPeopleParameters parameters, MailboxSession mailboxSession, OrganizationId organizationId, StoreObjectId folderId, ADObjectId addressListId) : base(parameters, null, false)
		{
			ServiceCommandBase.ThrowIfNull(mailboxSession, "mailboxSession", "SearchPeopleInMailboxAndDirectory::SearchPeopleInMailboxAndDirectory");
			ServiceCommandBase.ThrowIfNull(organizationId, "organizationId", "SearchPeopleInMailboxAndDirectory::SearchPeopleInMailboxAndDirectory");
			ServiceCommandBase.ThrowIfNull(folderId, "folderId", "SearchPeopleInMailboxAndDirectory::SearchPeopleInMailboxAndDirectory");
			ServiceCommandBase.ThrowIfNull(addressListId, "addressListId", "SearchPeopleInMailboxAndDirectory::SearchPeopleInMailboxAndDirectory");
			this.parameters = parameters;
			this.mailboxSession = mailboxSession;
			this.organizationId = organizationId;
			this.folderId = folderId;
			this.addressListId = addressListId;
		}

		public override FindPeopleResult Execute()
		{
			IdAndSession idAndSession = new IdAndSession(this.folderId, this.mailboxSession);
			Dictionary<Guid, IStorePropertyBag> adResults = new Dictionary<Guid, IStorePropertyBag>();
			AggregationExtension aggregationExtension = new SearchPeopleInMailboxAndDirectory.AggregationWithAdQueryResults(this.mailboxSession, adResults);
			SearchPeopleInDirectory searchPeopleInDirectory = new SearchPeopleInDirectory(this.parameters, this.organizationId, this.addressListId, this.mailboxSession, adResults);
			SearchPeopleInMailbox searchPeopleInMailbox = new SearchPeopleInMailbox(this.parameters, idAndSession, aggregationExtension);
			FindPeopleResult findPeopleResult = searchPeopleInDirectory.Execute();
			FindPeopleResult findPeopleResult2 = searchPeopleInMailbox.Execute();
			if (findPeopleResult2.PersonaList.Length == base.MaxRows)
			{
				return findPeopleResult2;
			}
			Persona[] personaList = SearchPeopleInMailboxAndDirectory.Merge(findPeopleResult2.PersonaList, findPeopleResult.PersonaList, base.MaxRows);
			return FindPeopleResult.CreateSearchResult(personaList);
		}

		internal static Persona[] Merge(Persona[] personalSearchResults, Persona[] galSearchResults, int maxRows)
		{
			if (personalSearchResults == null)
			{
				throw new ArgumentNullException("personalSearchResults");
			}
			if (galSearchResults == null)
			{
				throw new ArgumentNullException("galSearchResults");
			}
			if (maxRows <= 0)
			{
				throw new ArgumentException("maxRows had an invalid value: " + maxRows);
			}
			if (personalSearchResults.Length >= maxRows)
			{
				return personalSearchResults;
			}
			if (galSearchResults.Length == 0)
			{
				return personalSearchResults;
			}
			List<Persona> list = new List<Persona>(maxRows);
			HashSet<Guid> hashSet = new HashSet<Guid>();
			foreach (Persona persona in personalSearchResults)
			{
				if (persona.ADObjectId != Guid.Empty)
				{
					hashSet.Add(persona.ADObjectId);
				}
				list.Add(persona);
			}
			foreach (Persona persona2 in galSearchResults)
			{
				if (!hashSet.Contains(persona2.ADObjectId))
				{
					list.Add(persona2);
					if (list.Count >= maxRows)
					{
						break;
					}
				}
			}
			return list.ToArray();
		}

		private readonly FindPeopleParameters parameters;

		private readonly MailboxSession mailboxSession;

		private readonly OrganizationId organizationId;

		private readonly StoreObjectId folderId;

		private readonly ADObjectId addressListId;

		private sealed class AggregationWithAdQueryResults : PeopleAggregationExtension
		{
			public AggregationWithAdQueryResults(MailboxSession mailboxSession, Dictionary<Guid, IStorePropertyBag> adResults) : base(mailboxSession)
			{
				this.adResults = adResults;
			}

			public override void BeforeAggregation(IList<IStorePropertyBag> sources)
			{
				if (this.adResults != null && this.adResults.Count > 0)
				{
					Guid? galId = sources[0].GetValueOrDefault<Guid?>(ContactSchema.GALLinkID, null);
					IStorePropertyBag galContact = null;
					if (galId != null)
					{
						if (this.adResults.TryGetValue(galId.Value, out galContact))
						{
							this.AddContactFromADToSources(sources, galContact, galId);
							return;
						}
						string[] valueOrDefault = sources[0].GetValueOrDefault<string[]>(ContactSchema.SmtpAddressCache, null);
						if (valueOrDefault != null && valueOrDefault.Length > 0)
						{
							foreach (KeyValuePair<Guid, IStorePropertyBag> keyValuePair in this.adResults)
							{
								string valueOrDefault2 = keyValuePair.Value.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, string.Empty);
								if (SearchPeopleInMailboxAndDirectory.AggregationWithAdQueryResults.HasSMTPAddressCacheMatch(valueOrDefault, valueOrDefault2))
								{
									galId = new Guid?(keyValuePair.Key);
									galContact = keyValuePair.Value;
									this.AddContactFromADToSources(sources, galContact, galId);
									break;
								}
							}
						}
					}
				}
			}

			private void AddContactFromADToSources(IList<IStorePropertyBag> sources, IStorePropertyBag galContact, Guid? galId)
			{
				PersonId value = (PersonId)sources[0][ContactSchema.PersonId];
				galContact[ContactSchema.PersonId] = value;
				sources.Add(galContact);
				this.adResults.Remove(galId.Value);
			}

			private static bool HasSMTPAddressCacheMatch(string[] smtpAddressCache, string emailAddress)
			{
				string y = "SMTP:" + emailAddress;
				foreach (string x in smtpAddressCache)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(x, y))
					{
						return true;
					}
				}
				return false;
			}

			private Dictionary<Guid, IStorePropertyBag> adResults;
		}
	}
}
