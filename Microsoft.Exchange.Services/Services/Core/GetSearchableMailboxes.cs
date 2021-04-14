using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetSearchableMailboxes : SingleStepServiceCommand<GetSearchableMailboxesRequest, SearchableMailbox[]>, IDisposeTrackable, IDisposable
	{
		public GetSearchableMailboxes(CallContext callContext, GetSearchableMailboxesRequest request) : base(callContext, request)
		{
			if (MailboxSearchFlighting.IsFlighted(callContext, "GetSearchableMailboxes", out this.policy))
			{
				this.isFlighted = true;
				return;
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.SaveRequestData(request);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GetSearchableMailboxes>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetSearchableMailboxesResponse(base.Result.Code, base.Result.Error, base.Result.Value, this.excludedMailboxes.ToArray());
		}

		internal override ServiceResult<SearchableMailbox[]> Execute()
		{
			if (this.isFlighted)
			{
				return MailboxSearchFlighting.GetSearchableMailboxes(this.policy, base.Request);
			}
			this.ValidateRequestData();
			MailboxSearchHelper.PerformCommonAuthorization(base.CallContext.IsExternalUser, out this.runspaceConfig, out this.recipientSession);
			SearchableMailbox[] value = this.QuerySearchableMailboxes();
			return new ServiceResult<SearchableMailbox[]>(value);
		}

		private void Dispose(bool fromDispose)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.disposed)
			{
				this.disposed = true;
			}
		}

		private void SaveRequestData(GetSearchableMailboxesRequest request)
		{
			this.searchFilter = ((request.SearchFilter == null) ? request.SearchFilter : request.SearchFilter.Trim());
			this.expandGroupMembership = request.ExpandGroupMembership;
		}

		private void ValidateRequestData()
		{
			if (this.expandGroupMembership && (string.IsNullOrEmpty(this.searchFilter) || this.IsWildcard(this.searchFilter)))
			{
				throw new ServiceArgumentException((CoreResources.IDs)4083587704U);
			}
			if (this.IsSuffixSearchWildcard(this.searchFilter))
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorSuffixSearchNotAllowed);
			}
		}

		private SearchableMailbox[] QuerySearchableMailboxes()
		{
			List<QueryFilter> additionalSearchFilters = this.GetAdditionalSearchFilters();
			SortBy sortBy = new SortBy(MiniRecipientSchema.DisplayName, SortOrder.Ascending);
			ADPagedReader<MiniRecipient> adpagedReader = MailboxSearchHelper.QueryADObject(this.recipientSession, additionalSearchFilters, sortBy);
			List<SearchableMailbox> list = new List<SearchableMailbox>();
			foreach (MiniRecipient miniRecipient in adpagedReader)
			{
				bool flag = MailboxSearchHelper.IsMembershipGroup(miniRecipient);
				if (flag && this.expandGroupMembership)
				{
					using (Dictionary<ADObjectId, ADRawEntry>.Enumerator enumerator2 = MailboxSearchHelper.ProcessGroupExpansion(this.recipientSession, miniRecipient, this.recipientSession.SessionSettings.CurrentOrganizationId).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<ADObjectId, ADRawEntry> keyValuePair = enumerator2.Current;
							if (keyValuePair.Value != null)
							{
								MailboxSearchHelper.VerifyAndAddSearchableMailboxToCollection(list, keyValuePair.Value, false, this.runspaceConfig, this.excludedMailboxes, true);
								if (list.Count > 1500)
								{
									ExTraceGlobals.SearchTracer.TraceDebug<int>((long)this.GetHashCode(), "Reach max limit ({0}) of total recipients to return", 1500);
									throw new ServiceArgumentException(CoreResources.IDs.ErrorADSessionFilter, CoreResources.ErrorReturnTooManyMailboxesFromDG(miniRecipient.DisplayName, 1500));
								}
							}
							else
							{
								ExTraceGlobals.SearchTracer.TraceError<string>((long)this.GetHashCode(), "Unable to find mailbox {0}", keyValuePair.Key.Name);
							}
						}
						continue;
					}
				}
				MailboxSearchHelper.VerifyAndAddSearchableMailboxToCollection(list, miniRecipient, flag, this.runspaceConfig, this.excludedMailboxes, true);
				if (list.Count >= 1500)
				{
					ExTraceGlobals.SearchTracer.TraceDebug<int>((long)this.GetHashCode(), "Reach max limit ({0}) of total recipients to return", 1500);
					break;
				}
			}
			list.Sort();
			return list.ToArray();
		}

		private List<QueryFilter> GetAdditionalSearchFilters()
		{
			List<QueryFilter> list = new List<QueryFilter>();
			if (!string.IsNullOrEmpty(this.searchFilter))
			{
				if (this.searchFilter.StartsWith("*") && this.searchFilter.EndsWith("*") && this.searchFilter.Length <= 2)
				{
					return list;
				}
				Guid empty = Guid.Empty;
				if (Guid.TryParse(this.searchFilter, out empty))
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, empty));
					return list;
				}
				SmtpAddress smtpAddress = new SmtpAddress(this.searchFilter);
				if (smtpAddress.IsValidAddress)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "SMTP:" + smtpAddress.ToString()));
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ExternalEmailAddress, "SMTP:" + smtpAddress.ToString()));
					return list;
				}
				list.Add(this.CreateWildcardOrEqualFilter(ADUserSchema.UserPrincipalName, this.searchFilter));
				list.Add(this.CreateWildcardOrEqualFilter(ADRecipientSchema.Alias, this.searchFilter));
				list.Add(this.CreateWildcardOrEqualFilter(ADUserSchema.FirstName, this.searchFilter));
				list.Add(this.CreateWildcardOrEqualFilter(ADUserSchema.LastName, this.searchFilter));
				list.Add(this.CreateWildcardOrEqualFilter(ADRecipientSchema.DisplayName, this.searchFilter));
			}
			return list;
		}

		private QueryFilter CreateWildcardOrEqualFilter(ADPropertyDefinition schemaProperty, string searchFilter)
		{
			if (this.IsWildcard(searchFilter))
			{
				string text = searchFilter.Substring(0, searchFilter.Length - 1);
				return new TextFilter(schemaProperty, text, MatchOptions.Prefix, MatchFlags.IgnoreCase);
			}
			return new ComparisonFilter(ComparisonOperator.Equal, schemaProperty, searchFilter);
		}

		private bool IsWildcard(string s)
		{
			return !string.IsNullOrEmpty(s) && (s.StartsWith("*") || s.EndsWith("*"));
		}

		private bool IsSuffixSearchWildcard(string s)
		{
			return !string.IsNullOrEmpty(s) && s.StartsWith("*") && s.Length > 1;
		}

		private readonly DisposeTracker disposeTracker;

		private bool disposed;

		private string searchFilter;

		private bool expandGroupMembership;

		private ExchangeRunspaceConfiguration runspaceConfig;

		private IRecipientSession recipientSession;

		private List<FailedSearchMailbox> excludedMailboxes = new List<FailedSearchMailbox>(1);

		private readonly bool isFlighted;

		private readonly ISearchPolicy policy;
	}
}
