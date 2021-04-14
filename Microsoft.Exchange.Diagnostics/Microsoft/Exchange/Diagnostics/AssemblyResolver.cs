using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class AssemblyResolver : MarshalByRefObject
	{
		protected AssemblyResolver()
		{
			this.Name = base.GetType().Name;
			this.ErrorTracer = delegate(string error)
			{
			};
		}

		public event Action<object, AssemblyResolver.ResolutionCompletedEventArgs> ResolutionCompleted;

		public Func<string, bool> FileNameFilter { get; set; }

		public Action<string> ErrorTracer { get; set; }

		public string Name { get; set; }

		public static AssemblyResolver[] Install(params AssemblyResolver[] resolvers)
		{
			foreach (AssemblyResolver assemblyResolver in resolvers)
			{
				assemblyResolver.Install();
			}
			return resolvers;
		}

		public static void Uninstall(params AssemblyResolver[] resolvers)
		{
			foreach (AssemblyResolver assemblyResolver in resolvers)
			{
				assemblyResolver.Uninstall();
			}
		}

		public static bool ExchangePrefixedAssembliesOnly(string fileName)
		{
			return fileName.StartsWith("Microsoft.Exchange.", StringComparison.OrdinalIgnoreCase);
		}

		public void Install()
		{
			AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += this.CurrentDomain_ReflectionOnlyAssemblyResolve;
		}

		public void Uninstall()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= this.CurrentDomain_ReflectionOnlyAssemblyResolve;
		}

		internal string TestTryResolve(AssemblyName nameToResolve, out int filePathsTried, out int assembliesTried)
		{
			int localFilePathsTried = 0;
			int localAssembliesTried = 0;
			Assembly assembly = this.TryResolveAssembly(nameToResolve, new Func<string, Assembly>(Assembly.LoadFile), delegate(string filePath)
			{
				localFilePathsTried++;
			}, delegate(string filePath)
			{
				localAssembliesTried++;
			});
			filePathsTried = localFilePathsTried;
			assembliesTried = localAssembliesTried;
			if (!(assembly != null))
			{
				return null;
			}
			return assembly.Location;
		}

		protected internal static bool CanLoadAssembly(AssemblyName reference, AssemblyName definition)
		{
			AssemblyResolver.VerifyNameOfARealAssembly(definition);
			return definition != null && AssemblyResolver.NameEqualityComparer.CompareNames(reference, definition) && (reference.GetPublicKeyToken() == null || AssemblyResolver.NameEqualityComparer.ComparePublicKeyTokens(reference, definition)) && (reference.CultureInfo == null || AssemblyResolver.NameEqualityComparer.IsCompatibleCulture(reference, definition, true)) && (reference.Version == null || AssemblyResolver.NameEqualityComparer.CompareVersions(reference, definition));
		}

		protected static string GetAssemblyFileNameFromFullName(AssemblyName fullName)
		{
			return fullName.Name;
		}

		protected abstract IEnumerable<string> GetCandidateAssemblyPaths(AssemblyName assemblyFullName);

		protected IEnumerable<string> FilterDirectoryPaths(IEnumerable<string> paths)
		{
			if (paths != null)
			{
				foreach (string path in paths)
				{
					if (!string.IsNullOrEmpty(path))
					{
						if (!Directory.Exists(path))
						{
							this.ErrorTracer(string.Format("{0}: assembly lookup path {1} does not exist", this.Name, path));
						}
						else
						{
							yield return path;
						}
					}
				}
			}
			yield break;
		}

		protected IEnumerable<string> FindAssembly(string directoryPath, string fileName, bool recurse)
		{
			yield return Path.Combine(directoryPath, fileName + ".dll");
			yield return Path.Combine(directoryPath, fileName + ".exe");
			if (recurse)
			{
				string[] subdirs = null;
				try
				{
					subdirs = Directory.GetDirectories(directoryPath);
				}
				catch (DirectoryNotFoundException)
				{
				}
				catch (UnauthorizedAccessException arg)
				{
					this.ErrorTracer(string.Format("{0}: swallowing exception {1} inside {2}::FindAssembly", this.Name, arg, base.GetType().Name));
				}
				foreach (string path in (subdirs ?? new string[0]).SelectMany((string subdir) => this.FindAssembly(subdir, fileName, recurse)))
				{
					yield return path;
				}
			}
			yield break;
		}

		private static bool AlwaysTrue(string v)
		{
			return true;
		}

		private static IEnumerable<X> InvokeObserver<X>(IEnumerable<X> input, Action<X> observer)
		{
			if (observer == null)
			{
				return input;
			}
			return input.Select(delegate(X x)
			{
				observer(x);
				return x;
			});
		}

		private static void VerifyNameOfARealAssembly(AssemblyName name)
		{
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return this.TryHandleAssemblyResolution(args, false);
		}

		private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
		{
			return this.TryHandleAssemblyResolution(args, true);
		}

		private Assembly TryHandleAssemblyResolution(ResolveEventArgs args, bool reflectionOnlyContext)
		{
			if (Thread.GetData(this.inThisResolver) == null)
			{
				Assembly assembly = null;
				try
				{
					Thread.SetData(this.inThisResolver, true);
					assembly = this.TryResolveAssembly(new AssemblyName(args.Name), reflectionOnlyContext ? new Func<string, Assembly>(Assembly.ReflectionOnlyLoadFrom) : new Func<string, Assembly>(Assembly.LoadFile), null, null);
				}
				finally
				{
					Thread.SetData(this.inThisResolver, null);
					Action<object, AssemblyResolver.ResolutionCompletedEventArgs> resolutionCompleted = this.ResolutionCompleted;
					if (resolutionCompleted != null)
					{
						resolutionCompleted(null, new AssemblyResolver.ResolutionCompletedEventArgs(assembly, args, reflectionOnlyContext));
					}
				}
				return assembly;
			}
			return null;
		}

		private T TryDoAssemblyOperation<T>(string arg, Func<string, T> operation)
		{
			Exception arg2;
			try
			{
				return operation(arg);
			}
			catch (ArgumentException ex)
			{
				arg2 = ex;
			}
			catch (SecurityException ex2)
			{
				arg2 = ex2;
			}
			catch (PathTooLongException ex3)
			{
				arg2 = ex3;
			}
			catch (FileLoadException ex4)
			{
				arg2 = ex4;
			}
			catch (BadImageFormatException ex5)
			{
				arg2 = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				arg2 = ex6;
			}
			this.ErrorTracer(string.Format("{0}: swallowing exception {1} inside Task::AssemblyResolveEventHandler", this.Name, arg2));
			return default(T);
		}

		private AssemblyName TryGetAssemblyName(string filePath)
		{
			return this.TryDoAssemblyOperation<AssemblyName>(filePath, new Func<string, AssemblyName>(AssemblyName.GetAssemblyName));
		}

		private Assembly TryResolveAssembly(AssemblyName nameToResolve, Func<string, Assembly> loader, Action<string> candidatePathObserver, Action<string> testedAssemblyObserver)
		{
			string assemblyFileNameFromFullName = AssemblyResolver.GetAssemblyFileNameFromFullName(nameToResolve);
			if ((this.FileNameFilter ?? new Func<string, bool>(AssemblyResolver.AlwaysTrue))(assemblyFileNameFromFullName))
			{
				IEnumerable<string> input = AssemblyResolver.InvokeObserver<string>(this.GetCandidateAssemblyPaths(nameToResolve), candidatePathObserver).Where(new Func<string, bool>(File.Exists));
				return (from filePath in AssemblyResolver.InvokeObserver<string>(input, testedAssemblyObserver)
				where AssemblyResolver.CanLoadAssembly(nameToResolve, this.TryGetAssemblyName(filePath))
				select filePath into assemblyFilePath
				select this.TryDoAssemblyOperation<Assembly>(assemblyFilePath, loader)).FirstOrDefault<Assembly>();
			}
			return null;
		}

		private LocalDataStoreSlot inThisResolver = Thread.AllocateDataSlot();

		public class ResolutionCompletedEventArgs : EventArgs
		{
			public ResolutionCompletedEventArgs(Assembly assembly, ResolveEventArgs resolutionEventArgs, bool reflectionOnlyContext)
			{
				this.Assembly = assembly;
				this.ResolutionEventArgs = resolutionEventArgs;
				this.ReflectionOnlyContext = reflectionOnlyContext;
			}

			public Assembly Assembly { get; private set; }

			public ResolveEventArgs ResolutionEventArgs { get; private set; }

			public bool ReflectionOnlyContext { get; private set; }

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.ResolutionEventArgs.Name);
				PropertyInfo property = this.ResolutionEventArgs.GetType().GetProperty("RequestingAssembly");
				Assembly assembly = null;
				if (property != null)
				{
					assembly = (Assembly)property.GetValue(this.ResolutionEventArgs, null);
				}
				if (assembly != null)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("\t(by {0})", assembly);
				}
				if (this.ReflectionOnlyContext)
				{
					stringBuilder.Append(" for reflection only");
				}
				stringBuilder.AppendLine();
				stringBuilder.Append("\t=> ");
				stringBuilder.Append((this.Assembly != null) ? this.Assembly.Location : "FAILED");
				return stringBuilder.ToString();
			}
		}

		protected internal class NameEqualityComparer : IEqualityComparer<AssemblyName>
		{
			private NameEqualityComparer(bool honorPublicKeyToken, bool honorCultureInfo, bool honorVersion)
			{
				this.honorPublicKeyToken = honorPublicKeyToken;
				this.honorCultureInfo = honorCultureInfo;
				this.honorVersion = honorVersion;
			}

			public static bool CompareNames(AssemblyName x, AssemblyName y)
			{
				return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
			}

			public static bool CompareVersions(AssemblyName x, AssemblyName y)
			{
				return object.Equals(x.Version, y.Version);
			}

			public static bool CompareCulture(AssemblyName x, AssemblyName y)
			{
				return AssemblyResolver.NameEqualityComparer.IsCompatibleCulture(x, y, false);
			}

			public static bool IsCompatibleCulture(AssemblyName requested, AssemblyName actual, bool fallbackToParent)
			{
				CultureInfo cultureInfo = requested.CultureInfo;
				while (!object.Equals(cultureInfo, actual.CultureInfo))
				{
					if (!fallbackToParent || cultureInfo == cultureInfo.Parent)
					{
						return false;
					}
					cultureInfo = cultureInfo.Parent;
				}
				return true;
			}

			public static bool ComparePublicKeyTokens(AssemblyName x, AssemblyName y)
			{
				byte[] publicKeyToken = x.GetPublicKeyToken();
				byte[] publicKeyToken2 = y.GetPublicKeyToken();
				return publicKeyToken == publicKeyToken2 || (publicKeyToken != null && publicKeyToken2 != null && publicKeyToken.Length == publicKeyToken2.Length && (publicKeyToken.Length == 0 || (publicKeyToken.Length == 8 && BitConverter.ToUInt64(publicKeyToken, 0) == BitConverter.ToUInt64(publicKeyToken2, 0))));
			}

			public bool Equals(AssemblyName x, AssemblyName y)
			{
				return x == y || (x != null && y != null && AssemblyResolver.NameEqualityComparer.CompareNames(x, y) && (!this.honorPublicKeyToken || AssemblyResolver.NameEqualityComparer.ComparePublicKeyTokens(x, y)) && (!this.honorCultureInfo || AssemblyResolver.NameEqualityComparer.CompareCulture(x, y)) && (!this.honorVersion || AssemblyResolver.NameEqualityComparer.CompareVersions(x, y)));
			}

			public int GetHashCode(AssemblyName obj)
			{
				if (obj != null)
				{
					return obj.Name.GetHashCode();
				}
				return 0;
			}

			public static readonly AssemblyResolver.NameEqualityComparer Full = new AssemblyResolver.NameEqualityComparer(true, true, true);

			public static readonly AssemblyResolver.NameEqualityComparer Identity = new AssemblyResolver.NameEqualityComparer(true, true, false);

			public static readonly AssemblyResolver.NameEqualityComparer Partial = new AssemblyResolver.NameEqualityComparer(false, false, false);

			private readonly bool honorPublicKeyToken;

			private readonly bool honorCultureInfo;

			private readonly bool honorVersion;
		}
	}
}
