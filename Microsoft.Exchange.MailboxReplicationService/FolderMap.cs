using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderMap
	{
		public FolderMap(FolderHierarchyFlags folderHierarchyFlags)
		{
			this.Flags = folderHierarchyFlags;
			this.folders = new EntryIdMap<FolderRecWrapper>();
			this.roots = new List<FolderRecWrapper>();
			if (this.IsPublicFolderMailbox)
			{
				this.publicFolderDumpsters = new EntryIdMap<FolderRecWrapper>();
			}
		}

		public FolderMap(List<FolderRecWrapper> folders)
		{
			this.folders = new EntryIdMap<FolderRecWrapper>();
			this.roots = new List<FolderRecWrapper>();
			this.Flags = FolderHierarchyFlags.None;
			this.Config(folders);
		}

		public bool IsPublicFolderMailbox
		{
			get
			{
				return this.Flags.HasFlag(FolderHierarchyFlags.PublicFolderMailbox);
			}
		}

		public virtual FolderRecWrapper RootRec
		{
			get
			{
				this.ValidateMap();
				return this.roots[0];
			}
		}

		public CultureInfo TargetMailboxCulture { get; set; }

		public int Count
		{
			get
			{
				return this.folders.Count;
			}
		}

		public FolderRecWrapper this[byte[] folderId]
		{
			get
			{
				FolderRecWrapper result;
				if (folderId == null || !this.folders.TryGetValue(folderId, out result))
				{
					result = null;
				}
				return result;
			}
			set
			{
				if (value != null)
				{
					this.folders[folderId] = value;
					return;
				}
				if (folderId != null)
				{
					this.folders.Remove(folderId);
				}
			}
		}

		public FolderMap Copy()
		{
			List<FolderRecWrapper> copyList = new List<FolderRecWrapper>(this.folders.Count);
			this.EnumerateFolderHierarchy(EnumHierarchyFlags.AllFolders, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext ctx)
			{
				copyList.Add(new FolderRecWrapper(folderRec));
			});
			return new FolderMap(copyList);
		}

		public void Config(List<FolderRecWrapper> folders)
		{
			this.Clear();
			foreach (FolderRecWrapper folderRecWrapper in folders)
			{
				if (this.folders.ContainsKey(folderRecWrapper.EntryId))
				{
					FolderRecWrapper folderRecWrapper2 = this.folders[folderRecWrapper.EntryId];
					MrsTracer.Service.Error("Folder {0} is listed more than once in the input folder list", new object[]
					{
						folderRecWrapper.FolderRec.ToString()
					});
					this.TraceFolders(folders, "Input folder list");
					throw new FolderHierarchyContainsDuplicatesPermanentException(folderRecWrapper.FolderRec.ToString(), folderRecWrapper2.FolderRec.ToString());
				}
				if (this.IsPublicFolderMailbox && folderRecWrapper.IsPublicFolderDumpster)
				{
					byte[] array = (byte[])folderRecWrapper.FolderRec[PropTag.LTID];
					if (array != null)
					{
						this.publicFolderDumpsters.Add(array, folderRecWrapper);
					}
				}
				this.InsertFolderInternal(folderRecWrapper);
			}
			if (this.IsPublicFolderMailbox)
			{
				foreach (FolderRecWrapper folderRecWrapper3 in folders)
				{
					if (!folderRecWrapper3.IsPublicFolderDumpster)
					{
						byte[] array2 = (byte[])folderRecWrapper3.FolderRec[PropTag.IpmWasteBasketEntryId];
						FolderRecWrapper publicFolderDumpster;
						if (array2 != null && this.publicFolderDumpsters.TryGetValue(array2, out publicFolderDumpster))
						{
							folderRecWrapper3.PublicFolderDumpster = publicFolderDumpster;
						}
					}
				}
			}
			this.ValidateMap();
		}

		public List<FolderRecWrapper> GetFolderList()
		{
			List<FolderRecWrapper> result = new List<FolderRecWrapper>(this.folders.Count);
			this.EnumerateFolderHierarchy(EnumHierarchyFlags.AllFolders, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext ctx)
			{
				result.Add(folderRec);
			});
			return result;
		}

		public void EnumerateFolderHierarchy(EnumHierarchyFlags flags, FolderMap.EnumFolderCallback callback)
		{
			this.EnumerateSubtree(flags, this.RootRec, callback);
		}

		public virtual void EnumerateSubtree(EnumHierarchyFlags flags, FolderRecWrapper root, FolderMap.EnumFolderCallback callback)
		{
			FolderMap.EnumFolderContext ctx = new FolderMap.EnumFolderContext();
			this.EnumSingleFolder(root, ctx, callback, flags);
		}

		public IEnumerator<FolderRecWrapper> GetFolderHierarchyEnumerator(EnumHierarchyFlags flags)
		{
			if (this.subtreeEnumerator == null)
			{
				this.subtreeEnumerator = this.GetFolderList(flags, this.RootRec).GetEnumerator();
			}
			return this.subtreeEnumerator;
		}

		public void ResetFolderHierarchyEnumerator()
		{
			this.subtreeEnumerator = null;
		}

		public void InsertFolder(FolderRecWrapper rec)
		{
			this.InsertFolderInternal(rec);
			this.ValidateMap();
		}

		public void UpdateFolder(FolderRec updatedFolderData)
		{
			FolderRecWrapper folderRecWrapper = this[updatedFolderData.EntryId];
			if (folderRecWrapper == null)
			{
				return;
			}
			if (!CommonUtils.IsSameEntryId(updatedFolderData.ParentId, folderRecWrapper.ParentId))
			{
				FolderRecWrapper folderRecWrapper2 = this[updatedFolderData.ParentId];
				if (folderRecWrapper2 == null || folderRecWrapper2.IsChildOf(folderRecWrapper))
				{
					throw new UnableToApplyFolderHierarchyChangesTransientException();
				}
				folderRecWrapper.Parent = folderRecWrapper2;
			}
			folderRecWrapper.FolderRec.CopyFrom(updatedFolderData);
		}

		public void RemoveFolder(FolderRecWrapper rec)
		{
			if (rec == this.RootRec)
			{
				throw new UnexpectedErrorPermanentException(-2147467259);
			}
			this[rec.EntryId] = null;
			rec.Parent = null;
			if (rec.Children != null)
			{
				while (rec.Children.Count > 0)
				{
					this.RemoveFolder(rec.Children[0]);
				}
			}
		}

		public void RemoveFolder(byte[] folderId)
		{
			FolderRecWrapper folderRecWrapper = this[folderId];
			if (folderRecWrapper != null)
			{
				this.RemoveFolder(folderRecWrapper);
			}
		}

		public virtual void Clear()
		{
			this.folders.Clear();
			this.roots.Clear();
		}

		public override string ToString()
		{
			StringBuilder strBuilder = new StringBuilder();
			this.EnumerateFolderHierarchy(EnumHierarchyFlags.AllFolders, delegate(FolderRecWrapper fR, FolderMap.EnumFolderContext c)
			{
				strBuilder.AppendLine(fR.ToString());
			});
			return strBuilder.ToString();
		}

		protected static bool IsValidFolderType(EnumHierarchyFlags flags, FolderRecWrapper folderRec)
		{
			switch (folderRec.FolderType)
			{
			case FolderType.Root:
				return (flags & EnumHierarchyFlags.RootFolder) != EnumHierarchyFlags.None;
			case FolderType.Generic:
				return (flags & EnumHierarchyFlags.NormalFolders) != EnumHierarchyFlags.None;
			case FolderType.Search:
				return (flags & EnumHierarchyFlags.SearchFolders) != EnumHierarchyFlags.None;
			default:
				return false;
			}
		}

		protected virtual IEnumerable<FolderRecWrapper> GetFolderList(EnumHierarchyFlags flags, FolderRecWrapper folderRec)
		{
			Stack<FolderRecWrapper> stack = new Stack<FolderRecWrapper>();
			stack.Push(folderRec);
			while (stack.Count > 0)
			{
				FolderRecWrapper currentFolderRec = stack.Pop();
				if (!currentFolderRec.IsSpoolerQueue)
				{
					if (FolderMap.IsValidFolderType(flags, currentFolderRec))
					{
						yield return currentFolderRec;
					}
					if (currentFolderRec.Children != null)
					{
						foreach (FolderRecWrapper item in currentFolderRec.Children)
						{
							stack.Push(item);
						}
					}
				}
			}
			yield break;
		}

		private void EnumSingleFolder(FolderRecWrapper folderRec, FolderMap.EnumFolderContext ctx, FolderMap.EnumFolderCallback callback, EnumHierarchyFlags flags)
		{
			ctx.Result = EnumHierarchyResult.Continue;
			if (folderRec.IsSpoolerQueue)
			{
				return;
			}
			if (FolderMap.IsValidFolderType(flags, folderRec))
			{
				callback(folderRec, ctx);
			}
			if (ctx.Result == EnumHierarchyResult.Cancel || ctx.Result == EnumHierarchyResult.SkipSubtree)
			{
				return;
			}
			if (folderRec.Children != null)
			{
				foreach (FolderRecWrapper folderRec2 in folderRec.Children)
				{
					ctx.Result = EnumHierarchyResult.Continue;
					this.EnumSingleFolder(folderRec2, ctx, callback, flags);
					if (ctx.Result == EnumHierarchyResult.Cancel)
					{
						break;
					}
				}
			}
		}

		protected virtual void InsertFolderInternal(FolderRecWrapper rec)
		{
			FolderRecWrapper folderRecWrapper;
			if (rec.ParentId != null)
			{
				folderRecWrapper = this[rec.ParentId];
			}
			else
			{
				folderRecWrapper = null;
			}
			for (FolderRecWrapper folderRecWrapper2 = folderRecWrapper; folderRecWrapper2 != null; folderRecWrapper2 = folderRecWrapper2.Parent)
			{
				if (CommonUtils.IsSameEntryId(folderRecWrapper2.ParentId, rec.EntryId))
				{
					MrsTracer.Service.Error("Loop in the parent chain detected, folder {0}", new object[]
					{
						rec.FolderRec.ToString()
					});
					this.TraceFolders(this.folders.Values, "Folders");
					throw new FolderHierarchyContainsParentChainLoopPermanentException(rec.FolderRec.ToString());
				}
			}
			this[rec.EntryId] = rec;
			rec.Parent = folderRecWrapper;
			for (int i = this.roots.Count - 1; i >= 0; i--)
			{
				FolderRecWrapper folderRecWrapper3 = this.roots[i];
				if (CommonUtils.IsSameEntryId(folderRecWrapper3.ParentId, rec.EntryId))
				{
					folderRecWrapper3.Parent = rec;
					this.roots.RemoveAt(i);
				}
			}
			if (folderRecWrapper == null)
			{
				this.roots.Add(rec);
			}
		}

		protected virtual void ValidateMap()
		{
			if (this.roots.Count == 1)
			{
				return;
			}
			if (this.roots.Count == 0)
			{
				MrsTracer.Service.Error("No roots present in folder hierarchy.", new object[0]);
				this.TraceFolders(this.folders.Values, "Folders");
				throw new FolderHierarchyContainsNoRootsPermanentException();
			}
			MrsTracer.Service.Error("More than one root is present in folder hierarchy.", new object[0]);
			foreach (FolderRecWrapper folderRecWrapper in this.roots)
			{
				MrsTracer.Service.Error("Root: {0}", new object[]
				{
					folderRecWrapper.FolderRec.ToString()
				});
			}
			this.TraceFolders(this.folders.Values, "Folders");
			throw new FolderHierarchyContainsMultipleRootsTransientException(this.roots[0].FolderRec.ToString(), this.roots[1].FolderRec.ToString());
		}

		private void TraceFolders(ICollection<FolderRecWrapper> folders, string header)
		{
			MrsTracer.Service.Error("{0}", new object[]
			{
				header
			});
			foreach (FolderRecWrapper folderRecWrapper in folders)
			{
				MrsTracer.Service.Error("{0}", new object[]
				{
					folderRecWrapper.FolderRec.ToString()
				});
			}
		}

		private List<FolderRecWrapper> roots;

		private IEnumerator<FolderRecWrapper> subtreeEnumerator;

		protected EntryIdMap<FolderRecWrapper> publicFolderDumpsters;

		protected readonly FolderHierarchyFlags Flags;

		protected EntryIdMap<FolderRecWrapper> folders;

		public delegate void EnumFolderCallback(FolderRecWrapper folderRec, FolderMap.EnumFolderContext ctx);

		public class EnumFolderContext
		{
			public EnumFolderContext()
			{
				this.result = EnumHierarchyResult.Continue;
			}

			public EnumHierarchyResult Result
			{
				get
				{
					return this.result;
				}
				set
				{
					this.result = value;
				}
			}

			private EnumHierarchyResult result;
		}
	}
}
