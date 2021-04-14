using System;
using System.Text;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Common
{
	internal class InferenceClassificationTracking
	{
		private InferenceClassificationTracking(int maxTraceLength)
		{
			if (maxTraceLength < 0)
			{
				throw new ArgumentException("maxTraceLength");
			}
			this.traceBuilder = new StringBuilder(0);
			this.traceOverflow = false;
			this.maxTraceLength = maxTraceLength;
		}

		public static InferenceClassificationTracking Create()
		{
			return new InferenceClassificationTracking(10240);
		}

		public static InferenceClassificationTracking Create(int maxTraceLength)
		{
			return new InferenceClassificationTracking(maxTraceLength);
		}

		public override string ToString()
		{
			string text = this.traceBuilder.ToString();
			if (this.traceOverflow)
			{
				text = text.Substring(0, this.maxTraceLength - "...".Length);
				text += "...";
			}
			return text;
		}

		public void Trace(string key, string value)
		{
			Util.ThrowOnNullArgument(key, "key");
			this.Trace(string.Format("{0}={1}", key, value ?? "<null>"));
		}

		public void Trace(string key, object value)
		{
			this.Trace(key, (value == null) ? null : value.ToString());
		}

		public void Trace(string value)
		{
			Util.ThrowOnNullArgument(value, "value");
			if (this.traceOverflow)
			{
				this.traceOverflow = true;
				return;
			}
			if (this.traceBuilder.Length > 0)
			{
				this.traceBuilder.Append(";");
			}
			this.traceBuilder.Append(value);
			if (this.traceBuilder.Length >= this.maxTraceLength)
			{
				this.traceOverflow = true;
			}
		}

		public const string ServerVersion = "SV";

		public const string ServerName = "SN";

		public const string ModelVersion = "MV";

		public const string MessageClassificationTime = "CT";

		public const string TimeTakenToClassify = "TTC";

		public const string TimeTakenToInfer = "TI";

		public const string PredictedActions = "PA";

		public const string FeatureValuesWeights = "FVW";

		public const string FolderPredictionTimeTakenToInfer = "FPTI";

		public const string PredictedTopFolders = "PTF";

		public const string FolderPredictionFeatureValuesWeights = "FPFVW";

		public const string PredictedActionsThresholds = "PAT";

		public const string OriginalDeliveryFolder = "ODF";

		public const string ConversationClutterState = "CCS";

		public const string ConversationPreviousMessageCount = "CPMC";

		public const string MarkedAsBulk = "MAB";

		public const string ClutterValueBeforeOverrideRules = "CVBOR";

		public const string ClutterValueAfterOverrideRules = "CVAOR";

		private const int DefaultMaxTraceLength = 10240;

		private const string Ellipsis = "...";

		private StringBuilder traceBuilder;

		private bool traceOverflow;

		private readonly int maxTraceLength;
	}
}
