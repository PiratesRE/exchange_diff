using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetDiscoverySearchConfiguration : SingleStepServiceCommand<GetDiscoverySearchConfigurationRequest, DiscoverySearchConfiguration[]>, IDisposeTrackable, IDisposable
	{
		public GetDiscoverySearchConfiguration(CallContext callContext, GetDiscoverySearchConfigurationRequest request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.SaveRequestData(request);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GetDiscoverySearchConfiguration>(this);
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
			return new GetDiscoverySearchConfigurationResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<DiscoverySearchConfiguration[]> Execute()
		{
			MailboxSearchHelper.PerformCommonAuthorization(base.CallContext.IsExternalUser, out this.runspaceConfig, out this.recipientSession);
			DiscoverySearchDataProvider discoverySearchDataProvider = new DiscoverySearchDataProvider(this.recipientSession.SessionSettings.CurrentOrganizationId);
			List<DiscoverySearchConfiguration> list = new List<DiscoverySearchConfiguration>();
			if (!string.IsNullOrEmpty(this.searchId))
			{
				MailboxDiscoverySearch mailboxDiscoverySearch = discoverySearchDataProvider.Find<MailboxDiscoverySearch>(this.searchId);
				if (mailboxDiscoverySearch == null)
				{
					throw new ServiceArgumentException((CoreResources.IDs)2524429953U);
				}
				list.Add(this.QueryDiscoverySearchConfiguration(mailboxDiscoverySearch));
			}
			else
			{
				foreach (MailboxDiscoverySearch searchObject in discoverySearchDataProvider.GetAll<MailboxDiscoverySearch>())
				{
					list.Add(this.QueryDiscoverySearchConfiguration(searchObject));
				}
			}
			return new ServiceResult<DiscoverySearchConfiguration[]>(list.ToArray());
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

		private void SaveRequestData(GetDiscoverySearchConfigurationRequest request)
		{
			this.searchId = request.SearchId;
			this.expandGroupMembership = request.ExpandGroupMembership;
			this.inPlaceHoldConfigurationOnly = request.InPlaceHoldConfigurationOnly;
		}

		private DiscoverySearchConfiguration QueryDiscoverySearchConfiguration(MailboxDiscoverySearch searchObject)
		{
			if (this.inPlaceHoldConfigurationOnly)
			{
				return new DiscoverySearchConfiguration(searchObject.Name, null, searchObject.CalculatedQuery, searchObject.InPlaceHoldIdentity, searchObject.ManagedByOrganization, searchObject.Language);
			}
			List<FailedSearchMailbox> list;
			List<SearchableMailbox> searchableMailboxes = MailboxSearchHelper.GetSearchableMailboxes(searchObject, this.expandGroupMembership, this.recipientSession, this.runspaceConfig, out list);
			return new DiscoverySearchConfiguration(searchObject.Name, searchableMailboxes.ToArray(), searchObject.CalculatedQuery, null, null, searchObject.Language);
		}

		private readonly DisposeTracker disposeTracker;

		private bool disposed;

		private string searchId;

		private bool expandGroupMembership;

		private bool inPlaceHoldConfigurationOnly;

		private ExchangeRunspaceConfiguration runspaceConfig;

		private IRecipientSession recipientSession;
	}
}
