using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HierarchySynchronizationUploadContext : SynchronizationUploadContextBase<MapiHierarchyCollectorEx>
	{
		public HierarchySynchronizationUploadContext(CoreFolder folder, StorageIcsState initialState) : base(folder, initialState)
		{
		}

		internal void ImportChange(ExTimeZone exTimeZone, IList<PropertyDefinition> propertyDefinitions, IList<object> propertyValues)
		{
			this.CheckDisposed(null);
			PropValue[] propValues = base.GetPropValuesFromValues(exTimeZone, propertyDefinitions, propertyValues).ToArray();
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
				base.MapiCollector.ImportFolderChange(propValues);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportFolderChange, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of the folder change failed", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotImportFolderChange, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Import of the folder change failed", new object[0]),
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

		protected override MapiHierarchyCollectorEx MapiCreateCollector(StorageIcsState initialState)
		{
			return base.MapiFolder.CreateHierarchyCollectorEx(initialState.StateIdsetGiven, initialState.StateCnsetSeen, CollectorConfigFlags.None);
		}

		protected override void MapiGetCurrentState(ref StorageIcsState finalState)
		{
			byte[] stateIdsetGiven;
			byte[] stateCnsetSeen;
			base.MapiCollector.GetState(out stateIdsetGiven, out stateCnsetSeen);
			finalState.StateIdsetGiven = stateIdsetGiven;
			finalState.StateCnsetSeen = stateCnsetSeen;
		}

		protected override void MapiImportDeletes(ImportDeletionFlags importDeletionFlags, PropValue[] sourceKeys)
		{
			base.MapiCollector.ImportFolderDeletion(importDeletionFlags, sourceKeys);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<HierarchySynchronizationUploadContext>(this);
		}
	}
}
