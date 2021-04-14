using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class ValidationContext
	{
		internal ClassificationRuleCollectionOperationType OperationType { get; private set; }

		internal bool IsPayloadOobRuleCollection { get; private set; }

		internal bool IsPayloadFingerprintsRuleCollection { get; private set; }

		internal OrganizationId CurrentOrganizationId { get; private set; }

		internal DataClassificationConfig DcValidationConfig
		{
			get
			{
				if (this.organizationValidationConfig == null && !this.organizationValidationConfigRead)
				{
					this.organizationValidationConfig = this.classificationsDataReader.GetDataClassificationConfig(this.CurrentOrganizationId, this.dataSession);
					this.organizationValidationConfigRead = true;
				}
				return this.organizationValidationConfig;
			}
		}

		internal TransportRule ExistingRulePackDataObject { get; private set; }

		internal string ValidatedRuleCollectionDocument { get; set; }

		internal Dictionary<string, HashSet<string>> GetAllExistingClassificationIdentifiers(Func<TransportRule, bool> inclusiveFilter = null)
		{
			OrganizationId currentOrganizationId = this.CurrentOrganizationId;
			IConfigurationSession openedDataSession = this.dataSession;
			IClassificationDefinitionsDataReader dataReader = this.classificationsDataReader;
			IClassificationDefinitionsDiagnosticsReporter classificationDefinitionsDiagnosticsReporter = this.classificationDiagnosticsReporter;
			return DlpUtils.GetAllClassificationIdentifiers(currentOrganizationId, openedDataSession, inclusiveFilter, null, dataReader, classificationDefinitionsDiagnosticsReporter);
		}

		internal Version GetExistingRulePackVersion()
		{
			if (this.existingrulePackXDocument == null)
			{
				return null;
			}
			return XmlProcessingUtils.GetRulePackVersion(this.existingrulePackXDocument);
		}

		internal ISet<string> GetRuleIdentifiersFromExistingRulePack()
		{
			if (this.existingrulePackXDocument == null)
			{
				return null;
			}
			return new HashSet<string>(XmlProcessingUtils.GetAllRuleIds(this.existingrulePackXDocument), ClassificationDefinitionConstants.RuleIdComparer);
		}

		private void InitializeExistingRulePack(TransportRule existingRulePack)
		{
			this.ExistingRulePackDataObject = existingRulePack;
			if (ClassificationRuleCollectionOperationType.Update == this.OperationType)
			{
				ExAssert.RetailAssert(existingRulePack != null, "ValidationContext constructor must check that there's an existing rule pack data object for update operation.");
			}
			if (this.OperationType == ClassificationRuleCollectionOperationType.Import || existingRulePack == null)
			{
				return;
			}
			XDocument xdocument = null;
			try
			{
				xdocument = ClassificationDefinitionUtils.GetRuleCollectionDocumentFromTransportRule(existingRulePack);
			}
			catch (InvalidOperationException)
			{
				this.classificationDiagnosticsReporter.WriteInvalidObjectInformation(this.GetHashCode(), existingRulePack.OrganizationId, existingRulePack.DistinguishedName);
			}
			catch (ArgumentException underlyingException)
			{
				this.classificationDiagnosticsReporter.WriteCorruptRulePackageDiagnosticsInformation(this.GetHashCode(), existingRulePack.OrganizationId, existingRulePack.DistinguishedName, underlyingException);
			}
			catch (AggregateException ex)
			{
				this.classificationDiagnosticsReporter.WriteCorruptRulePackageDiagnosticsInformation(this.GetHashCode(), existingRulePack.OrganizationId, existingRulePack.DistinguishedName, ex.Flatten());
			}
			catch (XmlException ex2)
			{
				this.classificationDiagnosticsReporter.WriteCorruptRulePackageDiagnosticsInformation(this.GetHashCode(), existingRulePack.OrganizationId, existingRulePack.DistinguishedName, new AggregateException(new Exception[]
				{
					ex2
				}).Flatten());
			}
			this.existingrulePackXDocument = xdocument;
		}

		internal ValidationContext(ClassificationRuleCollectionOperationType operationType, OrganizationId currentOrganizationId, bool isPayloadOobRuleCollection, bool isPayloadFingerprintsRuleCollection, IConfigurationSession currentDataSession, IConfigurable existingDataObject = null, IClassificationDefinitionsDataReader dataReader = null, IClassificationDefinitionsDiagnosticsReporter diagnosticsReporter = null)
		{
			if (object.ReferenceEquals(null, currentOrganizationId))
			{
				throw new ArgumentNullException("currentOrganizationId");
			}
			TransportRule transportRule = existingDataObject as TransportRule;
			if (ClassificationRuleCollectionOperationType.Update == operationType && transportRule == null)
			{
				throw new ArgumentException("existingDataObject");
			}
			this.dataSession = currentDataSession;
			this.classificationsDataReader = (dataReader ?? ClassificationDefinitionsDataReader.DefaultInstance);
			this.classificationDiagnosticsReporter = (diagnosticsReporter ?? ClassificationDefinitionsDiagnosticsReporter.Instance);
			this.organizationValidationConfigRead = false;
			this.organizationValidationConfig = null;
			this.OperationType = operationType;
			this.CurrentOrganizationId = currentOrganizationId;
			this.IsPayloadOobRuleCollection = isPayloadOobRuleCollection;
			this.IsPayloadFingerprintsRuleCollection = isPayloadFingerprintsRuleCollection;
			this.InitializeExistingRulePack(transportRule);
			this.ValidatedRuleCollectionDocument = null;
		}

		private bool organizationValidationConfigRead;

		private DataClassificationConfig organizationValidationConfig;

		private readonly IConfigurationSession dataSession;

		private readonly IClassificationDefinitionsDataReader classificationsDataReader;

		private readonly IClassificationDefinitionsDiagnosticsReporter classificationDiagnosticsReporter;

		private XDocument existingrulePackXDocument;
	}
}
