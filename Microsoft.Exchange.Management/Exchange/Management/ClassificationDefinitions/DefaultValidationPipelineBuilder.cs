using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class DefaultValidationPipelineBuilder : IValidationPipelineBuilder
	{
		private static IList<IDataClassificationComplexityValidator> CreateDataClassificationComplexityValidatorsChain()
		{
			return new List<IDataClassificationComplexityValidator>
			{
				new RegexProcessorReferencesValidator(),
				new CustomKeywordProcessorReferencesValidator(),
				new FunctionProcessorReferencesValidator(),
				new AnyBlocksCountValidator(),
				new AnyBlocksDepthValidator()
			};
		}

		public void BuildCoreValidators()
		{
			if (this.areCoreValidatorsBuilt)
			{
				return;
			}
			Dictionary<TextProcessorType, TextProcessorGrouping> oobProcessorsGroupedByType = TextProcessorUtils.OobProcessorsGroupedByType;
			List<IClassificationRuleCollectionValidator> collection = new List<IClassificationRuleCollectionValidator>
			{
				new ClassificationRuleCollectionIdentifierValidator(),
				new OperationTypeValidator(),
				new ClassificationRuleCollectionVersionValidator(),
				new NameUniquenessValidator(),
				new DataClassificationIdentifierValidator(),
				new TextProcessorIdAndMatchReferencesValidator(oobProcessorsGroupedByType),
				new ClassificationRuleCollectionLocalizedInfoValidator(),
				new DataClassificationLocalizedInfoValidator(),
				new ComplexityValidator(oobProcessorsGroupedByType, DefaultValidationPipelineBuilder.CreateDataClassificationComplexityValidatorsChain()),
				new RegexProcessorsValidator(),
				new KeywordProcessorsValidator(),
				new FingerprintProcessorsValidator(),
				new ClassificationRuleCollectionRuntimeValidator()
			};
			this.validationPipeline.AddRange(collection);
			this.areCoreValidatorsBuilt = true;
		}

		public void BuildSupplementaryValidators()
		{
		}

		public IEnumerable<IClassificationRuleCollectionValidator> Result
		{
			get
			{
				return this.validationPipeline;
			}
		}

		private bool areCoreValidatorsBuilt;

		private readonly List<IClassificationRuleCollectionValidator> validationPipeline = new List<IClassificationRuleCollectionValidator>();
	}
}
