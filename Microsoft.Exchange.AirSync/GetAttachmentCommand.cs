using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class GetAttachmentCommand : Command
	{
		public GetAttachmentCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfGetAttachments;
		}

		internal override int MaxVersion
		{
			get
			{
				return 121;
			}
		}

		internal string AttachmentName
		{
			get
			{
				string legacyUrlParameter = base.Request.GetLegacyUrlParameter("AttachmentName");
				if (string.IsNullOrEmpty(legacyUrlParameter) || legacyUrlParameter.Length > 256)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentNameInvalid");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidIDs, null, false);
				}
				return legacyUrlParameter;
			}
		}

		protected sealed override string RootNodeName
		{
			get
			{
				return "Invalid";
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			string attachmentName = this.AttachmentName;
			string value = string.Empty;
			FolderSyncState folderSyncState = null;
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "GetAttachmentCommand.Execute(). AttachmentName: '{0}'", attachmentName);
			try
			{
				int incBy = 0;
				if (base.Request.ContentType != null && !string.Equals(base.Request.ContentType, "message/rfc822", StringComparison.OrdinalIgnoreCase))
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidContentType");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.First140Error, null, false);
				}
				int num = attachmentName.IndexOf(':');
				ItemIdMapping itemIdMapping = null;
				if (num != -1 && num != attachmentName.LastIndexOf(':'))
				{
					folderSyncState = base.SyncStateStorage.GetFolderSyncState(new MailboxSyncProviderFactory(base.MailboxSession), attachmentName.Substring(0, num));
					if (folderSyncState == null)
					{
						throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentName));
					}
					itemIdMapping = (ItemIdMapping)folderSyncState[CustomStateDatumType.IdMapping];
					if (itemIdMapping == null)
					{
						throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(attachmentName));
					}
				}
				PolicyData policyData = ADNotificationManager.GetPolicyData(base.User);
				if (policyData != null && !policyData.AttachmentsEnabled)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentsNotEnabledGetAttCmd");
					throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, null, false);
				}
				Unlimited<ByteQuantifiedSize> maxAttachmentSize = (policyData != null) ? policyData.MaxAttachmentSize : Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				value = AttachmentHelper.GetAttachment(base.MailboxSession, attachmentName, base.OutputStream, maxAttachmentSize, itemIdMapping, out incBy);
				base.ProtocolLogger.IncrementValue(ProtocolLoggerData.Attachments);
				base.ProtocolLogger.IncrementValueBy(ProtocolLoggerData.AttachmentBytes, incBy);
			}
			catch (FormatException innerException)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidAttachmentName");
				AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidIDs, innerException, false);
				throw ex;
			}
			catch (ObjectNotFoundException innerException2)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentNotFound");
				AirSyncPermanentException ex2 = new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.None, innerException2, false);
				throw ex2;
			}
			catch (IOException innerException3)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "IOException");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.None, innerException3, false);
			}
			catch (DataTooLargeException innerException4)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentIsTooLarge");
				PolicyData policyData2 = ADNotificationManager.GetPolicyData(base.User);
				if (policyData2 != null)
				{
					policyData2.MaxAttachmentSize.ToString();
				}
				else
				{
					GlobalSettings.MaxDocumentDataSize.ToString(CultureInfo.InvariantCulture);
				}
				throw new AirSyncPermanentException(HttpStatusCode.RequestEntityTooLarge, StatusCode.AttachmentIsTooLarge, innerException4, false);
			}
			finally
			{
				if (folderSyncState != null)
				{
					folderSyncState.Dispose();
					folderSyncState = null;
				}
			}
			base.Context.Response.AppendHeader("Content-Type", value);
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			base.Context.Response.SetErrorResponse(HttpStatusCode.InternalServerError, StatusCode.ServerError);
			return false;
		}
	}
}
