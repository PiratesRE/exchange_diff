using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleIKnowService : IPeopleIKnowService
	{
		public PeopleIKnowService(IPeopleIKnowSerializerFactory serializerFactory, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("serializerFactory", serializerFactory);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.tracer = tracer;
			this.serializer = serializerFactory.CreatePeopleIKnowSerializer();
		}

		public string GetSerializedPeopleIKnowGraph(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			List<RelevantPerson> relevantContacts = this.GetRelevantContacts(mailboxSession, xsoFactory);
			PeopleIKnowGraph peopleIKnowGraph = new PeopleIKnowGraph();
			peopleIKnowGraph.RelevantPeople = relevantContacts.ToArray();
			this.tracer.TraceDebug<int>((long)this.GetHashCode(), "PeopleIKnowService.GetSerializedPeopleIKnowGraph: Added {0} people to the PeopleIKnowGraph", relevantContacts.Count);
			return this.serializer.Serialize(peopleIKnowGraph);
		}

		public IComparer<string> GetRelevancyComparer(string serializedPeopleIKnow)
		{
			PeopleIKnowGraph peopleIKnowGraph = this.serializer.Deserialize(serializedPeopleIKnow);
			return new PersonaComparerByRelevanceScore(peopleIKnowGraph);
		}

		private List<RelevantPerson> GetRelevantContacts(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			ContactsEnumerator<IStorePropertyBag> contactsEnumerator = ContactsEnumerator<IStorePropertyBag>.CreateContactsOnlyEnumerator(mailboxSession, DefaultFolderType.RecipientCache, PeopleIKnowService.RecipientCacheSortColumns, PeopleIKnowService.RecipientCacheContactProperties, (IStorePropertyBag propertyBag) => propertyBag, xsoFactory);
			List<RelevantPerson> list = new List<RelevantPerson>(50);
			foreach (IStorePropertyBag storePropertyBag in contactsEnumerator)
			{
				string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, null);
				if (string.IsNullOrEmpty(valueOrDefault))
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "PeopleIKnowService.GetRelevantContacts: Skipped null or empty emailAddress");
				}
				else
				{
					int valueOrDefault2 = storePropertyBag.GetValueOrDefault<int>(ContactSchema.RelevanceScore, int.MaxValue);
					RelevantPerson relevantPerson = new RelevantPerson();
					relevantPerson.EmailAddress = valueOrDefault;
					relevantPerson.RelevanceScore = valueOrDefault2;
					list.Add(relevantPerson);
					this.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "PeopleIKnowService.GetRelevantContacts: Added person whose emailAddress is {0} and relevanceScore is {1}", relevantPerson.EmailAddress, relevantPerson.RelevanceScore);
					if (list.Count == 200)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "PeopleIKnowService.GetRelevantContacts: Reached max count of people that can be added.");
						break;
					}
				}
			}
			return list;
		}

		private const int MaxRecipientCacheEntriesToRetrieve = 200;

		private static readonly PropertyDefinition[] RecipientCacheContactProperties = new PropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.RelevanceScore
		};

		private static readonly SortBy[] RecipientCacheSortColumns = new SortBy[]
		{
			new SortBy(ContactSchema.RelevanceScore, SortOrder.Ascending)
		};

		private readonly ITracer tracer;

		private readonly IPeopleIKnowSerializer serializer;
	}
}
