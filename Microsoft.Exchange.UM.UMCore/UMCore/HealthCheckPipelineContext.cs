using System;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class HealthCheckPipelineContext : PipelineContext
	{
		internal HealthCheckPipelineContext(string workId) : base(new SubmissionHelper(string.Empty, PhoneNumber.Empty, Guid.Empty, string.Empty, "en-US", string.Empty, string.Empty, Guid.Empty))
		{
			base.MessageType = "HealthCheck";
			this.attachmentPath = Path.Combine(Utils.VoiceMailFilePath, workId + ".HealthCheck");
			base.HeaderFileName = Path.Combine(Utils.VoiceMailFilePath, workId + ".txt");
		}

		internal override Pipeline Pipeline
		{
			get
			{
				return HealthCheckPipeline.Instance;
			}
		}

		internal PipelineDispatcher.PipelineResourceType ResourceBeingChecked
		{
			get
			{
				return this.resourceBeingChecked;
			}
		}

		internal static void TryDeleteHealthCheckFiles()
		{
			foreach (string filename in HealthCheckPipelineContext.GetQueuedHealthCheckFilePaths())
			{
				Util.TryDeleteFile(filename);
			}
		}

		internal static string[] GetQueuedHealthCheckFilePaths()
		{
			string[] array = null;
			try
			{
				array = Directory.GetFiles(Utils.VoiceMailFilePath, "*.HealthCheck");
			}
			catch (IOException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "Ignoring IOException while cleaning up health-check files.  {0}", new object[]
				{
					ex
				});
			}
			return array ?? new string[0];
		}

		internal static bool IsPipelineHealthy()
		{
			return HealthCheckPipelineContext.GetQueuedHealthCheckFilePaths().Length <= 4;
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
		}

		internal override void SaveMessage()
		{
			using (File.Create(this.attachmentPath))
			{
			}
			base.SaveMessage();
		}

		public override string GetMailboxServerId()
		{
			return base.GetMailboxServerIdHelper();
		}

		public override string GetRecipientIdForThrottling()
		{
			return base.GetRecipientIdHelper();
		}

		internal void Passed(PipelineDispatcher.PipelineResourceType resourceType)
		{
			this.resourceBeingChecked++;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					HealthCheckPipelineContext.TryDeleteHealthCheckFiles();
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		public const string HealthCheckExtension = ".HealthCheck";

		public const int MaxMessagesAllowedInQueue = 4;

		private PipelineDispatcher.PipelineResourceType resourceBeingChecked;

		private string attachmentPath;
	}
}
