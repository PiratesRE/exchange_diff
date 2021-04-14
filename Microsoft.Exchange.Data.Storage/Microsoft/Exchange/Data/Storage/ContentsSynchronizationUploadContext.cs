using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContentsSynchronizationUploadContext : SynchronizationUploadContextBase<MapiCollectorEx>
	{
		public ContentsSynchronizationUploadContext(CoreFolder folder, StorageIcsState initialState) : base(folder, initialState)
		{
		}

		internal static ImportResult ConvertToImportResult(MapiCollectorStatus mapiCollectorStatus)
		{
			if (mapiCollectorStatus <= MapiCollectorStatus.NotFound)
			{
				switch (mapiCollectorStatus)
				{
				case MapiCollectorStatus.ObjectModified:
					return ImportResult.ObjectModified;
				case MapiCollectorStatus.ObjectDeleted:
					return ImportResult.ObjectDeleted;
				default:
					if (mapiCollectorStatus == MapiCollectorStatus.NotFound)
					{
						return ImportResult.NotFound;
					}
					break;
				}
			}
			else
			{
				switch (mapiCollectorStatus)
				{
				case MapiCollectorStatus.SyncObjectDeleted:
					return ImportResult.SyncObjectDeleted;
				case MapiCollectorStatus.SyncIgnore:
					return ImportResult.SyncIgnore;
				case MapiCollectorStatus.SyncConflict:
					return ImportResult.SyncConflict;
				default:
					if (mapiCollectorStatus == MapiCollectorStatus.Success)
					{
						return ImportResult.Success;
					}
					if (mapiCollectorStatus == MapiCollectorStatus.SyncClientChangeNewer)
					{
						return ImportResult.SyncClientChangeNewer;
					}
					break;
				}
			}
			return ImportResult.Unknown;
		}

		internal static MapiCollectorStatus ConvertToCollectorStatus(ImportResult result)
		{
			switch (result)
			{
			case ImportResult.Success:
				return MapiCollectorStatus.Success;
			case ImportResult.SyncClientChangeNewer:
				return MapiCollectorStatus.SyncClientChangeNewer;
			case ImportResult.SyncObjectDeleted:
				return MapiCollectorStatus.SyncObjectDeleted;
			case ImportResult.SyncIgnore:
				return MapiCollectorStatus.SyncIgnore;
			case ImportResult.SyncConflict:
				return MapiCollectorStatus.SyncConflict;
			case ImportResult.NotFound:
				return MapiCollectorStatus.NotFound;
			case ImportResult.ObjectModified:
				return MapiCollectorStatus.ObjectModified;
			case ImportResult.ObjectDeleted:
				return MapiCollectorStatus.ObjectDeleted;
			default:
				return MapiCollectorStatus.Failed;
			}
		}

		internal ImportResult ImportChange(CreateMessageType createMessageType, bool failOnConflict, IList<PropertyDefinition> propertyDefinitions, IList<object> propertyValues, out MapiMessage mapiMessage)
		{
			this.CheckDisposed(null);
			ImportMessageChangeFlags importMessageChangeFlags = ImportMessageChangeFlags.NewMessage;
			if (failOnConflict)
			{
				importMessageChangeFlags |= ImportMessageChangeFlags.FailOnConflict;
			}
			if (createMessageType == CreateMessageType.Associated)
			{
				importMessageChangeFlags |= ImportMessageChangeFlags.Associated;
			}
			else if (createMessageType != CreateMessageType.Normal)
			{
				throw new ArgumentOutOfRangeException("createMessageType", createMessageType, "Only Normal and Associated messages can be imported");
			}
			PropValue[] propValues = base.GetPropValuesFromValues(base.Session.ExTimeZone, propertyDefinitions, propertyValues).ToArray();
			MapiMessage mapiMessage2 = null;
			bool flag = false;
			ImportResult result;
			try
			{
				StoreSession session = base.Session;
				bool flag2 = false;
				MapiCollectorStatus mapiCollectorStatus;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiCollectorStatus = base.MapiCollector.ImportMessageChange(propValues, importMessageChangeFlags, out mapiMessage2);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportMessageChange, ex, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Import of the message change failed", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportMessageChange, ex2, session, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Import of the message change failed", new object[0]),
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
							if (flag2)
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
				ImportResult importResult = ContentsSynchronizationUploadContext.ConvertToImportResult(mapiCollectorStatus);
				mapiMessage = mapiMessage2;
				flag = true;
				result = importResult;
			}
			finally
			{
				if (!flag)
				{
					mapiMessage = null;
					if (mapiMessage2 != null)
					{
						mapiMessage2.Dispose();
					}
				}
			}
			return result;
		}

		internal ImportResult ImportMove(byte[] sourceParentSourceKey, byte[] sourceSourceKey, byte[] sourcePredecessorChangeList, byte[] destinationSourceKey, byte[] destinationChangeKey)
		{
			this.CheckDisposed(null);
			StoreSession session = base.Session;
			bool flag = false;
			MapiCollectorStatus mapiCollectorStatus;
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
				mapiCollectorStatus = base.MapiCollector.ImportMessageMove(new PropValue(PropTag.ParentSourceKey, sourceParentSourceKey), new PropValue(PropTag.SourceKey, sourceSourceKey), new PropValue(PropTag.PredecessorChangeList, sourcePredecessorChangeList), new PropValue(PropTag.SourceKey, destinationSourceKey), new PropValue(PropTag.ChangeKey, destinationChangeKey));
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportMessageMove, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of the message move failed", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportMessageMove, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of the message move failed", new object[0]),
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
			return ContentsSynchronizationUploadContext.ConvertToImportResult(mapiCollectorStatus);
		}

		internal void ImportReadStateChange(bool isRead, IList<byte[]> sourceKeys)
		{
			this.CheckDisposed(null);
			ReadState[] array = new ReadState[sourceKeys.Count];
			for (int i = 0; i < sourceKeys.Count; i++)
			{
				array[i] = new ReadState(new PropValue(PropTag.SourceKey, sourceKeys[i]), isRead);
			}
			StoreSession session = base.Session;
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
				base.MapiCollector.ImportPerUserReadStateChange(array);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportReadStateChange, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of the changes to read state failed", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportReadStateChange, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of the changes to read state failed", new object[0]),
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

		internal void ThrowOnImportResult(ImportResult result)
		{
			StoreSession session = base.Session;
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
				base.MapiCollector.ThrowOnCollectorStatus(ContentsSynchronizationUploadContext.ConvertToCollectorStatus(result));
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ImportResultContainedFailure, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("ImportResult contained a failure", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ImportResultContainedFailure, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("ImportResult contained a failure", new object[0]),
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

		protected override MapiCollectorEx MapiCreateCollector(StorageIcsState initialState)
		{
			return base.MapiFolder.CreateContentsCollectorEx(initialState.StateIdsetGiven, initialState.StateCnsetSeen, initialState.StateCnsetSeenFAI, initialState.StateCnsetRead, CollectorConfigFlags.None);
		}

		protected override void MapiGetCurrentState(ref StorageIcsState finalState)
		{
			byte[] stateIdsetGiven;
			byte[] stateCnsetSeen;
			byte[] stateCnsetSeenFAI;
			byte[] stateCnsetRead;
			base.MapiCollector.GetState(out stateIdsetGiven, out stateCnsetSeen, out stateCnsetSeenFAI, out stateCnsetRead);
			finalState.StateIdsetGiven = stateIdsetGiven;
			finalState.StateCnsetSeen = stateCnsetSeen;
			finalState.StateCnsetSeenFAI = stateCnsetSeenFAI;
			finalState.StateCnsetRead = stateCnsetRead;
		}

		protected override void MapiImportDeletes(ImportDeletionFlags importDeletionFlags, PropValue[] sourceKeys)
		{
			base.MapiCollector.ImportMessageDeletion(importDeletionFlags, sourceKeys);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ContentsSynchronizationUploadContext>(this);
		}
	}
}
