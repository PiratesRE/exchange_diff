using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PicwDynamicLoader
	{
		private static Assembly PicwAssembly
		{
			get
			{
				if (!PicwDynamicLoader.isAssemblyLoaded)
				{
					try
					{
						PicwDynamicLoader.assemblyInstance = Assembly.Load("Microsoft.Exchange.Inference.PeopleICommunicateWith");
					}
					catch (FileNotFoundException)
					{
						PicwDynamicLoader.assemblyInstance = null;
					}
					catch (Exception ex)
					{
						PicwDynamicLoader.assemblyInstance = null;
						PicwDynamicLoader.Tracer.TraceDebug<string>(0L, "Failed to load PICW assembly due to exception: '{0}'", ex.Message);
					}
					PicwDynamicLoader.isAssemblyLoaded = true;
				}
				return PicwDynamicLoader.assemblyInstance;
			}
		}

		private static Type LoadType(string typeName)
		{
			Type result;
			if (PicwDynamicLoader.PicwAssembly == null)
			{
				result = null;
			}
			else
			{
				try
				{
					result = PicwDynamicLoader.PicwAssembly.GetType(string.Join(".", new string[]
					{
						"Microsoft.Exchange.Inference.PeopleICommunicateWith",
						typeName
					}), true);
				}
				catch (Exception)
				{
					result = null;
				}
			}
			return result;
		}

		public static object CreateInstance(string typeName, params object[] paramList)
		{
			Type type = PicwDynamicLoader.LoadType(typeName);
			object result;
			if (type == null)
			{
				result = null;
			}
			else
			{
				try
				{
					result = Activator.CreateInstance(type, paramList);
				}
				catch (Exception)
				{
					result = null;
				}
			}
			return result;
		}

		private const string AssemblyName = "Microsoft.Exchange.Inference.PeopleICommunicateWith";

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private static bool isAssemblyLoaded = false;

		private static Assembly assemblyInstance;
	}
}
