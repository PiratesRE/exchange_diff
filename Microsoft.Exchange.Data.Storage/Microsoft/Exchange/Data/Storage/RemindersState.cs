using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemindersState<T> where T : IReminderState, new()
	{
		[DataMember]
		public int Version { get; private set; }

		[DataMember]
		public List<T> StateList
		{
			get
			{
				return this.stateList;
			}
			set
			{
				Util.ThrowOnNullArgument(value, "StateList");
				this.stateList = value;
			}
		}

		public RemindersState()
		{
			this.Initialize();
		}

		public static void Set(IItem item, PropertyDefinition propertyDefinition, RemindersState<T> newState)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Util.ThrowOnMismatchType<byte[]>(propertyDefinition, "propertyDefinition");
			ExTraceGlobals.RemindersTracer.TraceDebug<StoreObjectId, PropertyDefinition>(0L, "RemindersState.Set - item={0}, propertyDefinition={1}", item.StoreObjectId, propertyDefinition);
			if (newState == null)
			{
				ExTraceGlobals.RemindersTracer.TraceDebug<PropertyDefinition>(0L, "RemindersState.Set - newState is null, deleting property={0}", propertyDefinition);
				item.Delete(propertyDefinition);
				return;
			}
			RemindersState<T>.ValidateStateIdentifiers(newState);
			ExTraceGlobals.RemindersTracer.TraceDebug<int>(0L, "RemindersState.Set - Serializing reminder states, count={0}", newState.StateList.Count);
			using (Stream stream = item.OpenPropertyStream(propertyDefinition, PropertyOpenMode.Create))
			{
				if (newState.StateList.Count > 0)
				{
					IReminderState reminderState = newState.StateList[0];
					newState.Version = reminderState.GetCurrentVersion();
				}
				using (XmlWriter xmlWriter = XmlWriter.Create(stream))
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(RemindersState<T>));
					dataContractSerializer.WriteObject(xmlWriter, newState);
				}
			}
		}

		public static RemindersState<T> Get(IItem item, PropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			ExTraceGlobals.RemindersTracer.TraceDebug<StoreObjectId, PropertyDefinition>(0L, "RemindersState.Get - item={0}, propertyDefinition={1}", item.StoreObjectId, propertyDefinition);
			return RemindersState<T>.GetFromStream(new RemindersState<T>.OpenPropertyStreamDelegate(item.OpenPropertyStream), propertyDefinition);
		}

		public static RemindersState<T> Get(ICorePropertyBag propertyBag, PropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			ExTraceGlobals.RemindersTracer.TraceDebug<PropertyDefinition>(0L, "RemindersState.Get - from property bag propertyDefinition={0}", propertyDefinition);
			return RemindersState<T>.GetFromStream(new RemindersState<T>.OpenPropertyStreamDelegate(propertyBag.OpenPropertyStream), propertyDefinition);
		}

		[OnDeserializing]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		internal static void ValidateStateIdentifiers(RemindersState<T> newState)
		{
			ExTraceGlobals.RemindersTracer.TraceDebug(0L, "RemindersState.ValidateStateIdentifiers");
			List<T> list = newState.StateList;
			Util.ThrowOnNullArgument(list, "newStateList");
			ExTraceGlobals.RemindersTracer.TraceDebug<int>(0L, "RemindersState.ValidateStateIdentifiers - newStateList count={0}", list.Count);
			foreach (T t in list)
			{
				if (t.Identifier == Guid.Empty)
				{
					throw new ArgumentException("state.Identifier is Guid.Empty", "state.Identifier");
				}
			}
		}

		private static bool IsCorruptDataException(Exception e)
		{
			return e is XmlException || e is SerializationException || e is InvalidParamException;
		}

		private void Initialize()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			this.Version = t.GetCurrentVersion();
			this.StateList = new List<T>();
		}

		private static RemindersState<T> GetFromStream(RemindersState<T>.OpenPropertyStreamDelegate openPropertyStreamMethod, PropertyDefinition propertyDefinition)
		{
			RemindersState<T> result;
			try
			{
				using (Stream stream = openPropertyStreamMethod(propertyDefinition, PropertyOpenMode.ReadOnly))
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(RemindersState<T>));
					result = (RemindersState<T>)dataContractSerializer.ReadObject(stream);
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.RemindersTracer.TraceError<ObjectNotFoundException>(0L, "RemindersState.Get - object not found, exception={0}", arg);
				result = null;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.RemindersTracer.TraceError<Exception>(0L, "RemindersState.Get - exception={0}", ex);
				if (!RemindersState<T>.IsCorruptDataException(ex))
				{
					throw;
				}
				result = new RemindersState<T>();
			}
			return result;
		}

		private List<T> stateList;

		private delegate Stream OpenPropertyStreamDelegate(PropertyDefinition propertyDefinition, PropertyOpenMode openMode);
	}
}
