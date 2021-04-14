using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class RmsDecryptor
	{
		public static DrmEmailMessageContainer DrmEmailMessageContainerFromMessage(EmailMessage rmsMessage)
		{
			ArgumentValidator.ThrowIfNull("rmsMessage", rmsMessage);
			ArgumentValidator.ThrowIfNull("rmsMessage.Attachments", rmsMessage.Attachments);
			Attachment attachment = rmsMessage.Attachments[0];
			Stream stream = null;
			DrmEmailMessageContainer result;
			try
			{
				if (!attachment.TryGetContentReadStream(out stream) || stream == null)
				{
					throw new InvalidOperationException("Failed To read content from the protected attachment");
				}
				DrmEmailMessageContainer drmEmailMessageContainer = new DrmEmailMessageContainer();
				drmEmailMessageContainer.Load(stream, (object param0) => Streams.CreateTemporaryStorageStream());
				result = drmEmailMessageContainer;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
			return result;
		}

		public RmsDecryptor(RmsClientManagerContext context, EmailMessage rmsMessage, DrmEmailMessageContainer drmMsgContainer, string recipientAddress, string useLicense, OutboundConversionOptions contentConversionOption, Breadcrumbs<string> breadcrumbs, bool verifyExtractRights, bool verifyRightsForReEncryption, bool copyHeaders = true, bool convertToTNEF = true)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("rmsMessage", rmsMessage);
			ArgumentValidator.ThrowIfNull("drmMsgContainer", drmMsgContainer);
			ArgumentValidator.ThrowIfNull("contentConversionOption", contentConversionOption);
			ArgumentValidator.ThrowIfNull("rmsMessage.Attachments", rmsMessage.Attachments);
			if (rmsMessage.Attachments.Count == 0)
			{
				throw new ArgumentException("rmsMessage.Attachments.Count");
			}
			this.recipientAddress = recipientAddress;
			this.breadcrumbs = breadcrumbs;
			this.orgId = context.OrgId;
			this.contentConversionOption = contentConversionOption;
			this.rmsMessage = rmsMessage;
			this.messageId = rmsMessage.MessageId;
			this.useLicense = useLicense;
			this.verifyExtractRights = verifyExtractRights;
			this.verifyRightsForReEncryption = verifyRightsForReEncryption;
			this.objHashCode = this.GetHashCode();
			this.rmsContext = context;
			this.drmMsgContainer = drmMsgContainer;
			this.copyHeaders = copyHeaders;
			this.convertToTNEF = convertToTNEF;
			RmsClientManager.GetLicensingUri(this.orgId, this.drmMsgContainer.PublishLicense, out this.licenseUri, out this.publishLicenseAsXmlNodes, out this.isInternalRmsLicensingServer);
		}

		public Task<AsyncOperationResult<DecryptionResultData>> DecryptAsync()
		{
			return Task.Factory.FromAsync<AsyncOperationResult<DecryptionResultData>>(new Func<AsyncCallback, object, IAsyncResult>(this.BeginDecrypt), new Func<IAsyncResult, AsyncOperationResult<DecryptionResultData>>(this.EndDecrypt), null);
		}

		public IAsyncResult BeginDecrypt(AsyncCallback asyncCallback, object state)
		{
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(null, state, asyncCallback);
			bool flag = false;
			try
			{
				if (this.breadcrumbs != null)
				{
					this.breadcrumbs.Drop(RmsDecryptor.State.BeginDecrypt.ToString());
				}
				this.BeginAcquireUseLicense(lazyAsyncResult);
				flag = true;
			}
			finally
			{
				if (!flag && this.drmMsgContainer != null)
				{
					this.drmMsgContainer.Dispose();
					this.drmMsgContainer = null;
				}
			}
			return lazyAsyncResult;
		}

		public AsyncOperationResult<DecryptionResultData> EndDecrypt(IAsyncResult result)
		{
			if (this.breadcrumbs != null)
			{
				this.breadcrumbs.Drop(RmsDecryptor.State.EndDecrypt.ToString());
			}
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			return (AsyncOperationResult<DecryptionResultData>)lazyAsyncResult.InternalWaitForCompletion();
		}

		private void BeginAcquireUseLicense(object state)
		{
			if (!string.IsNullOrEmpty(this.useLicense))
			{
				AsyncDecryptionOperationResult<DecryptionResultData> value = this.DecryptMessage();
				LazyAsyncResult lazyAsyncResult = state as LazyAsyncResult;
				lazyAsyncResult.InvokeCallback(value);
				return;
			}
			if (this.breadcrumbs != null)
			{
				this.breadcrumbs.Drop(RmsDecryptor.State.BeginAcquireUseLicense.ToString());
			}
			RmsClientManager.SaveContextCallback = new RmsClientManager.SaveContextOnAsyncQueryCallback(this.SaveContext);
			RmsClientManager.BeginAcquireUseLicense(this.rmsContext, this.licenseUri, this.isInternalRmsLicensingServer, this.publishLicenseAsXmlNodes, this.recipientAddress, new AsyncCallback(this.OnAcquireUseLicenseComplete), state);
		}

		private string EndAcquireUseLicense(IAsyncResult result)
		{
			if (this.breadcrumbs != null)
			{
				this.breadcrumbs.Drop(RmsDecryptor.State.EndAcquireUseLicense.ToString());
			}
			return RmsClientManager.EndAcquireUseLicense(result).License;
		}

		private void OnAcquireUseLicenseComplete(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState cannot be null here");
			}
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult.AsyncState;
			Exception ex = null;
			try
			{
				this.useLicense = this.EndAcquireUseLicense(asyncResult);
			}
			catch (RightsManagementException ex2)
			{
				ex = ex2;
			}
			catch (ExchangeConfigurationException ex3)
			{
				ex = ex3;
			}
			AsyncDecryptionOperationResult<DecryptionResultData> value;
			if (ex != null)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError<string, OrganizationId, Exception>((long)this.objHashCode, "Failed to acquire use license {0}, orgId {1}, error {2}", this.messageId, this.orgId, ex);
				if (this.drmMsgContainer != null)
				{
					this.drmMsgContainer.Dispose();
					this.drmMsgContainer = null;
				}
				DecryptionResultData data = new DecryptionResultData(null, this.useLicense, this.licenseUri);
				value = new AsyncDecryptionOperationResult<DecryptionResultData>(data, ex);
			}
			else
			{
				value = this.DecryptMessage();
			}
			lazyAsyncResult.InvokeCallback(value);
		}

		private AsyncDecryptionOperationResult<DecryptionResultData> DecryptMessage()
		{
			Exception ex = null;
			EmailMessage emailMessage = null;
			try
			{
				if (this.breadcrumbs != null)
				{
					this.breadcrumbs.Drop(RmsDecryptor.State.DecryptMessage.ToString());
				}
				if (string.IsNullOrEmpty(this.useLicense))
				{
					throw new ArgumentNullException("this.useLicense");
				}
				if (this.verifyExtractRights || this.verifyRightsForReEncryption)
				{
					SafeRightsManagementQueryHandle safeRightsManagementQueryHandle = null;
					try
					{
						int hr = SafeNativeMethods.DRMParseUnboundLicense(this.useLicense, out safeRightsManagementQueryHandle);
						Errors.ThrowOnErrorCode(hr);
						this.VerifyHasSufficientRightToReEncrypt(safeRightsManagementQueryHandle);
						this.VerifyExtractRightsBeforeDecryptingMessage(safeRightsManagementQueryHandle);
					}
					finally
					{
						if (safeRightsManagementQueryHandle != null)
						{
							safeRightsManagementQueryHandle.Close();
						}
					}
				}
				RpMsgToMsgConverter rpMsgToMsgConverter = new RpMsgToMsgConverter(this.drmMsgContainer, this.orgId, true);
				using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.AcquireTenantLicenses(this.rmsContext, this.licenseUri))
				{
					using (MessageItem messageItem = rpMsgToMsgConverter.ConvertRpmsgToMsg(null, this.useLicense, disposableTenantLicensePair.EnablingPrincipalRac))
					{
						emailMessage = Utils.ConvertMessageItemToEmailMessage(messageItem, this.contentConversionOption, this.convertToTNEF);
					}
				}
				if (this.copyHeaders)
				{
					Utils.CopyHeadersDuringDecryption(this.rmsMessage, emailMessage);
				}
			}
			catch (RightsManagementException ex2)
			{
				ex = ex2;
			}
			catch (InvalidRpmsgFormatException ex3)
			{
				ex = ex3;
			}
			catch (ExchangeConfigurationException ex4)
			{
				ex = ex4;
			}
			catch (StorageTransientException ex5)
			{
				ex = ex5;
			}
			catch (StoragePermanentException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (this.drmMsgContainer != null)
				{
					this.drmMsgContainer.Dispose();
					this.drmMsgContainer = null;
				}
				if (ex != null)
				{
					if (this.breadcrumbs != null)
					{
						this.breadcrumbs.Drop(RmsDecryptor.State.AcquireUseLicenseFailed.ToString());
					}
					ExTraceGlobals.RightsManagementTracer.TraceError<string, OrganizationId, Exception>((long)this.objHashCode, "DecryptMessage failed for message {0}, orgId {1}, error {2}", this.messageId, this.orgId, ex);
				}
			}
			DecryptionResultData data = new DecryptionResultData(emailMessage, this.useLicense, this.licenseUri);
			return new AsyncDecryptionOperationResult<DecryptionResultData>(data, ex);
		}

		private void VerifyHasSufficientRightToReEncrypt(SafeRightsManagementQueryHandle queryRootHandle)
		{
			if (!this.verifyRightsForReEncryption)
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.objHashCode, "No need to VerifyHasSufficientRightToReEncrypt.");
				return;
			}
			ContentRight usageRightsFromLicense = DrmClientUtils.GetUsageRightsFromLicense(queryRootHandle);
			if ((usageRightsFromLicense & ContentRight.Owner) == ContentRight.Owner)
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.objHashCode, "VerifyHasSufficientRightToReEncrypt verified owner right");
				return;
			}
			if ((usageRightsFromLicense & ContentRight.Edit) == ContentRight.Edit)
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.objHashCode, "VerifyHasSufficientRightToReEncrypt verified Edit and ViewRightsData rights");
				return;
			}
			ExTraceGlobals.RightsManagementTracer.TraceError((long)this.objHashCode, "VerifyHasSufficientRightToReEncrypt doesn't detect enough rights to re-encrypt");
			throw new RightsManagementException(RightsManagementFailureCode.NotEnoughRightsToReEncrypt, Strings.NotEnoughRightsToReEncrypt((int)usageRightsFromLicense));
		}

		private void VerifyExtractRightsBeforeDecryptingMessage(SafeRightsManagementQueryHandle queryRootHandle)
		{
			if (!this.verifyExtractRights)
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.objHashCode, "No need to VerifyExtractRightsBeforeDecryptingMessage.");
				return;
			}
			if (this.isInternalRmsLicensingServer)
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.objHashCode, "No need to VerifyExtractRightsBeforeDecryptingMessage for internal RMS messages.");
				return;
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<OrganizationId, Uri>((long)this.objHashCode, "Verifying if the recipient org has rights to decrypt the message to archive in clear. Org Id {0}, License Uri {1}", this.orgId, this.licenseUri);
			if (!DrmClientUtils.IsExchangeRecipientOrganizationExtractAllowed(queryRootHandle))
			{
				ExTraceGlobals.RightsManagementTracer.TraceError<OrganizationId, Uri>((long)this.objHashCode, "Extract not allowed for message protected against an external RMS server. Org Id {0}, License Uri {1}", this.orgId, this.licenseUri);
				throw new RightsManagementException(RightsManagementFailureCode.ExtractNotAllowed, Strings.ExtractNotAllowed(this.licenseUri, this.orgId.ToString()));
			}
		}

		private void SaveContext(object state)
		{
			LazyAsyncResult lazyAsyncResult = state as LazyAsyncResult;
			if (lazyAsyncResult != null)
			{
				AgentAsyncState agentAsyncState = lazyAsyncResult.AsyncState as AgentAsyncState;
				if (agentAsyncState != null)
				{
					agentAsyncState.Resume();
				}
			}
		}

		private readonly EmailMessage rmsMessage;

		private readonly OutboundConversionOptions contentConversionOption;

		private readonly bool convertToTNEF;

		private readonly OrganizationId orgId;

		private readonly Breadcrumbs<string> breadcrumbs;

		private readonly string recipientAddress;

		private readonly bool verifyExtractRights;

		private readonly bool verifyRightsForReEncryption;

		private readonly Uri licenseUri;

		private readonly bool isInternalRmsLicensingServer;

		private readonly string messageId;

		private readonly XmlNode[] publishLicenseAsXmlNodes;

		private readonly int objHashCode;

		private readonly RmsClientManagerContext rmsContext;

		private readonly bool copyHeaders;

		private DrmEmailMessageContainer drmMsgContainer;

		private string useLicense;

		private enum State
		{
			BeginDecrypt,
			BeginAcquireUseLicense,
			AcquireUseLicenseFailed,
			EndAcquireUseLicense,
			EndDecrypt,
			DecryptMessage,
			OnAcquireUseLicenseComplete
		}
	}
}
