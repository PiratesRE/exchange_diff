using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OnPremisesOrganization", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOnPremisesOrganization : SetSystemConfigurationObjectTask<OnPremisesOrganizationIdParameter, OnPremisesOrganization>
	{
		[Parameter(Mandatory = false)]
		public InboundConnectorIdParameter InboundConnector
		{
			get
			{
				return (InboundConnectorIdParameter)base.Fields[OnPremisesOrganizationSchema.InboundConnectorLink];
			}
			set
			{
				base.Fields[OnPremisesOrganizationSchema.InboundConnectorLink] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OutboundConnectorIdParameter OutboundConnector
		{
			get
			{
				return (OutboundConnectorIdParameter)base.Fields[OnPremisesOrganizationSchema.OutboundConnectorLink];
			}
			set
			{
				base.Fields[OnPremisesOrganizationSchema.OutboundConnectorLink] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OrganizationName
		{
			get
			{
				return (string)base.Fields[OnPremisesOrganizationSchema.OrganizationName];
			}
			set
			{
				base.Fields[OnPremisesOrganizationSchema.OrganizationName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationRelationshipIdParameter OrganizationRelationship
		{
			get
			{
				return (OrganizationRelationshipIdParameter)base.Fields[OnPremisesOrganizationSchema.OrganizationRelationshipLink];
			}
			set
			{
				base.Fields[OnPremisesOrganizationSchema.OrganizationRelationshipLink] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			OnPremisesOrganization onPremisesOrganization = (OnPremisesOrganization)this.GetDynamicParameters();
			if (base.Fields.IsModified(OnPremisesOrganizationSchema.InboundConnectorLink))
			{
				onPremisesOrganization.InboundConnector = this.GetLinkedDataObject<InboundConnectorIdParameter, TenantInboundConnector>(this.InboundConnector, new Func<InboundConnectorIdParameter, LocalizedString>(Strings.OnPremisesOrganizationInboundConnectorNotExists), new Func<InboundConnectorIdParameter, LocalizedString>(Strings.OnPremisesOrganizationInboundConnectorNotUnique), new Func<string, InboundConnectorIdParameter>(InboundConnectorIdParameter.Parse));
			}
			if (base.Fields.IsModified(OnPremisesOrganizationSchema.OutboundConnectorLink))
			{
				onPremisesOrganization.OutboundConnector = this.GetLinkedDataObject<OutboundConnectorIdParameter, TenantOutboundConnector>(this.OutboundConnector, new Func<OutboundConnectorIdParameter, LocalizedString>(Strings.OnPremisesOrganizationOutboundConnectorNotExists), new Func<OutboundConnectorIdParameter, LocalizedString>(Strings.OnPremisesOrganizationOutboundConnectorNotUnique), new Func<string, OutboundConnectorIdParameter>(OutboundConnectorIdParameter.Parse));
			}
			if (base.Fields.IsModified(OnPremisesOrganizationSchema.OrganizationName))
			{
				onPremisesOrganization.OrganizationName = (string)base.Fields[OnPremisesOrganizationSchema.OrganizationName];
			}
			if (base.Fields.IsModified(OnPremisesOrganizationSchema.OrganizationRelationshipLink))
			{
				onPremisesOrganization.OrganizationRelationship = this.GetLinkedDataObject<OrganizationRelationshipIdParameter, OrganizationRelationship>(this.OrganizationRelationship, new Func<OrganizationRelationshipIdParameter, LocalizedString>(Strings.OnPremisesOrganizationOrganizationRelationshipNotExists), new Func<OrganizationRelationshipIdParameter, LocalizedString>(Strings.OnPremisesOrganizationOrganizationRelationshipNotUnique), new Func<string, OrganizationRelationshipIdParameter>(OrganizationRelationshipIdParameter.Parse));
			}
			TaskLogger.LogExit();
		}

		private ADObjectId GetLinkedDataObject<TLinkedIdentity, TLinkedDataObject>(TLinkedIdentity idParameter, Func<TLinkedIdentity, LocalizedString> notExists, Func<TLinkedIdentity, LocalizedString> notUnique, Func<string, TLinkedIdentity> createId) where TLinkedIdentity : ADIdParameter, new() where TLinkedDataObject : ADObject, new()
		{
			if (idParameter != null)
			{
				if (idParameter.InternalADObjectId == null && !idParameter.RawIdentity.Contains("\\"))
				{
					string arg = base.CurrentOrganizationId.ConfigurationUnit.ToString();
					idParameter = createId(string.Format("{0}\\{1}", arg, idParameter.ToString()));
				}
				TLinkedDataObject tlinkedDataObject = (TLinkedDataObject)((object)base.GetDataObject<TLinkedDataObject>(idParameter, base.GlobalConfigSession, this.RootId, new LocalizedString?(notExists(idParameter)), new LocalizedString?(notUnique(idParameter)), ExchangeErrorCategory.Client));
				return (ADObjectId)tlinkedDataObject.Identity;
			}
			return null;
		}
	}
}
