using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Inference.GroupingModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GetRecommendedModernGroup : ServiceCommand<GetModernGroupResponse>
	{
		public GetRecommendedModernGroup(CallContext context, GetModernGroupRequest request) : base(context)
		{
			this.request = request;
			request.ValidateRequest();
			if (!VariantConfiguration.GetSnapshot(base.CallContext.AccessingADUser.GetContext(null), null, null).Inference.InferenceGroupingModel.Enabled)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError<string>((long)this.GetHashCode(), "User {0} is not enabled for flighting the InferenceGroupingModel feature!", base.CallContext.AccessingADUser.UserPrincipalName);
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
		}

		protected override GetModernGroupResponse InternalExecute()
		{
			IRecommendedGroupInfo latentGroupRecommendation = GroupRecommendationsHelper.GetLatentGroupRecommendation(base.MailboxIdentityMailboxSession, new SmtpAddress(this.request.SmtpAddress), delegate(string message)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), message);
			}, delegate(Exception e)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError<string>((long)this.GetHashCode(), "Exception while retrieving modern group recommendations: {0}", e.Message);
			});
			if (latentGroupRecommendation == null)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError<string>((long)this.GetHashCode(), "[GetRecommendedGroups.InternalExecute: could not find recommendation object for recomendation id {0}.", this.request.SmtpAddress);
			}
			return GroupRecommendationsHelper.ConvertLatentGroupRecommendationToModernGroupResponse(latentGroupRecommendation, base.CallContext.AccessingADUser.PrimarySmtpAddress);
		}

		private readonly GetModernGroupRequest request;
	}
}
