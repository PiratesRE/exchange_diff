using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class SystemTrace
	{
		protected SystemTrace(string assemblyName)
		{
			this.assemblyName = assemblyName;
		}

		public SourceLevels SourceLevels
		{
			get
			{
				return this.sourceLevels;
			}
			set
			{
				this.sourceLevels = value;
				this.SafeUpdate();
			}
		}

		protected static bool GetFieldValue(FieldInfo field)
		{
			return field != null && (bool)field.GetValue(null);
		}

		protected static void SetFieldValue(FieldInfo field, object value)
		{
			if (field != null)
			{
				field.SetValue(null, value);
			}
		}

		protected static T GetPropertyValue<T>(PropertyInfo property)
		{
			if (property != null)
			{
				return (T)((object)property.GetValue(null, null));
			}
			return default(T);
		}

		protected static void SetPropertyValue(PropertyInfo property, object value)
		{
			if (property != null)
			{
				property.SetValue(null, value, null);
			}
		}

		protected static Assembly GetAssembly(string assemblyName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					if (SystemTrace.IsMatchingAssemblyName(assembly, assemblyName))
					{
						return assembly;
					}
				}
			}
			return null;
		}

		protected abstract bool Initialize(Assembly assembly);

		protected abstract void Update();

		protected void ConnectListener(TraceSource source, TraceListener listener, bool connect)
		{
			if (source != null && listener != null)
			{
				if (connect)
				{
					if (!source.Listeners.Contains(listener))
					{
						source.Listeners.Add(listener);
						return;
					}
				}
				else if (source.Listeners.Contains(listener))
				{
					source.Listeners.Remove(listener);
				}
			}
		}

		protected void Initialize()
		{
			Assembly assembly = this.GetAssembly();
			if (assembly != null)
			{
				this.SafeInitialize(assembly);
				return;
			}
			AppDomain.CurrentDomain.AssemblyLoad += this.AssemblyLoadHandler;
		}

		protected void SafeUpdate()
		{
			if (this.initialized)
			{
				Exception ex = null;
				try
				{
					lock (this)
					{
						this.Update();
					}
				}
				catch (InvalidCastException ex2)
				{
					ex = ex2;
				}
				catch (TargetInvocationException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					this.ReportFailure(ex);
				}
			}
		}

		private static bool IsMatchingAssemblyName(Assembly assembly, string assemblyName)
		{
			AssemblyName name = assembly.GetName();
			return name != null && StringComparer.OrdinalIgnoreCase.Equals(name.Name, assemblyName);
		}

		private void AssemblyLoadHandler(object sender, AssemblyLoadEventArgs args)
		{
			if (SystemTrace.IsMatchingAssemblyName(args.LoadedAssembly, this.assemblyName))
			{
				AppDomain.CurrentDomain.AssemblyLoad -= this.AssemblyLoadHandler;
				this.SafeInitialize(args.LoadedAssembly);
				this.Update();
			}
		}

		private Assembly GetAssembly()
		{
			return SystemTrace.GetAssembly(this.assemblyName);
		}

		private void SafeInitialize(Assembly assembly)
		{
			Exception ex = null;
			try
			{
				this.initialized = this.Initialize(assembly);
			}
			catch (MemberAccessException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentException ex3)
			{
				ex = ex3;
			}
			catch (TargetParameterCountException ex4)
			{
				ex = ex4;
			}
			catch (NotSupportedException ex5)
			{
				ex = ex5;
			}
			catch (InvalidCastException ex6)
			{
				ex = ex6;
			}
			catch (TargetInvocationException ex7)
			{
				ex = ex7;
			}
			if (ex != null)
			{
				this.ReportFailure(ex);
				this.initialized = false;
			}
		}

		private void ReportFailure(Exception exception)
		{
			ExTraceInternal.Trace<string, Exception>(0, TraceType.ErrorTrace, CommonTags.guid, 5, 0L, "Failed to setup trace capture from '{0}' component in the framework due exception: {1}", this.assemblyName, exception);
		}

		private string assemblyName;

		private bool initialized;

		private SourceLevels sourceLevels;
	}
}
