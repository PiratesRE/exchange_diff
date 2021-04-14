using System;
using System.Globalization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search
{
	internal static class SearchStoreHelper
	{
		internal static void CreateMessage(MailboxSession session, Folder folder, string subject)
		{
			using (MessageItem messageItem = MessageItem.Create(session, folder.Id))
			{
				messageItem.Subject = subject;
				ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus != SaveResult.Success && conflictResolutionResult.SaveStatus != SaveResult.SuccessWithConflictResolution)
				{
					throw new LocalizedException(Strings.SearchFailToSaveMessage);
				}
			}
		}

		internal static VersionedId GetMessageBySubject(Folder folder, string subject, out ExDateTime creationTime)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Subject, subject);
			return SearchStoreHelper.GetMessageByComparisonFilter(folder, filter, out creationTime);
		}

		internal static VersionedId GetMessageByInternetMessageId(Folder folder, string internetMessageId, out ExDateTime creationTime)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InternetMessageId, internetMessageId);
			return SearchStoreHelper.GetMessageByComparisonFilter(folder, filter, out creationTime);
		}

		internal static VersionedId GetMessageByComparisonFilter(Folder folder, ComparisonFilter filter, out ExDateTime creationTime)
		{
			creationTime = ExDateTime.MinValue;
			SortBy[] sortColumns = new SortBy[]
			{
				new SortBy(StoreObjectSchema.CreationTime, SortOrder.Descending)
			};
			VersionedId result;
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortColumns, new PropertyDefinition[]
			{
				ItemSchema.Id,
				StoreObjectSchema.CreationTime
			}))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, filter))
				{
					object[][] rows = queryResult.GetRows(1);
					if (rows.Length == 0)
					{
						result = null;
					}
					else
					{
						VersionedId versionedId = (VersionedId)rows[0][0];
						creationTime = (ExDateTime)rows[0][1];
						result = versionedId;
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal static int GetQueryHitCount(MailboxSession session, string query, int maxResultsCount)
		{
			SearchStoreHelper.BuildQueryFilter(query, CultureInfo.CurrentCulture);
			return SearchStoreHelper.GetQueryHitCount(session, query, maxResultsCount);
		}

		internal static int GetQueryHitCount(MailboxSession session, QueryFilter query, int maxResultsCount)
		{
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(query, new StoreObjectId[]
			{
				session.GetDefaultFolderId(DefaultFolderType.Inbox)
			});
			searchFolderCriteria.MaximumResultsCount = new int?(maxResultsCount);
			int result;
			using (SearchFolder searchFolder = SearchFolder.Create(session, session.GetDefaultFolderId(DefaultFolderType.SearchFolders), "SearchQueryStx" + DateTime.UtcNow.Ticks, CreateMode.InstantSearch))
			{
				searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
				searchFolder.Save();
				searchFolder.Load();
				VersionedId id = searchFolder.Id;
				IAsyncResult asyncResult = searchFolder.BeginApplyOneTimeSearch(searchFolderCriteria, null, null);
				if (!asyncResult.AsyncWaitHandle.WaitOne(180000))
				{
					throw new TimeoutException();
				}
				searchFolder.EndApplyOneTimeSearch(asyncResult);
				using (QueryResult queryResult = searchFolder.ItemQuery(ItemQueryType.None, null, new SortBy[]
				{
					new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
				}, new PropertyDefinition[]
				{
					ItemSchema.Id
				}))
				{
					object[][] rows = queryResult.GetRows(queryResult.EstimatedRowCount);
					result = rows.Length;
				}
			}
			return result;
		}

		internal static MailboxSession GetMailboxSession(string smtpAddress, bool allowCrossSiteServer = false, string client = "Monitoring")
		{
			if (!SmtpAddress.IsValidSmtpAddress(smtpAddress))
			{
				throw new ArgumentException("smtpAddress");
			}
			string domain = smtpAddress.Split(new char[]
			{
				'@'
			})[1];
			ADSessionSettings adSettings;
			if (Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.ExchangeDatacenter)
			{
				adSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
			}
			else
			{
				adSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			ExchangePrincipal mailboxOwner;
			if (allowCrossSiteServer)
			{
				mailboxOwner = ExchangePrincipal.FromProxyAddress(adSettings, smtpAddress, RemotingOptions.AllowCrossSite);
			}
			else
			{
				mailboxOwner = ExchangePrincipal.FromProxyAddress(adSettings, smtpAddress);
			}
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, string.Format("Client={0};Action=Monitoring Search", client));
		}

		internal static Folder GetInboxFolder(MailboxSession session)
		{
			return Folder.Bind(session, DefaultFolderType.Inbox);
		}

		private static QueryFilter BuildQueryFilter(string aqsQuery, CultureInfo culture)
		{
			QueryFilter queryFilter = AqsParser.ParseAndBuildQuery(aqsQuery, AqsParser.ParseOption.None, culture, null, null);
			OrFilter orFilter = new OrFilter(new QueryFilter[]
			{
				new TextFilter(StoreObjectSchema.ItemClass, "IPM.Note", MatchOptions.PrefixOnWords, MatchFlags.Loose),
				new TextFilter(StoreObjectSchema.ItemClass, "IPM.Schedule.Meeting", MatchOptions.PrefixOnWords, MatchFlags.Loose),
				new TextFilter(StoreObjectSchema.ItemClass, "IPM.OCTEL.VOICE", MatchOptions.PrefixOnWords, MatchFlags.Loose),
				new TextFilter(StoreObjectSchema.ItemClass, "IPM.VOICENOTES", MatchOptions.PrefixOnWords, MatchFlags.Loose)
			});
			return new AndFilter(new QueryFilter[]
			{
				queryFilter,
				orFilter
			});
		}

		internal const int SearchTimeOutSeconds = 180;
	}
}
