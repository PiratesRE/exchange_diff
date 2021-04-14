using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class MailboxSearchFlighting
	{
		public static bool IsFlighted(CallContext callContext, string serviceName, out ISearchPolicy policy)
		{
			policy = null;
			bool result = false;
			try
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.IsFlighted Trying to load flighting for", serviceName);
				if (callContext != null)
				{
					IRecipientSession recipientSession = null;
					ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = null;
					MailboxSearchHelper.PerformCommonAuthorization(callContext.IsExternalUser, out exchangeRunspaceConfiguration, out recipientSession);
					if (exchangeRunspaceConfiguration == null && callContext.EffectiveCaller != null && callContext.EffectiveCaller.ObjectSid != null)
					{
						exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(callContext.EffectiveCaller, null, false);
					}
					CallerInfo callerInfo = MailboxSearchConverter.GetCallerInfo(callContext, recipientSession.SessionSettings.CurrentOrganizationId);
					policy = SearchFactory.Current.GetSearchPolicy(recipientSession, callerInfo, exchangeRunspaceConfiguration, callContext.Budget);
					policy.Recorder.ReadTimestampHeader(callContext.HttpContext.Request.Headers);
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.IsFlighted Loaded configuration policy");
					if (policy.ExecutionSettings.Snapshot != null)
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.IsFlighted Flighting snapshot loaded");
						string flight = MailboxSearchFlighting.GetFlight(policy.ExecutionSettings.Snapshot, serviceName);
						Recorder.Trace(1L, TraceType.InfoTrace, new object[]
						{
							"MailboxSearchFlighting.IsFlighted current flight for",
							serviceName,
							"is",
							flight
						});
						if (!string.IsNullOrEmpty(flight))
						{
							Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.IsFlighted flighting enabled for", serviceName);
							callContext.HttpContext.Response.AddHeader("X-DiscoveryActiveFlight", flight);
							callContext.ProtocolLog.AppendGenericInfo("Flight", flight);
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Recorder.Trace(1L, TraceType.ErrorTrace, new object[]
				{
					"MailboxSearchFlighting.IsFlighted Failed:",
					ex,
					"Ignoring and Continuing"
				});
			}
			return result;
		}

		public static string GetFlight(VariantConfigurationSnapshot variantSnapshot, string serviceName)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.GetFlight Service:", serviceName);
			string[] source = new string[]
			{
				"GetSearchableMailboxes",
				"SearchMailboxes"
			};
			if (source.Contains(serviceName) && variantSnapshot.Discovery.SearchScale.Enabled)
			{
				return "SearchScale";
			}
			return null;
		}

		public static SearchMailboxesResponse SearchMailboxes(ISearchPolicy policy, SearchMailboxesRequest request)
		{
			long lastConversionTime = 0L;
			SearchMailboxesResponse searchResponse;
			try
			{
				lastConversionTime = policy.Recorder.Timestamp;
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.SearchMailboxes Request:", request);
				if (request.SearchQueries != null && request.SearchQueries.Length > policy.ExecutionSettings.DiscoveryMaxAllowedMailboxQueriesPerRequest)
				{
					Recorder.Trace(1L, TraceType.ErrorTrace, new object[]
					{
						"MailboxSearchFlighting.SearchMailboxes Number of search query objects",
						request.SearchQueries.Length,
						"exceeded limit",
						policy.ExecutionSettings.DiscoveryMaxAllowedMailboxQueriesPerRequest
					});
					throw new SearchException(KnownError.TooManyMailboxQueryObjects, new object[]
					{
						request.SearchQueries.Length,
						policy.ExecutionSettings.DiscoveryMaxAllowedMailboxQueriesPerRequest
					});
				}
				if (policy.ThrottlingSettings.DiscoveryMaxConcurrency <= 0U)
				{
					Recorder.Trace(1L, TraceType.ErrorTrace, new object[]
					{
						"MailboxSearchFlighting.SearchMailboxes discovery search is turned of, throttling policy allows",
						policy.ThrottlingSettings.DiscoveryMaxConcurrency,
						"searches"
					});
					throw new SearchException(KnownError.ErrorDiscoverySearchesDisabledException);
				}
				SearchMailboxesInputs parameters = MailboxSearchConverter.GetSearchInputs(policy, request);
				List<ServiceResult<SearchMailboxesResults>> list = new List<ServiceResult<SearchMailboxesResults>>();
				policy.Recorder.ConversionTime += policy.Recorder.Timestamp - lastConversionTime;
				MailboxQuery[] array;
				if ((array = request.SearchQueries) == null)
				{
					array = new MailboxQuery[]
					{
						new MailboxQuery()
					};
				}
				MailboxQuery[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					MailboxQuery mailboxQuery = array2[i];
					list.Add(ExceptionHandler<SearchMailboxesResults>.Execute(delegate(int t)
					{
						ServiceResult<SearchMailboxesResults> result;
						try
						{
							parameters.SearchQuery = mailboxQuery.Query;
							if (mailboxQuery.MailboxSearchScopes != null && mailboxQuery.MailboxSearchScopes.Length > 0)
							{
								lastConversionTime = policy.Recorder.Timestamp;
								parameters.Sources = new List<SearchSource>(MailboxSearchConverter.GetSources(policy, mailboxQuery.MailboxSearchScopes));
								policy.Recorder.ConversionTime += policy.Recorder.Timestamp - lastConversionTime;
								bool flag = true;
								SearchType searchType = parameters.SearchType;
								if (parameters.Sources != null && parameters.Sources.Count > 0)
								{
									using (List<SearchSource>.Enumerator enumerator = parameters.Sources.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											SearchSource searchSource = enumerator.Current;
											if (flag && searchSource.MailboxInfo == null)
											{
												flag = false;
											}
											if (searchSource.ExtendedAttributes != null)
											{
												if (Enum.TryParse<SearchType>(searchSource.ExtendedAttributes.FirstOrDefault((KeyValuePair<string, string> e) => string.Equals("SearchType", e.Key, StringComparison.InvariantCultureIgnoreCase)).Value, out searchType))
												{
													parameters.SearchType = searchType;
													if (searchType == SearchType.NonIndexedItemPreview && !parameters.PagingInfo.AscendingSort)
													{
														ReferenceItem referenceItem = parameters.PagingInfo.SortValue;
														if (parameters.PagingInfo.SortValue != null)
														{
															referenceItem = new ReferenceItem(new SortBy(parameters.PagingInfo.SortValue.SortBy.ColumnDefinition, SortOrder.Ascending), parameters.PagingInfo.SortValue.SortColumnValue, parameters.PagingInfo.SortValue.SecondarySortValue);
														}
														parameters.PagingInfo = new PagingInfo(parameters.PagingInfo.OriginalDataColumns, new SortBy(parameters.PagingInfo.SortBy.ColumnDefinition, SortOrder.Ascending), parameters.PagingInfo.PageSize, parameters.PagingInfo.Direction, referenceItem, parameters.PagingInfo.TimeZone, parameters.PagingInfo.ExcludeDuplicates, parameters.PagingInfo.BaseShape, parameters.PagingInfo.AdditionalProperties)
														{
															OriginalSortByReference = parameters.PagingInfo.OriginalSortByReference
														};
													}
												}
											}
										}
										goto IL_34E;
									}
								}
								flag = false;
								IL_34E:
								parameters.IsLocalCall = flag;
							}
							Recorder.Trace(1L, TraceType.InfoTrace, new object[]
							{
								"MailboxSearchFlighting.SearchMailboxes Executing Query:",
								parameters.SearchQuery,
								"SearchType:",
								parameters.SearchType,
								"IsLocal:",
								parameters.IsLocalCall,
								"Sources:",
								parameters.Sources.Count
							});
							SearchMailboxesResults value = Controller.SeachMailboxes(policy, parameters);
							Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.SearchMailboxes Completed Query:", parameters.SearchQuery);
							result = new ServiceResult<SearchMailboxesResults>(value);
						}
						catch (SearchException ex2)
						{
							Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchFlighting.SearchMailboxes Executing Query Failed", ex2);
							throw MailboxSearchConverter.GetException(ex2);
						}
						return result;
					}, 0, null));
					if (parameters.SearchType == SearchType.ExpandSources)
					{
						break;
					}
				}
				lastConversionTime = policy.Recorder.Timestamp;
				searchResponse = MailboxSearchConverter.GetSearchResponse(request, list, parameters);
			}
			catch (SearchException ex)
			{
				Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchFlighting.SearchMailboxes Failed", ex);
				throw MailboxSearchConverter.GetException(ex);
			}
			finally
			{
				policy.Recorder.ConversionTime += policy.Recorder.Timestamp - lastConversionTime;
				MailboxSearchFlighting.WriteLog(policy);
			}
			return searchResponse;
		}

		internal static ServiceResult<SearchableMailbox[]> GetSearchableMailboxes(ISearchPolicy policy, GetSearchableMailboxesRequest request)
		{
			long num = 0L;
			ServiceResult<SearchableMailbox[]> result;
			try
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.GetSearchableMailboxes Request:", request);
				GetSearchableMailboxesInputs getSearchableMailboxesInputs = new GetSearchableMailboxesInputs
				{
					Filter = request.SearchFilter,
					ExpandGroups = request.ExpandGroupMembership
				};
				Recorder.Trace(1L, TraceType.InfoTrace, new object[]
				{
					"MailboxSearchFlighting.GetSearchableMailboxes Executing Query:",
					getSearchableMailboxesInputs.Filter,
					"ExpandGroups:",
					getSearchableMailboxesInputs.ExpandGroups
				});
				GetSearchableMailboxesResults searchableMailboxes = Controller.GetSearchableMailboxes(policy, getSearchableMailboxesInputs);
				num = policy.Recorder.Timestamp;
				IEnumerable<SearchableMailbox> searchableMailboxes2 = MailboxSearchConverter.GetSearchableMailboxes(searchableMailboxes);
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.GetSearchableMailboxes Completed Query:", getSearchableMailboxesInputs.Filter);
				result = new ServiceResult<SearchableMailbox[]>(searchableMailboxes2.ToArray<SearchableMailbox>());
			}
			catch (SearchException ex)
			{
				Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchFlighting.GetSearchableMailboxes Failed:", ex);
				throw MailboxSearchConverter.GetException(ex);
			}
			finally
			{
				policy.Recorder.ConversionTime += policy.Recorder.Timestamp - num;
				MailboxSearchFlighting.WriteLog(policy);
			}
			return result;
		}

		private static void WriteLog(ISearchPolicy policy)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.WriteLog");
			if (CallContext.Current != null)
			{
				NameValueCollection headers = null;
				if (policy.ExecutionSettings.DiscoveryAggregateLogs && CallContext.Current.HttpContext != null && CallContext.Current.HttpContext.Response != null)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchFlighting.WriteLog Aggregation Enabled");
					headers = CallContext.Current.HttpContext.Response.Headers;
				}
				policy.Recorder.Write(headers, delegate(string key, string data)
				{
					CallContext.Current.ProtocolLog.AppendGenericInfo(key, data);
				});
			}
		}

		internal const string FlightStamp = "X-DiscoveryActiveFlight";
	}
}
