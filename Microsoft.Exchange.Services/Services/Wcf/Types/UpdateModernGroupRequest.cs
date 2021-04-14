using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "UpdateModernGroupRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateModernGroupRequest : ComposeModernGroupRequestBase
	{
		public UpdateModernGroupRequest(bool isOwner)
		{
			this.IsOwner = isOwner;
		}

		[DataMember(Name = "SmtpAddress", IsRequired = false)]
		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
			set
			{
				this.smtpAddress = value;
			}
		}

		[DataMember(Name = "DeletedMembers", IsRequired = false)]
		public string[] DeletedMembers
		{
			get
			{
				return this.deletedMembers ?? new string[0];
			}
			set
			{
				this.deletedMembers = value;
			}
		}

		[DataMember(Name = "DeletedOwners", IsRequired = false)]
		public string[] DeletedOwners
		{
			get
			{
				return this.deletedOwners ?? new string[0];
			}
			set
			{
				this.deletedOwners = value;
			}
		}

		[DataMember(Name = "RequireSenderAuthenticationEnabled", IsRequired = false)]
		public bool? RequireSenderAuthenticationEnabled { get; set; }

		[DataMember(Name = "AutoSubscribeNewGroupMembers", IsRequired = false)]
		public bool? AutoSubscribeNewGroupMembers { get; set; }

		[DataMember(Name = "CultureId", IsRequired = false)]
		public string CultureId { get; set; }

		[DataMember(Name = "IsOwner", IsRequired = true)]
		public bool IsOwner { get; private set; }

		public ProxyAddress ProxyAddress
		{
			get
			{
				if (this.proxyAddress == null)
				{
					this.proxyAddress = new SmtpProxyAddress(this.smtpAddress, true);
				}
				return this.proxyAddress;
			}
		}

		internal CultureInfo Language { get; private set; }

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override void Validate()
		{
			if (string.IsNullOrEmpty(this.SmtpAddress))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (base.AddedMembers == null)
			{
				base.AddedMembers = new string[0];
			}
			if (base.AddedOwners == null)
			{
				base.AddedOwners = new string[0];
			}
			if (this.DeletedMembers == null)
			{
				this.DeletedMembers = new string[0];
			}
			if (this.DeletedOwners == null)
			{
				this.DeletedOwners = new string[0];
			}
			if (this.CultureId != null)
			{
				try
				{
					this.Language = CultureInfo.CreateSpecificCulture(this.CultureId);
				}
				catch (CultureNotFoundException innerException)
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(innerException), FaultParty.Sender);
				}
			}
			if (base.Name != null)
			{
				base.Name = base.Name.Trim();
			}
			if (base.Description != null)
			{
				base.Description = base.Description.Trim();
			}
			string[] second = base.AddedMembers.Intersect(this.DeletedMembers).ToArray<string>();
			base.AddedMembers = base.AddedMembers.Distinct<string>().Except(second).ToArray<string>();
			this.DeletedMembers = this.DeletedMembers.Distinct<string>().Except(second).ToArray<string>();
			string[] second2 = base.AddedOwners.Intersect(this.DeletedOwners).ToArray<string>();
			base.AddedOwners = base.AddedOwners.Distinct<string>().Except(second2).ToArray<string>();
			this.DeletedOwners = this.DeletedOwners.Distinct<string>().Except(second2).ToArray<string>();
		}

		private string smtpAddress;

		private ProxyAddress proxyAddress;

		private string[] deletedMembers;

		private string[] deletedOwners;
	}
}
