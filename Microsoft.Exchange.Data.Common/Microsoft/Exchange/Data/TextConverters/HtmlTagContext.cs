using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	public abstract class HtmlTagContext
	{
		internal HtmlTagContext()
		{
		}

		public bool IsEndTag
		{
			get
			{
				this.AssertContextValid();
				return this.isEndTag;
			}
		}

		public bool IsEmptyElementTag
		{
			get
			{
				this.AssertContextValid();
				return this.isEmptyElementTag;
			}
		}

		public HtmlTagId TagId
		{
			get
			{
				this.AssertContextValid();
				return HtmlNameData.Names[(int)this.tagNameIndex].PublicTagId;
			}
		}

		public string TagName
		{
			get
			{
				this.AssertContextValid();
				return this.GetTagNameImpl();
			}
		}

		public HtmlTagContext.AttributeCollection Attributes
		{
			get
			{
				this.AssertContextValid();
				return new HtmlTagContext.AttributeCollection(this);
			}
		}

		internal HtmlNameIndex TagNameIndex
		{
			get
			{
				this.AssertContextValid();
				return this.tagNameIndex;
			}
		}

		internal HtmlTagParts TagParts
		{
			get
			{
				this.AssertContextValid();
				return this.tagParts;
			}
		}

		internal bool IsInvokeCallbackForEndTag
		{
			get
			{
				return this.invokeCallbackForEndTag;
			}
		}

		internal bool IsDeleteInnerContent
		{
			get
			{
				return this.deleteInnerContent;
			}
		}

		internal bool IsDeleteEndTag
		{
			get
			{
				return this.deleteEndTag;
			}
		}

		internal bool CopyPending
		{
			get
			{
				this.AssertContextValid();
				return this.GetCopyPendingStateImpl();
			}
		}

		public void WriteTag()
		{
			this.WriteTag(false);
		}

		public void WriteTag(bool copyInputAttributes)
		{
			this.AssertContextValid();
			if (this.writeState != HtmlTagContext.TagWriteState.Undefined)
			{
				throw new InvalidOperationException((this.writeState == HtmlTagContext.TagWriteState.Written) ? TextConvertersStrings.CallbackTagAlreadyWritten : TextConvertersStrings.CallbackTagAlreadyDeleted);
			}
			this.deleteEndTag = false;
			this.WriteTagImpl(!this.isEndTag && copyInputAttributes);
			this.writeState = HtmlTagContext.TagWriteState.Written;
		}

		public void DeleteTag()
		{
			this.DeleteTag(false);
		}

		public void DeleteTag(bool keepEndTag)
		{
			this.AssertContextValid();
			if (this.writeState != HtmlTagContext.TagWriteState.Undefined)
			{
				throw new InvalidOperationException((this.writeState == HtmlTagContext.TagWriteState.Written) ? TextConvertersStrings.CallbackTagAlreadyWritten : TextConvertersStrings.CallbackTagAlreadyDeleted);
			}
			if (!this.isEndTag && !this.isEmptyElementTag)
			{
				this.deleteEndTag = !keepEndTag;
			}
			else
			{
				this.deleteEndTag = false;
			}
			this.DeleteTagImpl();
			this.writeState = HtmlTagContext.TagWriteState.Deleted;
		}

		public void DeleteInnerContent()
		{
			this.AssertContextValid();
			if (!this.isEndTag && !this.isEmptyElementTag)
			{
				this.deleteInnerContent = true;
			}
		}

		public void InvokeCallbackForEndTag()
		{
			this.AssertContextValid();
			if (!this.isEndTag && !this.isEmptyElementTag)
			{
				this.invokeCallbackForEndTag = true;
			}
		}

		internal static byte ExtractCookie(int attributeIndexAndCookie)
		{
			return (byte)((uint)attributeIndexAndCookie >> 24);
		}

		internal static int ExtractIndex(int attributeIndexAndCookie)
		{
			return (attributeIndexAndCookie & 16777215) - 1;
		}

		internal static int ComposeIndexAndCookie(byte cookie, int attributeIndex)
		{
			return ((int)cookie << 24) + (attributeIndex + 1);
		}

		internal void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool droppedEndTag)
		{
			this.isEndTag = isEndTag;
			this.isEmptyElementTag = false;
			this.tagNameIndex = tagNameIndex;
			this.writeState = (droppedEndTag ? HtmlTagContext.TagWriteState.Deleted : HtmlTagContext.TagWriteState.Undefined);
			this.invokeCallbackForEndTag = false;
			this.deleteInnerContent = false;
			this.deleteEndTag = !this.isEndTag;
			this.cookie += 1;
		}

		internal void InitializeFragment(bool isEmptyElementTag, int attributeCount, HtmlTagParts tagParts)
		{
			if (attributeCount >= 16777215)
			{
				throw new TextConvertersException();
			}
			this.isEmptyElementTag = isEmptyElementTag;
			this.tagParts = tagParts;
			this.attributeCount = attributeCount;
			this.cookie += 1;
			this.valid = true;
		}

		internal void UninitializeFragment()
		{
			this.valid = false;
		}

		internal virtual bool GetCopyPendingStateImpl()
		{
			return false;
		}

		internal abstract string GetTagNameImpl();

		internal abstract HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex);

		internal abstract HtmlAttributeParts GetAttributePartsImpl(int attributeIndex);

		internal abstract string GetAttributeNameImpl(int attributeIndex);

		internal abstract string GetAttributeValueImpl(int attributeIndex);

		internal abstract int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count);

		internal abstract void WriteTagImpl(bool writeAttributes);

		internal virtual void DeleteTagImpl()
		{
		}

		internal abstract void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue);

		internal void AssertAttributeValid(int attributeIndexAndCookie)
		{
			if (!this.valid)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotValidInThisState);
			}
			if (HtmlTagContext.ExtractCookie(attributeIndexAndCookie) != this.cookie)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotValidForThisContext);
			}
			int num = HtmlTagContext.ExtractIndex(attributeIndexAndCookie);
			if (num < 0 || num >= this.attributeCount)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotValidForThisContext);
			}
		}

		internal void AssertContextValid()
		{
			if (!this.valid)
			{
				throw new InvalidOperationException(TextConvertersStrings.ContextNotValidInThisState);
			}
		}

		private HtmlTagContext.TagWriteState writeState;

		private byte cookie;

		private bool valid;

		private bool invokeCallbackForEndTag;

		private bool deleteInnerContent;

		private bool deleteEndTag;

		private bool isEndTag;

		private bool isEmptyElementTag;

		private HtmlNameIndex tagNameIndex;

		private HtmlTagParts tagParts;

		private int attributeCount;

		internal enum TagWriteState
		{
			Undefined,
			Written,
			Deleted
		}

		public struct AttributeCollection : IEnumerable<HtmlTagContextAttribute>, IEnumerable
		{
			internal AttributeCollection(HtmlTagContext tagContext)
			{
				this.tagContext = tagContext;
			}

			public int Count
			{
				get
				{
					this.AssertValid();
					return this.tagContext.attributeCount;
				}
			}

			public HtmlTagContextAttribute this[int index]
			{
				get
				{
					this.AssertValid();
					if (index < 0 || index >= this.tagContext.attributeCount)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return new HtmlTagContextAttribute(this.tagContext, HtmlTagContext.ComposeIndexAndCookie(this.tagContext.cookie, index));
				}
			}

			public HtmlTagContext.AttributeCollection.Enumerator GetEnumerator()
			{
				this.AssertValid();
				return new HtmlTagContext.AttributeCollection.Enumerator(this.tagContext);
			}

			IEnumerator<HtmlTagContextAttribute> IEnumerable<HtmlTagContextAttribute>.GetEnumerator()
			{
				this.AssertValid();
				return new HtmlTagContext.AttributeCollection.Enumerator(this.tagContext);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				this.AssertValid();
				return new HtmlTagContext.AttributeCollection.Enumerator(this.tagContext);
			}

			private void AssertValid()
			{
				if (this.tagContext == null)
				{
					throw new InvalidOperationException(TextConvertersStrings.AttributeCollectionNotInitialized);
				}
			}

			private HtmlTagContext tagContext;

			public struct Enumerator : IEnumerator<HtmlTagContextAttribute>, IDisposable, IEnumerator
			{
				internal Enumerator(HtmlTagContext tagContext)
				{
					this.tagContext = tagContext;
					this.attributeIndexAndCookie = HtmlTagContext.ComposeIndexAndCookie(this.tagContext.cookie, -1);
				}

				public HtmlTagContextAttribute Current
				{
					get
					{
						return new HtmlTagContextAttribute(this.tagContext, this.attributeIndexAndCookie);
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return new HtmlTagContextAttribute(this.tagContext, this.attributeIndexAndCookie);
					}
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie) < this.tagContext.attributeCount)
					{
						this.attributeIndexAndCookie++;
						return HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie) < this.tagContext.attributeCount;
					}
					return false;
				}

				void IEnumerator.Reset()
				{
					this.attributeIndexAndCookie = HtmlTagContext.ComposeIndexAndCookie(this.tagContext.cookie, -1);
				}

				private HtmlTagContext tagContext;

				private int attributeIndexAndCookie;
			}
		}
	}
}
