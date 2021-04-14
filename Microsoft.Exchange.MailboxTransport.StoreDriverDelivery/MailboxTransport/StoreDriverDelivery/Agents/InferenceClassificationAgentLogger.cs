using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Common.Diagnostics;
using Microsoft.Exchange.Inference.Learning;
using Microsoft.Exchange.Inference.Learning.Schema;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class InferenceClassificationAgentLogger : IDisposable
	{
		public InferenceClassificationAgentLogger(ILogConfig logConfig)
		{
			this.dataLogger = new DataLogger(logConfig, InferenceClassificationAgentLogger.Columns, (from column in InferenceClassificationAgentLogger.Columns
			select typeof(string)).ToList<Type>());
			this.columnCount = InferenceClassificationAgentLogger.Columns.Count;
		}

		public void Dispose()
		{
			if (this.dataLogger != null)
			{
				this.dataLogger.Dispose();
			}
		}

		internal IList<object> ExtractClassificationProperties(InferencePropertyBag classificationDiagnostics, IDocument document, InferenceClassificationTracking tracking)
		{
			object item = null;
			object item2 = null;
			object obj = null;
			object obj2 = null;
			object obj3 = null;
			object obj4 = null;
			object obj5 = null;
			object obj6 = null;
			object obj7 = null;
			object item3 = null;
			object item4 = null;
			object obj8 = null;
			string item5 = string.Empty;
			string item6 = string.Empty;
			string item7 = string.Empty;
			string item8 = string.Empty;
			string value = null;
			string item9 = null;
			string text = "Null";
			string item10 = null;
			string value2 = null;
			string item11 = null;
			object obj9 = null;
			object obj10 = null;
			string item12 = null;
			object item13 = null;
			object item14 = null;
			object item15 = null;
			object item16 = null;
			object item17 = null;
			object item18 = null;
			object item19 = null;
			object item20 = null;
			object obj11 = null;
			object item21 = null;
			if (classificationDiagnostics != null)
			{
				classificationDiagnostics.TryGetValue(InferenceSchema.InternetMessageId, out item2);
				classificationDiagnostics.TryGetValue(InferenceSchema.MessageClassificationTime, out obj);
				classificationDiagnostics.TryGetValue(InferenceSchema.OriginalDeliveryFolder, out obj2);
				classificationDiagnostics.TryGetValue(InferenceSchema.MarkedAsBulk, out obj3);
				classificationDiagnostics.TryGetValue(InferenceSchema.TimeTakenToClassify, out obj4);
				classificationDiagnostics.TryGetValue(InferenceSchema.ModelVersion, out obj5);
				classificationDiagnostics.TryGetValue(InferenceSchema.TimeTakenToInfer, out obj6);
				classificationDiagnostics.TryGetValue(InferenceSchema.ServerName, out obj7);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClassificationStatus, out item3);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClassificationStatusMessage, out item4);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClassificationAgentException, out obj8);
				classificationDiagnostics.TryGetValue(DocumentSchema.MailboxId, out item);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClutterValueBeforeOverride, out obj9);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClutterValueAfterOverride, out obj10);
				classificationDiagnostics.TryGetValue(InferenceSchema.Locale, out item13);
				classificationDiagnostics.TryGetValue(InferenceSchema.IsClutter, out item14);
				classificationDiagnostics.TryGetValue(InferenceSchema.TenantName, out item15);
				classificationDiagnostics.TryGetValue(InferenceSchema.IsUiEnabled, out item16);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClutterEnabled, out item18);
				classificationDiagnostics.TryGetValue(InferenceSchema.ClassificationEnabled, out item19);
				classificationDiagnostics.TryGetValue(InferenceSchema.MessageIdentifier, out item20);
				classificationDiagnostics.TryGetValue(InferenceSchema.HasBeenClutterInvited, out item21);
				classificationDiagnostics.TryGetValue(InferenceSchema.InferenceClassificationResult, out obj11);
				if (obj11 is InferenceClassificationResult)
				{
					obj11 = (int)obj11;
				}
				if (obj8 != null)
				{
					Exception ex = obj8 as Exception;
					if (ex != null)
					{
						item8 = InferenceCommonUtility.StringizeException(ex);
						if (ex.TargetSite != null)
						{
							item7 = ex.TargetSite.Name;
						}
						item5 = ex.GetType().Name;
						if (ex.InnerException != null)
						{
							item6 = ex.InnerException.GetType().Name;
						}
					}
				}
				object obj12 = null;
				if (classificationDiagnostics.TryGetValue(InferenceSchema.PredictedActionsThresholds, out obj12))
				{
					IDictionary<PredictedMessageAction, short> dictionary = obj12 as IDictionary<PredictedMessageAction, short>;
					if (dictionary != null)
					{
						value = string.Join(",", from kvp in dictionary
						select string.Format("{0}:{1}", (int)kvp.Key, kvp.Value));
						item9 = string.Join("#", from kvp in dictionary
						select string.Format("{0}:{1}", ActionSets.GetActionName(kvp.Key), kvp.Value));
					}
				}
				obj12 = null;
				if (classificationDiagnostics.TryGetValue(InferenceSchema.ConversationClutterInformation, out obj12))
				{
					ConversationClutterInformation conversationClutterInformation = obj12 as ConversationClutterInformation;
					if (conversationClutterInformation != null)
					{
						text = conversationClutterInformation.State.ToString();
					}
				}
				else
				{
					text = null;
				}
				obj12 = null;
				IList<PredictedActionAndProbability> list = null;
				if (classificationDiagnostics.TryGetValue(InferenceSchema.PredictedActionsAll, out obj12))
				{
					list = (obj12 as IList<PredictedActionAndProbability>);
				}
				if (list != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					StringBuilder stringBuilder2 = new StringBuilder();
					foreach (PredictedActionAndProbability predictedActionAndProbability in list)
					{
						stringBuilder2.Append(string.Format("{0}:{1},", (int)predictedActionAndProbability.Action, predictedActionAndProbability.Probability));
						stringBuilder.Append(string.Format("{0}:{1}#", ActionSets.GetActionName(predictedActionAndProbability.Action), predictedActionAndProbability.Probability));
					}
					item10 = stringBuilder.ToString();
					value2 = stringBuilder2.ToString();
				}
				obj12 = null;
				if (classificationDiagnostics.TryGetValue(InferenceSchema.UserFlightFeatures, out obj12))
				{
					VariantConfigurationSnapshot variantConfigurationSnapshot = (VariantConfigurationSnapshot)obj12;
					StringBuilder stringBuilder3 = new StringBuilder();
					foreach (IFeature feature in variantConfigurationSnapshot.Inference.GetObjectsOfType<IFeature>().Values)
					{
						stringBuilder3.Append(string.Format("{0}:{1}#", feature.Name, feature.Enabled));
					}
					item12 = stringBuilder3.ToString();
				}
			}
			int? num = null;
			string item22 = null;
			string value3 = null;
			object item23 = null;
			object item24 = null;
			object item25 = null;
			object item26 = null;
			if (document != null)
			{
				object obj13 = null;
				obj13 = null;
				ModelData modelData = null;
				if (document.TryGetProperty(InferenceSchema.ActionModel, out obj13))
				{
					modelData = (obj13 as ModelData);
				}
				obj13 = null;
				if (document.TryGetProperty(InferenceSchema.ConversationImportanceProperties, out obj13))
				{
					IConversationProperties conversationProperties = obj13 as IConversationProperties;
					if (conversationProperties != null)
					{
						num = new int?(conversationProperties.NumberOfPreviousMessages);
					}
				}
				if (modelData != null)
				{
					obj13 = null;
					if (document.TryGetProperty(InferenceSchema.MessageFeatureVectorData, out obj13))
					{
						IMessageData messageData = obj13 as IMessageData;
						if (messageData != null)
						{
							item22 = InferenceUtil.GetMessageFeatureValuesAsString(messageData.FeatureValues, modelData);
							value3 = InferenceUtil.GetMessageFeatureValuesAsCompactString(messageData.FeatureValues, modelData);
						}
					}
				}
				obj13 = null;
				PredictedActionAndProbability[] array = null;
				if (document.TryGetProperty(InferenceSchema.PredictedActions, out obj13))
				{
					array = (obj13 as PredictedActionAndProbability[]);
				}
				if (array != null)
				{
					StringBuilder stringBuilder4 = new StringBuilder();
					foreach (PredictedActionAndProbability predictedActionAndProbability2 in array)
					{
						stringBuilder4.Append(string.Format("{0}:{1}#", ActionSets.GetActionName(predictedActionAndProbability2.Action), predictedActionAndProbability2.Probability));
					}
					item11 = stringBuilder4.ToString();
				}
				document.TryGetProperty(InferenceSchema.ConversationId, out item23);
				document.TryGetProperty(InferenceSchema.ModelVersionBreadCrumb, out item26);
				obj13 = null;
				if (document.TryGetProperty(InferenceSchema.ModelVersionToLoad, out obj13))
				{
					ModelVersionSelector.ModelVersionInfo modelVersionInfo = obj13 as ModelVersionSelector.ModelVersionInfo;
					if (modelVersionInfo != null)
					{
						item24 = modelVersionInfo.Version;
					}
				}
				document.TryGetProperty(InferenceSchema.ComputedClutterValue, out item25);
			}
			tracking.Trace("SV", InferenceCommonUtility.ServerVersion);
			tracking.Trace("CT", obj);
			tracking.Trace("ODF", obj2);
			tracking.Trace("MAB", obj3);
			tracking.Trace("TTC", obj4);
			tracking.Trace("SN", obj7);
			if (text != null)
			{
				tracking.Trace("CCS", text);
			}
			tracking.Trace("MV", obj5);
			tracking.Trace("CPMC", num ?? 0);
			tracking.Trace("PA", value2);
			tracking.Trace("TI", obj6);
			tracking.Trace("PAT", value);
			tracking.Trace("CVBOR", obj9);
			tracking.Trace("CVAOR", obj10);
			tracking.Trace("FVW", value3);
			return new List<object>(this.columnCount)
			{
				DateTime.UtcNow,
				item,
				item2,
				item3,
				item4,
				obj7,
				InferenceCommonUtility.ServerVersion,
				item24,
				obj5,
				obj,
				obj4,
				obj2,
				obj3,
				text,
				num,
				item11,
				item10,
				item25,
				item9,
				obj6,
				item22,
				item5,
				item6,
				item7,
				item8,
				obj9,
				obj10,
				item12,
				item23,
				item14,
				item16,
				item15,
				item13,
				item26,
				item17,
				item18,
				item19,
				item20,
				obj11,
				item21
			};
		}

		internal void LogClassificationProperties(IList<object> logValues, string status)
		{
			if (status != null)
			{
				logValues[InferenceClassificationAgentLogger.StatusColumnIndex] = status;
			}
			this.dataLogger.Log(logValues);
		}

		private static readonly List<string> Columns = new List<string>
		{
			"Timestamp",
			"MailboxGuid",
			"InternetMessageId",
			"Status",
			"StatusMessage",
			"ServerName",
			"ServerVersion",
			"ModelVersionToLoad",
			"ModelVersion",
			"MessageClassificationTime",
			"TimeTakenToClassify",
			"OriginalDeliveryFolder",
			"MarkedAsBulk",
			"ConversationClutterState",
			"ConversationPreviousMessageCount",
			"PredictedActions",
			"PredictedActionsAll",
			"ComputedClutterValue",
			"PredictedActionsThresholds",
			"TimeTakenToInfer",
			"FeatureValuesWeights",
			"ExceptionType",
			"ExceptionInnerType",
			"ExceptionTargetSite",
			"ExceptionDetails",
			"ClutterValueBeforeOverride",
			"ClutterValueAfterOverride",
			"FlightFeatures",
			"ConversationId",
			"IsClutter",
			"IsUiEnabled",
			"TenantName",
			"Locale",
			"ModelVersionBreadCrumb",
			"NeverClutterOverrideApplied",
			"ClutterEnabled",
			"ClassificationEnabled",
			"MessageGuid",
			"ClassificationResult",
			"HasBeenClutterInvited"
		};

		private static readonly int StatusColumnIndex = InferenceClassificationAgentLogger.Columns.IndexOf("Status");

		private readonly DataLogger dataLogger;

		private readonly int columnCount;

		internal enum Status
		{
			Succeeded,
			Failed,
			Skipped,
			DeliveryFailed
		}
	}
}
