using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "FindMembersInUnifiedGroup", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindMembersInUnifiedGroupRequest : BaseRequest
	{
		[DataMember(Name = "SmtpAddress", IsRequired = true)]
		public string SmtpAddress { get; set; }

		[DataMember(Name = "Filter", IsRequired = false)]
		public string Filter { get; set; }

		internal ProxyAddress ProxyAddress
		{
			get
			{
				return new SmtpProxyAddress(this.SmtpAddress, true);
			}
		}

		internal override void Validate()
		{
			if (string.IsNullOrEmpty(this.SmtpAddress))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (this.Filter == null)
			{
				this.Filter = string.Empty;
			}
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}
	}
}
