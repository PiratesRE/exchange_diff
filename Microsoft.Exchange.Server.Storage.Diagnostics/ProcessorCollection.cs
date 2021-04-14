using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal static class ProcessorCollection
	{
		internal static IDictionary<string, ProcessorCollection.QueryableProcessor> Collection
		{
			get
			{
				if (ProcessorCollection.dictionary == null)
				{
					ProcessorCollection.dictionary = ProcessorCollection.CreateCollection();
				}
				return ProcessorCollection.dictionary;
			}
		}

		public static bool TryGetProcessorFactory(string name, out Func<IList<Column>, Processor> factory)
		{
			factory = null;
			ProcessorCollection.QueryableProcessor queryableProcessor;
			if (ProcessorCollection.Collection.TryGetValue(name, out queryableProcessor))
			{
				factory = queryableProcessor.Factory;
				return true;
			}
			return false;
		}

		public static IEnumerable<ProcessorCollection.QueryableProcessor> GetCollection()
		{
			return ProcessorCollection.Collection.Values;
		}

		private static IDictionary<string, ProcessorCollection.QueryableProcessor> CreateCollection()
		{
			IDictionary<string, ProcessorCollection.QueryableProcessor> dictionary = new Dictionary<string, ProcessorCollection.QueryableProcessor>(10);
			Type typeFromHandle = typeof(ProcessorCollection.QueryableProcessor);
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsSubclassOf(typeof(Processor)))
				{
					foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
					{
						MethodInfo getMethod = propertyInfo.GetGetMethod();
						if (getMethod != null && getMethod.ReturnType.Equals(typeFromHandle))
						{
							ProcessorCollection.QueryableProcessor queryableProcessor = (ProcessorCollection.QueryableProcessor)getMethod.Invoke(null, null);
							dictionary[queryableProcessor.Name] = queryableProcessor;
						}
					}
				}
			}
			return dictionary;
		}

		private static IDictionary<string, ProcessorCollection.QueryableProcessor> dictionary;

		internal sealed class QueryableProcessor
		{
			[Queryable(Index = 0)]
			public string Name { get; private set; }

			[Queryable]
			public string Consumes { get; private set; }

			[Queryable]
			public string Produces { get; private set; }

			[Queryable]
			public string Usage { get; private set; }

			public Func<IList<Column>, Processor> Factory { get; private set; }

			public static ProcessorCollection.QueryableProcessor Create(string name, string consumes, string produces, string usage, Func<IList<Column>, Processor> factory)
			{
				return new ProcessorCollection.QueryableProcessor
				{
					Name = name,
					Consumes = consumes,
					Produces = produces,
					Usage = usage,
					Factory = factory
				};
			}
		}
	}
}
