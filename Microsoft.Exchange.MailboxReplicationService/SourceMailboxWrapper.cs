using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SourceMailboxWrapper : MailboxWrapper, ISourceMailbox, IMailbox, IDisposable
	{
		public SourceMailboxWrapper(IMailbox mailbox, MailboxWrapperFlags flags, LocalizedString tracingId) : base(mailbox, flags, tracingId)
		{
		}

		public ISourceMailbox SourceMailbox
		{
			get
			{
				return this;
			}
		}

		protected override OperationSideDataContext SideOperationContext
		{
			get
			{
				return OperationSideDataContext.Source;
			}
		}

		public override IFolder GetFolder(byte[] folderId)
		{
			return this.SourceMailbox.GetFolder(folderId);
		}

		byte[] ISourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags flags)
		{
			byte[] result = null;
			base.CreateContext("ISourceMailbox.GetMailboxBasicInfo", new DataContext[]
			{
				new SimpleValueDataContext("Flags", flags)
			}).Execute(delegate
			{
				result = ((ISourceMailbox)this.WrappedObject).GetMailboxBasicInfo(flags);
			}, true);
			return result;
		}

		ISourceFolder ISourceMailbox.GetFolder(byte[] entryId)
		{
			ISourceFolder result = null;
			base.CreateContext("ISourceMailbox.GetFolder", new DataContext[]
			{
				new EntryIDsDataContext(entryId)
			}).Execute(delegate
			{
				result = ((ISourceMailbox)this.WrappedObject).GetFolder(entryId);
			}, true);
			if (result == null)
			{
				return null;
			}
			return new SourceFolderWrapper(result, base.CreateContext, base.ProviderInfo);
		}

		void ISourceMailbox.CopyTo(IFxProxy destMailbox, PropTag[] excludeProps)
		{
			base.CreateContext("ISourceMailbox.CopyTo", new DataContext[]
			{
				new PropTagsDataContext(excludeProps)
			}).Execute(delegate
			{
				((ISourceMailbox)this.WrappedObject).CopyTo(destMailbox, excludeProps);
			}, true);
		}

		void ISourceMailbox.SetMailboxSyncState(string syncStateStr)
		{
			base.CreateContext("ISourceMailbox.SetMailboxSyncState", new DataContext[]
			{
				new SimpleValueDataContext("StateLen", (syncStateStr != null) ? syncStateStr.Length : 0)
			}).Execute(delegate
			{
				((ISourceMailbox)this.WrappedObject).SetMailboxSyncState(syncStateStr);
			}, true);
		}

		string ISourceMailbox.GetMailboxSyncState()
		{
			string result = null;
			base.CreateContext("ISourceMailbox.GetMailboxSyncState", new DataContext[0]).Execute(delegate
			{
				result = ((ISourceMailbox)this.WrappedObject).GetMailboxSyncState();
			}, true);
			return result;
		}

		MailboxChangesManifest ISourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			MailboxChangesManifest result = null;
			base.CreateContext("ISourceMailbox.EnumerateHierarchyChanges", new DataContext[]
			{
				new SimpleValueDataContext("flags", flags),
				new SimpleValueDataContext("maxChanges", maxChanges)
			}).Execute(delegate
			{
				result = ((ISourceMailbox)this.WrappedObject).EnumerateHierarchyChanges(flags, maxChanges);
			}, true);
			return result;
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool destProxies, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			string text = "ISourceMailbox.ExportMessages";
			TimeSpan targetDuration = TimeSpan.Zero;
			Stopwatch stopwatch = Stopwatch.StartNew();
			base.CreateContext(text, new DataContext[]
			{
				new SimpleValueDataContext("Flags", flags),
				new PropTagsDataContext(excludeProps)
			}).Execute(delegate
			{
				using (FxProxyPoolFxCallbackWrapper fxProxyPoolFxCallbackWrapper = new FxProxyPoolFxCallbackWrapper(destProxies, true, delegate(TimeSpan duration)
				{
					targetDuration += duration;
				}))
				{
					((ISourceMailbox)this.WrappedObject).ExportMessages(messages, fxProxyPoolFxCallbackWrapper, flags, propsToCopyExplicitly, excludeProps);
				}
			}, false);
			base.UpdateDuration(text, stopwatch.Elapsed.Subtract(targetDuration));
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			string text = "ISourceMailbox.ExportFolders";
			TimeSpan targetDuration = TimeSpan.Zero;
			Stopwatch stopwatch = Stopwatch.StartNew();
			base.CreateContext(text, new DataContext[]
			{
				new EntryIDsDataContext(folderIds),
				new SimpleValueDataContext("exportFoldersDataToCopyFlags", exportFoldersDataToCopyFlags),
				new SimpleValueDataContext("folderRecFlags", folderRecFlags),
				new PropTagsDataContext(additionalFolderRecProps),
				new SimpleValueDataContext("copyPropertiesFlags", copyPropertiesFlags),
				new PropTagsDataContext(excludeProps),
				new SimpleValueDataContext("extendedAclFlags", extendedAclFlags)
			}).Execute(delegate
			{
				using (FxProxyPoolFxCallbackWrapper fxProxyPoolFxCallbackWrapper = new FxProxyPoolFxCallbackWrapper(proxyPool, true, delegate(TimeSpan duration)
				{
					targetDuration += duration;
				}))
				{
					((ISourceMailbox)this.WrappedObject).ExportFolders(folderIds, fxProxyPoolFxCallbackWrapper, exportFoldersDataToCopyFlags, folderRecFlags, additionalFolderRecProps, copyPropertiesFlags, excludeProps, extendedAclFlags);
				}
			}, false);
			base.UpdateDuration(text, stopwatch.Elapsed.Subtract(targetDuration));
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			List<ReplayActionResult> result = null;
			base.CreateContext("ISourceMailbox.ReplayActions", new DataContext[0]).Execute(delegate
			{
				result = ((ISourceMailbox)this.WrappedObject).ReplayActions(actions);
			}, true);
			return result;
		}
	}
}
