using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FolderHierarchyChangeDetector
	{
		public static FolderHierarchyChangeDetector.MailboxChangesManifest RunICSManifestSync(bool catchup, FolderHierarchyChangeDetector.SyncHierarchyManifestState hierState, MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			syncLogger.TraceDebug<SmtpAddress, bool>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchyChangeDetector.RunICSManifestSync] Checking for folder hierarhcy changes for Mailbox: {0}.  Catchup? {1}", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, catchup);
			MapiStore _ContainedMapiStore = mailboxSession.__ContainedMapiStore;
			FolderHierarchyChangeDetector.MailboxChangesManifest mailboxChangesManifest = new FolderHierarchyChangeDetector.MailboxChangesManifest();
			FolderHierarchyChangeDetector.ManifestHierarchyCallback iMapiManifestCallback = new FolderHierarchyChangeDetector.ManifestHierarchyCallback(catchup, mailboxChangesManifest);
			try
			{
				using (MapiFolder ipmSubtreeFolder = _ContainedMapiStore.GetIpmSubtreeFolder())
				{
					SyncConfigFlags syncConfigFlags = SyncConfigFlags.ManifestHierReturnDeletedEntryIds;
					int serverVersion = mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion;
					if ((serverVersion >= Server.E14MinVersion && serverVersion < Server.E15MinVersion) || (long)serverVersion >= FolderHierarchyChangeDetector.E15MinVersionSupportsOnlySpecifiedPropsForHierarchy)
					{
						syncConfigFlags |= SyncConfigFlags.OnlySpecifiedProps;
					}
					using (MapiHierarchyManifestEx mapiHierarchyManifestEx = ipmSubtreeFolder.CreateExportHierarchyManifestEx(syncConfigFlags, hierState.IdsetGiven, hierState.CnsetSeen, iMapiManifestCallback, FolderHierarchyChangeDetector.PropsToFetch, null))
					{
						while (mapiHierarchyManifestEx.Synchronize() != ManifestStatus.Done)
						{
						}
						byte[] idSetGiven;
						byte[] cnetSeen;
						mapiHierarchyManifestEx.GetState(out idSetGiven, out cnetSeen);
						syncLogger.TraceDebug<SmtpAddress, int, int>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchyChangeDetector.RunICSManifestSync] Updating ICS state for mailbox: '{0}'.  Change Count: {1}, Delete Count: {2}", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, mailboxChangesManifest.ChangedFolders.Count, mailboxChangesManifest.DeletedFolders.Count);
						hierState.Update(idSetGiven, cnetSeen);
					}
				}
			}
			catch (MapiPermanentException arg)
			{
				syncLogger.TraceDebug<SmtpAddress, MapiPermanentException>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchyChangeDetector.RunICSManifestSync] Caught MapiPermanentException when determining folder ICS changes for mailbox: {0}.  Exception: {1}", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, arg);
				return null;
			}
			catch (MapiRetryableException arg2)
			{
				syncLogger.TraceDebug<SmtpAddress, MapiRetryableException>(ExTraceGlobals.SyncProcessTracer, 0L, "[FolderHierarchyChangeDetector.RunICSManifestSync] Caught MapiRetryableException when determining folder ICS changes for mailbox: {0}.  Exception: {1}", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, arg2);
				return null;
			}
			return mailboxChangesManifest;
		}

		private static readonly long E15MinVersionSupportsOnlySpecifiedPropsForHierarchy = (long)new ServerVersion(15, 0, 922, 0).ToInt();

		private static readonly PropTag[] PropsToFetch = new PropTag[]
		{
			PropTag.EntryId,
			PropTag.DisplayName
		};

		private class ManifestHierarchyCallback : IMapiHierarchyManifestCallback
		{
			public ManifestHierarchyCallback(bool catchup, FolderHierarchyChangeDetector.MailboxChangesManifest changes)
			{
				this.catchup = catchup;
				this.changes = changes;
				this.changes.ChangedFolders = new Dictionary<StoreObjectId, string>();
				this.changes.DeletedFolders = new List<StoreObjectId>();
			}

			ManifestCallbackStatus IMapiHierarchyManifestCallback.Change(PropValue[] props)
			{
				if (this.catchup)
				{
					return ManifestCallbackStatus.Continue;
				}
				StoreObjectId key = null;
				string value = null;
				foreach (PropValue propValue in props)
				{
					if (propValue.PropTag == PropTag.EntryId)
					{
						byte[] bytes = propValue.GetBytes();
						key = StoreObjectId.FromProviderSpecificId(bytes);
					}
					else if (propValue.PropTag == PropTag.DisplayName)
					{
						value = propValue.GetString();
					}
				}
				if (!this.changes.ChangedFolders.ContainsKey(key))
				{
					this.changes.ChangedFolders[key] = value;
				}
				return ManifestCallbackStatus.Continue;
			}

			ManifestCallbackStatus IMapiHierarchyManifestCallback.Delete(byte[] entryId)
			{
				if (this.catchup)
				{
					return ManifestCallbackStatus.Continue;
				}
				StoreObjectId item = StoreObjectId.FromProviderSpecificId(entryId);
				if (!this.changes.DeletedFolders.Contains(item))
				{
					this.changes.DeletedFolders.Add(item);
				}
				return ManifestCallbackStatus.Continue;
			}

			private FolderHierarchyChangeDetector.MailboxChangesManifest changes;

			private readonly bool catchup;
		}

		internal class MailboxChangesManifest
		{
			public MailboxChangesManifest()
			{
				this.changedFolders = null;
				this.deletedFolders = null;
			}

			public Dictionary<StoreObjectId, string> ChangedFolders
			{
				get
				{
					return this.changedFolders;
				}
				set
				{
					this.changedFolders = value;
				}
			}

			public List<StoreObjectId> DeletedFolders
			{
				get
				{
					return this.deletedFolders;
				}
				set
				{
					this.deletedFolders = value;
				}
			}

			public bool HasChanges
			{
				get
				{
					return (this.changedFolders != null && this.changedFolders.Count > 0) || (this.deletedFolders != null && this.deletedFolders.Count > 0);
				}
			}

			private Dictionary<StoreObjectId, string> changedFolders;

			private List<StoreObjectId> deletedFolders;
		}

		public sealed class SyncHierarchyManifestState
		{
			public SyncHierarchyManifestState()
			{
				this.Update(Array<byte>.Empty, Array<byte>.Empty);
			}

			public byte[] IdsetGiven { get; private set; }

			public byte[] CnsetSeen { get; private set; }

			public void Update(byte[] idSetGiven, byte[] cnetSeen)
			{
				lock (this.instanceLock)
				{
					this.IdsetGiven = idSetGiven;
					this.CnsetSeen = cnetSeen;
				}
			}

			public FolderHierarchyChangeDetector.SyncHierarchyManifestState Clone()
			{
				FolderHierarchyChangeDetector.SyncHierarchyManifestState result;
				lock (this.instanceLock)
				{
					result = new FolderHierarchyChangeDetector.SyncHierarchyManifestState
					{
						IdsetGiven = (byte[])this.IdsetGiven.Clone(),
						CnsetSeen = (byte[])this.CnsetSeen.Clone()
					};
				}
				return result;
			}

			private object instanceLock = new object();
		}
	}
}
