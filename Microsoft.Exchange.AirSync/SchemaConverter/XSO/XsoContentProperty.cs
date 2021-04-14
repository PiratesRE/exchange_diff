using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoContentProperty : XsoProperty, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public XsoContentProperty(PropertyType type) : base(null, type)
		{
		}

		public XsoContentProperty() : base(null, PropertyType.ReadWrite)
		{
		}

		public Stream Body
		{
			get
			{
				Item item = base.XsoItem as Item;
				if (item == null)
				{
					return null;
				}
				long num;
				IList<AttachmentLink> list;
				return BodyConversionUtilities.ConvertToBodyStream(item, -1L, out num, out list);
			}
		}

		public bool IsOnSMIMEMessage
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				return messageItem != null && ObjectClass.IsSmime(messageItem.ClassName);
			}
		}

		public Stream MIMEData
		{
			get
			{
				if (this.mimeData == null)
				{
					this.mimeData = BodyUtility.ConvertItemToMime(base.XsoItem);
				}
				return this.mimeData;
			}
			set
			{
				throw new NotImplementedException("set_MIMEData");
			}
		}

		public long MIMESize { get; set; }

		public bool RtfPresent
		{
			get
			{
				return this.actualBody != null && this.actualBody.Format == Microsoft.Exchange.Data.Storage.BodyFormat.ApplicationRtf;
			}
		}

		public long Size
		{
			get
			{
				Item item = base.XsoItem as Item;
				if (item == null)
				{
					return 0L;
				}
				if (string.Equals(item.ClassName, "IPM.Note.SMIME", StringComparison.OrdinalIgnoreCase))
				{
					string text = Strings.SMIMENotSupportedBodyText.ToString(item.Session.PreferedCulture);
					return (long)text.Length;
				}
				if (this.actualBody.Format == Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain)
				{
					return this.actualBody.Size / 2L;
				}
				return this.actualBody.Size;
			}
		}

		public bool IsIrmErrorMessage
		{
			get
			{
				return this.isIrmErrorMessage;
			}
		}

		public virtual void PreProcessProperty()
		{
			Item item = (Item)base.XsoItem;
			if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
			{
				this.actualBody = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			else
			{
				this.actualBody = item.Body;
			}
			this.originalItem = null;
			this.isIrmErrorMessage = false;
			if (BodyConversionUtilities.IsIRMFailedToDecode(item))
			{
				MessageItem messageItem = this.CreateIrmErrorMessage();
				if (messageItem != null)
				{
					base.XsoItem = messageItem;
					this.originalItem = item;
					this.isIrmErrorMessage = true;
				}
			}
		}

		public virtual void PostProcessProperty()
		{
			this.actualBody = null;
			if (this.originalItem != null)
			{
				MessageItem messageItem = (MessageItem)base.XsoItem;
				if (messageItem != null)
				{
					messageItem.Dispose();
				}
				base.XsoItem = this.originalItem;
				this.originalItem = null;
			}
		}

		public Stream GetData(BodyType type, long truncationSize, out long totalDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments)
		{
			Item item = base.XsoItem as Item;
			attachments = null;
			if (item == null)
			{
				totalDataSize = 0L;
				return null;
			}
			IList<AttachmentLink> list = null;
			Stream stream;
			if (string.Equals(item.ClassName, "IPM.Note.SMIME", StringComparison.OrdinalIgnoreCase) && truncationSize != 0L)
			{
				switch (type)
				{
				case BodyType.PlainText:
				{
					string s = Strings.SMIMENotSupportedBodyText.ToString(item.Session.PreferedCulture);
					stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
					totalDataSize = stream.Length;
					return stream;
				}
				case BodyType.Html:
				{
					string s = XsoContentProperty.GetSMIMENotSupportedBodyHtml(item.Session);
					stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
					totalDataSize = stream.Length;
					return stream;
				}
				}
			}
			switch (type)
			{
			case BodyType.None:
				throw new ConversionException("Invalid body type requested");
			case BodyType.PlainText:
				stream = BodyConversionUtilities.ConvertToPlainTextStream(item, truncationSize, out totalDataSize, out list);
				break;
			case BodyType.Html:
				stream = BodyConversionUtilities.ConvertHtmlStream(item, truncationSize, out totalDataSize, out list);
				break;
			case BodyType.Rtf:
				return this.GetRtfData(truncationSize, out totalDataSize, out list);
			case BodyType.Mime:
				throw new ConversionException("Invalid body type requested");
			default:
				stream = null;
				totalDataSize = 0L;
				break;
			}
			if (list != null)
			{
				attachments = from attachmentLink in list
				select new AirSyncAttachmentInfo
				{
					AttachmentId = attachmentLink.AttachmentId,
					IsInline = (attachmentLink.IsMarkedInline ?? attachmentLink.IsOriginallyInline),
					ContentId = attachmentLink.ContentId
				};
			}
			return stream;
		}

		public BodyType GetNativeType()
		{
			if (!(base.XsoItem is Item))
			{
				return BodyType.None;
			}
			BodyType result;
			switch (this.actualBody.Format)
			{
			case Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain:
				result = BodyType.PlainText;
				break;
			case Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml:
				result = BodyType.Html;
				break;
			case Microsoft.Exchange.Data.Storage.BodyFormat.ApplicationRtf:
				result = BodyType.Rtf;
				break;
			default:
				throw new ConversionException("Unknown BodyFormat implemented by XSO");
			}
			return result;
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			Item item = (Item)base.XsoItem;
			IContentProperty contentProperty = (IContentProperty)srcProperty;
			switch (contentProperty.GetNativeType())
			{
			case BodyType.PlainText:
				break;
			case BodyType.Html:
				goto IL_D3;
			case BodyType.Rtf:
				if (contentProperty.Body.Length > 0L)
				{
					using (Stream stream = XsoContentProperty.OpenBodyWriteStream(item, Microsoft.Exchange.Data.Storage.BodyFormat.ApplicationRtf))
					{
						try
						{
							StreamHelper.CopyStreamWithBase64Conversion(contentProperty.Body, stream, -1, false);
						}
						catch (FormatException innerException)
						{
							throw new AirSyncPermanentException(StatusCode.Sync_ServerError, innerException, false)
							{
								ErrorStringForProtocolLogger = "RtfToBase64StreamError"
							};
						}
						return;
					}
				}
				using (TextWriter textWriter = item.Body.OpenTextWriter(Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain))
				{
					textWriter.Write(string.Empty);
					return;
				}
				break;
			default:
				goto IL_F8;
			}
			using (Stream stream2 = XsoContentProperty.OpenBodyWriteStream(item, Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain))
			{
				StreamHelper.CopyStream(contentProperty.Body, stream2);
				return;
			}
			IL_D3:
			using (Stream stream3 = XsoContentProperty.OpenBodyWriteStream(item, Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml))
			{
				StreamHelper.CopyStream(contentProperty.Body, stream3);
				return;
			}
			IL_F8:
			throw new ConversionException("Source body property does not have Rtf or Text body present");
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			Item item = (Item)base.XsoItem;
			using (TextWriter textWriter = item.Body.OpenTextWriter(Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain))
			{
				textWriter.Write(string.Empty);
			}
		}

		public override void Unbind()
		{
			this.mimeData = null;
			base.Unbind();
		}

		private static Stream OpenBodyWriteStream(Item item, Microsoft.Exchange.Data.Storage.BodyFormat bodyFormat)
		{
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(bodyFormat, "utf-8");
			return item.Body.OpenWriteStream(configuration);
		}

		private static string GetSMIMENotSupportedBodyHtml(StoreSession session)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<html>");
			stringBuilder.Append("<body>");
			stringBuilder.Append("<font color=\"red\">");
			stringBuilder.Append(Strings.SMIMENotSupportedBodyText.ToString(session.PreferedCulture));
			stringBuilder.Append("</font>");
			stringBuilder.Append("</body>");
			stringBuilder.Append("</html>");
			return stringBuilder.ToString();
		}

		private Stream GetRtfData(long truncationSize, out long totalDataSize, out IList<AttachmentLink> attachmentLinks)
		{
			if (truncationSize == 0L)
			{
				totalDataSize = this.Size;
				attachmentLinks = null;
				return XsoContentProperty.emptyStream;
			}
			Item item = (Item)base.XsoItem;
			item.Load();
			return BodyConversionUtilities.ConvertToRtfStream(item, truncationSize, out totalDataSize, out attachmentLinks);
		}

		private MessageItem CreateIrmErrorMessage()
		{
			RightsManagedMessageItem rightsManagedMessageItem = ((Item)base.XsoItem) as RightsManagedMessageItem;
			if (rightsManagedMessageItem == null)
			{
				return null;
			}
			bool flag = false;
			MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
			if (messageItem == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.XsoTracer, null, "Failed to create in memory message item");
				return null;
			}
			try
			{
				Item.CopyItemContent(rightsManagedMessageItem, messageItem);
				using (TextWriter textWriter = messageItem.Body.OpenTextWriter(Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain))
				{
					RightsManagementFailureCode failureCode = rightsManagedMessageItem.DecryptionStatus.FailureCode;
					if (failureCode > RightsManagementFailureCode.PreLicenseAcquisitionFailed)
					{
						switch (failureCode)
						{
						case RightsManagementFailureCode.FailedToExtractTargetUriFromMex:
						case RightsManagementFailureCode.FailedToDownloadMexData:
							textWriter.Write(Strings.IRMReachNotConfiguredBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
							goto IL_1F7;
						case RightsManagementFailureCode.GetServerInfoFailed:
							goto IL_1AF;
						case RightsManagementFailureCode.InternalLicensingDisabled:
						case RightsManagementFailureCode.ExternalLicensingDisabled:
							break;
						default:
							switch (failureCode)
							{
							case RightsManagementFailureCode.ServerRightNotGranted:
								textWriter.Write(Strings.IRMServerNotConfiguredBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
								goto IL_1F7;
							case RightsManagementFailureCode.InvalidLicensee:
								goto IL_149;
							case RightsManagementFailureCode.FeatureDisabled:
								break;
							case RightsManagementFailureCode.NotSupported:
							case RightsManagementFailureCode.MissingLicense:
							case RightsManagementFailureCode.InvalidLicensingLocation:
								goto IL_1AF;
							case RightsManagementFailureCode.CorruptData:
								textWriter.Write(Strings.IRMCorruptProtectedMessageBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
								goto IL_1F7;
							case RightsManagementFailureCode.ExpiredLicense:
								textWriter.Write(Strings.IRMLicenseExpiredBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
								goto IL_1F7;
							default:
								goto IL_1AF;
							}
							break;
						}
						flag = true;
						goto IL_1F7;
					}
					if (failureCode == RightsManagementFailureCode.UserRightNotGranted)
					{
						textWriter.Write(Strings.IRMNoViewRightsBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
						goto IL_1F7;
					}
					if (failureCode != RightsManagementFailureCode.PreLicenseAcquisitionFailed)
					{
						goto IL_1AF;
					}
					IL_149:
					textWriter.Write(Strings.IRMPreLicensingFailureBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
					goto IL_1F7;
					IL_1AF:
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.XsoTracer, null, "IRM decryption status {0}", rightsManagedMessageItem.DecryptionStatus.FailureCode.ToString());
					textWriter.Write(Strings.IRMServerNotAvailableBodyText.ToString(rightsManagedMessageItem.Session.PreferedCulture));
					IL_1F7:;
				}
			}
			catch (Exception)
			{
				if (messageItem != null)
				{
					flag = true;
				}
				throw;
			}
			finally
			{
				if (flag)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return messageItem;
		}

		private static readonly Stream emptyStream = new MemoryStream(0);

		private Stream mimeData;

		private bool isIrmErrorMessage;

		protected Body actualBody;

		protected Item originalItem;
	}
}
