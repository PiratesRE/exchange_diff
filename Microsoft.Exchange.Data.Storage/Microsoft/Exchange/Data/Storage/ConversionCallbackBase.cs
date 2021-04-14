using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversionCallbackBase
	{
		protected ConversionCallbackBase()
		{
			this.AttachmentCollection = null;
			this.itemBody = null;
			this.attachmentLinks = null;
			this.attachmentsByPositionEnumerator = null;
		}

		protected ConversionCallbackBase(ICoreItem containerItem) : this(containerItem.AttachmentCollection, containerItem.Body)
		{
		}

		protected ConversionCallbackBase(CoreAttachmentCollection attachmentCollection, Body itemBody)
		{
			this.AttachmentCollection = attachmentCollection;
			this.itemBody = itemBody;
			this.attachmentLinks = null;
			this.attachmentsByPositionEnumerator = null;
		}

		public ReadOnlyCollection<AttachmentLink> AttachmentLinks
		{
			get
			{
				if (this.attachmentLinks == null)
				{
					this.InitializeAttachmentLinks(null);
				}
				return this.attachmentLinks;
			}
		}

		public ReadOnlyCollection<AttachmentLink> InitializeAttachmentLinks(IList<AttachmentLink> linksToMerge)
		{
			this.attachmentLinks = AttachmentLink.MergeAttachmentLinks(linksToMerge, this.AttachmentCollection);
			return this.attachmentLinks;
		}

		internal bool AttachmentListInitialized
		{
			get
			{
				return this.attachmentLinks != null;
			}
		}

		public virtual CoreAttachmentCollection AttachmentCollection { get; private set; }

		public Body ItemBody
		{
			get
			{
				return this.itemBody;
			}
		}

		public bool ClearInlineOnUnmarkedAttachments
		{
			get
			{
				return this.requireMarkInline;
			}
			set
			{
				this.requireMarkInline = value;
			}
		}

		public bool RemoveUnlinkedAttachments
		{
			get
			{
				return this.removeUnlinkedAttachments;
			}
			protected set
			{
				this.removeUnlinkedAttachments = value;
			}
		}

		protected bool NeedsSave()
		{
			if (this.AttachmentCollection == null)
			{
				return false;
			}
			foreach (AttachmentLink attachmentLink in this.AttachmentLinks)
			{
				if (attachmentLink.NeedsSave(this.requireMarkInline))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool SaveChanges()
		{
			if (this.AttachmentCollection == null)
			{
				throw new InvalidOperationException("Target item not specified; callback cannot invoke attachment-specific methods");
			}
			bool result = false;
			if (this.attachmentLinks != null)
			{
				using (IEnumerator<AttachmentLink> enumerator = this.attachmentLinks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AttachmentLink attachmentLink = enumerator.Current;
						using (CoreAttachment coreAttachment = this.AttachmentCollection.Open(attachmentLink.AttachmentId))
						{
							using (Attachment attachment = Microsoft.Exchange.Data.Storage.AttachmentCollection.CreateTypedAttachment(coreAttachment, null))
							{
								if (this.removeUnlinkedAttachments && !attachmentLink.IsInline(this.requireMarkInline) && attachment.IsInline)
								{
									attachment.Dispose();
									this.AttachmentCollection.Remove(attachmentLink.AttachmentId);
								}
								else
								{
									if (attachmentLink.NeedsConversionToImage && attachment.AttachmentType == AttachmentType.Ole)
									{
										OleAttachment oleAttachment = attachment as OleAttachment;
										if (oleAttachment == null)
										{
											continue;
										}
										using (Attachment attachment2 = oleAttachment.ConvertToImageAttachment(this.AttachmentCollection, ImageFormat.Jpeg))
										{
											result = true;
											attachmentLink.MakeAttachmentChanges(attachment2, this.requireMarkInline);
											attachment2.Save();
											continue;
										}
									}
									if (attachmentLink.MakeAttachmentChanges(attachment, this.requireMarkInline))
									{
										result = true;
										attachment.Save();
									}
								}
							}
						}
					}
					return result;
				}
			}
			if (this.requireMarkInline)
			{
				List<AttachmentId> list = null;
				foreach (AttachmentHandle handle in this.AttachmentCollection)
				{
					using (CoreAttachment coreAttachment2 = this.AttachmentCollection.Open(handle))
					{
						if (coreAttachment2.IsInline)
						{
							if (this.removeUnlinkedAttachments)
							{
								if (list == null)
								{
									list = new List<AttachmentId>();
								}
								list.Add(coreAttachment2.Id);
							}
							else
							{
								coreAttachment2.IsInline = false;
								result = true;
								coreAttachment2.Save();
							}
						}
					}
				}
				if (list != null)
				{
					foreach (AttachmentId id in list)
					{
						this.AttachmentCollection.Remove(id);
					}
				}
			}
			return result;
		}

		private void CreateAttachmentsByPositionEnumerator()
		{
			List<KeyValuePair<int, AttachmentLink>> list = new List<KeyValuePair<int, AttachmentLink>>();
			foreach (AttachmentLink attachmentLink in this.AttachmentLinks)
			{
				if (attachmentLink.RenderingPosition >= 0)
				{
					list.Add(new KeyValuePair<int, AttachmentLink>(attachmentLink.RenderingPosition, attachmentLink));
				}
			}
			list.Sort(new ConversionCallbackBase.SortByAttachmentPositionComparer());
			this.attachmentsByPositionEnumerator = list.GetEnumerator();
		}

		public AttachmentLink FindAttachmentByIdOrContentId(AttachmentId attachmentId, string contentId)
		{
			Util.ThrowOnNullArgument(attachmentId, "attachmentId");
			if (this.AttachmentCollection == null)
			{
				throw new InvalidOperationException("Target item not specified; cannot invoke attachment-specific methods");
			}
			bool flag = string.IsNullOrEmpty(contentId);
			foreach (AttachmentLink attachmentLink in this.AttachmentLinks)
			{
				if (attachmentLink.AttachmentId.Equals(attachmentId) || (!flag && contentId.Equals(attachmentLink.ContentId, StringComparison.Ordinal)))
				{
					return attachmentLink;
				}
			}
			return null;
		}

		public AttachmentLink FindAttachmentByBodyReference(string bodyReference)
		{
			Util.ThrowOnNullArgument(bodyReference, "bodyReference");
			return this.InternalFindByBodyReference(bodyReference, null);
		}

		public AttachmentLink FindAttachmentByBodyReference(string bodyReference, Uri baseUri)
		{
			Util.ThrowOnNullArgument(bodyReference, "bodyReference");
			Util.ThrowOnNullArgument(baseUri, "baseUri");
			if (baseUri.IsWellFormedOriginalString())
			{
				return this.InternalFindByBodyReference(bodyReference, baseUri);
			}
			return this.InternalFindByBodyReference(bodyReference, null);
		}

		private AttachmentLink InternalFindByBodyReference(string bodyReference, Uri baseUri)
		{
			Uri uri;
			if (!Uri.TryCreate(bodyReference, UriKind.RelativeOrAbsolute, out uri))
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "BodyConversionCallbacksBase.InternalFindId: bodyReference not a valid URI\r\n'{0}'", bodyReference);
				return null;
			}
			if (!uri.IsWellFormedOriginalString())
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "BodyConversionCallbacksBase.InternalFindId: bodyReference not a valid URI\r\n'{0}'", bodyReference);
				return null;
			}
			if (uri.IsAbsoluteUri && uri.Scheme == "cid")
			{
				using (IEnumerator<AttachmentLink> enumerator = this.AttachmentLinks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AttachmentLink attachmentLink = enumerator.Current;
						AttachmentId attachmentId = attachmentLink.AttachmentId;
						string text;
						if (string.Equals(uri.LocalPath, attachmentLink.ContentId, StringComparison.OrdinalIgnoreCase))
						{
							text = attachmentLink.ContentId;
						}
						else if (string.Equals(uri.LocalPath, attachmentLink.Filename, StringComparison.OrdinalIgnoreCase))
						{
							text = attachmentLink.Filename;
						}
						else
						{
							if (!string.Equals(uri.LocalPath, attachmentLink.DisplayName, StringComparison.OrdinalIgnoreCase))
							{
								continue;
							}
							text = attachmentLink.DisplayName;
						}
						Uri contentLocation;
						if (!Uri.TryCreate("cid:" + text, UriKind.RelativeOrAbsolute, out contentLocation))
						{
							ExTraceGlobals.StorageTracer.TraceError<string, AttachmentId>((long)this.GetHashCode(), "BodyConversionCallbacksBase.InternalFindId: attachmentContentKey[{1}] not a valid URI\r\n'{0}'", text, attachmentId);
							return null;
						}
						if (!contentLocation.IsWellFormedOriginalString())
						{
							ExTraceGlobals.StorageTracer.TraceError<string, AttachmentId>((long)this.GetHashCode(), "BodyConversionCallbacksBase.InternalFindId: attachmentContentKey[{1}] not a valid URI\r\n'{0}'", text, attachmentId);
							return null;
						}
						return attachmentLink;
					}
					goto IL_2A1;
				}
			}
			if (uri.IsAbsoluteUri && uri.Scheme == "objattph")
			{
				if (this.attachmentsByPositionEnumerator == null)
				{
					this.CreateAttachmentsByPositionEnumerator();
				}
				if (this.attachmentsByPositionEnumerator.MoveNext())
				{
					KeyValuePair<int, AttachmentLink> keyValuePair = this.attachmentsByPositionEnumerator.Current;
					return keyValuePair.Value;
				}
				return null;
			}
			else
			{
				Uri uri2;
				if (uri.IsAbsoluteUri)
				{
					uri2 = uri;
				}
				else if (baseUri != null)
				{
					if (!Uri.TryCreate(baseUri, uri, out uri2))
					{
						ExTraceGlobals.StorageTracer.TraceError<Uri, Uri>((long)this.GetHashCode(), "AttachmentCollection.InternalFindId: can't build absolute URI from bodyReference and base\r\n'{0}'\r\n'{1}'", uri, baseUri);
						uri2 = uri;
					}
				}
				else
				{
					uri2 = uri;
				}
				foreach (AttachmentLink attachmentLink2 in this.AttachmentLinks)
				{
					AttachmentId attachmentId2 = attachmentLink2.AttachmentId;
					Uri contentLocation = attachmentLink2.ContentLocation;
					Uri contentBase = attachmentLink2.ContentBase;
					if (!(contentLocation == null))
					{
						Uri obj;
						if (contentLocation.IsAbsoluteUri)
						{
							obj = contentLocation;
						}
						else if (contentBase != null)
						{
							if (!Uri.TryCreate(contentBase, contentLocation, out obj))
							{
								ExTraceGlobals.StorageTracer.TraceError<Uri, AttachmentId>((long)this.GetHashCode(), "AttachmentCollection.InternalFindId: attachContentLocation[{1}] not a valid URI\r\n'{0}'", contentLocation, attachmentId2);
								obj = contentLocation;
							}
						}
						else
						{
							obj = contentLocation;
						}
						try
						{
							if (uri2.Equals(obj))
							{
								return attachmentLink2;
							}
						}
						catch (UriFormatException)
						{
						}
					}
				}
			}
			IL_2A1:
			return null;
		}

		private ReadOnlyCollection<AttachmentLink> attachmentLinks;

		private Body itemBody;

		private IEnumerator<KeyValuePair<int, AttachmentLink>> attachmentsByPositionEnumerator;

		private bool requireMarkInline = true;

		private bool removeUnlinkedAttachments;

		private class SortByAttachmentPositionComparer : IComparer<KeyValuePair<int, AttachmentLink>>
		{
			public int Compare(KeyValuePair<int, AttachmentLink> lhs, KeyValuePair<int, AttachmentLink> rhs)
			{
				return lhs.Key.CompareTo(rhs.Key);
			}
		}
	}
}
