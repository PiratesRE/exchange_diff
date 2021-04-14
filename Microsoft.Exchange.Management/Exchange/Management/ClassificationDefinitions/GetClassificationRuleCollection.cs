using System;
using System.Management.Automation;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("Get", "ClassificationRuleCollection", DefaultParameterSetName = "Identity")]
	public sealed class GetClassificationRuleCollection : GetMultitenancySystemConfigurationObjectTask<ClassificationRuleCollectionIdParameter, TransportRule>
	{
		protected override ObjectId RootId
		{
			get
			{
				return null;
			}
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = ClassificationDefinitionConstants.ClassificationDefinitionsRdn;
			}
			if (this.Identity == null)
			{
				this.isIdentityArgumentSpecified = false;
				this.Identity = ClassificationRuleCollectionIdParameter.Parse("*");
			}
			this.Identity.ShouldIncludeOutOfBoxCollections = true;
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (ManagementObjectNotFoundException)
			{
				if (this.isIdentityArgumentSpecified)
				{
					throw;
				}
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)dataObject;
			ClassificationRuleCollectionPresentationObject dataObject2;
			try
			{
				dataObject2 = ExportableClassificationRuleCollectionPresentationObject.Create(transportRule);
			}
			catch (InvalidOperationException)
			{
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteInvalidObjectInformation(this.GetHashCode(), transportRule.OrganizationId, transportRule.DistinguishedName);
				TaskLogger.LogExit();
				return;
			}
			catch (ArgumentException underlyingException)
			{
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteCorruptRulePackageDiagnosticsInformation(this.GetHashCode(), transportRule.OrganizationId, transportRule.DistinguishedName, underlyingException);
				TaskLogger.LogExit();
				return;
			}
			catch (AggregateException ex)
			{
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteCorruptRulePackageDiagnosticsInformation(this.GetHashCode(), transportRule.OrganizationId, transportRule.DistinguishedName, ex.Flatten());
				TaskLogger.LogExit();
				return;
			}
			catch (XmlException ex2)
			{
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteCorruptRulePackageDiagnosticsInformation(this.GetHashCode(), transportRule.OrganizationId, transportRule.DistinguishedName, new AggregateException(new Exception[]
				{
					ex2
				}).Flatten());
				TaskLogger.LogExit();
				return;
			}
			base.WriteResult(dataObject2);
			TaskLogger.LogExit();
		}

		private bool isIdentityArgumentSpecified = true;
	}
}
