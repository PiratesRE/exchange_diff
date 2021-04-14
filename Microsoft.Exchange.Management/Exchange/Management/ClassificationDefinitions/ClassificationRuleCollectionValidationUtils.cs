using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal static class ClassificationRuleCollectionValidationUtils
	{
		internal static string ValidateRulePackageContents(ValidationContext context, XDocument rulePackXDocument)
		{
			IValidationPipelineBuilder validationPipelineBuilder = new DefaultValidationPipelineBuilder();
			validationPipelineBuilder.BuildCoreValidators();
			validationPipelineBuilder.BuildSupplementaryValidators();
			IEnumerable<IClassificationRuleCollectionValidator> result = validationPipelineBuilder.Result;
			foreach (IClassificationRuleCollectionValidator classificationRuleCollectionValidator in result)
			{
				classificationRuleCollectionValidator.Validate(context, rulePackXDocument);
			}
			string result2;
			if ((result2 = context.ValidatedRuleCollectionDocument) == null)
			{
				result2 = (context.ValidatedRuleCollectionDocument = XmlProcessingUtils.XDocumentToStringWithDeclaration(rulePackXDocument));
			}
			return result2;
		}

		internal static IList<string> GetDeletedRulesInUse(IConfigDataProvider dataSession, TransportRule existingRulePackObject, ISet<string> referencingRules = null, XDocument updatedRulePackXDoc = null)
		{
			ExAssert.RetailAssert(dataSession != null && existingRulePackObject != null, "The data session and rule package object to be updated / deleted must be specified in order to check if any deleted rules are in-use by ETR.");
			HashSet<string> hashSet;
			try
			{
				XDocument ruleCollectionDocumentFromTransportRule = ClassificationDefinitionUtils.GetRuleCollectionDocumentFromTransportRule(existingRulePackObject);
				hashSet = new HashSet<string>(XmlProcessingUtils.GetAllRuleIds(ruleCollectionDocumentFromTransportRule), ClassificationDefinitionConstants.RuleIdComparer);
			}
			catch (ArgumentException)
			{
				((ClassificationDefinitionsDiagnosticsReporter)ClassificationDefinitionsDiagnosticsReporter.Instance).Tracer.TraceWarning<string>(0L, "Deleting classification rule collection '{0}' with null compressed data.", existingRulePackObject.DistinguishedName);
				return Enumerable.Empty<string>().ToList<string>();
			}
			catch (AggregateException ex)
			{
				((ClassificationDefinitionsDiagnosticsReporter)ClassificationDefinitionsDiagnosticsReporter.Instance).Tracer.TraceWarning<string, string>(0L, "Deleting classification rule collection '{0}' with invalid compressed data or invalid decompressed data. Details: {1}", existingRulePackObject.DistinguishedName, ex.Flatten().ToString());
				return Enumerable.Empty<string>().ToList<string>();
			}
			catch (XmlException ex2)
			{
				((ClassificationDefinitionsDiagnosticsReporter)ClassificationDefinitionsDiagnosticsReporter.Instance).Tracer.TraceWarning<string, string>(0L, "Deleting classification rule collection '{0}' with invalid XML contents. Details: {1}", existingRulePackObject.DistinguishedName, new AggregateException(new Exception[]
				{
					ex2
				}).Flatten().ToString());
				return Enumerable.Empty<string>().ToList<string>();
			}
			if (updatedRulePackXDoc != null)
			{
				foreach (string item in XmlProcessingUtils.GetAllRuleIds(updatedRulePackXDoc))
				{
					hashSet.Remove(item);
				}
			}
			return ClassificationRuleCollectionValidationUtils.ProcessClassificationReferences(DlpUtils.GetDataClassificationsInUse(dataSession, hashSet, ClassificationDefinitionConstants.RuleIdComparer), referencingRules);
		}

		internal static byte[] PackAndValidateCompressedRulePackage(byte[] uncompressedSerializedRulePackageData, ValidationContext validationContext)
		{
			byte[] array;
			if (!ClassificationDefinitionUtils.TryCompressXmlBytes(uncompressedSerializedRulePackageData, out array))
			{
				throw new ClassificationRuleCollectionStorageException();
			}
			ByteQuantifiedSize value = (ByteQuantifiedSize)DataClassificationConfigSchema.MaxRulePackageSize.DefaultValue;
			if (validationContext != null && validationContext.DcValidationConfig != null)
			{
				value = validationContext.DcValidationConfig.MaxRulePackageSize;
			}
			ByteQuantifiedSize value2 = ByteQuantifiedSize.FromBytes((ulong)((long)array.Length));
			if (value2 > value)
			{
				throw new ClassificationRuleCollectionPayloadSizeExceededLimitException(value2.ToKB(), value.ToKB());
			}
			return array;
		}

		private static IList<string> ProcessClassificationReferences(ILookup<string, Rule> classificationReferencesGroups, ISet<string> referencingTransportRules)
		{
			if (referencingTransportRules != null)
			{
				referencingTransportRules.Clear();
			}
			IList<string> list = new List<string>();
			foreach (IGrouping<string, Rule> grouping in classificationReferencesGroups)
			{
				list.Add(grouping.Key);
				if (referencingTransportRules != null)
				{
					referencingTransportRules.UnionWith(from transportRule in grouping
					select transportRule.Name);
				}
			}
			return list;
		}
	}
}
