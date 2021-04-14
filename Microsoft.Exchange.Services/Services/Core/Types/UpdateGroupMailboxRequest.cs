using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UpdateGroupMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateGroupMailboxRequest : BaseRequest
	{
		[XmlElement("GroupSmtpAddress")]
		[DataMember(Name = "GroupSmtpAddress", IsRequired = true)]
		public string GroupSmtpAddress { get; set; }

		[XmlElement("ExecutingUserSmtpAddress")]
		[DataMember(Name = "ExecutingUserSmtpAddress", IsRequired = false)]
		public string ExecutingUserSmtpAddress { get; set; }

		[DataMember(Name = "DomainController", IsRequired = false)]
		[XmlElement("DomainController")]
		public string DomainController { get; set; }

		[DataMember(Name = "ForceConfigurationAction", IsRequired = false)]
		[XmlElement("ForceConfigurationAction")]
		public GroupMailboxConfigurationAction ForceConfigurationAction { get; set; }

		[DataMember(Name = "MemberIdentifierType", IsRequired = false)]
		[XmlElement("MemberIdentifierType")]
		public GroupMemberIdentifierType? MemberIdentifierType { get; set; }

		[XmlArrayItem("String", typeof(string), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = true)]
		[DataMember(Name = "AddedMembers", IsRequired = false)]
		[XmlArray("AddedMembers")]
		public string[] AddedMembers { get; set; }

		[DataMember(Name = "RemovedMembers", IsRequired = false)]
		[XmlArray("RemovedMembers")]
		[XmlArrayItem("String", typeof(string), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = true)]
		public string[] RemovedMembers { get; set; }

		[DataMember(Name = "AddedPendingMembers", IsRequired = false)]
		[XmlArrayItem("String", typeof(string), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = true)]
		[XmlArray("AddedPendingMembers")]
		public string[] AddedPendingMembers { get; set; }

		[DataMember(Name = "RemovedPendingMembers", IsRequired = false)]
		[XmlArray("RemovedPendingMembers")]
		[XmlArrayItem("String", typeof(string), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = true)]
		public string[] RemovedPendingMembers { get; set; }

		[XmlElement("PermissionsVersion")]
		[DataMember(Name = "PermissionsVersion", IsRequired = false)]
		public int? PermissionsVersion { get; set; }

		internal SmtpAddress GroupPrimarySmtpAddress { get; set; }

		internal string DomainControllerOrNull
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.DomainController))
				{
					return null;
				}
				return this.DomainController;
			}
		}

		internal bool IsAddedMembersSpecified
		{
			get
			{
				return this.AddedMembers != null && this.AddedMembers.Length > 0;
			}
		}

		internal bool IsRemovedMembersSpecified
		{
			get
			{
				return this.RemovedMembers != null && this.RemovedMembers.Length > 0;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateGroupMailbox(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}

		internal override void Validate()
		{
			base.Validate();
			SmtpAddress groupPrimarySmtpAddress = new SmtpAddress(this.GroupSmtpAddress);
			if (!groupPrimarySmtpAddress.IsValidAddress)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSmtpAddressException(new LocalizedException(ServerStrings.InvalidSmtpAddress(this.GroupSmtpAddress))), FaultParty.Sender);
			}
			this.GroupPrimarySmtpAddress = groupPrimarySmtpAddress;
		}
	}
}
