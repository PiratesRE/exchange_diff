using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InferenceTags
	{
		public const int ImportanceAutoLabeler = 0;

		public const int MdbInferenceModelDataBinder = 1;

		public const int ImportanceClassifier = 2;

		public const int FeatureVectorCalculator = 3;

		public const int InferenceModelWriter = 4;

		public const int MdbInferenceModelWriter = 5;

		public const int Service = 6;

		public const int ImportanceTrainer = 7;

		public const int SynchronousComponentBase = 8;

		public const int ContactListIndexExtractor = 9;

		public const int NestedTrainingPipelineFeeder = 10;

		public const int MdbInferenceModelLoader = 11;

		public const int MdbTrainingFeeder = 12;

		public const int DwellEstimator = 13;

		public const int ClassificationModelAccuracyCalculator = 14;

		public const int OrganizationContentExtractor = 15;

		public const int NaturalLanguageExtractor = 16;

		public const int ConversationPropertiesExtractor = 17;

		public const int InferenceLogWriter = 18;

		public const int SamAccountExtractor = 19;

		public const int MetadataLogger = 20;

		public const int MetadataCollectionFeeder = 21;

		public const int MetadataCollectionNestedPipelineFeeder = 22;

		public const int NestedSentItemsPipelineFeeder = 23;

		public const int RecipientExtractor = 24;

		public const int ResultLogger = 25;

		public const int PeopleRelevanceClassifier = 26;

		public const int RecipientCacheContactWriter = 27;

		public const int InferenceModelLogger = 28;

		public const int FolderPredictionClassifier = 29;

		public const int FolderPredictionTrainer = 30;

		public const int InferenceModelPruner = 31;

		public const int ClassificationModelVersionSelector = 32;

		public const int ConversationClutterInfo = 33;

		public const int NoisyLabelMessageRemover = 34;

		public const int ActionAnalyzer = 35;

		public const int MessageDeduplicator = 36;

		public static Guid guid = new Guid("A9CFCC80-4C92-4060-AE34-C78406D6D4EE");
	}
}
