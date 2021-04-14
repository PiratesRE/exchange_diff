using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MimePartInfo
	{
		public Charset Charset
		{
			get
			{
				return this.charset;
			}
		}

		public List<MimePartInfo> Children
		{
			get
			{
				return this.children;
			}
		}

		public MimePartInfo AttachedItemStructure
		{
			get
			{
				return this.attachedItem;
			}
			set
			{
				this.attachedItem = value;
			}
		}

		internal AttachmentId AttachmentId
		{
			get
			{
				return this.attachmentId;
			}
		}

		public MimePartHeaders Headers
		{
			get
			{
				return this.headers;
			}
			set
			{
				this.headers = value;
			}
		}

		public int BodySize
		{
			get
			{
				return this.bodyBytes;
			}
		}

		public int BodyLineCount
		{
			get
			{
				return this.bodyLines;
			}
		}

		public MimePartInfo(Charset charset, MimePartInfo.Callback writerCallback, MimePartContentType contentType, ref int partIndex) : this(charset, writerCallback, contentType, null, null, null, ref partIndex)
		{
			EnumValidator.ThrowIfInvalid<MimePartContentType>(contentType, "contentType");
		}

		internal MimePartInfo(Charset charset, MimePartInfo.Callback writerCallback, MimePartContentType contentType, AttachmentId attachmentId, ref int partIndex) : this(charset, writerCallback, contentType, attachmentId, null, null, ref partIndex)
		{
		}

		internal MimePartInfo(Charset charset, MimePartInfo.Callback writerCallback, MimePartContentType contentType, AttachmentId attachmentId, MimePart skeletonPart, MimeDocument skeleton, ref int partIndex) : this(charset, writerCallback, contentType, attachmentId, skeletonPart, skeleton, null, null, ref partIndex)
		{
		}

		internal MimePartInfo(Charset charset, MimePartInfo.Callback writerCallback, MimePartContentType contentType, AttachmentId attachmentId, MimePart skeletonPart, MimeDocument skeleton, MimePart smimePart, MimeDocument smimeDocument, ref int partIndex)
		{
			this.charset = charset;
			this.contentType = contentType;
			this.writerCallback = writerCallback;
			this.partIndex = partIndex++;
			this.attachmentId = attachmentId;
			this.skeletonPart = skeletonPart;
			this.skeleton = skeleton;
			this.smimePart = smimePart;
			this.smimeDocument = smimeDocument;
			this.bodyLines = -1;
			this.bodyBytes = -1;
		}

		internal MimePartContentType ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		internal bool IsBodyToRemoveFromSkeleton
		{
			get
			{
				return this.isBodyToRemoveFromSkeleton;
			}
			set
			{
				this.isBodyToRemoveFromSkeleton = value;
			}
		}

		public bool IsMultipart
		{
			get
			{
				return this.contentType >= MimePartContentType.FirstMultipartType;
			}
		}

		internal bool IsAttachment
		{
			get
			{
				return this.attachmentId != null;
			}
		}

		internal string TypeName
		{
			get
			{
				return MimePartInfo.GetContentTypeName(this.contentType);
			}
		}

		internal static string GetContentTypeName(MimePartContentType contentType)
		{
			switch (contentType)
			{
			case MimePartContentType.TextPlain:
				return "text/plain";
			case MimePartContentType.TextHtml:
				return "text/html";
			case MimePartContentType.TextEnriched:
				return "text/enriched";
			case MimePartContentType.Tnef:
				return "application/ms-tnef";
			case MimePartContentType.Calendar:
				return "text/calendar";
			case MimePartContentType.FirstMultipartType:
				return "multipart/alternative";
			case MimePartContentType.MultipartRelated:
				return "multipart/related";
			case MimePartContentType.MultipartMixed:
				return "multipart/mixed";
			case MimePartContentType.MultipartReportDsn:
				return "multipart/report";
			case MimePartContentType.MultipartReportMdn:
				return "multipart/report";
			}
			return null;
		}

		public static MimePartContentType GetContentType(string contentTypeName)
		{
			if (contentTypeName.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.TextPlain;
			}
			if (contentTypeName.Equals("text/html", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.TextHtml;
			}
			if (contentTypeName.Equals("text/enriched", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.TextEnriched;
			}
			if (contentTypeName.Equals("application/ms-tnef", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.Tnef;
			}
			if (contentTypeName.Equals("text/calendar", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.Calendar;
			}
			if (contentTypeName.Equals("multipart/alternative", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.FirstMultipartType;
			}
			if (contentTypeName.Equals("multipart/related", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.MultipartRelated;
			}
			if (contentTypeName.Equals("multipart/mixed", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.MultipartMixed;
			}
			if (contentTypeName.Equals("multipart/report", StringComparison.OrdinalIgnoreCase))
			{
				return MimePartContentType.MultipartReportDsn;
			}
			return MimePartContentType.Attachment;
		}

		internal string SubpartContentType
		{
			get
			{
				if (this.children != null && this.children.Count != 0)
				{
					return this.children[0].TypeName;
				}
				return null;
			}
		}

		internal int PartIndex
		{
			get
			{
				return this.partIndex;
			}
		}

		internal MimePartInfo.Callback WriterCallback
		{
			get
			{
				return this.writerCallback;
			}
		}

		public void AddChild(MimePartInfo newChild)
		{
			if (this.children == null)
			{
				this.children = new List<MimePartInfo>();
			}
			this.children.Add(newChild);
		}

		internal void AddChildren(List<MimePartInfo> children)
		{
			this.children.AddRange(children);
		}

		internal void AddHeader(Header header)
		{
			if (this.headers == null)
			{
				this.headers = new MimePartHeaders(this.charset);
			}
			this.headers.AddHeader(header);
		}

		public void SetBodySize(int bodySize, int lineCount)
		{
			this.bodyBytes = bodySize;
			this.bodyLines = lineCount;
		}

		internal void ChildrenWrittenOut()
		{
			if (this.IsMultipart)
			{
				this.bodyBytes = 0;
				this.bodyLines = 0;
			}
		}

		internal bool IsBodySizeComputed
		{
			get
			{
				if (this.bodyBytes == -1)
				{
					return false;
				}
				if (this.Children != null)
				{
					return this.Children.TrueForAll((MimePartInfo info) => info.IsBodySizeComputed);
				}
				return true;
			}
		}

		internal MimePart SkeletonPart
		{
			get
			{
				return this.skeletonPart;
			}
		}

		internal MimeDocument Skeleton
		{
			get
			{
				return this.skeleton;
			}
		}

		internal MimePart SmimePart
		{
			get
			{
				return this.smimePart;
			}
		}

		internal MimeDocument SmimeDocument
		{
			get
			{
				return this.smimeDocument;
			}
		}

		private Charset charset;

		private MimePartContentType contentType;

		private List<MimePartInfo> children;

		private MimePartInfo attachedItem;

		private MimePartHeaders headers;

		private AttachmentId attachmentId;

		private MimePartInfo.Callback writerCallback;

		private int partIndex;

		private int bodyLines;

		private int bodyBytes;

		private bool isBodyToRemoveFromSkeleton;

		private MimePart skeletonPart;

		private MimeDocument skeleton;

		private MimePart smimePart;

		private MimeDocument smimeDocument;

		internal delegate ConversionResult Callback(ItemToMimeConverter converter, MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags);
	}
}
