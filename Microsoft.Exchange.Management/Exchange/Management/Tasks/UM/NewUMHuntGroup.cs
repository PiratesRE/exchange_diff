using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("New", "UMHuntGroup", SupportsShouldProcess = true)]
	public sealed class NewUMHuntGroup : NewMultitenancySystemConfigurationObjectTask<UMHuntGroup>
	{
		[Parameter(Mandatory = false)]
		public string PilotIdentifier
		{
			get
			{
				return this.DataObject.PilotIdentifier;
			}
			set
			{
				this.DataObject.PilotIdentifier = value;
			}
		}

		[Parameter(Mandatory = true)]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public UMIPGatewayIdParameter UMIPGateway
		{
			get
			{
				return (UMIPGatewayIdParameter)base.Fields["IPGateway"];
			}
			set
			{
				base.Fields["IPGateway"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewUMHuntGroup(base.Name.ToString(), this.PilotIdentifier.ToString(), this.UMDialPlan.ToString(), this.UMIPGateway.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			UMHuntGroup umhuntGroup = (UMHuntGroup)base.PrepareDataObject();
			IConfigurationSession session = (IConfigurationSession)base.DataSession;
			UMDialPlanIdParameter umdialPlan = this.UMDialPlan;
			UMDialPlan umdialPlan2 = (UMDialPlan)base.GetDataObject<UMDialPlan>(umdialPlan, session, this.RootId, new LocalizedString?(Strings.NonExistantDialPlan(umdialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(umdialPlan.ToString())));
			umhuntGroup.UMDialPlan = umdialPlan2.Id;
			if (umdialPlan2.URIType == UMUriType.SipName && !VariantConfiguration.InvariantNoFlightingSnapshot.UM.HuntGroupCreationForSipDialplans.Enabled)
			{
				base.WriteError(new CannotCreateHuntGroupForHostedSipDialPlanException(), ErrorCategory.InvalidOperation, umhuntGroup);
			}
			UMIPGatewayIdParameter umipgateway = this.UMIPGateway;
			UMIPGateway umipgateway2 = (UMIPGateway)base.GetDataObject<UMIPGateway>(umipgateway, session, this.RootId, new LocalizedString?(Strings.NonExistantIPGateway(umipgateway.ToString())), new LocalizedString?(Strings.MultipleIPGatewaysWithSameId(umipgateway.ToString())));
			bool flag = false;
			foreach (UMHuntGroup umhuntGroup2 in umipgateway2.HuntGroups)
			{
				if (umhuntGroup2.PilotIdentifier == umhuntGroup.PilotIdentifier)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				HuntGroupAlreadyExistsException exception = new HuntGroupAlreadyExistsException(umipgateway2.Name, umhuntGroup.PilotIdentifier);
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			else
			{
				umhuntGroup.SetId(umipgateway2.Id.GetChildId(base.Name));
			}
			TaskLogger.LogExit();
			return umhuntGroup;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_NewHuntGroupCreated, null, new object[]
				{
					this.DataObject.DistinguishedName,
					this.DataObject.PilotIdentifier,
					this.DataObject.UMDialPlan.Name
				});
			}
			TaskLogger.LogExit();
		}
	}
}
