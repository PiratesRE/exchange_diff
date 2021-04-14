using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Learning;
using Microsoft.Exchange.Inference.Learning.Schema;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Core.Pipeline;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class InferenceClassificationAgent : StoreDriverDeliveryAgent
	{
		public InferenceClassificationAgent(SmtpServer server, Pipeline pipeline, bool isEnabled, bool isPipelineEnabled, IDiagnosticsSession diagnosticsSession, InferenceClassificationAgentLogger classificationAgentLogger, InferenceClassificationComparisonLogger classificationComparisonLogger)
		{
			if (!isEnabled)
			{
				return;
			}
			this.isPipelineEnabled = isPipelineEnabled;
			this.pipeline = pipeline;
			this.server = server;
			this.modelConfiguration = ServerModelConfigurationWrapper.CurrentWrapper;
			this.diagnosticsSession = diagnosticsSession;
			this.classificationAgentLogger = classificationAgentLogger;
			this.classificationComparisonLogger = classificationComparisonLogger;
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
			base.OnDeliveredMessage += this.OnDeliveredMessageHandler;
			base.OnCompletedMessage += this.OnCompletedMessageHandler;
		}

		private static void CopyIfValid(PropertyDefinition propertyDefinition, IItem sourceItem, Dictionary<PropertyDefinition, object> targetDictionary)
		{
			object obj = sourceItem.TryGetProperty(propertyDefinition);
			if (!(obj is PropertyError))
			{
				targetDictionary[propertyDefinition] = obj;
			}
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			this.wasMessageDelivered = false;
			this.logValues = null;
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			Stopwatch stopwatch = Stopwatch.StartNew();
			InferencePropertyBag inferencePropertyBag = new InferencePropertyBag();
			MdbDocument mdbDocument = null;
			VariantConfigurationSnapshot variantConfigurationSnapshot = null;
			try
			{
				InferenceClassificationAgent.tracer.TraceDebug(0L, "Called InferenceClassificationAgent.OnPromotedMessageHandler");
				inferencePropertyBag.Add(InferenceSchema.MessageClassificationTime, ExDateTime.UtcNow);
				inferencePropertyBag.Add(InferenceSchema.ServerName, this.server.Name);
				if (storeDriverDeliveryEventArgsImpl.MailboxSession != null)
				{
					inferencePropertyBag.Add(DocumentSchema.MailboxId, storeDriverDeliveryEventArgsImpl.MailboxSession.MailboxGuid.ToString());
					inferencePropertyBag.Add(InferenceSchema.ClutterEnabled, storeDriverDeliveryEventArgsImpl.MailboxSession.Mailbox.GetValueOrDefault<bool>(MailboxSchema.InferenceClutterEnabled, false));
					inferencePropertyBag.Add(InferenceSchema.ClassificationEnabled, storeDriverDeliveryEventArgsImpl.MailboxSession.Mailbox.GetValueOrDefault<bool>(MailboxSchema.InferenceClassificationEnabled, false));
					inferencePropertyBag.Add(InferenceSchema.HasBeenClutterInvited, storeDriverDeliveryEventArgsImpl.MailboxSession.Mailbox.GetValueOrDefault<bool>(MailboxSchema.InferenceHasBeenClutterInvited, false));
				}
				inferencePropertyBag.Add(InferenceSchema.InternetMessageId, storeDriverDeliveryEventArgsImpl.ReplayItem.InternetMessageId);
				inferencePropertyBag.Add(InferenceSchema.Locale, InferenceClassificationAgent.GetLocale(storeDriverDeliveryEventArgsImpl));
				if (storeDriverDeliveryEventArgsImpl.MailboxOwner != null && storeDriverDeliveryEventArgsImpl.MailboxOwner.OrganizationId != null && storeDriverDeliveryEventArgsImpl.MailboxOwner.OrganizationId.OrganizationalUnit != null)
				{
					inferencePropertyBag.Add(InferenceSchema.TenantName, storeDriverDeliveryEventArgsImpl.MailboxOwner.OrganizationId.OrganizationalUnit.Name);
				}
				if (!InferenceClassificationAgent.IsMailboxInteresting(storeDriverDeliveryEventArgsImpl))
				{
					inferencePropertyBag.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Skipped);
					inferencePropertyBag.Add(InferenceSchema.ClassificationStatusMessage, "MailboxNotInteresting");
				}
				else if (!InferenceClassificationAgent.IsMessageInteresting(storeDriverDeliveryEventArgsImpl))
				{
					inferencePropertyBag.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Skipped);
					inferencePropertyBag.Add(InferenceSchema.ClassificationStatusMessage, "MessageNotInteresting");
				}
				else
				{
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies = (storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies ?? new Dictionary<PropertyDefinition, object>());
					Guid guid = Guid.NewGuid();
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies[MessageItemSchema.InferenceMessageIdentifier] = guid;
					inferencePropertyBag.Add(InferenceSchema.MessageIdentifier, guid);
					variantConfigurationSnapshot = FlightModule.GetFlightFeatures(storeDriverDeliveryEventArgsImpl.MailboxOwner);
					if (variantConfigurationSnapshot != null)
					{
						inferencePropertyBag.Add(InferenceSchema.UserFlightFeatures, variantConfigurationSnapshot);
					}
					IDeliveryClassificationStrategy deliveryClassificationStrategy = ClassificationStrategyFactory.Create(storeDriverDeliveryEventArgsImpl.MailboxSession, variantConfigurationSnapshot);
					inferencePropertyBag.Add(InferenceSchema.IsUiEnabled, deliveryClassificationStrategy != null);
					OriginalDeliveryFolderInfo originalDeliveryFolderInfo = InferenceClassificationAgent.GetOriginalDeliveryFolderInfo(storeDriverDeliveryEventArgsImpl);
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies[ItemSchema.OriginalDeliveryFolderInfo] = originalDeliveryFolderInfo.Serialize();
					inferencePropertyBag.Add(InferenceSchema.OriginalDeliveryFolder, originalDeliveryFolderInfo);
					if (!InferenceClassificationAgent.IsDeliveryFolderInteresting(originalDeliveryFolderInfo))
					{
						inferencePropertyBag.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Skipped);
						inferencePropertyBag.Add(InferenceSchema.ClassificationStatusMessage, "DeliveryFolderNotInteresting");
					}
					else if (!this.isPipelineEnabled)
					{
						inferencePropertyBag.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Failed);
						inferencePropertyBag.Add(InferenceSchema.ClassificationStatusMessage, "PipelineDisabled");
						InferenceClassificationProcessing.NumberOfItemsSkippedDueToDisabledPipeline.Increment();
					}
					else
					{
						ConversationClutterInformation conversationClutterInformation = null;
						bool flag = false;
						InferenceClassificationResult inferenceClassificationResult = InferenceClassificationResult.None;
						if (this.RunClassificationPipeline(storeDriverDeliveryEventArgsImpl, inferencePropertyBag, variantConfigurationSnapshot, out mdbDocument, out flag, out conversationClutterInformation))
						{
							if (flag)
							{
								inferenceClassificationResult |= InferenceClassificationResult.IsClutterModel;
							}
							string value = string.Empty;
							bool flag2 = false;
							bool flag3;
							InferenceClassificationResult inferenceClassificationResult2;
							string text;
							bool flag4;
							if (InferenceClassificationAgent.CheckForRuleAndConversationOverrides(storeDriverDeliveryEventArgsImpl.SharedPropertiesBetweenAgents, originalDeliveryFolderInfo, out flag3, out inferenceClassificationResult2, out text, out flag4))
							{
								flag = flag3;
								value = text;
							}
							inferenceClassificationResult |= inferenceClassificationResult2;
							if (flag4 && conversationClutterInformation != null)
							{
								flag = conversationClutterInformation.IsNewMessageClutter(flag, out flag2);
							}
							inferencePropertyBag.Add(InferenceSchema.IsClutter, flag);
							if (flag)
							{
								inferenceClassificationResult |= InferenceClassificationResult.IsClutterFinal;
							}
							if (deliveryClassificationStrategy != null)
							{
								deliveryClassificationStrategy.ApplyClassification(storeDriverDeliveryEventArgsImpl, inferenceClassificationResult);
								if (flag2 && conversationClutterInformation != null)
								{
									conversationClutterInformation.MarkItemsAsNotClutter();
								}
							}
							storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies[ItemSchema.InferenceClassificationResult] = inferenceClassificationResult;
							inferencePropertyBag.Add(InferenceSchema.InferenceClassificationResult, inferenceClassificationResult);
							inferencePropertyBag.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Succeeded);
							inferencePropertyBag.Add(InferenceSchema.ClassificationStatusMessage, value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException)
				{
					throw;
				}
				Exception innerException = ex.InnerException;
				if (innerException is StorageTransientException)
				{
					InferenceClassificationProcessing.NumberOfTransientExceptions.Increment();
				}
				else if (innerException is QuotaExceededException)
				{
					InferenceClassificationProcessing.NumberOfQuotaExceededExceptions.Increment();
				}
				this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Received exception {0}", new object[]
				{
					ex
				});
				inferencePropertyBag.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Failed);
				inferencePropertyBag.Add(InferenceSchema.ClassificationStatusMessage, "Exception");
				inferencePropertyBag.Add(InferenceSchema.ClassificationAgentException, ex);
			}
			finally
			{
				stopwatch.Stop();
				InferenceClassificationAgent.tracer.TraceDebug<long>((long)this.GetHashCode(), "InferenceClassification agent took {0} ms to process the item.", stopwatch.ElapsedMilliseconds);
				inferencePropertyBag.Add(InferenceSchema.TimeTakenToClassify, stopwatch.ElapsedMilliseconds);
				if (storeDriverDeliveryEventArgsImpl.MailboxSession != null)
				{
					InferenceClassificationAgent.RecordPerformanceCounter(storeDriverDeliveryEventArgsImpl.MailboxSession.MdbGuid.ToString(), stopwatch.ElapsedMilliseconds);
				}
				InferenceClassificationTracking inferenceClassificationTracking = InferenceClassificationTracking.Create();
				this.logValues = this.classificationAgentLogger.ExtractClassificationProperties(inferencePropertyBag, mdbDocument, inferenceClassificationTracking);
				if (variantConfigurationSnapshot != null && variantConfigurationSnapshot.Inference.InferenceStampTracking.Enabled)
				{
					storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies[MessageItemSchema.InferenceClassificationTrackingEx] = inferenceClassificationTracking.ToString();
				}
				if (mdbDocument != null)
				{
					mdbDocument.Dispose();
					mdbDocument = null;
				}
			}
		}

		public void OnDeliveredMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			this.wasMessageDelivered = true;
		}

		public void OnCompletedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			if (this.logValues != null)
			{
				this.classificationAgentLogger.LogClassificationProperties(this.logValues, (!this.wasMessageDelivered) ? InferenceClassificationAgentLogger.Status.DeliveryFailed.ToString() : null);
			}
		}

		internal bool RunClassificationPipeline(StoreDriverDeliveryEventArgsImpl argsImpl, InferencePropertyBag classificationDiagnostics, VariantConfigurationSnapshot flightFeatures, out MdbDocument document, out bool isClutter, out ConversationClutterInformation conversationClutterInformation)
		{
			document = null;
			isClutter = false;
			conversationClutterInformation = null;
			InferenceClassificationAgent.tracer.TraceDebug((long)this.GetHashCode(), "Processing incoming message");
			ModelVersionBreadCrumb breadCrumb = ClutterUtilities.GetModelVersionBreadCrumb(argsImpl.MailboxSession);
			ModelVersionSelector versionSelector = new ModelVersionSelector(this.modelConfiguration, breadCrumb, delegate(string str)
			{
				this.diagnosticsSession.TraceDebug(str, new object[0]);
			});
			if (versionSelector.ClassificationModelVersion.Version == -2147483648)
			{
				classificationDiagnostics.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Skipped);
				classificationDiagnostics.Add(InferenceSchema.ClassificationStatusMessage, "ClassificationModelNotFound");
				return false;
			}
			MdbInferenceRecipient ownerAsInferenceRecipient;
			if (string.Equals(argsImpl.MailboxSession.MailboxOwner.MailboxInfo.DisplayName, argsImpl.MailboxSession.MailboxOwner.LegacyDn, StringComparison.OrdinalIgnoreCase))
			{
				ownerAsInferenceRecipient = new MdbInferenceRecipient(argsImpl.MailboxSession.MailboxOwner, argsImpl.MailboxSession.Culture, argsImpl.MailboxOwner.DisplayName);
			}
			else
			{
				ownerAsInferenceRecipient = new MdbInferenceRecipient(argsImpl.MailboxSession.MailboxOwner, argsImpl.MailboxSession.Culture, null);
			}
			bool? conversationLoadRequired = null;
			if (argsImpl.SharedPropertiesBetweenAgents != null && argsImpl.SharedPropertiesBetweenAgents.ContainsKey(ItemSchema.ConversationLoadRequiredByInference))
			{
				conversationLoadRequired = new bool?((bool)argsImpl.SharedPropertiesBetweenAgents[ItemSchema.ConversationLoadRequiredByInference]);
			}
			DocumentProcessingContext processingContext = new DocumentProcessingContext(argsImpl.MailboxSession);
			MdbCompositeItemIdentity mdbCompositeItemIdentity = new MdbCompositeItemIdentity(argsImpl.MailboxSession.MdbGuid, argsImpl.MailboxSession.MailboxGuid, StoreObjectId.DummyId, 1);
			MdbInMemoryDocumentAdapter mdbInMemoryDocumentAdapter = new MdbInMemoryDocumentAdapter(mdbCompositeItemIdentity, argsImpl.ReplayItem, MdbInferencePropertyMap.Instance);
			document = new MdbDocument(mdbCompositeItemIdentity, DocumentOperation.Insert, mdbInMemoryDocumentAdapter);
			object uniqueBodyWords;
			document.TryGetProperty(InferenceSchema.UniqueBodyWordsFromProvider, out uniqueBodyWords);
			this.ProcessClassificationDocument(document, classificationDiagnostics, versionSelector.ClassificationModelVersion, processingContext, ownerAsInferenceRecipient, breadCrumb, flightFeatures, uniqueBodyWords, conversationLoadRequired);
			IItem inMemoryItem = mdbInMemoryDocumentAdapter.InMemoryItem;
			if (inMemoryItem == null)
			{
				InferenceClassificationAgent.tracer.TraceError((long)this.GetHashCode(), "The InMemoryItem of the inMemDocAdapter was null");
				classificationDiagnostics.Add(InferenceSchema.ClassificationStatus, InferenceClassificationAgentLogger.Status.Skipped);
				classificationDiagnostics.Add(InferenceSchema.ClassificationStatusMessage, "InMemoryItemNull");
				return false;
			}
			object obj;
			if (document.TryGetProperty(InferenceSchema.ConversationClutterInformation, out obj))
			{
				conversationClutterInformation = (obj as ConversationClutterInformation);
			}
			classificationDiagnostics.Add(InferenceSchema.ConversationClutterInformation, conversationClutterInformation);
			classificationDiagnostics.Add(InferenceSchema.MarkedAsBulk, InferenceClassificationAgent.IsBulkMail(argsImpl, argsImpl.MailboxOwner));
			isClutter = document.GetProperty<bool>(InferenceSchema.ComputedClutterValue);
			foreach (StorePropertyDefinition propertyDefinition in InferenceClassificationAgent.PropertiesToReplicate)
			{
				InferenceClassificationAgent.CopyIfValid(propertyDefinition, inMemoryItem, argsImpl.PropertiesForAllMessageCopies);
			}
			if (this.classificationComparisonLogger != null && flightFeatures != null && flightFeatures.Inference.InferenceModelComparison.Enabled)
			{
				IEnumerable<int> enumerable = from entry in versionSelector.TrainingModelVersions
				where entry.Version != versionSelector.ClassificationModelVersion.Version && (breadCrumb.Contains((short)entry.Version, ModelVersionBreadCrumb.VersionType.Ready) || breadCrumb.Contains((short)entry.Version, ModelVersionBreadCrumb.VersionType.NotReady))
				select entry.Version;
				if (enumerable != null && enumerable.Any<int>())
				{
					Dictionary<OrderedFeatureSet, FeatureValues> dictionary = new Dictionary<OrderedFeatureSet, FeatureValues>();
					dictionary.Add(ModelConfiguration.GetModelVersionConfiguration(versionSelector.ClassificationModelVersion.Version).FeatureSet, document.GetProperty<FeatureValues>(InferenceSchema.ImportanceFeatureValues));
					this.classificationComparisonLogger.LogModelComparisonData(document, classificationDiagnostics);
					foreach (int comparisonVersion in enumerable)
					{
						MdbDocument mdbDocument = new MdbDocument(mdbCompositeItemIdentity, DocumentOperation.Insert, mdbInMemoryDocumentAdapter);
						InferencePropertyBag inferencePropertyBag = new InferencePropertyBag();
						this.ProcessClassificationDocumentForComparisonModel(mdbDocument, inferencePropertyBag, comparisonVersion, document, classificationDiagnostics, dictionary, processingContext, ownerAsInferenceRecipient, breadCrumb, flightFeatures, uniqueBodyWords, conversationLoadRequired);
						this.classificationComparisonLogger.LogModelComparisonData(mdbDocument, inferencePropertyBag);
					}
				}
			}
			return true;
		}

		private static bool CheckForRuleAndConversationOverrides(Dictionary<PropertyDefinition, object> sharedPropertiesBetweenAgents, OriginalDeliveryFolderInfo deliveryFolderInfo, out bool isClutter, out InferenceClassificationResult classificationResult, out string overrideMessage, out bool isConversationFixupNeeded)
		{
			isClutter = false;
			isConversationFixupNeeded = true;
			classificationResult = InferenceClassificationResult.None;
			overrideMessage = string.Empty;
			if (sharedPropertiesBetweenAgents == null)
			{
				return false;
			}
			if (sharedPropertiesBetweenAgents.ContainsKey(ItemSchema.InferenceConversationClutterActionApplied))
			{
				isClutter = (bool)sharedPropertiesBetweenAgents[ItemSchema.InferenceConversationClutterActionApplied];
				classificationResult = InferenceClassificationResult.ConversationActionOverride;
				overrideMessage = "OverriddenByConversationClutterAction";
				isConversationFixupNeeded = false;
				return true;
			}
			if (sharedPropertiesBetweenAgents.ContainsKey(ItemSchema.InferenceNeverClutterOverrideApplied))
			{
				isClutter = false;
				classificationResult |= InferenceClassificationResult.NeverClutterOverride;
				overrideMessage = "OverriddenByNeverClutterRule";
				isConversationFixupNeeded = true;
				return true;
			}
			bool flag = sharedPropertiesBetweenAgents.ContainsKey(ItemSchema.ItemMovedByRule) && (bool)sharedPropertiesBetweenAgents[ItemSchema.ItemMovedByRule];
			bool flag2 = sharedPropertiesBetweenAgents.ContainsKey(ItemSchema.ItemMovedByConversationAction) && (bool)sharedPropertiesBetweenAgents[ItemSchema.ItemMovedByConversationAction];
			if (flag2 && deliveryFolderInfo.IsDeliveryFolderInbox)
			{
				isClutter = false;
				classificationResult = InferenceClassificationResult.NeverClutterOverride;
				overrideMessage = "OverriddenByNeverClutterConversationAction";
				isConversationFixupNeeded = false;
				return true;
			}
			if (flag2 && deliveryFolderInfo.IsDeliveryFolderClutter)
			{
				isClutter = true;
				classificationResult = InferenceClassificationResult.AlwaysClutterOverride;
				overrideMessage = "OverriddenByAlwaysClutterConversationAction";
				isConversationFixupNeeded = false;
				return true;
			}
			if (flag && deliveryFolderInfo.IsDeliveryFolderInbox)
			{
				isClutter = false;
				classificationResult = InferenceClassificationResult.NeverClutterOverride;
				overrideMessage = "OverriddenByNeverClutterRule";
				isConversationFixupNeeded = true;
				return true;
			}
			if (flag && deliveryFolderInfo.IsDeliveryFolderClutter)
			{
				isClutter = true;
				classificationResult = InferenceClassificationResult.AlwaysClutterOverride;
				overrideMessage = "OverriddenByAlwaysClutterRule";
				isConversationFixupNeeded = false;
				return true;
			}
			object obj;
			if (sharedPropertiesBetweenAgents.TryGetValue(ItemSchema.IsStopProcessingRuleApplicable, out obj) && (bool)obj)
			{
				classificationResult = InferenceClassificationResult.StopProcessingRulesOverride;
				return false;
			}
			return false;
		}

		private static bool IsMailboxInteresting(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			if (argsImpl.IsPublicFolderRecipient)
			{
				InferenceClassificationProcessing.NumberOfItemsSkipped.Increment();
				return false;
			}
			return true;
		}

		private static bool IsMessageInteresting(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			if (!InferenceClassificationAgent.IsObjectClassInteresting(argsImpl.MessageClass) || argsImpl.IsJournalReport)
			{
				InferenceClassificationProcessing.NumberOfItemsSkipped.Increment();
				return false;
			}
			return true;
		}

		private static bool IsObjectClassInteresting(string objClass)
		{
			return !string.IsNullOrEmpty(objClass) && !ObjectClass.IsMeetingMessage(objClass);
		}

		private static bool IsDeliveryFolderInteresting(OriginalDeliveryFolderInfo deliveryFolderInfo)
		{
			if (deliveryFolderInfo.FolderType == OriginalDeliveryFolderInfo.DeliveryFolderType.Inbox || deliveryFolderInfo.FolderType == OriginalDeliveryFolderInfo.DeliveryFolderType.Clutter)
			{
				return true;
			}
			InferenceClassificationProcessing.NumberOfItemsSkippedDueToDeliveryFolder.Increment();
			InferenceClassificationProcessing.NumberOfItemsSkipped.Increment();
			return false;
		}

		internal static void RecordPerformanceCounter(string mdbGuid, long elapsedTime)
		{
			if (elapsedTime <= 100L)
			{
				ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedIn0to100Milliseconds.Increment();
				return;
			}
			if (elapsedTime <= 200L)
			{
				ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedIn100to200Milliseconds.Increment();
				return;
			}
			if (elapsedTime <= 500L)
			{
				ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedIn200to500Milliseconds.Increment();
				return;
			}
			if (elapsedTime <= 750L)
			{
				ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedIn500to750Milliseconds.Increment();
				return;
			}
			if (elapsedTime <= 1000L)
			{
				ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedIn750to1000Milliseconds.Increment();
				return;
			}
			if (elapsedTime <= 5000L)
			{
				ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedIn1000to5000Milliseconds.Increment();
				return;
			}
			ClassificationLatency.GetInstance(mdbGuid).NumberOfItemsProcessedInGreaterThan5000Milliseconds.Increment();
		}

		private static OriginalDeliveryFolderInfo GetOriginalDeliveryFolderInfo(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			StoreObjectId storeObjectId = argsImpl.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			if (argsImpl.DeliverToFolder != null)
			{
				StoreObjectId storeObjectId2 = StoreId.GetStoreObjectId(argsImpl.DeliverToFolder);
				if (storeObjectId2 != null && storeObjectId2.ProviderLevelItemId != null)
				{
					storeObjectId = storeObjectId2;
				}
			}
			return InferenceXsoUtil.GetOriginalDeliveryFolderInfo(argsImpl.MailboxSession, storeObjectId);
		}

		private static bool IsBulkMail(StoreDriverDeliveryEventArgsImpl argsImpl, MiniRecipient recipient)
		{
			bool result = false;
			string name = InferenceXsoUtil.IsMicrosoft(recipient) ? "X-Forefront-Antispam-Report-Untrusted" : "X-MS-Exchange-Organization-Antispam-Report";
			Header header = argsImpl.MailItem.Message.MimeDocument.RootPart.Headers.FindFirst(name);
			string data;
			if (header != null && header.TryGetValue(out data))
			{
				result = InferenceCommonUtility.MatchBulkHeader(data);
			}
			return result;
		}

		private static string GetLocale(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			int? valueAsNullable = argsImpl.ReplayItem.GetValueAsNullable<int>(MessageItemSchema.MessageLocaleId);
			if (valueAsNullable != null && valueAsNullable.Value >= 0)
			{
				try
				{
					CultureInfo cultureInfo = new CultureInfo(valueAsNullable.Value);
					return cultureInfo.Name;
				}
				catch (CultureNotFoundException)
				{
				}
			}
			return null;
		}

		private void ProcessClassificationDocument(MdbDocument document, InferencePropertyBag classificationDiagnostics, ModelVersionSelector.ModelVersionInfo versionInfo, DocumentProcessingContext processingContext, MdbInferenceRecipient ownerAsInferenceRecipient, ModelVersionBreadCrumb breadCrumb, VariantConfigurationSnapshot flightFeatures, object uniqueBodyWords, bool? conversationLoadRequired)
		{
			document.SetProperty(InferenceSchema.MailboxOwner, ownerAsInferenceRecipient);
			document.SetProperty(InferenceSchema.ClassificationDiagnostics, classificationDiagnostics);
			document.SetProperty(InferenceSchema.ModelVersionToLoad, versionInfo);
			document.SetProperty(InferenceSchema.ModelVersionBreadCrumb, breadCrumb);
			if (conversationLoadRequired != null)
			{
				document.SetProperty(InferenceSchema.ConversationLoadRequired, conversationLoadRequired);
			}
			if (flightFeatures != null)
			{
				document.SetProperty(InferenceSchema.UserFlightFeatures, flightFeatures);
			}
			if (uniqueBodyWords != null)
			{
				document.SetProperty(InferenceSchema.UniqueBodyWords, uniqueBodyWords);
			}
			this.pipeline.ProcessDocument(document, processingContext);
		}

		private void ProcessClassificationDocumentForComparisonModel(MdbDocument comparisonDocument, InferencePropertyBag comparisonDiagnostics, int comparisonVersion, MdbDocument classificationDocument, InferencePropertyBag classificationDiagnostics, Dictionary<OrderedFeatureSet, FeatureValues> cachedFeatureValues, DocumentProcessingContext processingContext, MdbInferenceRecipient ownerAsInferenceRecipient, ModelVersionBreadCrumb breadCrumb, VariantConfigurationSnapshot flightFeatures, object uniqueBodyWords, bool? conversationLoadRequired)
		{
			try
			{
				object obj;
				if (classificationDiagnostics.TryGetValue(InferenceSchema.MessageClassificationTime, out obj))
				{
					comparisonDiagnostics.Add(InferenceSchema.MessageClassificationTime, obj);
				}
				if (classificationDiagnostics.TryGetValue(DocumentSchema.MailboxId, out obj))
				{
					comparisonDiagnostics.Add(DocumentSchema.MailboxId, obj);
				}
				if (classificationDiagnostics.TryGetValue(InferenceSchema.InternetMessageId, out obj))
				{
					comparisonDiagnostics.Add(InferenceSchema.InternetMessageId, obj);
				}
				if (classificationDiagnostics.TryGetValue(InferenceSchema.MessageIdentifier, out obj))
				{
					comparisonDiagnostics.Add(InferenceSchema.MessageIdentifier, obj);
				}
				if (classificationDocument.TryGetProperty(InferenceSchema.IsNewConversation, out obj))
				{
					comparisonDocument.SetProperty(InferenceSchema.IsNewConversation, obj);
				}
				if (classificationDocument.TryGetProperty(InferenceSchema.ConversationClutterInformation, out obj))
				{
					comparisonDocument.SetProperty(InferenceSchema.ConversationClutterInformation, obj);
				}
				if (classificationDocument.TryGetProperty(InferenceSchema.ConversationImportanceProperties, out obj))
				{
					comparisonDocument.SetProperty(InferenceSchema.ConversationImportanceProperties, obj);
				}
				comparisonDocument.SetProperty(InferenceSchema.SkipConversationExtraction, true);
				bool flag = false;
				OrderedFeatureSet featureSet = ModelConfiguration.GetModelVersionConfiguration(comparisonVersion).FeatureSet;
				if (cachedFeatureValues.ContainsKey(featureSet))
				{
					comparisonDocument.SetProperty(InferenceSchema.ImportanceFeatureValues, cachedFeatureValues[featureSet]);
					comparisonDocument.SetProperty(InferenceSchema.SkipFeatureVectorCalculation, true);
					flag = true;
				}
				comparisonDocument.SetProperty(InferenceSchema.SkipFeatureVectorPersistance, true);
				comparisonDocument.SetProperty(InferenceSchema.SkipPredictedActionsPersistance, true);
				this.ProcessClassificationDocument(comparisonDocument, comparisonDiagnostics, new ModelVersionSelector.ModelVersionInfo(comparisonVersion, false), processingContext, ownerAsInferenceRecipient, breadCrumb, flightFeatures, uniqueBodyWords, conversationLoadRequired);
				if (!flag && comparisonDocument.TryGetProperty(InferenceSchema.ImportanceFeatureValues, out obj))
				{
					cachedFeatureValues[featureSet] = (FeatureValues)obj;
				}
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException)
				{
					throw;
				}
				comparisonDiagnostics.Add(InferenceSchema.ClassificationAgentException, ex);
			}
		}

		private static readonly IList<StorePropertyDefinition> PropertiesToReplicate = new List<StorePropertyDefinition>
		{
			MessageItemSchema.TriageFeatureVector,
			ItemSchema.PredictedActions,
			MessageItemSchema.ExtractionResult,
			ItemSchema.InferencePredictedReplyForwardReasons,
			ItemSchema.InferencePredictedIgnoreReasons,
			ItemSchema.InferencePredictedDeleteReasons
		};

		private static readonly Microsoft.Exchange.Diagnostics.Trace tracer = ExTraceGlobals.ImportanceClassifierTracer;

		private readonly Pipeline pipeline;

		private readonly SmtpServer server;

		private readonly IServerModelConfiguration modelConfiguration;

		private readonly bool isPipelineEnabled;

		private readonly InferenceClassificationAgentLogger classificationAgentLogger;

		private readonly InferenceClassificationComparisonLogger classificationComparisonLogger;

		private readonly IDiagnosticsSession diagnosticsSession;

		private IList<object> logValues;

		private bool wasMessageDelivered;
	}
}
