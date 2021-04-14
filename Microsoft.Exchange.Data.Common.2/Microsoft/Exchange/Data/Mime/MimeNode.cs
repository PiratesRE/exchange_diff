using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.Mime
{
	public abstract class MimeNode : IEnumerable<MimeNode>, IEnumerable
	{
		internal MimeNode(MimeNode parent)
		{
			this.parentNode = parent;
		}

		internal MimeNode()
		{
		}

		public bool HasChildren
		{
			get
			{
				return null != this.FirstChild;
			}
		}

		public MimeNode Parent
		{
			get
			{
				return this.parentNode;
			}
		}

		public MimeNode FirstChild
		{
			get
			{
				if (this.lastChild == null)
				{
					return this.ParseNextChild();
				}
				return this.lastChild.nextSibling;
			}
		}

		public MimeNode LastChild
		{
			get
			{
				while (this.ParseNextChild() != null)
				{
				}
				return this.lastChild;
			}
		}

		public MimeNode NextSibling
		{
			get
			{
				if (this.parentNode == null)
				{
					return null;
				}
				if (this != this.parentNode.lastChild)
				{
					return this.nextSibling;
				}
				return this.parentNode.ParseNextChild();
			}
		}

		public MimeNode PreviousSibling
		{
			get
			{
				if (this.parentNode == null || this.parentNode.lastChild.nextSibling == this)
				{
					return null;
				}
				MimeNode mimeNode = this.parentNode.lastChild.nextSibling;
				int num = 0;
				while (mimeNode.nextSibling != this)
				{
					num++;
					this.CheckLoopCount(num);
					mimeNode = mimeNode.nextSibling;
				}
				return mimeNode;
			}
		}

		internal MimeNode InternalLastChild
		{
			get
			{
				return this.lastChild;
			}
		}

		internal MimeNode InternalNextSibling
		{
			get
			{
				if (this.parentNode == null || this == this.parentNode.lastChild)
				{
					return null;
				}
				return this.nextSibling;
			}
		}

		public MimeNode InsertBefore(MimeNode newChild, MimeNode refChild)
		{
			this.ThrowIfReadOnly("MimeNode.InsertBefore");
			if (refChild == null)
			{
				refChild = this.LastChild;
			}
			else if (this.lastChild != null && refChild == this.lastChild.nextSibling)
			{
				refChild = null;
			}
			else
			{
				refChild = refChild.PreviousSibling;
				if (refChild == null)
				{
					throw new ArgumentException(Strings.RefChildIsNotMyChild, "refChild");
				}
			}
			return this.InsertAfter(newChild, refChild);
		}

		public MimeNode InsertAfter(MimeNode newChild, MimeNode refChild)
		{
			this.ThrowIfReadOnly("MimeNode.InsertAfter");
			newChild = this.InternalInsertAfter(newChild, refChild);
			this.SetDirty();
			return newChild;
		}

		public MimeNode AppendChild(MimeNode newChild)
		{
			this.ThrowIfReadOnly("MimeNode.AppendChild");
			return this.InsertAfter(newChild, this.LastChild);
		}

		public MimeNode PrependChild(MimeNode newChild)
		{
			this.ThrowIfReadOnly("MimeNode.PrependChild");
			return this.InsertAfter(newChild, null);
		}

		public MimeNode RemoveChild(MimeNode oldChild)
		{
			this.ThrowIfReadOnly("MimeNode.RemoveChild");
			oldChild = this.InternalRemoveChild(oldChild);
			this.SetDirty();
			return oldChild;
		}

		public MimeNode ReplaceChild(MimeNode newChild, MimeNode oldChild)
		{
			this.ThrowIfReadOnly("MimeNode.ReplaceChild");
			if (oldChild == null)
			{
				throw new ArgumentNullException("oldChild");
			}
			if (this != oldChild.parentNode)
			{
				throw new ArgumentException(Strings.OldChildIsNotMyChild, "oldChild");
			}
			if (newChild == oldChild)
			{
				return oldChild;
			}
			MimeNode result = this.InsertAfter(newChild, oldChild);
			if (this == oldChild.parentNode)
			{
				this.RemoveChild(oldChild);
			}
			return result;
		}

		public void RemoveAll()
		{
			this.ThrowIfReadOnly("MimeNode.RemoveAll");
			this.InternalRemoveAll();
			this.SetDirty();
		}

		internal virtual void RemoveAllUnparsed()
		{
		}

		public void RemoveFromParent()
		{
			this.ThrowIfReadOnly("MimeNode.RemoveFromParent");
			if (this.parentNode != null)
			{
				this.parentNode.RemoveChild(this);
			}
		}

		public long WriteTo(Stream stream)
		{
			return this.WriteTo(stream, null);
		}

		public long WriteTo(Stream stream, EncodingOptions encodingOptions)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encodingOptions == null)
			{
				encodingOptions = this.GetDocumentEncodingOptions();
			}
			byte[] array = null;
			MimeStringLength mimeStringLength = new MimeStringLength(0);
			return this.WriteTo(stream, encodingOptions, null, ref mimeStringLength, ref array);
		}

		public void WriteTo(MimeWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteMimeNode(this);
		}

		public MimeNode.Enumerator<MimeNode> GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeNode>(this);
		}

		IEnumerator<MimeNode> IEnumerable<MimeNode>.GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeNode>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeNode>(this);
		}

		public virtual MimeNode Clone()
		{
			throw new NotSupportedException(Strings.ThisNodeDoesNotSupportCloning(base.GetType().ToString()));
		}

		public virtual void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			MimeNode mimeNode = destination as MimeNode;
			if (mimeNode == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			mimeNode.RemoveAll();
			if (this.lastChild != null)
			{
				for (MimeNode internalNextSibling = this.lastChild.nextSibling; internalNextSibling != null; internalNextSibling = internalNextSibling.InternalNextSibling)
				{
					mimeNode.InternalAppendChild(internalNextSibling.Clone());
				}
			}
			mimeNode.SetDirty();
		}

		internal MimeNode InternalInsertAfter(MimeNode newChild, MimeNode refChild)
		{
			this.ThrowIfReadOnly("MimeNode.InternalInsertAfter");
			if (newChild == null)
			{
				throw new ArgumentNullException("newChild");
			}
			if (refChild != null)
			{
				if (refChild == newChild)
				{
					return refChild;
				}
				if (this != refChild.parentNode)
				{
					throw new ArgumentException(Strings.RefChildIsNotMyChild, "refChild");
				}
			}
			if (newChild.parentNode != null)
			{
				throw new ArgumentException(Strings.NewChildCannotHaveDifferentParent);
			}
			refChild = this.ValidateNewChild(newChild, refChild);
			newChild.parentNode = this;
			if (refChild == null)
			{
				if (this.lastChild == null)
				{
					newChild.nextSibling = newChild;
					this.lastChild = newChild;
				}
				else
				{
					newChild.nextSibling = this.lastChild.nextSibling;
					this.lastChild.nextSibling = newChild;
				}
			}
			else
			{
				newChild.nextSibling = refChild.nextSibling;
				refChild.nextSibling = newChild;
				if (refChild == this.lastChild)
				{
					this.lastChild = newChild;
				}
			}
			return newChild;
		}

		internal MimeNode InternalAppendChild(MimeNode newChild)
		{
			this.ThrowIfReadOnly("MimeNode.InternalAppendChild");
			return this.InternalInsertAfter(newChild, this.LastChild);
		}

		internal MimeNode InternalRemoveChild(MimeNode oldChild)
		{
			this.ThrowIfReadOnly("MimeNode.InternalRemoveChild");
			if (oldChild == null)
			{
				throw new ArgumentNullException("oldChild");
			}
			if (this != oldChild.parentNode)
			{
				throw new ArgumentException(Strings.OldChildIsNotMyChild, "oldChild");
			}
			if (oldChild == this.lastChild.nextSibling)
			{
				if (oldChild == this.lastChild)
				{
					this.lastChild = null;
				}
				else
				{
					this.lastChild.nextSibling = oldChild.nextSibling;
				}
			}
			else
			{
				MimeNode previousSibling = oldChild.PreviousSibling;
				previousSibling.nextSibling = oldChild.nextSibling;
				if (oldChild == this.lastChild)
				{
					this.lastChild = previousSibling;
				}
			}
			oldChild.parentNode = null;
			oldChild.nextSibling = null;
			this.ChildRemoved(oldChild);
			return oldChild;
		}

		internal void InternalRemoveAll()
		{
			this.ThrowIfReadOnly("MimeNode.InternalRemoveAll");
			while (this.lastChild != null)
			{
				MimeNode mimeNode = this.lastChild.nextSibling;
				if (mimeNode == this.lastChild)
				{
					this.lastChild = null;
				}
				else
				{
					this.lastChild.nextSibling = mimeNode.nextSibling;
				}
				mimeNode.nextSibling = null;
				mimeNode.parentNode = null;
				this.ChildRemoved(mimeNode);
			}
			this.RemoveAllUnparsed();
		}

		internal void InternalDetachParent()
		{
			this.parentNode = null;
		}

		internal virtual MimeNode ParseNextChild()
		{
			return null;
		}

		internal virtual void CheckChildrenLimit(int countLimit, int bytesLimit)
		{
		}

		internal virtual MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			return refChild;
		}

		internal abstract long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer);

		internal virtual void ChildRemoved(MimeNode oldChild)
		{
		}

		internal virtual void SetDirty()
		{
			this.ThrowIfReadOnly("MimeNode.SetDirty");
			if (this.parentNode != null)
			{
				this.parentNode.SetDirty();
			}
		}

		internal EncodingOptions GetDocumentEncodingOptions()
		{
			MimeDocument mimeDocument;
			MimeNode mimeNode;
			this.GetMimeDocumentOrTreeRoot(out mimeDocument, out mimeNode);
			if (mimeDocument != null)
			{
				return mimeDocument.EncodingOptions;
			}
			MimePart mimePart = mimeNode as MimePart;
			return new EncodingOptions((mimePart == null) ? null : mimePart.FindMimeTreeCharset());
		}

		internal DecodingOptions GetHeaderDecodingOptions()
		{
			MimeDocument mimeDocument;
			MimeNode mimeNode;
			this.GetMimeDocumentOrTreeRoot(out mimeDocument, out mimeNode);
			DecodingOptions result;
			if (mimeDocument != null)
			{
				result = mimeDocument.EffectiveHeaderDecodingOptions;
				if (result.Charset == null)
				{
					result.Charset = DecodingOptions.DefaultCharset;
				}
			}
			else
			{
				result = DecodingOptions.Default;
				result.Charset = mimeNode.GetDefaultHeaderDecodingCharset(null, mimeNode);
			}
			return result;
		}

		internal Charset GetDefaultHeaderDecodingCharset(MimeDocument document, MimeNode treeRoot)
		{
			if (treeRoot == null)
			{
				this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
			}
			Charset charset;
			if (document != null)
			{
				charset = document.EffectiveHeaderDecodingOptions.Charset;
			}
			else
			{
				MimePart mimePart = treeRoot as MimePart;
				charset = ((mimePart == null) ? null : mimePart.FindMimeTreeCharset());
			}
			if (charset == null)
			{
				charset = DecodingOptions.DefaultCharset;
			}
			return charset;
		}

		internal MimeNode GetTreeRoot()
		{
			MimeNode mimeNode = this;
			while (mimeNode.parentNode != null)
			{
				mimeNode = mimeNode.parentNode;
			}
			return mimeNode;
		}

		internal void GetMimeDocumentOrTreeRoot(out MimeDocument document, out MimeNode treeRoot)
		{
			document = MimeNode.GetParentDocument(this, out treeRoot);
		}

		protected static MimeDocument GetParentDocument(MimeNode node, out MimeNode treeRoot)
		{
			treeRoot = node.GetTreeRoot();
			MimePart mimePart = treeRoot as MimePart;
			if (mimePart != null)
			{
				return mimePart.ParentDocument;
			}
			return null;
		}

		protected bool IsReadOnly
		{
			get
			{
				MimeNode mimeNode;
				MimeDocument parentDocument = MimeNode.GetParentDocument(this, out mimeNode);
				return parentDocument != null && parentDocument.IsReadOnly;
			}
		}

		protected void ThrowIfReadOnly(string method)
		{
			if (this.IsReadOnly)
			{
				throw new ReadOnlyMimeException(method);
			}
		}

		private void CheckLoopCount(int count)
		{
			if (!this.loopLimitInitialized)
			{
				MimeLimits mimeLimits = MimeLimits.Default;
				MimeDocument mimeDocument;
				MimeNode mimeNode;
				this.GetMimeDocumentOrTreeRoot(out mimeDocument, out mimeNode);
				if (mimeDocument != null)
				{
					mimeLimits = mimeDocument.MimeLimits;
				}
				if (this is Header)
				{
					this.loopLimit = mimeLimits.MaxHeaders;
				}
				else if (this is AddressItem)
				{
					this.loopLimit = mimeLimits.MaxAddressItemsPerHeader;
				}
				else if (this is MimeParameter)
				{
					this.loopLimit = mimeLimits.MaxParametersPerHeader;
				}
				else
				{
					this.loopLimit = mimeLimits.MaxParts;
				}
				this.loopLimitInitialized = true;
			}
			if (count > this.loopLimit)
			{
				string message = string.Format("Loop detected in sibling collection. Loop count: {0}", this.loopLimit);
				throw new InvalidOperationException(message);
			}
		}

		private const string LoopLimitMessage = "Loop detected in sibling collection. Loop count: {0}";

		private bool loopLimitInitialized;

		private int loopLimit;

		private MimeNode parentNode;

		private MimeNode nextSibling;

		private MimeNode lastChild;

		public struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator where T : MimeNode
		{
			internal Enumerator(MimeNode node)
			{
				this.node = node;
				this.current = default(T);
				this.next = (this.node.FirstChild as T);
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.current == null)
					{
						throw new InvalidOperationException((this.next == null) ? Strings.ErrorAfterLast : Strings.ErrorBeforeFirst);
					}
					return this.current;
				}
			}

			public T Current
			{
				get
				{
					if (this.current == null)
					{
						throw new InvalidOperationException((this.next == null) ? Strings.ErrorAfterLast : Strings.ErrorBeforeFirst);
					}
					return this.current;
				}
			}

			public bool MoveNext()
			{
				this.current = this.next;
				if (this.current == null)
				{
					return false;
				}
				this.next = (this.current.NextSibling as T);
				return true;
			}

			public void Reset()
			{
				this.current = default(T);
				this.next = (this.node.FirstChild as T);
			}

			public void Dispose()
			{
				this.Reset();
			}

			private MimeNode node;

			private T current;

			private T next;
		}
	}
}
