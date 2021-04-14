using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal interface IValidationPipelineBuilder
	{
		void BuildCoreValidators();

		void BuildSupplementaryValidators();

		IEnumerable<IClassificationRuleCollectionValidator> Result { get; }
	}
}
