using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentHandle
	{
		public int AttachNumber
		{
			get
			{
				return this.attachmentNumber;
			}
		}

		public AttachmentId AttachmentId
		{
			get
			{
				return this.attachmentId;
			}
			set
			{
				this.attachmentId = value;
			}
		}

		internal string CharsetDetectionData
		{
			get
			{
				return this.charsetDetectionData;
			}
		}

		public bool IsCalendarException
		{
			get
			{
				return this.isCalendarException;
			}
		}

		public bool IsInline
		{
			get
			{
				return this.isInline && this.AttachMethod != 7;
			}
		}

		public int AttachMethod
		{
			get
			{
				return this.attachMethod;
			}
		}

		internal AttachmentHandle(int attachmentNumber)
		{
			this.attachmentNumber = attachmentNumber;
		}

		internal void UpdateProperties(AttachmentPropertyBag attachmentBag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			attachmentBag.ComputeCharsetDetectionData(stringBuilder);
			this.charsetDetectionData = stringBuilder.ToString();
			this.attachmentId = attachmentBag.AttachmentId;
			this.isCalendarException = attachmentBag.IsCalendarException;
			this.isInline = attachmentBag.IsInline;
			this.attachMethod = attachmentBag.AttachMethod;
			this.cachedPropertyBag = null;
		}

		internal void SetCachedPropertyBag(PropertyBag propertyBag)
		{
			this.cachedPropertyBag = propertyBag;
		}

		internal PropertyBag GetAndRemoveCachedPropertyBag()
		{
			PropertyBag result = this.cachedPropertyBag;
			this.cachedPropertyBag = null;
			return result;
		}

		private readonly int attachmentNumber;

		private AttachmentId attachmentId;

		private string charsetDetectionData;

		private bool isCalendarException;

		private bool isInline;

		private int attachMethod;

		private PropertyBag cachedPropertyBag;
	}
}
