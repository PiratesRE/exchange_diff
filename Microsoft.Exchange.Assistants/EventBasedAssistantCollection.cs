using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class EventBasedAssistantCollection : Base, IDisposable, IEnumerable<AssistantCollectionEntry>, IEnumerable
	{
		private EventBasedAssistantCollection(DatabaseInfo databaseInfo, AssistantType[] assistantTypes)
		{
			this.databaseInfo = databaseInfo;
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			this.eventMask = (MapiEventTypeFlags)0;
			this.needsMailboxSession = false;
			foreach (AssistantType assistantType in assistantTypes)
			{
				if (!databaseInfo.IsPublic || assistantType.ProcessesPublicDatabases)
				{
					this.eventMask |= assistantType.EventMask;
					this.needsMailboxSession |= assistantType.NeedsMailboxSession;
					if (assistantType.PreloadItemProperties != null && assistantType.PreloadItemProperties.Length > 0)
					{
						list.AddRange(assistantType.PreloadItemProperties);
					}
				}
			}
			this.preloadItemProperties = list.ToArray();
			this.listOfAssistants = new List<AssistantCollectionEntry>(assistantTypes.Length);
			bool flag = false;
			try
			{
				foreach (AssistantType assistantType2 in assistantTypes)
				{
					if (!databaseInfo.IsPublic || assistantType2.ProcessesPublicDatabases)
					{
						this.listOfAssistants.Add(new AssistantCollectionEntry(assistantType2, databaseInfo));
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public int Count
		{
			get
			{
				return this.listOfAssistants.Count;
			}
		}

		public MapiEventTypeFlags EventMask
		{
			get
			{
				return this.eventMask;
			}
		}

		public bool NeedsMailboxSession
		{
			get
			{
				return this.needsMailboxSession;
			}
		}

		public PropertyDefinition[] PreloadItemProperties
		{
			get
			{
				return this.preloadItemProperties;
			}
		}

		public static EventBasedAssistantCollection Create(DatabaseInfo databaseInfo, AssistantType[] assistantTypes)
		{
			EventBasedAssistantCollection eventBasedAssistantCollection = new EventBasedAssistantCollection(databaseInfo, assistantTypes);
			if (eventBasedAssistantCollection.Count == 0)
			{
				eventBasedAssistantCollection.Dispose();
				return null;
			}
			return eventBasedAssistantCollection;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "Event-based assistant collection for database '" + this.databaseInfo.DisplayName + "'";
			}
			return this.toString;
		}

		public AssistantCollectionEntry GetAssistantForPublicFolder()
		{
			return this.listOfAssistants[0];
		}

		public void ShutdownAssistants(HangDetector hangDetector)
		{
			foreach (AssistantCollectionEntry assistantCollectionEntry in this.listOfAssistants)
			{
				try
				{
					if (assistantCollectionEntry != null)
					{
						AIBreadcrumbs.ShutdownTrail.Drop("Stopping event assistant " + assistantCollectionEntry.Name);
						hangDetector.AssistantName = assistantCollectionEntry.Name;
						assistantCollectionEntry.Shutdown();
						AIBreadcrumbs.ShutdownTrail.Drop("Finished stopping " + assistantCollectionEntry.Name);
					}
				}
				finally
				{
					hangDetector.AssistantName = "Common Code";
				}
			}
		}

		public void Dispose()
		{
			if (this.listOfAssistants != null)
			{
				foreach (AssistantCollectionEntry assistantCollectionEntry in this.listOfAssistants)
				{
					IDisposable disposable = assistantCollectionEntry.Instance as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				this.listOfAssistants = null;
			}
		}

		public void RemoveAssistant(AssistantCollectionEntry assistantToRemove)
		{
			if (assistantToRemove != null)
			{
				this.listOfAssistants.Remove(assistantToRemove);
			}
		}

		public IEnumerator<AssistantCollectionEntry> GetEnumerator()
		{
			return ((IEnumerable<AssistantCollectionEntry>)this.listOfAssistants).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.listOfAssistants.GetEnumerator();
		}

		internal static EventBasedAssistantCollection CreateForTest(DatabaseInfo databaseInfo, IEventBasedAssistantType[] eventBasedAssistantTypes)
		{
			PerformanceCountersPerAssistantInstance performanceCountersPerAssistantsTotal = new PerformanceCountersPerAssistantInstance("TestAssistant-Total", null);
			AssistantType[] assistantTypes = AssistantType.CreateArray(eventBasedAssistantTypes, performanceCountersPerAssistantsTotal);
			EventBasedAssistantCollection eventBasedAssistantCollection = new EventBasedAssistantCollection(databaseInfo, assistantTypes);
			if (eventBasedAssistantCollection.Count == 0)
			{
				eventBasedAssistantCollection.Dispose();
				return null;
			}
			return eventBasedAssistantCollection;
		}

		private MapiEventTypeFlags eventMask;

		private bool needsMailboxSession;

		private PropertyDefinition[] preloadItemProperties;

		private List<AssistantCollectionEntry> listOfAssistants;

		private string toString;

		private DatabaseInfo databaseInfo;
	}
}
