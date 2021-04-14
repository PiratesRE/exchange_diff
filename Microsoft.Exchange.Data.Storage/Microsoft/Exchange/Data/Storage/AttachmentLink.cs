using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentLink
	{
		internal AttachmentLink(Attachment attachment)
		{
			this.attachmentType = attachment.AttachmentType;
			this.attachmentId = attachment.Id;
			this.contentId = attachment.ContentId;
			this.contentBase = attachment.ContentBase;
			this.contentLocation = attachment.ContentLocation;
			this.filename = attachment.FileName;
			this.displayName = attachment.DisplayName;
			this.size = attachment.Size;
			this.originalIsInline = attachment.IsInline;
			this.markedInline = null;
			this.contentType = AttachmentLink.GetContentType(attachment);
			this.renderingPosition = attachment.GetValueOrDefault<int>(InternalSchema.RenderingPosition, -1);
			this.isHidden = attachment.GetValueOrDefault<bool>(InternalSchema.AttachCalendarHidden);
			this.isChanged = false;
		}

		internal static string CreateContentId(ICoreItem containerItem, AttachmentId id, string domain)
		{
			byte[] array;
			if (containerItem != null && containerItem.Session != null && containerItem.Id != null && id != null)
			{
				array = Util.MergeArrays<byte>(new ICollection<byte>[]
				{
					containerItem.Id.GetBytes(),
					id.ToByteArray()
				});
				array = CryptoUtil.GetSha1Hash(array);
			}
			else
			{
				array = Guid.NewGuid().ToByteArray();
			}
			if (!string.IsNullOrEmpty(domain))
			{
				return string.Format("{0}@{1}", HexConverter.ByteArrayToHexString(array), domain);
			}
			return string.Format("{0}@1", HexConverter.ByteArrayToHexString(array));
		}

		private static string GetContentType(Attachment attachment)
		{
			string calculatedContentType = attachment.ContentType;
			if (calculatedContentType == null || calculatedContentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
			{
				calculatedContentType = attachment.CalculatedContentType;
			}
			return calculatedContentType;
		}

		internal static AttachmentLink Find(AttachmentId attachmentId, IList<AttachmentLink> attachmentLinks)
		{
			if (attachmentId != null && attachmentLinks != null)
			{
				foreach (AttachmentLink attachmentLink in attachmentLinks)
				{
					if (attachmentId.Equals(attachmentLink.AttachmentId))
					{
						return attachmentLink;
					}
				}
			}
			return null;
		}

		internal static ReadOnlyCollection<AttachmentLink> MergeAttachmentLinks(IList<AttachmentLink> existingLinks, CoreAttachmentCollection attachments)
		{
			IList<AttachmentLink> list;
			if (attachments != null)
			{
				list = ((existingLinks == null) ? new List<AttachmentLink>(attachments.Count) : new List<AttachmentLink>(existingLinks));
				ICollection<PropertyDefinition> preloadProperties = new PropertyDefinition[]
				{
					AttachmentSchema.AttachContentId
				};
				using (IEnumerator<AttachmentHandle> enumerator = attachments.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AttachmentHandle handle = enumerator.Current;
						using (CoreAttachment coreAttachment = attachments.Open(handle, preloadProperties))
						{
							using (Attachment attachment = AttachmentCollection.CreateTypedAttachment(coreAttachment, null))
							{
								if (AttachmentLink.Find(attachment.Id, list) == null)
								{
									AttachmentLink item = new AttachmentLink(attachment);
									list.Add(item);
								}
							}
						}
					}
					goto IL_C5;
				}
			}
			list = ((existingLinks == null) ? new List<AttachmentLink>(0) : new List<AttachmentLink>(existingLinks));
			IL_C5:
			return new ReadOnlyCollection<AttachmentLink>(list);
		}

		public void ConvertToImage()
		{
			this.needConversionToImage = true;
		}

		public AttachmentType AttachmentType
		{
			get
			{
				return this.attachmentType;
			}
		}

		public AttachmentId AttachmentId
		{
			get
			{
				return this.attachmentId;
			}
		}

		public string ContentId
		{
			get
			{
				return this.contentId;
			}
			set
			{
				if (this.contentId != value)
				{
					this.contentId = value;
					this.contentBase = null;
					this.contentLocation = null;
					this.isChanged = true;
				}
			}
		}

		public int RenderingPosition
		{
			get
			{
				return this.renderingPosition;
			}
			set
			{
				if (this.renderingPosition != value)
				{
					this.renderingPosition = value;
					this.isChanged = true;
				}
			}
		}

		public bool IsOriginallyInline
		{
			get
			{
				return this.originalIsInline;
			}
		}

		public bool? IsMarkedInline
		{
			get
			{
				return this.markedInline;
			}
		}

		public bool IsInline(bool requireMarkInline)
		{
			if (this.markedInline != null)
			{
				return this.markedInline.Value;
			}
			return !requireMarkInline && this.originalIsInline;
		}

		public bool IsHidden
		{
			get
			{
				return this.isHidden;
			}
			set
			{
				if (this.isHidden != value)
				{
					this.isHidden = value;
					this.isChanged = true;
				}
			}
		}

		public bool NeedsSave(bool requireMarkInline)
		{
			return this.isChanged || this.IsInline(requireMarkInline) != this.originalIsInline;
		}

		public bool NeedsConversionToImage
		{
			get
			{
				return this.needConversionToImage;
			}
		}

		public Uri ContentBase
		{
			get
			{
				return this.contentBase;
			}
		}

		public Uri ContentLocation
		{
			get
			{
				return this.contentLocation;
			}
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public string Filename
		{
			get
			{
				return this.filename;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string FileExtension
		{
			get
			{
				string text = null;
				string text2 = null;
				Attachment.TryFindFileExtension(this.Filename, out text, out text2);
				return text ?? string.Empty;
			}
		}

		public long Size
		{
			get
			{
				return this.size;
			}
		}

		public void MarkInline(bool isInline)
		{
			this.markedInline = new bool?(isInline);
		}

		internal bool MakeAttachmentChanges(Attachment attachment, bool requireMarkInline)
		{
			if (this.NeedsSave(requireMarkInline))
			{
				attachment.IsInline = this.IsInline(requireMarkInline);
				attachment.ContentId = this.contentId;
				attachment.RenderingPosition = (attachment.IsInline ? this.renderingPosition : -1);
				attachment[InternalSchema.AttachCalendarHidden] = this.isHidden;
				return true;
			}
			return false;
		}

		private static Guid defaultContentIdPrefix = new Guid("b4dcd1dd-dee4-4724-81cc-6dde78879c0d");

		private AttachmentId attachmentId;

		private Uri contentBase;

		private Uri contentLocation;

		private string contentId;

		private string contentType;

		private string filename;

		private string displayName;

		private int renderingPosition;

		private long size;

		private bool? markedInline;

		private bool originalIsInline;

		private bool isHidden;

		private bool isChanged;

		private bool needConversionToImage;

		private AttachmentType attachmentType;
	}
}
