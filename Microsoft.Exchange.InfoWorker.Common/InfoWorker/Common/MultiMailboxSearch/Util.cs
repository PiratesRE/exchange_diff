using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal static class Util
	{
		internal static string AppVersion
		{
			get
			{
				if (Util.appVersion == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						Version version;
						if (ExWatson.TryGetRealApplicationVersion(currentProcess, out version))
						{
							Util.appVersion = version.ToString();
						}
						else
						{
							Util.appVersion = "0";
						}
					}
				}
				return Util.appVersion;
			}
		}

		internal static Dictionary<Guid, List<MailboxInfo>> GroupMailboxByDatabase(MailboxInfo[] mailboxes)
		{
			Dictionary<Guid, List<MailboxInfo>> dictionary = new Dictionary<Guid, List<MailboxInfo>>(2);
			foreach (MailboxInfo mailboxInfo in mailboxes)
			{
				Guid key = mailboxInfo.IsPrimary ? mailboxInfo.MdbGuid : mailboxInfo.ArchiveDatabase;
				List<MailboxInfo> list = null;
				if (!dictionary.TryGetValue(key, out list))
				{
					list = new List<MailboxInfo>(1);
					dictionary.Add(key, list);
				}
				list.Add(mailboxInfo);
			}
			return dictionary;
		}

		internal static IEnumerable<List<T>> PartitionInSetsOf<T>(IEnumerable<T> source, int batchSize)
		{
			List<T> toReturn = new List<T>(batchSize);
			foreach (T item in source)
			{
				toReturn.Add(item);
				if (toReturn.Count == batchSize)
				{
					yield return toReturn;
					toReturn = new List<T>(batchSize);
				}
			}
			if (toReturn.Any<T>())
			{
				yield return toReturn;
			}
			yield break;
		}

		public static void ThrowOnNull(object value, string argumentName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(argumentName, Strings.ArgumentValidationFailedException(argumentName));
			}
		}

		internal static int GetSearchResultPageSize(PagingInfo pagingInfo, IRecipientSession session)
		{
			Util.ThrowOnNull(pagingInfo, "pagingInfo");
			int maximumAllowedPageSizeForLocalSearch = Factory.Current.GetMaximumAllowedPageSizeForLocalSearch(pagingInfo.PageSize, session);
			if (pagingInfo.ExcludeDuplicates)
			{
				return maximumAllowedPageSizeForLocalSearch * 2;
			}
			return maximumAllowedPageSizeForLocalSearch;
		}

		internal static PreviewItem[] CreateSearchPreviewItems(MailboxInfo mailboxInfo, object[][] rows, List<KeyValuePair<int, long>> messageIdPairs, StoreSession mailboxSession, PagingInfo pagingInfo)
		{
			Util.ThrowOnNull(rows, "rows");
			return Util.InternalCreateSearchPreviewItems(mailboxInfo, rows, pagingInfo, mailboxSession, messageIdPairs, false);
		}

		internal static PreviewItem[] CreateSearchPreviewItems(MailboxInfo mailboxInfo, object[][] rows, StoreSession mailboxSession, PagingInfo pagingInfo)
		{
			Util.ThrowOnNull(rows, "rows");
			return Util.InternalCreateSearchPreviewItems(mailboxInfo, rows, pagingInfo, mailboxSession, null, true);
		}

		internal static PreviewItem[] ExcludeDuplicateItems(PreviewItem[] previewItems)
		{
			if (previewItems == null || previewItems.Length == 0)
			{
				return previewItems;
			}
			HashSet<UniqueItemHash> hashSet = new HashSet<UniqueItemHash>();
			List<PreviewItem> list = new List<PreviewItem>(previewItems.Length);
			int i = 0;
			while (i < previewItems.Length)
			{
				PreviewItem previewItem = previewItems[i];
				UniqueItemHash itemHash = previewItem.ItemHash;
				if (itemHash == null)
				{
					goto IL_42;
				}
				if (!hashSet.Contains(itemHash))
				{
					hashSet.Add(itemHash);
					goto IL_42;
				}
				IL_49:
				i++;
				continue;
				IL_42:
				list.Add(previewItem);
				goto IL_49;
			}
			return list.ToArray();
		}

		private static PreviewItem[] InternalCreateSearchPreviewItems(MailboxInfo mailboxInfo, object[][] rows, PagingInfo pagingInfo, StoreSession mailboxSession, List<KeyValuePair<int, long>> messageIdPairs, bool usingSearchFolders)
		{
			if (!usingSearchFolders)
			{
				Util.ThrowOnNull(messageIdPairs, "messageIdPairs");
			}
			if (rows.Length == 0 || rows[0].Length != pagingInfo.DataColumns.Count)
			{
				throw new ArgumentException("Invalid result rows");
			}
			List<PreviewItem> list = new List<PreviewItem>(rows.Length);
			StoreId sentItemsFolderId = null;
			if (!mailboxSession.IsPublicFolderSession)
			{
				sentItemsFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems);
			}
			Uri owaMailboxItemLink = LinkUtils.GetOwaMailboxItemLink(delegate
			{
			}, mailboxInfo, true);
			for (int i = 0; i < rows.Length; i++)
			{
				object[] array = rows[i];
				Dictionary<PropertyDefinition, object> properties = new Dictionary<PropertyDefinition, object>(4);
				int num = 0;
				foreach (PropertyDefinition key in pagingInfo.DataColumns)
				{
					properties.Add(key, array[num]);
					num++;
				}
				ReferenceItem sortValue = null;
				if (!usingSearchFolders)
				{
					sortValue = new ReferenceItem(pagingInfo.SortBy, properties[pagingInfo.SortColumn], messageIdPairs.Find((KeyValuePair<int, long> x) => x.Key == (int)properties[ItemSchema.DocumentId]).Value);
				}
				else
				{
					sortValue = new ReferenceItem(pagingInfo.SortBy, properties[pagingInfo.SortColumn], (long)mailboxSession.MailboxGuid.GetHashCode() << 32 | (long)((ulong)((int)properties[ItemSchema.DocumentId])));
				}
				List<PropertyDefinition> list2 = null;
				if (pagingInfo.AdditionalProperties != null && pagingInfo.AdditionalProperties.Count > 0)
				{
					list2 = new List<PropertyDefinition>(pagingInfo.AdditionalProperties.Count);
					foreach (ExtendedPropertyInfo extendedPropertyInfo in pagingInfo.AdditionalProperties)
					{
						list2.Add(extendedPropertyInfo.XsoPropertyDefinition);
					}
				}
				PreviewItem item = new PreviewItem(properties, mailboxInfo.MailboxGuid, owaMailboxItemLink, sortValue, Util.CalculateUniqueItemHash(mailboxSession, array, pagingInfo, sentItemsFolderId), list2)
				{
					MailboxInfo = mailboxInfo
				};
				list.Add(item);
			}
			PreviewItem[] array2 = list.ToArray();
			if (pagingInfo.ExcludeDuplicates)
			{
				array2 = Util.ExcludeDuplicateItems(array2);
			}
			return array2;
		}

		internal static bool HandleGenericMailboxFailureForMailboxOperation(Util.MethodDelegate methodDelegate, Util.MailboxOperationsErrorHandlingDelegate handleExceptionDelegate)
		{
			if (methodDelegate != null)
			{
				try
				{
					methodDelegate();
					return true;
				}
				catch (MailboxUnavailableException ex)
				{
					if (handleExceptionDelegate != null)
					{
						handleExceptionDelegate(ex);
					}
				}
				catch (MailboxInTransitException ex2)
				{
					if (handleExceptionDelegate != null)
					{
						handleExceptionDelegate(ex2);
					}
				}
				catch (MailboxOfflineException ex3)
				{
					if (handleExceptionDelegate != null)
					{
						handleExceptionDelegate(ex3);
					}
				}
				catch (ConnectionFailedPermanentException ex4)
				{
					if (handleExceptionDelegate != null)
					{
						handleExceptionDelegate(ex4);
					}
				}
				catch (StorageTransientException ex5)
				{
					if (handleExceptionDelegate != null)
					{
						handleExceptionDelegate(ex5);
					}
				}
				catch (StoragePermanentException ex6)
				{
					if (handleExceptionDelegate != null)
					{
						handleExceptionDelegate(ex6);
					}
				}
				return false;
			}
			return false;
		}

		internal static void HandleExceptions(Util.MethodDelegate method, Util.GrayExceptionHandler handler)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					method();
				});
			}
			catch (GrayException ex)
			{
				Util.SendGrayExceptionWatsonReport(ex.ToString());
				handler(ex);
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
			}
		}

		internal static bool CheckLegacyDnExistInProxyAddresses(string legacyDn, ADRawEntry adRawEntry)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)adRawEntry[ADRecipientSchema.EmailAddresses];
			foreach (ProxyAddress proxyAddress in proxyAddressCollection)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.X500 && string.Compare(proxyAddress.AddressString, legacyDn, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static void SendGrayExceptionWatsonReport(string exceptionDetails)
		{
			StackTrace stackTrace = new StackTrace(1);
			MethodBase method = stackTrace.GetFrame(0).GetMethod();
			AssemblyName name = method.DeclaringType.Assembly.GetName();
			int hashCode = (method.Name + "Unexpected GrayException").GetHashCode();
			ExWatson.SendGenericWatsonReport("E12", Util.AppVersion, ExWatson.AppName, name.Version.ToString(), name.Name, "Unexpected GrayException", stackTrace.ToString(), hashCode.ToString("x"), method.Name, exceptionDetails);
		}

		private static UniqueItemHash CalculateUniqueItemHash(StoreSession mailboxSession, object[] row, PagingInfo pagingInfo, StoreId sentItemsFolderId)
		{
			UniqueItemHash result = null;
			Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>(4);
			int num = 0;
			foreach (PropertyDefinition key in pagingInfo.DataColumns)
			{
				dictionary.Add(key, row[num]);
				num++;
			}
			object obj = null;
			object obj2 = null;
			object obj3 = null;
			object obj4 = null;
			object obj5 = null;
			object obj6 = null;
			object obj7 = null;
			dictionary.TryGetValue(ItemSchema.Id, out obj);
			dictionary.TryGetValue(StoreObjectSchema.ParentItemId, out obj2);
			dictionary.TryGetValue(ItemSchema.InternetMessageId, out obj3);
			dictionary.TryGetValue(ItemSchema.ConversationId, out obj4);
			dictionary.TryGetValue(ItemSchema.ConversationTopic, out obj5);
			dictionary.TryGetValue(ItemSchema.BodyTag, out obj6);
			dictionary.TryGetValue(StoreObjectSchema.ItemClass, out obj7);
			if (obj != null && obj2 != null && obj3 != null && obj4 != null && obj5 != null)
			{
				StoreId storeId = (StoreId)obj;
				string internetMsgId = obj3 as string;
				string text = obj5 as string;
				StoreId storeId2 = obj2 as StoreId;
				bool flag = storeId2 != null && sentItemsFolderId != null && storeId2.Equals(sentItemsFolderId);
				if (obj6 == null && !flag && ObjectClass.IsMessage(obj7 as string, false))
				{
					using (Item item = Item.Bind(mailboxSession, storeId))
					{
						if (item.Body != null)
						{
							obj6 = item.Body.CalculateBodyTag();
						}
					}
				}
				byte[] array = obj6 as byte[];
				result = new UniqueItemHash(internetMsgId, text ?? string.Empty, (array != null) ? BodyTagInfo.FromByteArray(array) : null, flag);
			}
			return result;
		}

		private static string GetArchiveItemIdForOWAUrl(StoreId id, string legacyExchangeDN)
		{
			VersionedId versionedId = id as VersionedId;
			StoreObjectId storeObjectId;
			if (versionedId == null)
			{
				storeObjectId = (StoreObjectId)id;
			}
			else
			{
				storeObjectId = versionedId.ObjectId;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(legacyExchangeDN);
			StringBuilder stringBuilder = new StringBuilder("AMB");
			stringBuilder.AppendFormat("{0}{1}{0}{2}", ".", storeObjectId.ToBase64String(), Convert.ToBase64String(bytes));
			return stringBuilder.ToString();
		}

		public static GenericIdentity GetGenericIdentity(CallerInfo caller)
		{
			if (caller.ClientSecurityContext == null)
			{
				return new GenericIdentity("S2S");
			}
			WindowsIdentity identity = caller.ClientSecurityContext.Identity;
			if (identity != null)
			{
				return new GenericIdentity(identity.Name, identity.AuthenticationType);
			}
			return new GenericIdentity(caller.ClientSecurityContext.UserSid.ToString());
		}

		public static Dictionary<PropertyDefinition, object> GetSelectedAdditionalPropertyValues(Dictionary<PropertyDefinition, object> allPropertyValues, List<PropertyDefinition> additionalProperties)
		{
			if (allPropertyValues == null || allPropertyValues.Count == 0 || additionalProperties == null || additionalProperties.Count == 0)
			{
				return null;
			}
			Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>(additionalProperties.Count);
			foreach (PropertyDefinition key in additionalProperties)
			{
				if (allPropertyValues.ContainsKey(key))
				{
					dictionary.Add(key, allPropertyValues[key]);
				}
			}
			if (dictionary.Count != 0)
			{
				return dictionary;
			}
			return null;
		}

		public static bool IsNestedFanoutCall(CallerInfo callerInfo)
		{
			return callerInfo != null && !string.IsNullOrEmpty(callerInfo.UserAgent) && callerInfo.UserAgent.StartsWith(DiscoveryEwsClient.CrossServerUserAgent, StringComparison.OrdinalIgnoreCase);
		}

		internal static EDiscoverySearchVerdict ComputeDiscoverySearchVerdict(IRecipientSession session, SearchType searchType, int actualMailboxesCount)
		{
			int maxAllowedMailboxes = Factory.Current.GetMaxAllowedMailboxes(session, searchType);
			string arg = searchType.Equals(SearchType.Preview) ? "preview" : "statistics";
			if (actualMailboxesCount <= maxAllowedMailboxes)
			{
				return EDiscoverySearchVerdict.Allowed;
			}
			Factory.Current.LocalTaskTracer.TraceWarning<int, int, string>(0L, "Number of mailboxes ({0}) being search is greater than max allowed mailboxes of ({1}) for {2} search.", actualMailboxesCount, maxAllowedMailboxes, arg);
			return EDiscoverySearchVerdict.Blocked;
		}

		internal static void SerializeIdentityCustomSoapHeaders(XmlSerializer xmlSerializer, XmlWriter writer, string identity)
		{
			OpenAsAdminOrSystemServiceType o = new OpenAsAdminOrSystemServiceType
			{
				ConnectingSID = new ConnectingSIDType
				{
					Item = new PrimarySmtpAddressType
					{
						Value = identity
					}
				},
				LogonType = SpecialLogonType.Admin,
				BudgetType = 1,
				BudgetTypeSpecified = true
			};
			xmlSerializer.Serialize(writer, o);
		}

		internal static QueryFilter CreateNewQueryFilterForGroupExpansionRecipientsIfApplicable(QueryFilter sourceFilter)
		{
			PropertyDefinition property = null;
			QueryFilter queryFilter = null;
			if (sourceFilter is TextFilter)
			{
				TextFilter textFilter = (TextFilter)sourceFilter;
				if (Util.IsPropertyRelatedToGroupExpansion(textFilter.Property, out property))
				{
					if (textFilter.Property == InternalSchema.SearchRecipients)
					{
						TextFilter textFilter2 = new TextFilter(InternalSchema.ToGroupExpansionRecipients, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
						TextFilter textFilter3 = new TextFilter(InternalSchema.CcGroupExpansionRecipients, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
						TextFilter textFilter4 = new TextFilter(InternalSchema.BccGroupExpansionRecipients, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
						queryFilter = new OrFilter(new QueryFilter[]
						{
							textFilter,
							textFilter2,
							textFilter3,
							textFilter4
						});
					}
					else
					{
						TextFilter textFilter5 = new TextFilter(property, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
						queryFilter = new OrFilter(new QueryFilter[]
						{
							textFilter,
							textFilter5
						});
					}
				}
			}
			else if (sourceFilter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = (ComparisonFilter)sourceFilter;
				if (Util.IsPropertyRelatedToGroupExpansion(comparisonFilter.Property, out property))
				{
					if (comparisonFilter.Property == InternalSchema.SearchRecipients)
					{
						ComparisonFilter comparisonFilter2 = new ComparisonFilter(comparisonFilter.ComparisonOperator, InternalSchema.ToGroupExpansionRecipients, comparisonFilter.PropertyValue);
						ComparisonFilter comparisonFilter3 = new ComparisonFilter(comparisonFilter.ComparisonOperator, InternalSchema.CcGroupExpansionRecipients, comparisonFilter.PropertyValue);
						ComparisonFilter comparisonFilter4 = new ComparisonFilter(comparisonFilter.ComparisonOperator, InternalSchema.BccGroupExpansionRecipients, comparisonFilter.PropertyValue);
						queryFilter = new OrFilter(new QueryFilter[]
						{
							comparisonFilter,
							comparisonFilter2,
							comparisonFilter3,
							comparisonFilter4
						});
					}
					else
					{
						ComparisonFilter comparisonFilter5 = new ComparisonFilter(comparisonFilter.ComparisonOperator, property, comparisonFilter.PropertyValue);
						queryFilter = new OrFilter(new QueryFilter[]
						{
							comparisonFilter,
							comparisonFilter5
						});
					}
				}
			}
			else if (sourceFilter is NotFilter)
			{
				NotFilter notFilter = (NotFilter)sourceFilter;
				QueryFilter queryFilter2 = Util.CreateNewQueryFilterForGroupExpansionRecipientsIfApplicable(notFilter.Filter);
				if (notFilter != queryFilter2)
				{
					queryFilter = new NotFilter(queryFilter2);
				}
			}
			else if (sourceFilter is AndFilter || sourceFilter is OrFilter)
			{
				CompositeFilter compositeFilter = (CompositeFilter)sourceFilter;
				bool flag = false;
				List<QueryFilter> list = new List<QueryFilter>(compositeFilter.Filters.Count);
				foreach (QueryFilter queryFilter3 in compositeFilter.Filters)
				{
					QueryFilter queryFilter4 = Util.CreateNewQueryFilterForGroupExpansionRecipientsIfApplicable(queryFilter3);
					list.Add(queryFilter4);
					if (queryFilter3 != queryFilter4)
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (sourceFilter is AndFilter)
					{
						queryFilter = new AndFilter(list.ToArray());
					}
					else
					{
						queryFilter = new OrFilter(list.ToArray());
					}
				}
			}
			if (queryFilter != null)
			{
				return queryFilter;
			}
			return sourceFilter;
		}

		private static bool IsPropertyRelatedToGroupExpansion(PropertyDefinition sourceProperty, out PropertyDefinition groupExpansionProperty)
		{
			groupExpansionProperty = null;
			if (sourceProperty == InternalSchema.SearchRecipients || sourceProperty == InternalSchema.SearchRecipientsTo || sourceProperty == InternalSchema.SearchRecipientsCc || sourceProperty == InternalSchema.SearchRecipientsBcc)
			{
				if (sourceProperty == InternalSchema.SearchRecipientsTo)
				{
					groupExpansionProperty = InternalSchema.ToGroupExpansionRecipients;
				}
				else if (sourceProperty == InternalSchema.SearchRecipientsCc)
				{
					groupExpansionProperty = InternalSchema.CcGroupExpansionRecipients;
				}
				else if (sourceProperty == InternalSchema.SearchRecipientsBcc)
				{
					groupExpansionProperty = InternalSchema.BccGroupExpansionRecipients;
				}
				else
				{
					groupExpansionProperty = InternalSchema.GroupExpansionRecipients;
				}
				return true;
			}
			return false;
		}

		private static string appVersion;

		internal delegate void MethodDelegate();

		internal delegate void MailboxOperationsErrorHandlingDelegate(LocalizedException ex);

		internal delegate void GrayExceptionHandler(GrayException e);
	}
}
