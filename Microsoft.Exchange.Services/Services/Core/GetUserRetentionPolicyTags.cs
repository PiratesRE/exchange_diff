using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUserRetentionPolicyTags : SingleStepServiceCommand<GetUserRetentionPolicyTagsRequest, Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag[]>, IDisposeTrackable, IDisposable
	{
		public GetUserRetentionPolicyTags(CallContext callContext, GetUserRetentionPolicyTagsRequest request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<GetUserRetentionPolicyTags>(this);
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
			return new GetUserRetentionPolicyTagsResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag[]> Execute()
		{
			Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag[] value = this.FetchRetentionPolicyTags();
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag[]>(value);
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

		private Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag[] FetchRetentionPolicyTags()
		{
			List<Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag> list = new List<Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag>();
			if (base.MailboxIdentityMailboxSession != null)
			{
				PolicyTagList policyTagList = base.MailboxIdentityMailboxSession.GetPolicyTagList((Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType)0);
				if (policyTagList != null)
				{
					foreach (PolicyTag policyTag in policyTagList.Values)
					{
						list.Add(new Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag(policyTag));
					}
				}
			}
			base.CallContext.ProtocolLog.AppendGenericInfo("UserRetentionPolicyTagsCount", list.Count);
			ExTraceGlobals.ELCTracer.TraceDebug<int>((long)this.GetHashCode(), "[GetUserRetentionPolicyTags::FetchRetentionPolicyTags] retrieved {0} retention policy tags", list.Count);
			return list.ToArray();
		}

		private readonly DisposeTracker disposeTracker;

		private bool disposed;
	}
}
