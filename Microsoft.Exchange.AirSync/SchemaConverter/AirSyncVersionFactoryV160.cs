using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV160;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal class AirSyncVersionFactoryV160 : IAirSyncVersionFactory
	{
		static AirSyncVersionFactoryV160()
		{
			AirSyncVersionFactoryV160.classToQueryFilterDictionary.Add("Email", EmailPrototypeSchemaState.SupportedClassQueryFilter);
			AirSyncVersionFactoryV160.classToQueryFilterDictionary.Add("Calendar", CalendarPrototypeSchemaState.SupportedClassQueryFilter);
			AirSyncVersionFactoryV160.classToQueryFilterDictionary.Add("Contacts", ContactsPrototypeSchemaState.SupportedClassQueryFilter);
			AirSyncVersionFactoryV160.classToQueryFilterDictionary.Add("Tasks", TasksPrototypeSchemaState.SupportedClassQueryFilter);
			AirSyncVersionFactoryV160.classToQueryFilterDictionary.Add("Notes", NotesPrototypeSchemaState.SupportedClassQueryFilter);
			AirSyncVersionFactoryV160.classToQueryFilterDictionary.Add("SMS", SmsPrototypeSchemaState.SupportedClassQueryFilter);
		}

		public string VersionString
		{
			get
			{
				return "16.0";
			}
		}

		public AirSyncSchemaState CreateCalendarSchema()
		{
			return new CalendarPrototypeSchemaState();
		}

		public AirSyncSchemaState CreateEmailSchema(IdMapping identifierMapping)
		{
			return new EmailPrototypeSchemaState(identifierMapping);
		}

		public AirSyncSchemaState CreateContactsSchema()
		{
			return new ContactsPrototypeSchemaState();
		}

		public AirSyncSchemaState CreateNotesSchema()
		{
			return new NotesPrototypeSchemaState();
		}

		public AirSyncSchemaState CreateSmsSchema()
		{
			return new SmsPrototypeSchemaState();
		}

		public AirSyncSchemaState CreateConsumerSmsAndMmsSchema()
		{
			return null;
		}

		public AirSyncSchemaState CreateTasksSchema()
		{
			return new TasksPrototypeSchemaState();
		}

		public AirSyncSchemaState CreateRecipientInfoCacheSchema()
		{
			return new RecipientInfoCacheSchemaState();
		}

		public IAirSyncMissingPropertyStrategy CreateMissingPropertyStrategy(Dictionary<string, bool> supportedTags)
		{
			return new AirSyncSetToDefaultStrategy(supportedTags);
		}

		public IAirSyncMissingPropertyStrategy CreateReadFlagMissingPropertyStrategy()
		{
			return new AirSyncSetToUnmodifiedStrategy();
		}

		public string GetClassFromMessageClass(string messageClass)
		{
			SinglePropertyBag propertyBag = new SinglePropertyBag(StoreObjectSchema.ItemClass, messageClass);
			foreach (KeyValuePair<string, QueryFilter> keyValuePair in AirSyncVersionFactoryV160.classToQueryFilterDictionary)
			{
				if (EvaluatableFilter.Evaluate(keyValuePair.Value, propertyBag))
				{
					return keyValuePair.Key;
				}
			}
			return null;
		}

		private static Dictionary<string, QueryFilter> classToQueryFilterDictionary = new Dictionary<string, QueryFilter>(6);
	}
}
