using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class Reminders<T> where T : IReminder, new()
	{
		[DataMember]
		public int Version { get; private set; }

		[DataMember]
		public List<T> ReminderList
		{
			get
			{
				return this.reminderList;
			}
			set
			{
				Util.ThrowOnNullArgument(value, "ReminderList");
				if (value.Count > 12)
				{
					throw new InvalidParamException(ServerStrings.MaxRemindersExceeded(value.Count, 12));
				}
				this.reminderList = value;
			}
		}

		public Reminders()
		{
			this.Initialize();
		}

		public static void Set(IItem item, PropertyDefinition propertyDefinition, Reminders<T> newReminders)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnMismatchType<byte[]>(propertyDefinition, "propertyDefinition");
			ExTraceGlobals.RemindersTracer.TraceDebug<StoreObjectId, PropertyDefinition>(0L, "Reminders.Set - item={0}, propertyDefinition={1}", item.StoreObjectId, propertyDefinition);
			if (newReminders == null)
			{
				ExTraceGlobals.RemindersTracer.TraceDebug<PropertyDefinition>(0L, "Reminders.Set - Reminder list is null, deleting property={0}", propertyDefinition);
				item.Delete(propertyDefinition);
				return;
			}
			Reminders<T>.UpdateReminderIdentifiers(newReminders);
			ExTraceGlobals.RemindersTracer.TraceDebug<int>(0L, "Reminders.Set - Serializing reminders, count={0}", newReminders.ReminderList.Count);
			using (Stream stream = item.OpenPropertyStream(propertyDefinition, PropertyOpenMode.Create))
			{
				if (newReminders.ReminderList.Count > 0)
				{
					IReminder reminder = newReminders.ReminderList[0];
					newReminders.Version = reminder.GetCurrentVersion();
				}
				using (XmlWriter xmlWriter = XmlWriter.Create(stream))
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(Reminders<T>));
					dataContractSerializer.WriteObject(xmlWriter, newReminders);
				}
			}
		}

		public static Reminders<T> Get(IItem item, PropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			ExTraceGlobals.RemindersTracer.TraceDebug<StoreObjectId, PropertyDefinition>(0L, "Reminders.Get - item={0}, propertyDefinition={1}", item.StoreObjectId, propertyDefinition);
			Reminders<T> result;
			try
			{
				using (Stream stream = item.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(Reminders<T>));
					result = (Reminders<T>)dataContractSerializer.ReadObject(stream);
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.RemindersTracer.TraceError<ObjectNotFoundException>(0L, "Reminders.Get - object not found, exception={0}", arg);
				result = null;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.RemindersTracer.TraceError<Exception>(0L, "Reminders.Get - exception={0}", ex);
				if (!Reminders<T>.IsCorruptDataException(ex))
				{
					throw;
				}
				result = new Reminders<T>();
			}
			return result;
		}

		internal static void UpdateReminderIdentifiers(Reminders<T> newReminders)
		{
			ExTraceGlobals.RemindersTracer.TraceDebug(0L, "Reminders.UpdateReminderIdentifiers");
			List<T> list = newReminders.ReminderList;
			Util.ThrowOnNullArgument(list, "newReminderList");
			ExTraceGlobals.RemindersTracer.TraceDebug<int>(0L, "Reminders.UpdateReminderIdentifiers - newReminders count={0}", list.Count);
			foreach (T t in list)
			{
				if (t.Identifier == Guid.Empty)
				{
					ExTraceGlobals.RemindersTracer.TraceDebug(0L, "Generating new reminder identifier");
					t.Identifier = Guid.NewGuid();
				}
			}
		}

		public IReminder GetReminder(Guid reminderId)
		{
			foreach (T t in this.ReminderList)
			{
				if (t.Identifier == reminderId)
				{
					return t;
				}
			}
			return null;
		}

		[OnDeserializing]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		private static bool IsCorruptDataException(Exception e)
		{
			return e is XmlException || e is SerializationException || e is InvalidParamException;
		}

		private void Initialize()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			this.Version = t.GetCurrentVersion();
			this.ReminderList = new List<T>();
		}

		internal const int MaxReminderCount = 12;

		private List<T> reminderList;
	}
}
