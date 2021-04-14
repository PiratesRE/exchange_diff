using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("New", "DataClassification", SupportsShouldProcess = true)]
	public sealed class NewDataClassification : NewMultitenancyFixedNameSystemConfigurationObjectTask<TransportRule>
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateLength(1, 256)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateLength(1, 256)]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter]
		public CultureInfo Locale
		{
			get
			{
				return (CultureInfo)base.Fields["Locale"];
			}
			set
			{
				base.Fields["Locale"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<Fingerprint> Fingerprints
		{
			get
			{
				return (MultiValuedProperty<Fingerprint>)base.Fields["Fingerprints"];
			}
			set
			{
				base.Fields["Fingerprints"] = value;
			}
		}

		[Parameter]
		public ClassificationRuleCollectionIdParameter ClassificationRuleCollectionIdentity
		{
			get
			{
				return (ClassificationRuleCollectionIdParameter)base.Fields["ClassificationRuleCollectionIdentity"];
			}
			set
			{
				base.Fields["ClassificationRuleCollectionIdentity"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewDataClassification(this.Name);
			}
		}

		protected override void InternalValidate()
		{
			this.ClassificationRuleCollectionIdentity = ClassificationRuleCollectionIdParameter.Parse("00000000-0000-0000-0001-000000000001");
			if (this.Locale == null)
			{
				this.Locale = CultureInfo.CurrentCulture;
			}
			base.InternalValidate();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = null;
			try
			{
				transportRule = this.TryGetDataObject();
				XDocument rulePackXDoc;
				if (transportRule == null)
				{
					transportRule = (TransportRule)base.PrepareDataObject();
					string rawIdentity = this.ClassificationRuleCollectionIdentity.RawIdentity;
					transportRule.SetId(ClassificationDefinitionUtils.GetClassificationRuleCollectionContainerId(base.DataSession).GetChildId(rawIdentity));
					transportRule.OrganizationId = base.CurrentOrganizationId;
					transportRule.Xml = null;
					string organizationId;
					string name;
					if (base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null)
					{
						organizationId = base.CurrentOrganizationId.OrganizationalUnit.ObjectGuid.ToString();
						name = base.CurrentOrganizationId.OrganizationalUnit.Name;
					}
					else
					{
						organizationId = base.CurrentOrgContainerId.ObjectGuid.ToString();
						name = base.CurrentOrgContainerId.DomainId.Name;
					}
					rulePackXDoc = ClassificationDefinitionUtils.CreateRuleCollectionDocumentFromTemplate(rawIdentity, organizationId, name);
				}
				else
				{
					rulePackXDoc = ClassificationDefinitionUtils.GetRuleCollectionDocumentFromTransportRule(transportRule);
				}
				this.implementation = new DataClassificationCmdletsImplementation(this);
				this.implementation.Initialize(transportRule, rulePackXDoc);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
			return transportRule;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.implementation.DataClassificationPresentationObject.SetDefaultResource(this.Locale, this.Name, this.Description);
			this.implementation.DataClassificationPresentationObject.Fingerprints = this.Fingerprints;
			ValidationContext validationContext = new ValidationContext(ClassificationRuleCollectionOperationType.ImportOrUpdate, base.CurrentOrganizationId, false, true, (IConfigurationSession)base.DataSession, null, null, null);
			this.implementation.Save(validationContext);
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = dataObject as TransportRule;
			if (transportRule != null)
			{
				XDocument ruleCollectionDocumentFromTransportRule = ClassificationDefinitionUtils.GetRuleCollectionDocumentFromTransportRule(transportRule);
				if (ruleCollectionDocumentFromTransportRule != null)
				{
					string[] ruleIdQueries = new string[]
					{
						((DataClassificationObjectId)this.implementation.DataClassificationPresentationObject.Identity).Name
					};
					List<QueryMatchResult> list = XmlProcessingUtils.GetMatchingRulesById(ruleCollectionDocumentFromTransportRule, ruleIdQueries).ToList<QueryMatchResult>();
					if (list.Count > 0)
					{
						ClassificationRuleCollectionPresentationObject rulePackPresentationObject = ClassificationRuleCollectionPresentationObject.Create(transportRule);
						DataClassificationPresentationObject result = DataClassificationPresentationObject.Create(list[0].MatchingRuleId, list[0].MatchingRuleXElement, list[0].MatchingResourceXElement, rulePackPresentationObject);
						this.WriteResult(result);
					}
				}
			}
			else
			{
				base.WriteResult(dataObject);
			}
			TaskLogger.LogExit();
		}

		private TransportRule TryGetDataObject()
		{
			TransportRule result = null;
			try
			{
				result = (TransportRule)base.GetDataObject(this.ClassificationRuleCollectionIdentity);
			}
			catch (ManagementObjectNotFoundException)
			{
			}
			return result;
		}

		private DataClassificationCmdletsImplementation implementation;
	}
}
