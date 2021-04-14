using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("New", "UMDialPlan", SupportsShouldProcess = true)]
	public sealed class NewUMDialPlan : NewMultitenancySystemConfigurationObjectTask<UMDialPlan>
	{
		[Parameter(Mandatory = true)]
		public int NumberOfDigitsInExtension
		{
			get
			{
				return this.DataObject.NumberOfDigitsInExtension;
			}
			set
			{
				this.DataObject.NumberOfDigitsInExtension = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMUriType URIType
		{
			get
			{
				return this.DataObject.URIType;
			}
			set
			{
				this.DataObject.URIType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSubscriberType SubscriberType
		{
			get
			{
				return this.DataObject.SubscriberType;
			}
			set
			{
				this.DataObject.SubscriberType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMVoIPSecurityType VoIPSecurity
		{
			get
			{
				return this.DataObject.VoIPSecurity;
			}
			set
			{
				this.DataObject.VoIPSecurity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AccessTelephoneNumbers
		{
			get
			{
				return this.DataObject.AccessTelephoneNumbers;
			}
			set
			{
				this.DataObject.AccessTelephoneNumbers = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FaxEnabled
		{
			get
			{
				return this.DataObject.FaxEnabled;
			}
			set
			{
				this.DataObject.FaxEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SipResourceIdentifierRequired
		{
			get
			{
				return this.DataObject.SipResourceIdentifierRequired;
			}
			set
			{
				this.DataObject.SipResourceIdentifierRequired = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DefaultOutboundCallingLineId
		{
			get
			{
				return this.DataObject.DefaultOutboundCallingLineId;
			}
			set
			{
				this.DataObject.DefaultOutboundCallingLineId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool GenerateUMMailboxPolicy
		{
			get
			{
				return (bool)base.Fields["GenerateUMMailboxPolicy"];
			}
			set
			{
				base.Fields["GenerateUMMailboxPolicy"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string CountryOrRegionCode
		{
			get
			{
				return this.DataObject.CountryOrRegionCode;
			}
			set
			{
				this.DataObject.CountryOrRegionCode = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMGlobalCallRoutingScheme GlobalCallRoutingScheme
		{
			get
			{
				return this.DataObject.GlobalCallRoutingScheme;
			}
			set
			{
				this.DataObject.GlobalCallRoutingScheme = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMLanguage DefaultLanguage
		{
			get
			{
				return this.DataObject.DefaultLanguage;
			}
			set
			{
				this.DataObject.DefaultLanguage = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUMDialPlan(base.Name.ToString(CultureInfo.InvariantCulture), this.NumberOfDigitsInExtension.ToString(CultureInfo.InvariantCulture));
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			UMDialPlan umdialPlan = (UMDialPlan)base.PrepareDataObject();
			umdialPlan.SetId((IConfigurationSession)base.DataSession, base.Name);
			return umdialPlan;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (string.IsNullOrEmpty(this.CountryOrRegionCode))
				{
					base.WriteError(new InvalidParameterException(Strings.EmptyCountryOrRegionCode), ErrorCategory.InvalidArgument, null);
				}
				if (base.Fields["GenerateUMMailboxPolicy"] == null || (bool)base.Fields["GenerateUMMailboxPolicy"])
				{
					this.defaultPolicyName = this.GetValidPolicyName();
					if (this.defaultPolicyName == null)
					{
						base.WriteError(new DefaultPolicyCreationException(string.Empty), ErrorCategory.InvalidArgument, null);
					}
					else if (this.defaultPolicyName.Length > 64)
					{
						base.WriteError(new DefaultPolicyCreationException(Strings.DefaultPolicyCreationNameTooLong(this.DataObject.Name)), ErrorCategory.InvalidArgument, null);
					}
				}
				if (!string.IsNullOrEmpty(this.DataObject.DefaultOutboundCallingLineId) && !Utils.IsUriValid(this.DataObject.DefaultOutboundCallingLineId, this.DataObject))
				{
					base.WriteError(new InvalidParameterException(Strings.InvalidDefaultOutboundCallingLineId), ErrorCategory.WriteError, this.DataObject);
				}
				if (!this.DataObject.IsModified(UMDialPlanSchema.GlobalCallRoutingScheme))
				{
					if (CommonConstants.UseDataCenterCallRouting)
					{
						this.GlobalCallRoutingScheme = UMGlobalCallRoutingScheme.GatewayGuid;
					}
					else
					{
						this.GlobalCallRoutingScheme = UMGlobalCallRoutingScheme.None;
					}
				}
				if (this.DataObject.IsModified(UMDialPlanSchema.DefaultLanguage) && !Utility.IsUMLanguageAvailable(this.DefaultLanguage))
				{
					base.WriteError(new InvalidParameterException(Strings.DefaultLanguageNotAvailable(this.DefaultLanguage.DisplayName)), ErrorCategory.WriteError, this.DataObject);
				}
				if (!this.DataObject.IsModified(UMDialPlanSchema.VoIPSecurity))
				{
					if (CommonConstants.UseDataCenterCallRouting)
					{
						this.VoIPSecurity = UMVoIPSecurityType.Secured;
					}
					else
					{
						this.VoIPSecurity = UMVoIPSecurityType.Unsecured;
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.CreateParentContainerIfNeeded(this.DataObject);
			if (CommonConstants.UseDataCenterCallRouting)
			{
				this.DataObject.PhoneContext = base.Name + "." + Guid.NewGuid().ToString("D");
			}
			else
			{
				this.DataObject.PhoneContext = base.Name + "." + ADForest.GetLocalForest().Fqdn;
			}
			if (this.DataObject.SubscriberType == UMSubscriberType.Consumer)
			{
				this.DataObject.CallSomeoneEnabled = false;
				this.DataObject.SendVoiceMsgEnabled = false;
			}
			this.DataObject.AudioCodec = AudioCodecEnum.Mp3;
			UMMailboxPolicy ummailboxPolicy = null;
			if (base.Fields["GenerateUMMailboxPolicy"] == null || (bool)base.Fields["GenerateUMMailboxPolicy"])
			{
				base.DataSession.Save(this.DataObject);
				ummailboxPolicy = this.AutoGeneratePolicy();
			}
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_NewDialPlanCreated, null, new object[]
				{
					base.Name
				});
			}
			else if (ummailboxPolicy != null)
			{
				base.DataSession.Delete(ummailboxPolicy);
				base.DataSession.Delete(this.DataObject);
			}
			TaskLogger.LogExit();
		}

		private UMMailboxPolicy AutoGeneratePolicy()
		{
			UMMailboxPolicy ummailboxPolicy = new UMMailboxPolicy();
			ummailboxPolicy.UMDialPlan = this.DataObject.Id;
			if (this.DataObject.SubscriberType == UMSubscriberType.Consumer)
			{
				ummailboxPolicy.AllowDialPlanSubscribers = false;
				ummailboxPolicy.AllowExtensions = false;
			}
			ADObjectId descendantId = base.CurrentOrgContainerId.GetDescendantId(new ADObjectId("CN=UM Mailbox Policies", Guid.Empty));
			AdName adName = new AdName("CN", this.defaultPolicyName);
			ADObjectId descendantId2 = descendantId.GetDescendantId(new ADObjectId(adName.ToString(), Guid.Empty));
			ummailboxPolicy.SetId(descendantId2);
			if (base.CurrentOrganizationId != null)
			{
				ummailboxPolicy.OrganizationId = base.CurrentOrganizationId;
			}
			else
			{
				ummailboxPolicy.OrganizationId = base.ExecutingUserOrganizationId;
			}
			ummailboxPolicy.SourceForestPolicyNames.Add(adName.EscapedName);
			base.CreateParentContainerIfNeeded(ummailboxPolicy);
			base.DataSession.Save(ummailboxPolicy);
			return ummailboxPolicy;
		}

		private string GetValidPolicyName()
		{
			int num = 0;
			bool flag = false;
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			string text = Strings.DefaultUMMailboxPolicyName(this.DataObject.Name).ToString();
			string text2 = text;
			do
			{
				if (configurationSession.FindMailboxPolicyByName<UMMailboxPolicy>(text2) == null)
				{
					flag = true;
				}
				else
				{
					num++;
					text2 = text + num.ToString(CultureInfo.InvariantCulture);
				}
			}
			while (!flag && num <= 10);
			if (flag)
			{
				return text2;
			}
			return null;
		}

		private const string GenerateUMMailboxPolicyField = "GenerateUMMailboxPolicy";

		private string defaultPolicyName;
	}
}
