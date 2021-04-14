using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("Set", "ClassificationRuleCollection", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetClassificationRuleCollection : SetClassificationRuleCollectionBase
	{
		[Parameter(ParameterSetName = "Identity", Mandatory = true, ValueFromPipeline = true, Position = 0)]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(ParameterSetName = "Identity", Mandatory = false, ValueFromPipeline = false)]
		public SwitchParameter OutOfBoxCollection
		{
			get
			{
				return (SwitchParameter)(base.Fields["OutOfBoxCollection"] ?? false);
			}
			set
			{
				base.Fields["OutOfBoxCollection"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = false)]
		[ValidateNotNullOrEmpty]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		private new ClassificationRuleCollectionIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetClassificationRuleCollection(this.localizedName);
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			byte[] array = null;
			byte[] array2 = null;
			bool flag = false;
			try
			{
				flag = RulePackageDecrypter.DecryptRulePackage(this.FileData, out array, out array2);
			}
			catch (Exception innerException)
			{
				base.WriteError(new ClassificationRuleCollectionDecryptionException(innerException), ErrorCategory.InvalidData, null);
			}
			XDocument xdocument = this.ValidateAgainstSchema(flag ? array : this.FileData);
			this.SetIdentityParameter(xdocument);
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = ClassificationDefinitionConstants.ClassificationDefinitionsRdn;
			}
			this.DataObject = (TransportRule)this.ResolveDataObject();
			if (base.HasErrors)
			{
				return;
			}
			ExAssert.RetailAssert(this.DataObject != null, "DataObject must not be null at this point.");
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				this.ValidateOperationScope();
			}
			string s = this.ValidateAgainstBusinessRulesAndReadMetadata(xdocument);
			this.FileData = (flag ? array2 : Encoding.Unicode.GetBytes(s));
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)dataObject;
			byte[] replicationSignature = null;
			try
			{
				replicationSignature = ClassificationRuleCollectionValidationUtils.PackAndValidateCompressedRulePackage(this.FileData, this.validationContext);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			transportRule.AdminDisplayName = this.defaultName;
			transportRule.ReplicationSignature = replicationSignature;
			base.StampChangesOn(dataObject);
			TaskLogger.LogEnter();
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			if (!object.ReferenceEquals(this.Organization, null))
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 306, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\ClassificationDefinitions\\SetClassificationRuleCollection.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.ExecutingUserOrganizationId;
		}

		private XDocument ValidateAgainstSchema(byte[] rulePackageRawData)
		{
			XDocument result = null;
			try
			{
				result = XmlProcessingUtils.ValidateRulePackageXmlContentsLite(rulePackageRawData);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			return result;
		}

		private void ValidateOperationScope()
		{
			bool flag = this.OutOfBoxCollection;
			bool flag2 = OrganizationId.ForestWideOrgId.Equals(this.DataObject.OrganizationId);
			if (flag && !flag2)
			{
				this.WriteWarning(Strings.ClassificationRuleCollectionIllegalScopedSetOperation);
				this.OutOfBoxCollection = false;
				return;
			}
			if (!flag && flag2)
			{
				base.WriteError(new ClassificationRuleCollectionIllegalScopeException(Strings.ClassificationRuleCollectionIllegalScopedSetOobOperation), ErrorCategory.InvalidOperation, null);
			}
		}

		private string ValidateAgainstBusinessRulesAndReadMetadata(XDocument rulePackXDoc)
		{
			ExAssert.RetailAssert(this.rulePackageIdentifier != null && this.DataObject != null, "Business rules validation in Set-ClassificationRuleCollection must take place after the DataObject resolution");
			string result = string.Empty;
			try
			{
				this.validationContext = new ValidationContext(ClassificationRuleCollectionOperationType.Update, base.CurrentOrganizationId, VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && this.OutOfBoxCollection, false, (IConfigurationSession)base.DataSession, this.DataObject, null, null);
				result = ClassificationRuleCollectionValidationUtils.ValidateRulePackageContents(this.validationContext, rulePackXDoc);
				ClassificationRuleCollectionLocalizableDetails classificationRuleCollectionLocalizableDetails = XmlProcessingUtils.ReadDefaultRulePackageMetadata(rulePackXDoc);
				this.defaultName = classificationRuleCollectionLocalizableDetails.Name;
				ClassificationRuleCollectionLocalizableDetails classificationRuleCollectionLocalizableDetails2 = XmlProcessingUtils.ReadRulePackageMetadata(rulePackXDoc, CultureInfo.CurrentCulture);
				this.localizedName = ((classificationRuleCollectionLocalizableDetails2 != null && classificationRuleCollectionLocalizableDetails2.Name != null) ? classificationRuleCollectionLocalizableDetails2.Name : this.defaultName);
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && OrganizationId.ForestWideOrgId.Equals(((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId))
				{
					this.WriteWarning(Strings.ClassificationRuleCollectionIneffectiveSharingViolationCheck);
				}
				SortedSet<string> sortedSet = new SortedSet<string>();
				IList<string> deletedRulesInUse = ClassificationRuleCollectionValidationUtils.GetDeletedRulesInUse(base.DataSession, this.DataObject, sortedSet, rulePackXDoc);
				if (deletedRulesInUse.Count > 0)
				{
					LocalizedString message = Strings.ClassificationRuleCollectionSharingViolationSetOperationVerbose(this.localizedName ?? this.rulePackageIdentifier, string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, deletedRulesInUse), string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, sortedSet));
					throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionSharingViolationException, IList<string>>(new ClassificationRuleCollectionSharingViolationException(message), deletedRulesInUse);
				}
			}
			catch (ClassificationRuleCollectionSharingViolationException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (ClassificationRuleCollectionInternalValidationException ex)
			{
				base.WriteError(ex, (-2147287038 == ex.Error) ? ErrorCategory.ObjectNotFound : ErrorCategory.InvalidResult, null);
			}
			catch (ClassificationRuleCollectionTimeoutException exception2)
			{
				base.WriteError(exception2, ErrorCategory.OperationTimeout, null);
			}
			catch (LocalizedException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidData, null);
			}
			return result;
		}

		private void SetIdentityParameter(XDocument rulePackXDoc)
		{
			this.rulePackageIdentifier = XmlProcessingUtils.GetRulePackId(rulePackXDoc);
			this.Identity = ClassificationRuleCollectionIdParameter.Parse(this.rulePackageIdentifier);
		}

		private string rulePackageIdentifier;

		private string defaultName;

		private string localizedName;

		private ValidationContext validationContext;
	}
}
