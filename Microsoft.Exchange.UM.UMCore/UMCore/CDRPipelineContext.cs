using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CDRPipelineContext : PipelineContext
	{
		internal override Pipeline Pipeline
		{
			get
			{
				return CDRPipeline.Instance;
			}
		}

		public ADUser CDRMessageRecipient { get; set; }

		public CDRPipelineContext(CDRData cdrData)
		{
			bool flag = false;
			try
			{
				ValidateArgument.NotNull(cdrData, "cdrData");
				base.MessageType = "CDR";
				this.cdrDataXml = UMTypeSerializer.SerializeToString<CDRData>(cdrData);
				this.cdrData = cdrData;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Submitted a CDR message to the pipeline. CDRXML = {0}", new object[]
				{
					this.cdrDataXml
				});
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public CDRPipelineContext(CDRData cdrData, string cdrDataXml) : base(new SubmissionHelper(cdrData.CallIdentity, PhoneNumber.Empty, cdrData.EDiscoveryUserObjectGuid, "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}", CultureInfo.InvariantCulture.Name, null, null, cdrData.TenantGuid))
		{
			base.MessageType = "CDR";
			this.cdrData = cdrData;
			this.cdrDataXml = cdrDataXml;
			this.CDRMessageRecipient = (base.CreateADRecipientFromObjectGuid(cdrData.EDiscoveryUserObjectGuid, cdrData.TenantGuid) as ADUser);
		}

		public static CDRPipelineContext Deserialize(string cdrDataXml)
		{
			CDRData cdrdata = UMTypeSerializer.DeserializeFromString<CDRData>(cdrDataXml);
			return new CDRPipelineContext(cdrdata, cdrDataXml);
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			headerStream.WriteLine("CDRData : " + this.cdrDataXml);
		}

		public override PipelineDispatcher.WIThrottleData GetThrottlingData()
		{
			return new PipelineDispatcher.WIThrottleData
			{
				Key = this.CDRMessageRecipient.Database.ObjectGuid.ToString(),
				RecipientId = this.GetRecipientIdForThrottling(),
				WorkItemType = PipelineDispatcher.ThrottledWorkItemType.CDRWorkItem
			};
		}

		public override string GetMailboxServerId()
		{
			return this.CDRMessageRecipient.ServerLegacyDN + ":CDR";
		}

		public override string GetRecipientIdForThrottling()
		{
			return null;
		}

		public override void PrepareUnProtectedMessage()
		{
			try
			{
				using (IUMCallDataRecordStorage umcallDataRecordsAcessor = InterServerMailboxAccessor.GetUMCallDataRecordsAcessor(this.CDRMessageRecipient))
				{
					umcallDataRecordsAcessor.CreateUMCallDataRecord(this.cdrData);
				}
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, this, ex.ToString(), new object[0]);
				if (!(ex is QuotaExceededException) && !(ex is InvalidObjectGuidException) && !(ex is ObjectNotFoundException) && !(ex is StorageTransientException))
				{
					throw;
				}
				if (ex is QuotaExceededException)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToSaveCDR, null, new object[]
					{
						Strings.EDiscoveryMailboxFull(this.CDRMessageRecipient.DistinguishedName, CommonUtil.ToEventLogString(ex))
					});
				}
				else
				{
					if (ex is StorageTransientException)
					{
						throw new MailboxUnavailableException(base.MessageType, this.CDRMessageRecipient.Database.DistinguishedName, ex.Message, ex);
					}
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToSaveCDR, null, new object[]
					{
						CommonUtil.ToEventLogString(ex)
					});
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CDRPipelineContext>(this);
		}

		protected override void WriteCommonHeaderFields(StreamWriter headerStream)
		{
		}

		private CDRData cdrData;

		private string cdrDataXml;
	}
}
