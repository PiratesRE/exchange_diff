using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeopleIKnowEmailAddressCollectionFolderProperty : IPeopleIKnowPublisher
	{
		public PeopleIKnowEmailAddressCollectionFolderProperty(IXSOFactory xsoFactory, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.xsoFactory = xsoFactory;
			this.tracer = tracer;
			this.traceId = traceId;
		}

		public void Publish(IMailboxSession session)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			try
			{
				IDictionary<string, PeopleIKnowMetadata> systemFavorites = this.GetSystemFavorites(session);
				if (systemFavorites.Count > 0)
				{
					PeopleIKnowEmailAddressCollection peopleIKnowEmailAddressCollection = PeopleIKnowEmailAddressCollection.CreateFromStringCollection(systemFavorites, this.tracer, this.traceId, 1);
					byte[] data = peopleIKnowEmailAddressCollection.Data;
					PeopleIKnowEmailAddressCollection peopleIKnowEmailAddressCollection2 = PeopleIKnowEmailAddressCollection.CreateFromStringCollection(systemFavorites, this.tracer, this.traceId, 2);
					byte[] data2 = peopleIKnowEmailAddressCollection2.Data;
					using (IFolder folder = this.xsoFactory.BindToFolder(session, DefaultFolderType.Inbox, PeopleIKnowEmailAddressCollectionFolderProperty.PeopleIKnowEmailAddressCollectionPropertyArray))
					{
						folder[FolderSchema.PeopleIKnowEmailAddressCollection] = data;
						folder[FolderSchema.PeopleIKnowEmailAddressRelevanceScoreCollection] = data2;
						folder.Save();
						goto IL_DA;
					}
				}
				using (IFolder folder2 = this.xsoFactory.BindToFolder(session, DefaultFolderType.Inbox, PeopleIKnowEmailAddressCollectionFolderProperty.PeopleIKnowEmailAddressCollectionPropertyArray))
				{
					folder2.Delete(FolderSchema.PeopleIKnowEmailAddressCollection);
					folder2.Delete(FolderSchema.PeopleIKnowEmailAddressRelevanceScoreCollection);
					folder2.Save();
				}
				IL_DA:;
			}
			catch (ObjectNotFoundException arg)
			{
				this.tracer.TraceDebug<IMailboxSession, ObjectNotFoundException>((long)this.GetHashCode(), "People I Know email addresses container has not been initialized or has been deleted for mailbox '{0}'.  Exception: {1}", session, arg);
			}
		}

		private IDictionary<string, PeopleIKnowMetadata> GetSystemFavorites(IMailboxSession session)
		{
			Dictionary<string, PeopleIKnowMetadata> dictionary = new Dictionary<string, PeopleIKnowMetadata>();
			using (IFolder folder = this.xsoFactory.BindToFolder(session, DefaultFolderType.RecipientCache))
			{
				using (IQueryResult queryResult = folder.PersonItemQuery(PeopleIKnowEmailAddressCollectionFolderProperty.EmptyFilter, PeopleIKnowEmailAddressCollectionFolderProperty.EmptyFilter, PeopleIKnowEmailAddressCollectionFolderProperty.AnySort, PeopleIKnowEmailAddressCollectionFolderProperty.RecipientCacheProperties))
				{
					IStorePropertyBag[] propertyBags;
					do
					{
						propertyBags = queryResult.GetPropertyBags(10000);
						if (propertyBags != null && propertyBags.Length > 0)
						{
							foreach (IStorePropertyBag storePropertyBag in propertyBags)
							{
								PersonType valueOrDefault = storePropertyBag.GetValueOrDefault<PersonType>(PersonSchema.PersonType, PersonType.Unknown);
								if (valueOrDefault == PersonType.Person)
								{
									Participant[] valueOrDefault2 = storePropertyBag.GetValueOrDefault<Participant[]>(PersonSchema.EmailAddresses, null);
									int valueOrDefault3 = storePropertyBag.GetValueOrDefault<int>(PersonSchema.RelevanceScore, int.MaxValue);
									if (valueOrDefault2 != null && valueOrDefault2.Length > 0)
									{
										foreach (Participant participant in valueOrDefault2)
										{
											if (!string.IsNullOrEmpty(participant.EmailAddress) && !this.DoesEmailAddressMatchMailboxOwner(participant.EmailAddress, session.MailboxOwner))
											{
												dictionary[participant.EmailAddress] = new PeopleIKnowMetadata
												{
													RelevanceScore = valueOrDefault3
												};
											}
										}
									}
								}
							}
						}
					}
					while (propertyBags.Length > 0);
				}
			}
			return dictionary;
		}

		private bool DoesEmailAddressMatchMailboxOwner(string emailAddress, IExchangePrincipal exchangePrincipal)
		{
			foreach (string value in exchangePrincipal.GetAllEmailAddresses())
			{
				if (emailAddress.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly QueryFilter EmptyFilter = null;

		private static readonly SortBy[] AnySort = null;

		private static readonly PropertyDefinition[] PeopleIKnowEmailAddressCollectionPropertyArray = new PropertyDefinition[]
		{
			FolderSchema.PeopleIKnowEmailAddressCollection,
			FolderSchema.PeopleIKnowEmailAddressRelevanceScoreCollection
		};

		private static readonly PropertyDefinition[] RecipientCacheProperties = new PropertyDefinition[]
		{
			PersonSchema.PersonType,
			PersonSchema.EmailAddresses,
			PersonSchema.RelevanceScore
		};

		private readonly ITracer tracer;

		private readonly int traceId;

		private readonly IXSOFactory xsoFactory;
	}
}
