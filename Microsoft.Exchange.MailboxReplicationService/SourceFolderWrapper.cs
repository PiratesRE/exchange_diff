using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SourceFolderWrapper : FolderWrapper, ISourceFolder, IFolder, IDisposable
	{
		public SourceFolderWrapper(ISourceFolder folder, CommonUtils.CreateContextDelegate createContext, ProviderInfo mailboxProviderInfo) : base(folder, createContext)
		{
			base.ProviderInfo = mailboxProviderInfo;
		}

		void ISourceFolder.CopyTo(IFxProxy destFolder, CopyPropertiesFlags flags, PropTag[] excludeTags)
		{
			string text = "ISourceFolder.CopyTo";
			TimeSpan targetDuration = TimeSpan.Zero;
			Stopwatch stopwatch = Stopwatch.StartNew();
			base.CreateContext(text, new DataContext[]
			{
				new PropTagsDataContext(excludeTags)
			}).Execute(delegate
			{
				using (FxProxyCallbackWrapper fxProxyCallbackWrapper = new FxProxyCallbackWrapper(destFolder, true, delegate(TimeSpan duration)
				{
					targetDuration += duration;
				}))
				{
					((ISourceFolder)this.WrappedObject).CopyTo(fxProxyCallbackWrapper, flags, excludeTags);
				}
			}, false);
			base.UpdateDuration(text, stopwatch.Elapsed.Subtract(targetDuration));
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			base.CreateContext("ISourceFolder.ExportMessages", new DataContext[]
			{
				new SimpleValueDataContext("Flags", flags),
				new EntryIDsDataContext(entryIds)
			}).Execute(delegate
			{
				((ISourceFolder)this.WrappedObject).ExportMessages(destFolderProxy, flags, entryIds);
			}, true);
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			FolderChangesManifest result = null;
			base.CreateContext("ISourceFolder.EnumerateChanges", new DataContext[]
			{
				new SimpleValueDataContext("flags", flags),
				new SimpleValueDataContext("maxChanges", maxChanges)
			}).Execute(delegate
			{
				result = ((ISourceFolder)this.WrappedObject).EnumerateChanges(flags, maxChanges);
			}, true);
			return result;
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			List<MessageRec> result = null;
			base.CreateContext("ISourceFolder.EnumerateMessagesPaged", new DataContext[]
			{
				new SimpleValueDataContext("maxPageSize", maxPageSize)
			}).Execute(delegate
			{
				result = ((ISourceFolder)this.WrappedObject).EnumerateMessagesPaged(maxPageSize);
			}, true);
			return result;
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			int result = 0;
			base.CreateContext("ISourceFolder.GetEstimatedItemCount", new DataContext[0]).Execute(delegate
			{
				result = ((ISourceFolder)this.WrappedObject).GetEstimatedItemCount();
			}, true);
			return result;
		}
	}
}
