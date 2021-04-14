using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetModernGroupMembershipJsonRequest : BaseRequest
	{
		[DataMember(IsRequired = true)]
		public string GroupSmtpAddress { get; set; }

		[DataMember(IsRequired = true)]
		public ModernGroupMembershipOperationType OperationType { get; set; }

		[DataMember(IsRequired = false)]
		public string AttachedMessage { get; set; }

		internal ProxyAddress GroupProxyAddress { get; private set; }

		internal override void Validate()
		{
			if (string.IsNullOrEmpty(this.GroupSmtpAddress) || !SmtpAddress.IsValidSmtpAddress(this.GroupSmtpAddress))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorInvalidSmtpAddress), FaultParty.Sender);
			}
			if (!Enum.IsDefined(typeof(ModernGroupMembershipOperationType), this.OperationType))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (this.OperationType == ModernGroupMembershipOperationType.RequestJoin && this.AttachedMessage == null)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorRequiredPropertyMissing), FaultParty.Sender);
			}
			this.GroupProxyAddress = new SmtpProxyAddress(this.GroupSmtpAddress, true);
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
