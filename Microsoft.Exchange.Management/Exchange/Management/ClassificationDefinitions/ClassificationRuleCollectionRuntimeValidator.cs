using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class ClassificationRuleCollectionRuntimeValidator : IClassificationRuleCollectionValidator
	{
		private static bool HandleClassificationEngineValidatorException(Exception exception, OrganizationId currentOrganizationId)
		{
			COMException ex = exception as COMException;
			if (ex == null)
			{
				return false;
			}
			int errorCode = ex.ErrorCode;
			if (errorCode == -2147220981)
			{
				throw new ClassificationRuleCollectionValidationException(Strings.ClassificationRuleCollectionEngineValidationFailure, ex);
			}
			if (errorCode == -2147220978)
			{
				List<string> list = (List<string>)ex.Data[ClassificationEngineValidator.BadRegexesKey];
				string names = string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list);
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionRegexValidationException, List<string>>(new ClassificationRuleCollectionRegexValidationException(Strings.ClassificationRuleCollectionRegexValidationFailure(names), ex), list);
			}
			int errorCode2 = ex.ErrorCode;
			ClassificationDefinitionsDiagnosticsReporter.Instance.WriteClassificationEngineUnexpectedFailureInValidation(0, currentOrganizationId, errorCode2);
			throw new ClassificationRuleCollectionInternalValidationException(errorCode2, ex);
		}

		private static void ValidateRulePackageContentsAgainstMce(OrganizationId currentOrganizationId, XDocument rulePackXDocument, string rulePackContents)
		{
			Task task = Task.Factory.StartNew(delegate()
			{
				ClassificationEngineValidator.ValidateRulePackage(rulePackXDocument, rulePackContents);
			});
			try
			{
				task.Wait(30000);
			}
			catch (AggregateException ex)
			{
				ex.Handle((Exception exception) => ClassificationRuleCollectionRuntimeValidator.HandleClassificationEngineValidatorException(exception, currentOrganizationId));
			}
			if (task.Status != TaskStatus.RanToCompletion)
			{
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteClassificationEngineTimeoutInValidation(0, currentOrganizationId, 30000);
				throw new ClassificationRuleCollectionTimeoutException();
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			if (context.IsPayloadOobRuleCollection)
			{
				return;
			}
			string text = context.ValidatedRuleCollectionDocument ?? XmlProcessingUtils.XDocumentToStringWithDeclaration(rulePackXDocument);
			ClassificationRuleCollectionRuntimeValidator.ValidateRulePackageContentsAgainstMce(context.CurrentOrganizationId, rulePackXDocument, text);
			if (context.ValidatedRuleCollectionDocument == null)
			{
				context.ValidatedRuleCollectionDocument = text;
			}
		}

		private const int ValidateRulePackageTimeout = 30000;
	}
}
