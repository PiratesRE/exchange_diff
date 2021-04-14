using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Common.Diagnostics;
using Microsoft.Exchange.Inference.Learning;
using Microsoft.Exchange.Inference.Learning.Schema;
using Microsoft.Exchange.Inference.Mdb;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InferenceClassificationComparisonLogger : IDisposable
	{
		public InferenceClassificationComparisonLogger(ILogConfig config)
		{
			this.dataLogger = new DataLogger(config, InferenceClassificationComparisonLogger.Columns, (from column in InferenceClassificationComparisonLogger.Columns
			select typeof(string)).ToList<Type>());
		}

		internal string[] RecentlyLoggedRows
		{
			get
			{
				return this.dataLogger.RecentlyLoggedRows;
			}
		}

		public void Dispose()
		{
			this.dataLogger.Dispose();
		}

		public void LogModelComparisonData(MdbDocument document, InferencePropertyBag classificationDiagnostics)
		{
			if (document != null && classificationDiagnostics != null)
			{
				object obj;
				object item = classificationDiagnostics.TryGetValue(DocumentSchema.MailboxId, out obj) ? obj : null;
				object item2 = classificationDiagnostics.TryGetValue(InferenceSchema.MessageIdentifier, out obj) ? obj : null;
				object item3 = classificationDiagnostics.TryGetValue(InferenceSchema.InternetMessageId, out obj) ? obj : null;
				object item4 = classificationDiagnostics.TryGetValue(InferenceSchema.MessageClassificationTime, out obj) ? obj : null;
				Exception ex = classificationDiagnostics.TryGetValue(InferenceSchema.ClassificationAgentException, out obj) ? ((Exception)obj) : null;
				ModelVersionSelector.ModelVersionInfo modelVersionInfo = document.TryGetProperty(InferenceSchema.ModelVersionToLoad, out obj) ? ((ModelVersionSelector.ModelVersionInfo)obj) : null;
				object item5 = -1;
				object item6 = -1;
				object item7 = null;
				UserConfigurationOverride userConfigurationOverride = document.TryGetProperty(InferenceSchema.UserConfigurationOverride, out obj) ? ((UserConfigurationOverride)obj) : null;
				PredictedActionAndProbability predictedActionAndProbability = null;
				List<PredictedActionAndProbability> list = classificationDiagnostics.TryGetValue(InferenceSchema.PredictedActionsAll, out obj) ? ((List<PredictedActionAndProbability>)obj) : null;
				if (list != null)
				{
					predictedActionAndProbability = list.FirstOrDefault((PredictedActionAndProbability entry) => entry.Action == PredictedMessageAction.Clutter);
				}
				IList<object> list2 = new List<object>(InferenceClassificationComparisonLogger.Columns.Count);
				list2.Add(DateTime.UtcNow);
				list2.Add(InferenceCommonUtility.ServerVersion);
				list2.Add(item);
				list2.Add(item2);
				list2.Add((modelVersionInfo == null) ? string.Empty : modelVersionInfo.Version.ToString());
				list2.Add(item5);
				list2.Add(item6);
				list2.Add(item4);
				list2.Add((predictedActionAndProbability == null) ? string.Empty : predictedActionAndProbability.Probability.ToString());
				list2.Add((userConfigurationOverride == null) ? string.Empty : userConfigurationOverride.ClutterThreshold.ToString());
				list2.Add(item3);
				list2.Add((ex == null) ? string.Empty : InferenceCommonUtility.StringizeException(ex));
				list2.Add(item7);
				this.dataLogger.Log(list2);
			}
		}

		private static readonly List<string> Columns = new List<string>
		{
			"Timestamp",
			"ServerVersion",
			"MailboxGuid",
			"MessageGuid",
			"ModelVersion",
			"ModelIteration",
			"TrainedMessageCount",
			"ClassificationTime",
			"PredictedProbability",
			"SelectedThreshold",
			"InternetMessageId",
			"Exception",
			"Details"
		};

		private readonly DataLogger dataLogger;
	}
}
