using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetNonIndexableItemStatistics : SingleStepServiceCommand<GetNonIndexableItemStatisticsRequest, NonIndexableItemStatisticResult[]>, IDisposeTrackable, IDisposable
	{
		public GetNonIndexableItemStatistics(CallContext callContext, GetNonIndexableItemStatisticsRequest request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.mailboxes = request.Mailboxes;
			this.searchArchiveOnly = request.SearchArchiveOnly;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GetNonIndexableItemStatistics>(this);
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
			return new GetNonIndexableItemStatisticsResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<NonIndexableItemStatisticResult[]> Execute()
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

		private ServiceResult<NonIndexableItemStatisticResult[]> ProcessRequest()
		{
			if (this.mailboxes.Length != 1)
			{
				throw new ServiceArgumentException((CoreResources.IDs)4136809189U);
			}
			Dictionary<string, ADRawEntry> dictionary = MailboxSearchHelper.FindADEntriesByLegacyExchangeDNs(this.recipientSession, this.mailboxes, MailboxSearchHelper.AdditionalProperties);
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, ADRawEntry> keyValuePair in dictionary)
				{
					if (!MailboxSearchHelper.HasPermissionToSearchMailbox(this.runspaceConfig, keyValuePair.Value))
					{
						throw new ServiceInvalidOperationException((CoreResources.IDs)2354781453U);
					}
				}
			}
			List<NonIndexableItemStatisticResult> list = new List<NonIndexableItemStatisticResult>(this.mailboxes.Length);
			CallerInfo callerInfo = new CallerInfo(MailboxSearchHelper.IsOpenAsAdmin(base.CallContext), MailboxSearchConverter.GetCommonAccessToken(base.CallContext), base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallContext.EffectiveCaller.PrimarySmtpAddress, this.recipientSession.SessionSettings.CurrentOrganizationId, string.Empty, MailboxSearchHelper.GetQueryCorrelationId(), MailboxSearchHelper.GetUserRolesFromAuthZClientInfo(base.CallContext.EffectiveCaller), MailboxSearchHelper.GetApplicationRolesFromAuthZClientInfo(base.CallContext.EffectiveCaller));
			NonIndexableItemStatisticsProvider nonIndexableItemStatisticsProvider = new NonIndexableItemStatisticsProvider(this.recipientSession, MailboxSearchHelper.GetTimeZone(), callerInfo, this.recipientSession.SessionSettings.CurrentOrganizationId, this.mailboxes, this.searchArchiveOnly);
			nonIndexableItemStatisticsProvider.ExecuteSearch();
			foreach (NonIndexableItemStatisticsInfo nonIndexableItemStatisticsInfo in nonIndexableItemStatisticsProvider.Results)
			{
				list.Add(new NonIndexableItemStatisticResult
				{
					Mailbox = nonIndexableItemStatisticsInfo.Mailbox,
					ItemCount = (long)nonIndexableItemStatisticsInfo.ItemCount,
					ErrorMessage = nonIndexableItemStatisticsInfo.ErrorMessage
				});
			}
			return new ServiceResult<NonIndexableItemStatisticResult[]>((list.Count == 0) ? null : list.ToArray());
		}

		private readonly DisposeTracker disposeTracker;

		private readonly string[] mailboxes;

		private readonly bool searchArchiveOnly;

		private bool disposed;

		private ExchangeRunspaceConfiguration runspaceConfig;

		private IRecipientSession recipientSession;
	}
}
