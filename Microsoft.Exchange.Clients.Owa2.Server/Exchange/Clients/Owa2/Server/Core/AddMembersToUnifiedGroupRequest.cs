using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "AddMembersToUnifiedGroupRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddMembersToUnifiedGroupRequest : BaseRequest
	{
		[DataMember(Name = "ExternalDirectoryObjectId", IsRequired = true)]
		public string ExternalDirectoryObjectId { get; set; }

		[DataMember(Name = "AddedMembers", IsRequired = false)]
		public string[] AddedMembers { get; set; }

		internal override void Validate()
		{
			if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (this.AddedMembers == null)
			{
				this.AddedMembers = new string[0];
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
