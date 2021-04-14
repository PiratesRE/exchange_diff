using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReferenceAttachment : StreamAttachmentBase, IReferenceAttachment, IAttachment, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal ReferenceAttachment(CoreAttachment coreAttachment) : base(coreAttachment)
		{
		}

		public override AttachmentType AttachmentType
		{
			get
			{
				base.CheckDisposed("AttachmentType::get");
				return AttachmentType.Reference;
			}
		}

		public string AttachLongPathName
		{
			get
			{
				base.CheckDisposed("AttachLongPathName::get");
				return base.GetValueOrDefault<string>(InternalSchema.AttachLongPathName, null);
			}
			set
			{
				base.CheckDisposed("AttachLongPathName::set");
				base[InternalSchema.AttachLongPathName] = value;
			}
		}

		public string ProviderEndpointUrl
		{
			get
			{
				base.CheckDisposed("ProviderEndpointUrl::get");
				return base.GetValueOrDefault<string>(InternalSchema.AttachmentProviderEndpointUrl, null);
			}
			set
			{
				base.CheckDisposed("ProviderEndpointUrl::set");
				base[InternalSchema.AttachmentProviderEndpointUrl] = value;
			}
		}

		public string ProviderType
		{
			get
			{
				base.CheckDisposed("ProviderType::get");
				return base.GetValueOrDefault<string>(InternalSchema.AttachmentProviderType, null);
			}
			set
			{
				base.CheckDisposed("ProviderType::set");
				base[InternalSchema.AttachmentProviderType] = value;
			}
		}

		protected override Schema Schema
		{
			get
			{
				return ReferenceAttachmentSchema.Instance;
			}
		}

		protected override PropertyTagPropertyDefinition ContentStreamProperty
		{
			get
			{
				return null;
			}
		}

		internal static void CoreObjectUpdateReferenceAttachmentName(CoreAttachment coreAttachment)
		{
			ICorePropertyBag propertyBag = coreAttachment.PropertyBag;
			string text = propertyBag.TryGetProperty(InternalSchema.DisplayName) as string;
			string text2 = propertyBag.TryGetProperty(InternalSchema.AttachExtension) as string;
			if (!string.IsNullOrEmpty(text))
			{
				text = Attachment.TrimFilename(text);
				propertyBag[InternalSchema.DisplayName] = text;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = '.' + Attachment.TrimFilename(text2);
				propertyBag[InternalSchema.AttachExtension] = (text2 ?? string.Empty);
			}
		}

		public override Stream GetContentStream()
		{
			base.CheckDisposed("GetContentStream");
			return new MemoryStream();
		}

		public override Stream GetContentStream(PropertyOpenMode openMode)
		{
			base.CheckDisposed("GetContentStream");
			return new MemoryStream();
		}

		public override Stream TryGetContentStream(PropertyOpenMode openMode)
		{
			base.CheckDisposed("TryGetContentStream");
			return new MemoryStream();
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ReferenceAttachment>(this);
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.OnBeforeSaveUpdateAttachMethod();
			this.OnBeforeSaveUpdateExtraProperties();
		}

		protected override void OnBeforeSaveUpdateAttachSize()
		{
			if (base.MapiAttach != null)
			{
				return;
			}
			base.Load(new PropertyDefinition[]
			{
				InternalSchema.AttachSize
			});
			base[InternalSchema.AttachSize] = 0;
		}

		private void OnBeforeSaveUpdateAttachMethod()
		{
			object obj = base.PropertyBag.TryGetProperty(InternalSchema.AttachMethod);
			if (obj is PropertyError)
			{
				obj = 7;
			}
			else
			{
				int num = (int)obj;
				if (num != 3 && num != 2 && num != 4)
				{
					num = 7;
				}
				obj = num;
			}
			base.PropertyBag[InternalSchema.AttachMethod] = obj;
		}

		private void OnBeforeSaveUpdateExtraProperties()
		{
			base.PropertyBag[InternalSchema.AttachCalendarHidden] = true;
		}

		internal const int AttachMethod = 7;
	}
}
