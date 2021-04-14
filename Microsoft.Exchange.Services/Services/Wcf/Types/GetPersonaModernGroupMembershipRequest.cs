using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetPersonaModernGroupMembershipRequest : BaseRequest
	{
		[DataMember(Name = "SmtpAddress", IsRequired = true)]
		public string SmtpAddress { get; set; }

		internal ProxyAddress ProxyAddress { get; private set; }

		internal void ValidateRequest()
		{
			if (string.IsNullOrEmpty(this.SmtpAddress) || !Microsoft.Exchange.Data.SmtpAddress.IsValidSmtpAddress(this.SmtpAddress))
			{
				ExTraceGlobals.ModernGroupsTracer.TraceDebug<string>((long)this.GetHashCode(), "Invalid smtp address {0}", this.SmtpAddress);
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (this.PagingOptions == null)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), "Paging options are missing.");
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			this.ProxyAddress = new SmtpProxyAddress(this.SmtpAddress, true);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		[DataMember(Name = "PagingOptions", IsRequired = true)]
		public IndexedPageView PagingOptions;
	}
}
