using System;

namespace Microsoft.Exchange.Diagnostics.Components.Inference
{
	public static class ExTraceGlobals
	{
		public static Trace ImportanceAutoLabelerTracer
		{
			get
			{
				if (ExTraceGlobals.importanceAutoLabelerTracer == null)
				{
					ExTraceGlobals.importanceAutoLabelerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.importanceAutoLabelerTracer;
			}
		}

		public static Trace MdbInferenceModelDataBinderTracer
		{
			get
			{
				if (ExTraceGlobals.mdbInferenceModelDataBinderTracer == null)
				{
					ExTraceGlobals.mdbInferenceModelDataBinderTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mdbInferenceModelDataBinderTracer;
			}
		}

		public static Trace ImportanceClassifierTracer
		{
			get
			{
				if (ExTraceGlobals.importanceClassifierTracer == null)
				{
					ExTraceGlobals.importanceClassifierTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.importanceClassifierTracer;
			}
		}

		public static Trace FeatureVectorCalculatorTracer
		{
			get
			{
				if (ExTraceGlobals.featureVectorCalculatorTracer == null)
				{
					ExTraceGlobals.featureVectorCalculatorTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.featureVectorCalculatorTracer;
			}
		}

		public static Trace InferenceModelWriterTracer
		{
			get
			{
				if (ExTraceGlobals.inferenceModelWriterTracer == null)
				{
					ExTraceGlobals.inferenceModelWriterTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.inferenceModelWriterTracer;
			}
		}

		public static Trace MdbInferenceModelWriterTracer
		{
			get
			{
				if (ExTraceGlobals.mdbInferenceModelWriterTracer == null)
				{
					ExTraceGlobals.mdbInferenceModelWriterTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.mdbInferenceModelWriterTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace ImportanceTrainerTracer
		{
			get
			{
				if (ExTraceGlobals.importanceTrainerTracer == null)
				{
					ExTraceGlobals.importanceTrainerTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.importanceTrainerTracer;
			}
		}

		public static Trace SynchronousComponentBaseTracer
		{
			get
			{
				if (ExTraceGlobals.synchronousComponentBaseTracer == null)
				{
					ExTraceGlobals.synchronousComponentBaseTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.synchronousComponentBaseTracer;
			}
		}

		public static Trace ContactListIndexExtractorTracer
		{
			get
			{
				if (ExTraceGlobals.contactListIndexExtractorTracer == null)
				{
					ExTraceGlobals.contactListIndexExtractorTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.contactListIndexExtractorTracer;
			}
		}

		public static Trace NestedTrainingPipelineFeederTracer
		{
			get
			{
				if (ExTraceGlobals.nestedTrainingPipelineFeederTracer == null)
				{
					ExTraceGlobals.nestedTrainingPipelineFeederTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.nestedTrainingPipelineFeederTracer;
			}
		}

		public static Trace MdbInferenceModelLoaderTracer
		{
			get
			{
				if (ExTraceGlobals.mdbInferenceModelLoaderTracer == null)
				{
					ExTraceGlobals.mdbInferenceModelLoaderTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.mdbInferenceModelLoaderTracer;
			}
		}

		public static Trace MdbTrainingFeederTracer
		{
			get
			{
				if (ExTraceGlobals.mdbTrainingFeederTracer == null)
				{
					ExTraceGlobals.mdbTrainingFeederTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.mdbTrainingFeederTracer;
			}
		}

		public static Trace DwellEstimatorTracer
		{
			get
			{
				if (ExTraceGlobals.dwellEstimatorTracer == null)
				{
					ExTraceGlobals.dwellEstimatorTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.dwellEstimatorTracer;
			}
		}

		public static Trace ClassificationModelAccuracyCalculatorTracer
		{
			get
			{
				if (ExTraceGlobals.classificationModelAccuracyCalculatorTracer == null)
				{
					ExTraceGlobals.classificationModelAccuracyCalculatorTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.classificationModelAccuracyCalculatorTracer;
			}
		}

		public static Trace OrganizationContentExtractorTracer
		{
			get
			{
				if (ExTraceGlobals.organizationContentExtractorTracer == null)
				{
					ExTraceGlobals.organizationContentExtractorTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.organizationContentExtractorTracer;
			}
		}

		public static Trace NaturalLanguageExtractorTracer
		{
			get
			{
				if (ExTraceGlobals.naturalLanguageExtractorTracer == null)
				{
					ExTraceGlobals.naturalLanguageExtractorTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.naturalLanguageExtractorTracer;
			}
		}

		public static Trace ConversationPropertiesExtractorTracer
		{
			get
			{
				if (ExTraceGlobals.conversationPropertiesExtractorTracer == null)
				{
					ExTraceGlobals.conversationPropertiesExtractorTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.conversationPropertiesExtractorTracer;
			}
		}

		public static Trace InferenceLogWriterTracer
		{
			get
			{
				if (ExTraceGlobals.inferenceLogWriterTracer == null)
				{
					ExTraceGlobals.inferenceLogWriterTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.inferenceLogWriterTracer;
			}
		}

		public static Trace SamAccountExtractorTracer
		{
			get
			{
				if (ExTraceGlobals.samAccountExtractorTracer == null)
				{
					ExTraceGlobals.samAccountExtractorTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.samAccountExtractorTracer;
			}
		}

		public static Trace MetadataLoggerTracer
		{
			get
			{
				if (ExTraceGlobals.metadataLoggerTracer == null)
				{
					ExTraceGlobals.metadataLoggerTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.metadataLoggerTracer;
			}
		}

		public static Trace MetadataCollectionFeederTracer
		{
			get
			{
				if (ExTraceGlobals.metadataCollectionFeederTracer == null)
				{
					ExTraceGlobals.metadataCollectionFeederTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.metadataCollectionFeederTracer;
			}
		}

		public static Trace MetadataCollectionNestedPipelineFeederTracer
		{
			get
			{
				if (ExTraceGlobals.metadataCollectionNestedPipelineFeederTracer == null)
				{
					ExTraceGlobals.metadataCollectionNestedPipelineFeederTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.metadataCollectionNestedPipelineFeederTracer;
			}
		}

		public static Trace NestedSentItemsPipelineFeederTracer
		{
			get
			{
				if (ExTraceGlobals.nestedSentItemsPipelineFeederTracer == null)
				{
					ExTraceGlobals.nestedSentItemsPipelineFeederTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.nestedSentItemsPipelineFeederTracer;
			}
		}

		public static Trace RecipientExtractorTracer
		{
			get
			{
				if (ExTraceGlobals.recipientExtractorTracer == null)
				{
					ExTraceGlobals.recipientExtractorTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.recipientExtractorTracer;
			}
		}

		public static Trace ResultLoggerTracer
		{
			get
			{
				if (ExTraceGlobals.resultLoggerTracer == null)
				{
					ExTraceGlobals.resultLoggerTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.resultLoggerTracer;
			}
		}

		public static Trace PeopleRelevanceClassifierTracer
		{
			get
			{
				if (ExTraceGlobals.peopleRelevanceClassifierTracer == null)
				{
					ExTraceGlobals.peopleRelevanceClassifierTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.peopleRelevanceClassifierTracer;
			}
		}

		public static Trace RecipientCacheContactWriterTracer
		{
			get
			{
				if (ExTraceGlobals.recipientCacheContactWriterTracer == null)
				{
					ExTraceGlobals.recipientCacheContactWriterTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.recipientCacheContactWriterTracer;
			}
		}

		public static Trace InferenceModelLoggerTracer
		{
			get
			{
				if (ExTraceGlobals.inferenceModelLoggerTracer == null)
				{
					ExTraceGlobals.inferenceModelLoggerTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.inferenceModelLoggerTracer;
			}
		}

		public static Trace FolderPredictionClassifierTracer
		{
			get
			{
				if (ExTraceGlobals.folderPredictionClassifierTracer == null)
				{
					ExTraceGlobals.folderPredictionClassifierTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.folderPredictionClassifierTracer;
			}
		}

		public static Trace FolderPredictionTrainerTracer
		{
			get
			{
				if (ExTraceGlobals.folderPredictionTrainerTracer == null)
				{
					ExTraceGlobals.folderPredictionTrainerTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.folderPredictionTrainerTracer;
			}
		}

		public static Trace InferenceModelPrunerTracer
		{
			get
			{
				if (ExTraceGlobals.inferenceModelPrunerTracer == null)
				{
					ExTraceGlobals.inferenceModelPrunerTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.inferenceModelPrunerTracer;
			}
		}

		public static Trace ClassificationModelVersionSelectorTracer
		{
			get
			{
				if (ExTraceGlobals.classificationModelVersionSelectorTracer == null)
				{
					ExTraceGlobals.classificationModelVersionSelectorTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.classificationModelVersionSelectorTracer;
			}
		}

		public static Trace ConversationClutterInfoTracer
		{
			get
			{
				if (ExTraceGlobals.conversationClutterInfoTracer == null)
				{
					ExTraceGlobals.conversationClutterInfoTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.conversationClutterInfoTracer;
			}
		}

		public static Trace NoisyLabelMessageRemoverTracer
		{
			get
			{
				if (ExTraceGlobals.noisyLabelMessageRemoverTracer == null)
				{
					ExTraceGlobals.noisyLabelMessageRemoverTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.noisyLabelMessageRemoverTracer;
			}
		}

		public static Trace ActionAnalyzerTracer
		{
			get
			{
				if (ExTraceGlobals.actionAnalyzerTracer == null)
				{
					ExTraceGlobals.actionAnalyzerTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.actionAnalyzerTracer;
			}
		}

		public static Trace MessageDeduplicatorTracer
		{
			get
			{
				if (ExTraceGlobals.messageDeduplicatorTracer == null)
				{
					ExTraceGlobals.messageDeduplicatorTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.messageDeduplicatorTracer;
			}
		}

		private static Guid componentGuid = new Guid("A9CFCC80-4C92-4060-AE34-C78406D6D4EE");

		private static Trace importanceAutoLabelerTracer = null;

		private static Trace mdbInferenceModelDataBinderTracer = null;

		private static Trace importanceClassifierTracer = null;

		private static Trace featureVectorCalculatorTracer = null;

		private static Trace inferenceModelWriterTracer = null;

		private static Trace mdbInferenceModelWriterTracer = null;

		private static Trace serviceTracer = null;

		private static Trace importanceTrainerTracer = null;

		private static Trace synchronousComponentBaseTracer = null;

		private static Trace contactListIndexExtractorTracer = null;

		private static Trace nestedTrainingPipelineFeederTracer = null;

		private static Trace mdbInferenceModelLoaderTracer = null;

		private static Trace mdbTrainingFeederTracer = null;

		private static Trace dwellEstimatorTracer = null;

		private static Trace classificationModelAccuracyCalculatorTracer = null;

		private static Trace organizationContentExtractorTracer = null;

		private static Trace naturalLanguageExtractorTracer = null;

		private static Trace conversationPropertiesExtractorTracer = null;

		private static Trace inferenceLogWriterTracer = null;

		private static Trace samAccountExtractorTracer = null;

		private static Trace metadataLoggerTracer = null;

		private static Trace metadataCollectionFeederTracer = null;

		private static Trace metadataCollectionNestedPipelineFeederTracer = null;

		private static Trace nestedSentItemsPipelineFeederTracer = null;

		private static Trace recipientExtractorTracer = null;

		private static Trace resultLoggerTracer = null;

		private static Trace peopleRelevanceClassifierTracer = null;

		private static Trace recipientCacheContactWriterTracer = null;

		private static Trace inferenceModelLoggerTracer = null;

		private static Trace folderPredictionClassifierTracer = null;

		private static Trace folderPredictionTrainerTracer = null;

		private static Trace inferenceModelPrunerTracer = null;

		private static Trace classificationModelVersionSelectorTracer = null;

		private static Trace conversationClutterInfoTracer = null;

		private static Trace noisyLabelMessageRemoverTracer = null;

		private static Trace actionAnalyzerTracer = null;

		private static Trace messageDeduplicatorTracer = null;
	}
}
