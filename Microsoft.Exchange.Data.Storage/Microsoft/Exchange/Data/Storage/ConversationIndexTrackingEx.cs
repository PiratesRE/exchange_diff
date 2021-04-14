using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationIndexTrackingEx
	{
		private ConversationIndexTrackingEx()
		{
			this.traceBuilder = new StringBuilder(0);
			this.traceOverflow = false;
		}

		public static ConversationIndexTrackingEx Create()
		{
			return new ConversationIndexTrackingEx();
		}

		public void Trace(string key, string value)
		{
			Util.ThrowOnNullArgument(key, "key");
			this.Trace(string.Format("{0}={1}", key, value ?? "<null>"));
		}

		public void Trace(ConversationIndex index)
		{
			Util.ThrowOnNullArgument(index, "index");
			StringBuilder stringBuilder = new StringBuilder(0);
			stringBuilder.Append("[");
			stringBuilder.Append(string.Format("{0}={1}", "CID", index.Guid));
			stringBuilder.Append(";");
			stringBuilder.Append(string.Format("{0}={1}", "IDXHEAD", GlobalObjectId.ByteArrayToHexString(index.Header)));
			stringBuilder.Append(";");
			stringBuilder.Append(string.Format("{0}={1}", "IDXCOUNT", index.Components.Count));
			stringBuilder.Append("]");
			this.Trace("II", stringBuilder.ToString());
		}

		public void TraceVersionAndHeuristics(string fixupStage)
		{
			this.Trace(string.Format("Version={0}, Stage={1}", ConversationIndexTrackingEx.GetBuildVersion(), fixupStage));
		}

		internal static ServerVersion GetBuildVersion()
		{
			if (ConversationIndexTrackingEx.buildVersion == null)
			{
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Util.GetAssemblyLocation());
				ConversationIndexTrackingEx.buildVersion = new ServerVersion(1879048192 | (versionInfo.FileMajorPart & 63) << 22 | (versionInfo.FileMinorPart & 63) << 16 | 32768 | (versionInfo.FileBuildPart & 32767));
			}
			return ConversationIndexTrackingEx.buildVersion;
		}

		public override string ToString()
		{
			string text = this.traceBuilder.ToString();
			if (this.traceOverflow)
			{
				text = text.Substring(0, 1024 - "...".Length);
				text += "...";
			}
			return text;
		}

		private void Trace(string value)
		{
			if (this.traceOverflow)
			{
				return;
			}
			if (this.traceBuilder.Length >= 1024)
			{
				this.traceOverflow = true;
				return;
			}
			if (this.traceBuilder.Length > 0)
			{
				this.traceBuilder.Append(";");
			}
			this.traceBuilder.Append(value);
			if (this.traceBuilder.Length > 1024)
			{
				this.traceOverflow = true;
			}
		}

		private const int MaxTraceLength = 1024;

		private const string Ellipsis = "...";

		public const string BodyTagInMilliSeconds = "BT";

		public const string SearchByMessageIdInMilliSeconds = "SBMID";

		public const string SearchByConversationIdInMilliSeconds = "SBCID";

		public const string SearchByTopicInMilliSeconds = "SBT";

		public const string SearchBySMSConversationIdInMilliSeconds = "SBSMSCID";

		public const string FixupInMilliSeconds = "FIXUP";

		public const string IncomingIndex = "II";

		public const string Stage1 = "S1";

		public const string Stage2 = "S2";

		public const string Stage3 = "S3";

		public const string TopicHashAdded = "THA";

		private const string ConversationId = "CID";

		private const string ConversationIndexHeader = "IDXHEAD";

		private const string ConversationIndexComponentsCount = "IDXCOUNT";

		private const string TraceSeparator = ";";

		private const string traceFormat = "{0}={1}";

		private static ServerVersion buildVersion;

		private StringBuilder traceBuilder;

		private bool traceOverflow;
	}
}
