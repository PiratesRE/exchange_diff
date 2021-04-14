using System;
using System.Xml.Linq;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class OperationTypeValidator : IClassificationRuleCollectionValidator
	{
		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			if (context.OperationType == ClassificationRuleCollectionOperationType.Import && context.ExistingRulePackDataObject != null)
			{
				throw new ClassificationRuleCollectionAlreadyExistsException();
			}
		}
	}
}
