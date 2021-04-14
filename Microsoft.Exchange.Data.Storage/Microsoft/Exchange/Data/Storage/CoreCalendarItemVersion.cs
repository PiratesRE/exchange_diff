using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CoreCalendarItemVersion : DisposableObject
	{
		private CoreCalendarItemVersion() : this(null)
		{
		}

		private CoreCalendarItemVersion(StoreSession session)
		{
			this.session = session;
			this.underlyingMessage = null;
		}

		internal static StoreObjectId CreateVersion(StoreSession session, StoreObjectId itemId, StoreObjectId versionFolderId)
		{
			StoreObjectId result;
			using (CoreCalendarItemVersion coreCalendarItemVersion = new CoreCalendarItemVersion(session))
			{
				using (MapiProp mapiProp = session.GetMapiProp(itemId, OpenEntryFlags.DeferredErrors | OpenEntryFlags.ShowSoftDeletes))
				{
					coreCalendarItemVersion.underlyingMessage = Folder.InternalCreateMapiMessage(session, versionFolderId, CreateMessageType.Normal);
					coreCalendarItemVersion.CopyRequiredData((MapiMessage)mapiProp);
					object thisObject = null;
					bool flag = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						coreCalendarItemVersion.underlyingMessage.SetReadFlag(SetReadFlags.ClearRnPending | SetReadFlags.CleanNrnPending);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CoreCalendarItemVersion::CreateVersion. Failed to set read flag of underlying message.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CoreCalendarItemVersion::CreateVersion. Failed to set read flag of underlying message.", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (session != null)
							{
								session.EndMapiCall();
								if (flag)
								{
									session.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
				}
				coreCalendarItemVersion.Save();
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(coreCalendarItemVersion.underlyingMessage.GetProp(PropTag.EntryId).GetBytes(), itemId.ObjectType);
				CoreCalendarItemVersion.perfCounters.DumpsterCalendarLogsRate.Increment();
				result = storeObjectId;
			}
			return result;
		}

		private void CopyRequiredData(MapiMessage sourceMessage)
		{
			CoreObject.MapiCopyTo(sourceMessage, this.underlyingMessage, this.session, this.session, CopyPropertiesFlags.None, CopySubObjects.Copy, CoreCalendarItemVersion.ItemBodyAndAttachmentProperties);
		}

		private void Save()
		{
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.underlyingMessage.SaveChanges();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreCalendarItemVersion::Save. Failed to save the underlying message.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreCalendarItemVersion::Save. Failed to save the underlying message.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreCalendarItemVersion>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.underlyingMessage != null)
			{
				this.underlyingMessage.Dispose();
				this.underlyingMessage = null;
			}
			base.InternalDispose(disposing);
		}

		private static readonly NativeStorePropertyDefinition[] ItemBodyAndAttachmentProperties = new NativeStorePropertyDefinition[]
		{
			InternalSchema.HtmlBody,
			InternalSchema.RtfBody,
			InternalSchema.TextBody,
			InternalSchema.MessageDeepAttachments
		};

		private static readonly MiddleTierStoragePerformanceCountersInstance perfCounters = DumpsterFolderHelper.GetPerfCounters();

		private readonly StoreSession session;

		private MapiMessage underlyingMessage;
	}
}
