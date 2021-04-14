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
	public class GetModernGroupRequest : BaseRequest
	{
		[DataMember(Name = "SmtpAddress", IsRequired = true)]
		public string SmtpAddress { get; set; }

		[DataMember(Name = "ResultSet", IsRequired = true)]
		public ModernGroupRequestResultSet ResultSet { get; set; }

		[DataMember(Name = "MemberSortOrder")]
		public ModernGroupMembersSortOrder MemberSortOrder { get; set; }

		internal ProxyAddress ProxyAddress
		{
			get
			{
				return new SmtpProxyAddress(this.SmtpAddress, true);
			}
		}

		internal bool IsMemberRequested { get; private set; }

		internal bool IsOwnerListRequested { get; private set; }

		internal bool IsGeneralInfoRequested { get; private set; }

		internal bool IsExternalResourcesRequested { get; private set; }

		internal bool IsMailboxInfoRequested { get; private set; }

		internal bool IsForceReloadRequested { get; private set; }

		internal void ValidateRequest()
		{
			if (string.IsNullOrEmpty(this.SmtpAddress) || !Microsoft.Exchange.Data.SmtpAddress.IsValidSmtpAddress(this.SmtpAddress))
			{
				ExTraceGlobals.ModernGroupsTracer.TraceDebug<string>((long)this.GetHashCode(), "Invalid smtp address {0}", this.SmtpAddress);
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			this.IsGeneralInfoRequested = ((this.ResultSet & ModernGroupRequestResultSet.General) == ModernGroupRequestResultSet.General);
			this.IsMemberRequested = ((this.ResultSet & ModernGroupRequestResultSet.Members) == ModernGroupRequestResultSet.Members);
			this.IsOwnerListRequested = ((this.ResultSet & ModernGroupRequestResultSet.Owners) == ModernGroupRequestResultSet.Owners);
			this.IsExternalResourcesRequested = ((this.ResultSet & ModernGroupRequestResultSet.ExternalResources) == ModernGroupRequestResultSet.ExternalResources);
			this.IsMailboxInfoRequested = ((this.ResultSet & ModernGroupRequestResultSet.GroupMailboxProperties) == ModernGroupRequestResultSet.GroupMailboxProperties);
			this.IsForceReloadRequested = ((this.ResultSet & ModernGroupRequestResultSet.ForceReload) == ModernGroupRequestResultSet.ForceReload);
			if (this.IsMemberRequested && this.MembersPageRequest == null)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), "Members requested but paging information was missing");
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
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

		[DataMember(Name = "MembersPageRequest", IsRequired = false)]
		public IndexedPageView MembersPageRequest;

		[DataMember(Name = "SerializedPeopleIKnowGraph")]
		public string SerializedPeopleIKnowGraph;
	}
}
