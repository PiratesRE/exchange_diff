using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetHoldOnMailboxes : SingleStepServiceCommand<GetHoldOnMailboxesRequest, MailboxHoldResult>, IDisposeTrackable, IDisposable
	{
		public GetHoldOnMailboxes(CallContext callContext, GetHoldOnMailboxesRequest request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.holdId = request.HoldId;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GetHoldOnMailboxes>(this);
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
			return new GetHoldOnMailboxesResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<MailboxHoldResult> Execute()
		{
			MailboxSearchHelper.PerformCommonAuthorization(base.CallContext.IsExternalUser, out this.runspaceConfig, out this.recipientSession);
			return this.ProcessRequest();
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

		private ServiceResult<MailboxHoldResult> ProcessRequest()
		{
			List<MailboxHoldStatus> list = new List<MailboxHoldStatus>();
			DiscoverySearchDataProvider discoverySearchDataProvider = new DiscoverySearchDataProvider(this.recipientSession.SessionSettings.CurrentOrganizationId);
			MailboxDiscoverySearch mailboxDiscoverySearch = discoverySearchDataProvider.FindByAlternativeId<MailboxDiscoverySearch>(this.holdId);
			if (mailboxDiscoverySearch == null)
			{
				ExTraceGlobals.SearchTracer.TraceError<string>((long)this.GetHashCode(), "Specific hold id: {0} is not found in the system.", this.holdId);
				throw new MailboxHoldNotFoundException(CoreResources.IDs.ErrorHoldIsNotFound);
			}
			MailboxSearchHelper.GetMailboxHoldStatuses(mailboxDiscoverySearch, this.recipientSession, list);
			return new ServiceResult<MailboxHoldResult>(new MailboxHoldResult(this.holdId, mailboxDiscoverySearch.Query, list.ToArray()));
		}

		private readonly DisposeTracker disposeTracker;

		private bool disposed;

		private string holdId;

		private ExchangeRunspaceConfiguration runspaceConfig;

		private IRecipientSession recipientSession;
	}
}
