using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public sealed class WorkItemFactory : IWorkItemFactory
	{
		public WorkItemFactory()
		{
			this.LocalPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		public static Dictionary<string, Assembly> DefaultAssemblies
		{
			get
			{
				return WorkItemFactory.defaultAssemblies;
			}
		}

		public string LocalPath { get; internal set; }

		public T CreateWorkItem<T>(WorkDefinition definition) where T : WorkItem
		{
			T result = default(T);
			Type type = definition.WorkItemType;
			if (type == null)
			{
				Assembly assembly;
				if (WorkItemFactory.DefaultAssemblies.ContainsKey(definition.AssemblyPath))
				{
					assembly = WorkItemFactory.DefaultAssemblies[definition.AssemblyPath];
				}
				else
				{
					string text = definition.AssemblyPath;
					if (!Path.IsPathRooted(text))
					{
						text = Path.Combine(this.LocalPath, text);
					}
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CoreTracer, definition.TraceContext, "WorkItemFactory.CreateWorkItem: Loading assembly: {0}", text, null, "CreateWorkItem", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItemFactory.cs", 90);
					assembly = Assembly.LoadFile(text);
				}
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CoreTracer, definition.TraceContext, "WorkItemFactory.CreateWorkItem: Loaded assembly: {0}", definition.AssemblyPath, null, "CreateWorkItem", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItemFactory.cs", 97);
				type = assembly.GetType(definition.TypeName);
				if (type == null)
				{
					string text2 = string.Format("Unable to load class {0}. Reason: Assembly.GetType() returned null when getting the class type.", definition.TypeName);
					WTFDiagnostics.TraceError(ExTraceGlobals.CoreTracer, definition.TraceContext, "WorkItemFactory.CreateWorkItem: " + text2, null, "CreateWorkItem", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItemFactory.cs", 103);
					throw new InvalidOperationException(text2);
				}
				definition.WorkItemType = type;
			}
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CoreTracer, definition.TraceContext, "WorkItemFactory.CreateWorkItem: Creating an instance of the work item: {0}", definition.TypeName, null, "CreateWorkItem", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItemFactory.cs", 111);
			result = (T)((object)Activator.CreateInstance(type, new object[0]));
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CoreTracer, definition.TraceContext, "WorkItemFactory.CreateWorkItem: Created an instance of: {0}", definition.TypeName, null, "CreateWorkItem", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\WorkItemFactory.cs", 114);
			return result;
		}

		private static Dictionary<string, Assembly> defaultAssemblies = new Dictionary<string, Assembly>();
	}
}
