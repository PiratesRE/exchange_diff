using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxAuditBypassAssociation", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxAuditBypassAssociation : GetRecipientBase<MailboxAuditBypassAssociationIdParameter, ADRecipient>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MailboxAuditBypassAssociationSchema>();
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return null;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADRecipient dataObject2 = (ADRecipient)dataObject;
			return MailboxAuditBypassAssociation.FromDataObject(dataObject2);
		}

		internal new OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return null;
			}
		}

		internal new string Anr
		{
			get
			{
				return null;
			}
		}

		internal new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return new SwitchParameter(false);
			}
		}

		internal new PSCredential Credential
		{
			get
			{
				return null;
			}
		}

		internal new string Filter
		{
			get
			{
				return null;
			}
		}

		internal new string SortBy
		{
			get
			{
				return null;
			}
		}

		internal new SwitchParameter ReadFromDomainController
		{
			get
			{
				return new SwitchParameter(false);
			}
		}

		internal new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
