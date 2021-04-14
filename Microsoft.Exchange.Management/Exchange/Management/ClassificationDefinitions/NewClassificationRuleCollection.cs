using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml.Linq;
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
	[Cmdlet("New", "ClassificationRuleCollection", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "ArbitraryCollection")]
	public sealed class NewClassificationRuleCollection : NewMultitenancyFixedNameSystemConfigurationObjectTask<TransportRule>
	{
		[Parameter(ParameterSetName = "ArbitraryCollection", Mandatory = true, ValueFromPipeline = true, Position = 0)]
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

		[Parameter(ParameterSetName = "ArbitraryCollection", Mandatory = false, ValueFromPipeline = false)]
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

		[Parameter(ParameterSetName = "OutOfBoxInstall", Mandatory = true)]
		public SwitchParameter InstallDefaultCollection
		{
			get
			{
				return (SwitchParameter)(base.Fields["InstallDefaultCollection"] ?? false);
			}
			set
			{
				base.Fields["InstallDefaultCollection"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.FileData != null)
				{
					return Strings.ConfirmationMessageNewClassificationRuleCollection(this.localizedName);
				}
				return LocalizedString.Empty;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TransportRule transportRule;
			if (this.InstallDefaultCollection && this.existingRulePack != null)
			{
				transportRule = this.existingRulePack;
			}
			else
			{
				transportRule = (TransportRule)base.PrepareDataObject();
				transportRule.SetId(ClassificationDefinitionUtils.GetClassificationRuleCollectionContainerId(base.DataSession).GetChildId(this.rulePackageIdentifier));
				transportRule.OrganizationId = base.CurrentOrganizationId;
			}
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
			transportRule.Xml = null;
			transportRule.ReplicationSignature = replicationSignature;
			TaskLogger.LogExit();
			return transportRule;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.InstallDefaultCollection)
			{
				using (Stream stream = ClassificationDefinitionUtils.LoadStreamFromEmbeddedResource("DefaultClassificationDefinitions.xml"))
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, Convert.ToInt32(stream.Length));
					this.FileData = array;
					goto IL_71;
				}
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				this.ValidateOperationScope();
			}
			IL_71:
			byte[] rulePackageRawData = null;
			byte[] array2 = null;
			try
			{
				this.isEncrypted = RulePackageDecrypter.DecryptRulePackage(this.FileData, out rulePackageRawData, out array2);
			}
			catch (Exception innerException)
			{
				base.WriteError(new ClassificationRuleCollectionDecryptionException(innerException), ErrorCategory.InvalidData, null);
			}
			if (this.isEncrypted)
			{
				ExAssert.RetailAssert(!this.InstallDefaultCollection, "Installation of encrypted default OOB rule pack is not supported due to versioning!");
				string text = this.ValidateAndReadMetadata(rulePackageRawData);
				this.FileData = ((text == null) ? null : array2);
			}
			else
			{
				string text2 = this.ValidateAndReadMetadata(this.FileData);
				this.FileData = ((text2 == null) ? null : Encoding.Unicode.GetBytes(text2));
			}
			if (this.FileData != null)
			{
				base.InternalValidate();
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.FileData == null)
			{
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			IConfigurable result2 = ClassificationRuleCollectionPresentationObject.Create((TransportRule)result, this.rulePackVersion, this.rulePackageDetailsElement, this.isEncrypted);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		private void ValidateOperationScope()
		{
			ExAssert.RetailAssert(!this.InstallDefaultCollection, "Shouldn't validate New-ClassificationRuleCollection scope when installing default rule collection");
			bool flag = this.OutOfBoxCollection;
			bool flag2 = OrganizationId.ForestWideOrgId.Equals(base.CurrentOrganizationId);
			if (flag && !flag2)
			{
				base.WriteError(new ClassificationRuleCollectionIllegalScopeException(Strings.ClassificationRuleCollectionIllegalScopedNewOobOperation), ErrorCategory.InvalidOperation, null);
				return;
			}
			if (!flag && flag2)
			{
				base.WriteError(new ClassificationRuleCollectionIllegalScopeException(Strings.ClassificationRuleCollectionIllegalScopedNewOperation), ErrorCategory.InvalidOperation, null);
			}
		}

		private string ValidateAndReadMetadata(byte[] rulePackageRawData)
		{
			string result = null;
			try
			{
				XDocument xdocument = XmlProcessingUtils.ValidateRulePackageXmlContentsLite(rulePackageRawData);
				this.rulePackageIdentifier = XmlProcessingUtils.GetRulePackId(xdocument);
				this.rulePackVersion = (this.InstallDefaultCollection ? XmlProcessingUtils.SetRulePackVersionFromAssemblyFileVersion(xdocument) : XmlProcessingUtils.GetRulePackVersion(xdocument));
				ClassificationRuleCollectionIdParameter classificationRuleCollectionIdParameter = ClassificationRuleCollectionIdParameter.Parse("*");
				classificationRuleCollectionIdParameter.ShouldIncludeOutOfBoxCollections = true;
				List<TransportRule> list = base.GetDataObjects<TransportRule>(classificationRuleCollectionIdParameter, base.DataSession, ClassificationDefinitionUtils.GetClassificationRuleCollectionContainerId(base.DataSession)).ToList<TransportRule>();
				this.existingRulePack = list.FirstOrDefault((TransportRule transportRule) => transportRule.Name.Equals(this.rulePackageIdentifier, StringComparison.OrdinalIgnoreCase));
				this.validationContext = new ValidationContext(this.InstallDefaultCollection ? ClassificationRuleCollectionOperationType.ImportOrUpdate : ClassificationRuleCollectionOperationType.Import, base.CurrentOrganizationId, this.InstallDefaultCollection || (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && this.OutOfBoxCollection), false, (IConfigurationSession)base.DataSession, this.existingRulePack, null, null);
				if (this.validationContext.DcValidationConfig != null && list.Count >= this.validationContext.DcValidationConfig.MaxRulePackages)
				{
					base.WriteError(new ClassificationRuleCollectionNumberExceedLimit(this.validationContext.DcValidationConfig.MaxRulePackages), ErrorCategory.InvalidOperation, null);
				}
				result = ClassificationRuleCollectionValidationUtils.ValidateRulePackageContents(this.validationContext, xdocument);
				this.rulePackageDetailsElement = XmlProcessingUtils.GetRulePackageMetadataElement(xdocument);
				ClassificationRuleCollectionLocalizableDetails classificationRuleCollectionLocalizableDetails = XmlProcessingUtils.ReadDefaultRulePackageMetadata(this.rulePackageDetailsElement);
				this.defaultName = classificationRuleCollectionLocalizableDetails.Name;
				ClassificationRuleCollectionLocalizableDetails classificationRuleCollectionLocalizableDetails2 = XmlProcessingUtils.ReadRulePackageMetadata(this.rulePackageDetailsElement, CultureInfo.CurrentCulture);
				this.localizedName = ((classificationRuleCollectionLocalizableDetails2 != null && classificationRuleCollectionLocalizableDetails2.Name != null) ? classificationRuleCollectionLocalizableDetails2.Name : this.defaultName);
			}
			catch (ClassificationRuleCollectionVersionValidationException ex)
			{
				this.WriteWarning(ex.LocalizedString);
			}
			catch (ClassificationRuleCollectionAlreadyExistsException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (ClassificationRuleCollectionInternalValidationException ex2)
			{
				base.WriteError(ex2, (-2147287038 == ex2.Error) ? ErrorCategory.ObjectNotFound : ErrorCategory.InvalidResult, null);
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

		private string rulePackageIdentifier;

		private Version rulePackVersion;

		private XElement rulePackageDetailsElement;

		private string defaultName;

		private string localizedName;

		private bool isEncrypted;

		private ValidationContext validationContext;

		private TransportRule existingRulePack;
	}
}
