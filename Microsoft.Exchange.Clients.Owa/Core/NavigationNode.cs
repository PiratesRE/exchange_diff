using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class NavigationNode
	{
		protected NavigationNode(MemoryPropertyBag propertyBag)
		{
			this.propertyBag = propertyBag;
			this.propertyBag.SetAllPropertiesLoaded();
			this.isNew = false;
		}

		protected NavigationNode(PropertyDefinition[] additionalProperties, object[] values, Dictionary<PropertyDefinition, int> propertyMap)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>(NavigationNode.coreProperties);
			hashSet.UnionWith(additionalProperties);
			ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.AllRead, hashSet);
			object[] array = new object[nativePropertyDefinitions.Count];
			int num = 0;
			foreach (NativeStorePropertyDefinition key in nativePropertyDefinitions)
			{
				array[num++] = values[propertyMap[key]];
			}
			this.propertyBag = new MemoryPropertyBag();
			this.propertyBag.PreLoadStoreProperty<NativeStorePropertyDefinition>(nativePropertyDefinitions, array);
			this.propertyBag.SetAllPropertiesLoaded();
			this.ClearDirty();
			this.isNew = false;
		}

		protected NavigationNode(NavigationNodeType navigationNodeType, string subject, NavigationNodeGroupSection navigationNodeGroupSection)
		{
			this.propertyBag = new MemoryPropertyBag();
			this.propertyBag.SetAllPropertiesLoaded();
			this.NavigationNodeType = navigationNodeType;
			this.Subject = subject;
			this.NavigationNodeFlags = NavigationNodeFlags.None;
			this.NavigationNodeClassId = NavigationNode.GetFolderTypeClassId(navigationNodeGroupSection);
			this.NavigationNodeGroupSection = navigationNodeGroupSection;
			this.propertyBag.Load(null);
			this.isNew = true;
		}

		public virtual string Subject
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
			}
			set
			{
				if (!string.IsNullOrEmpty(value.Trim()))
				{
					this.propertyBag.SetOrDeleteProperty(ItemSchema.Subject, value);
				}
			}
		}

		internal static string GetFolderClass(NavigationNodeGroupSection groupSection)
		{
			switch (groupSection)
			{
			case NavigationNodeGroupSection.Calendar:
				return "IPF.Appointment";
			case NavigationNodeGroupSection.Contacts:
				return "IPF.Contact";
			case NavigationNodeGroupSection.Tasks:
				return "IPF.Task";
			default:
				return "IPF.Note";
			}
		}

		internal VersionedId NavigationNodeId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id);
			}
		}

		internal NavigationNodeType NavigationNodeType
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<NavigationNodeType>(NavigationNodeSchema.Type, NavigationNodeType.NormalFolder);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.Type, value);
			}
		}

		protected NavigationNodeFlags NavigationNodeFlags
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<NavigationNodeFlags>(NavigationNodeSchema.Flags);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.Flags, value);
			}
		}

		internal bool IsFlagSet(NavigationNodeFlags flag)
		{
			return Utilities.IsFlagSet((int)this.NavigationNodeFlags, (int)flag);
		}

		internal byte[] NavigationNodeOrdinal
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.Ordinal);
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.Ordinal, value);
			}
		}

		internal ExDateTime LastModifiedTime
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.propertyBag.IsDirty;
			}
		}

		internal bool IsNew
		{
			get
			{
				return this.isNew;
			}
			set
			{
				this.isNew = value;
			}
		}

		internal NavigationNodeGroupSection NavigationNodeGroupSection
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<NavigationNodeGroupSection>(NavigationNodeSchema.GroupSection);
			}
			private set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.GroupSection, value);
			}
		}

		private byte[] NavigationNodeClassId
		{
			get
			{
				byte[] array = this.propertyBag.GetValueOrDefault<byte[]>(NavigationNodeSchema.ClassId);
				if (array == null)
				{
					array = NavigationNode.GetFolderTypeClassId(this.NavigationNodeGroupSection);
				}
				return array;
			}
			set
			{
				this.propertyBag.SetOrDeleteProperty(NavigationNodeSchema.ClassId, value);
			}
		}

		internal void Save(MailboxSession session)
		{
			MessageItem messageItem = null;
			try
			{
				if (this.NavigationNodeId == null)
				{
					messageItem = MessageItem.CreateAssociated(session, session.GetDefaultFolderId(DefaultFolderType.CommonViews));
					messageItem[StoreObjectSchema.ItemClass] = "IPM.Microsoft.WunderBar.Link";
				}
				else
				{
					messageItem = MessageItem.Bind(session, this.NavigationNodeId);
				}
				this.UpdateMessage(messageItem);
				messageItem.Save(SaveMode.NoConflictResolution);
				this.ClearDirty();
				this.isNew = false;
			}
			catch (StorageTransientException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "NavigationNode.Save. Unable to save navigation node. Exception: {0}.", ex.Message);
			}
			catch (StoragePermanentException ex2)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "NavigationNode.Save. Unable to get save navigation node. Exception: {0}.", ex2.Message);
			}
			finally
			{
				if (messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
		}

		protected virtual void UpdateMessage(MessageItem message)
		{
			message[ItemSchema.Subject] = this.Subject;
			message[NavigationNodeSchema.OutlookTagId] = NavigationNode.UpdateOutlookTagId();
			message[NavigationNodeSchema.Type] = this.NavigationNodeType;
			message[NavigationNodeSchema.Flags] = this.NavigationNodeFlags;
			message[NavigationNodeSchema.Ordinal] = this.NavigationNodeOrdinal;
			message[NavigationNodeSchema.ClassId] = this.NavigationNodeClassId;
			message[NavigationNodeSchema.GroupSection] = this.NavigationNodeGroupSection;
		}

		protected void ClearDirty()
		{
			this.propertyBag.ClearChangeInfo();
		}

		private static int UpdateOutlookTagId()
		{
			return NavigationNode.random.Next();
		}

		private static byte[] GetFolderTypeClassId(NavigationNodeGroupSection navigationNodeGroupSection)
		{
			switch (navigationNodeGroupSection)
			{
			case NavigationNodeGroupSection.First:
				return NavigationNode.MailFavoritesClassId.ToByteArray();
			case NavigationNodeGroupSection.Mail:
				return NavigationNode.MailClassId.ToByteArray();
			case NavigationNodeGroupSection.Calendar:
				return NavigationNode.CalendarClassId.ToByteArray();
			case NavigationNodeGroupSection.Contacts:
				return NavigationNode.ContactsClassId.ToByteArray();
			case NavigationNodeGroupSection.Tasks:
				return NavigationNode.TasksClassId.ToByteArray();
			case NavigationNodeGroupSection.Notes:
				return NavigationNode.NotesClassId.ToByteArray();
			case NavigationNodeGroupSection.Journal:
				return NavigationNode.JournalClassId.ToByteArray();
			default:
				throw new ArgumentOutOfRangeException("navigationNodeGroupSection");
			}
		}

		protected void GuidSetter(PropertyDefinition propertyDefinition, Guid guid)
		{
			this.propertyBag.SetOrDeleteProperty(propertyDefinition, guid.Equals(Guid.Empty) ? null : guid.ToByteArray());
		}

		protected Guid GuidGetter(PropertyDefinition propertyDefinition)
		{
			byte[] valueOrDefault = this.propertyBag.GetValueOrDefault<byte[]>(propertyDefinition);
			if (valueOrDefault != null)
			{
				return new Guid(valueOrDefault);
			}
			return Guid.Empty;
		}

		protected bool IsFavorites
		{
			get
			{
				return this.NavigationNodeGroupSection == NavigationNodeGroupSection.First;
			}
		}

		internal const string OutlookWunderBarLinkAssociatedMessageClass = "IPM.Microsoft.WunderBar.Link";

		private static readonly Guid MailFavoritesClassId = new Guid("{00067800-0000-0000-C000-000000000046}");

		private static readonly Guid MailClassId = new Guid("{0006780D-0000-0000-C000-000000000046}");

		private static readonly Guid CalendarClassId = new Guid("{00067802-0000-0000-c000-000000000046}");

		private static readonly Guid ContactsClassId = new Guid("{00067801-0000-0000-C000-000000000046}");

		private static readonly Guid TasksClassId = new Guid("{00067803-0000-0000-C000-000000000046}");

		private static readonly Guid NotesClassId = new Guid("{00067804-0000-0000-C000-000000000046}");

		private static readonly Guid JournalClassId = new Guid("{00067808-0000-0000-C000-000000000046}");

		private static readonly PropertyDefinition[] coreProperties = new PropertyDefinition[]
		{
			NavigationNodeSchema.Type,
			StoreObjectSchema.ItemClass,
			NavigationNodeSchema.OutlookTagId,
			NavigationNodeSchema.Flags,
			NavigationNodeSchema.Ordinal,
			NavigationNodeSchema.ClassId,
			NavigationNodeSchema.GroupSection,
			ItemSchema.Id,
			ItemSchema.Subject,
			StoreObjectSchema.LastModifiedTime
		};

		protected MemoryPropertyBag propertyBag;

		private static readonly Random random = new Random((int)ExDateTime.UtcNow.UtcTicks);

		private bool isNew = true;
	}
}
