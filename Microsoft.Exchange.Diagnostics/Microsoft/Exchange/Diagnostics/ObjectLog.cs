using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ObjectLog<T>
	{
		public ObjectLog(ObjectLogSchema schema, ObjectLogConfiguration configuration)
		{
			this.configuration = configuration;
			this.schemaEntries = ObjectLog<T>.GetSchemaEntries(schema);
			this.logSchema = ObjectLog<T>.GetLogSchema(schema, this.schemaEntries);
			this.log = new Log(configuration.FilenamePrefix, new LogHeaderFormatter(this.logSchema, LogHeaderCsvOption.CsvStrict), configuration.LogComponentName, true);
			this.log.Configure(configuration.LoggingFolder, configuration.MaxLogAge, configuration.MaxLogDirSize, configuration.MaxLogFileSize, configuration.BufferLength, configuration.StreamFlushInterval, configuration.Note, configuration.FlushToDisk);
		}

		public static List<IObjectLogPropertyDefinition<T>> GetSchemaEntries(ObjectLogSchema schema)
		{
			List<IObjectLogPropertyDefinition<T>> list = new List<IObjectLogPropertyDefinition<T>>();
			FieldInfo[] fields = schema.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(null);
				ObjectLogSchema objectLogSchema = value as ObjectLogSchema;
				if (objectLogSchema != null)
				{
					list.AddRange(ObjectLog<T>.GetSchemaEntries(objectLogSchema));
				}
				else
				{
					IObjectLogPropertyDefinition<T> objectLogPropertyDefinition = value as IObjectLogPropertyDefinition<T>;
					if (objectLogPropertyDefinition != null)
					{
						if (schema.ExcludedProperties == null || !schema.ExcludedProperties.Contains(objectLogPropertyDefinition.FieldName))
						{
							list.Add(objectLogPropertyDefinition);
						}
					}
					else
					{
						IEnumerable<IObjectLogPropertyDefinition<T>> enumerable = value as IEnumerable<IObjectLogPropertyDefinition<T>>;
						if (enumerable != null)
						{
							foreach (IObjectLogPropertyDefinition<T> objectLogPropertyDefinition2 in enumerable)
							{
								if (schema.ExcludedProperties == null || !schema.ExcludedProperties.Contains(objectLogPropertyDefinition2.FieldName))
								{
									list.Add(objectLogPropertyDefinition2);
								}
							}
						}
					}
				}
			}
			return list;
		}

		public static LogSchema GetLogSchema(ObjectLogSchema schema)
		{
			List<IObjectLogPropertyDefinition<T>> list = ObjectLog<T>.GetSchemaEntries(schema);
			return ObjectLog<T>.GetLogSchema(schema, list);
		}

		public void LogObject(T objectToLog)
		{
			if (!this.configuration.IsEnabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			int num = 1;
			foreach (IObjectLogPropertyDefinition<T> objectLogPropertyDefinition in this.schemaEntries)
			{
				logRowFormatter[num++] = objectLogPropertyDefinition.GetValue(objectToLog);
			}
			this.log.Append(logRowFormatter, 0);
		}

		public void CloseLog()
		{
			this.log.Close();
		}

		public void Flush()
		{
			this.log.Flush();
		}

		private static LogSchema GetLogSchema(ObjectLogSchema schema, List<IObjectLogPropertyDefinition<T>> schemaEntries)
		{
			List<string> list = new List<string>();
			list.Add("Time");
			foreach (IObjectLogPropertyDefinition<T> objectLogPropertyDefinition in schemaEntries)
			{
				list.Add(objectLogPropertyDefinition.FieldName);
			}
			return new LogSchema(schema.Software, schema.Version, schema.LogType, list.ToArray());
		}

		private readonly LogSchema logSchema;

		private readonly Log log;

		private readonly List<IObjectLogPropertyDefinition<T>> schemaEntries;

		private readonly ObjectLogConfiguration configuration;
	}
}
