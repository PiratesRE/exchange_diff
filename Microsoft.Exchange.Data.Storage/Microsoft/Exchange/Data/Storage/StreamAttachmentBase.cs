using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class StreamAttachmentBase : Attachment, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
	{
		internal StreamAttachmentBase(CoreAttachment coreAttachment) : base(coreAttachment)
		{
		}

		public virtual Stream GetContentStream()
		{
			base.CheckDisposed("GetContentStream");
			return base.PropertyBag.OpenPropertyStream(this.ContentStreamProperty, base.CalculateOpenMode());
		}

		public virtual Stream GetContentStream(PropertyOpenMode openMode)
		{
			base.CheckDisposed("GetContentStream");
			EnumValidator.ThrowIfInvalid<PropertyOpenMode>(openMode, "openMode");
			return base.PropertyBag.OpenPropertyStream(this.ContentStreamProperty, openMode);
		}

		public virtual Stream TryGetContentStream(PropertyOpenMode openMode)
		{
			base.CheckDisposed("TryGetContentStream");
			EnumValidator.ThrowIfInvalid<PropertyOpenMode>(openMode, "openMode");
			Stream result;
			try
			{
				result = base.PropertyBag.OpenPropertyStream(this.ContentStreamProperty, openMode);
			}
			catch (ObjectNotFoundException)
			{
				if (openMode != PropertyOpenMode.ReadOnly)
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		protected abstract PropertyTagPropertyDefinition ContentStreamProperty { get; }

		protected override Schema Schema
		{
			get
			{
				return StreamAttachmentBaseSchema.Instance;
			}
		}

		private static string GetExtensionByContentType(string contentType)
		{
			if (StandaloneFuzzing.IsEnabled)
			{
				return string.Empty;
			}
			string text = ExtensionToContentTypeMapper.Instance.GetExtensionByContentType(contentType);
			if (!string.IsNullOrEmpty(text))
			{
				text = "." + text;
			}
			return text;
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.OnBeforeSaveUpdateAttachSize();
		}

		protected virtual void OnBeforeSaveUpdateAttachSize()
		{
			if (base.MapiAttach != null)
			{
				return;
			}
			base.Load(new PropertyDefinition[]
			{
				InternalSchema.AttachSize,
				this.ContentStreamProperty
			});
			int num = base.GetValueOrDefault<int>(InternalSchema.AttachSize);
			if (num != 0)
			{
				return;
			}
			object obj = base.TryGetProperty(this.ContentStreamProperty);
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				if (!PropertyError.IsPropertyValueTooBig(propertyError))
				{
					goto IL_8B;
				}
				try
				{
					using (Stream stream = this.OpenPropertyStream(this.ContentStreamProperty, PropertyOpenMode.ReadOnly))
					{
						num = (int)stream.Length;
					}
					goto IL_8B;
				}
				catch (ObjectNotFoundException)
				{
					goto IL_8B;
				}
			}
			num = ((byte[])obj).Length;
			IL_8B:
			base[InternalSchema.AttachSize] = num;
		}

		internal static void CoreObjectUpdateStreamAttachmentName(CoreAttachment coreAttachment)
		{
			ICorePropertyBag propertyBag = coreAttachment.PropertyBag;
			string text = (propertyBag.TryGetProperty(InternalSchema.AttachLongFileName) as string) ?? (propertyBag.TryGetProperty(InternalSchema.AttachFileName) as string);
			string text2 = propertyBag.TryGetProperty(InternalSchema.DisplayName) as string;
			string text3 = propertyBag.TryGetProperty(InternalSchema.AttachExtension) as string;
			if (!string.IsNullOrEmpty(text))
			{
				text = Attachment.TrimFilename(text);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = Attachment.TrimFilename(text2);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text3 = '.' + Attachment.TrimFilename(text3);
			}
			string text4 = null;
			string text5 = null;
			string text6 = null;
			string text7 = null;
			Attachment.TryFindFileExtension(text, out text5, out text4);
			Attachment.TryFindFileExtension(text2, out text7, out text6);
			if (!string.IsNullOrEmpty(text5))
			{
				text3 = text5;
			}
			if (!string.IsNullOrEmpty(text7) && string.Compare(text3, text7, StringComparison.OrdinalIgnoreCase) != 0)
			{
				text6 += text7;
			}
			if (string.IsNullOrEmpty(text4))
			{
				text4 = Attachment.GenerateFilename();
			}
			if (EmailMessageHelpers.IsGeneratedFileName(text4) && string.IsNullOrEmpty(text3))
			{
				string text8 = propertyBag.TryGetProperty(InternalSchema.AttachMimeTag) as string;
				if (!string.IsNullOrEmpty(text8))
				{
					text3 = StreamAttachmentBase.GetExtensionByContentType(text8);
				}
			}
			if (string.IsNullOrEmpty(text6))
			{
				text6 = text4;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text = text4 + text3;
				text2 = text6 + text3;
			}
			else
			{
				text = text4;
				text2 = text6;
				propertyBag.Delete(InternalSchema.AttachExtension);
			}
			propertyBag[InternalSchema.AttachLongFileName] = text;
			propertyBag[InternalSchema.DisplayName] = text2;
			propertyBag[InternalSchema.AttachExtension] = (text3 ?? string.Empty);
			bool flag = false;
			ICoreItem containerItem = coreAttachment.ParentCollection.ContainerItem;
			if (containerItem != null)
			{
				containerItem.PropertyBag.Load(new PropertyDefinition[]
				{
					InternalSchema.IsAssociated
				});
				flag = containerItem.PropertyBag.GetValueOrDefault<bool>(InternalSchema.IsAssociated, false);
			}
			if (!flag)
			{
				propertyBag[InternalSchema.AttachFileName] = Attachment.Make8x3FileName(text, coreAttachment != null && coreAttachment.Session != null && coreAttachment.Session.IsMoveUser);
			}
		}

		internal static void CoreObjectUpdateImageThumbnail(CoreAttachment coreAttachment)
		{
		}

		private static HashSet<string> SupportedThumbnailTypes = new HashSet<string>(new string[]
		{
			".bmp",
			".gif",
			".jpg",
			".png"
		}, StringComparer.OrdinalIgnoreCase);

		private class ImageThumbnailGenerationSettings
		{
			private ImageThumbnailGenerationSettings()
			{
				this.maxSizeOfImageToConvert = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole\\AttachmentImageThumbnail", "MaxSizeOfImageToConvertInKB", 2048, (int i) => i <= 5120);
				this.maxSideInPixels = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole\\AttachmentImageThumbnail", "MaxSideSizeInPixels", 480, (int i) => i <= 480);
				this.maxThumbnailsPerEmail = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole\\AttachmentImageThumbnail", "MaxThumbnailsPerEmail", 30, (int i) => i <= 30);
				this.disableThumbnailGeneration = (StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole\\AttachmentImageThumbnail", "DisableThumbnailGeneration", 0, null) != 0);
				this.bypassFlightForThumbnailGeneration = (StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole\\AttachmentImageThumbnail", "BypassFlightForThumbnailGeneration", 0, null) != 0);
			}

			public static StreamAttachmentBase.ImageThumbnailGenerationSettings Instance
			{
				get
				{
					return StreamAttachmentBase.ImageThumbnailGenerationSettings.instance.Value;
				}
			}

			public bool IsFeatureEnabled(IExchangePrincipal mailboxOwner, IRecipientSession recipientSession)
			{
				if (this.disableThumbnailGeneration)
				{
					return false;
				}
				if (this.bypassFlightForThumbnailGeneration)
				{
					return true;
				}
				if (mailboxOwner == null || recipientSession == null)
				{
					return false;
				}
				ADUser aduser = null;
				try
				{
					aduser = (DirectoryHelper.ReadADRecipient(mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.IsArchive, recipientSession) as ADUser);
				}
				catch (DataValidationException)
				{
					return false;
				}
				catch (DataSourceOperationException)
				{
					return false;
				}
				catch (DataSourceTransientException)
				{
					return false;
				}
				if (aduser == null)
				{
					return false;
				}
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(aduser.GetContext(null), null, null);
				return snapshot.DataStorage.StorageAttachmentImageAnalysis.Enabled;
			}

			public int MaxSizeOfImageToConvertInKB
			{
				get
				{
					return this.maxSizeOfImageToConvert;
				}
			}

			public int MaxSideInPixels
			{
				get
				{
					return this.maxSideInPixels;
				}
			}

			public int MaxThumbnailsPerEmail
			{
				get
				{
					return this.maxThumbnailsPerEmail;
				}
			}

			private const string AttachmentImageThumbnail = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole\\AttachmentImageThumbnail";

			private static readonly Lazy<StreamAttachmentBase.ImageThumbnailGenerationSettings> instance = new Lazy<StreamAttachmentBase.ImageThumbnailGenerationSettings>(() => new StreamAttachmentBase.ImageThumbnailGenerationSettings());

			private readonly int maxSizeOfImageToConvert;

			private readonly int maxSideInPixels;

			private readonly int maxThumbnailsPerEmail;

			private readonly bool disableThumbnailGeneration;

			private readonly bool bypassFlightForThumbnailGeneration;
		}
	}
}
