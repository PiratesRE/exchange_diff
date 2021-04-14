using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Ceres.CoreServices.Services.Package;

namespace Microsoft.Exchange.Search.Query
{
	internal class DictionaryPackageManager : IPackageManager, IDictionary<string, IPackage>, ICollection<KeyValuePair<string, IPackage>>, IEnumerable<KeyValuePair<string, IPackage>>, IEnumerable
	{
		public DictionaryPackageManager(string[] rootPath)
		{
			this.rootDirectories = rootPath;
		}

		public int Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IDictionary<string, IPackage> Packages
		{
			get
			{
				return this;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ICollection<IPackage> Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IPackage IDictionary<string, IPackage>.this[string key]
		{
			get
			{
				return this.Get(key);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IPackage this[AssemblyName packageName]
		{
			get
			{
				return this.Get(packageName.FullName);
			}
		}

		public IEnumerable<IPackage> GetBundles(Predicate<IPackage> filter)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(Predicate<IPackage> filter)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(Predicate<IPackage> filter, SelectOptions options)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(AssemblyName partialPackageName, Predicate<IPackage> filter)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(AssemblyName partialPackageName, Predicate<IPackage> filter, SelectOptions options)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(string simpleNamePattern, Predicate<IPackage> filter)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(string simpleNamePattern, Predicate<IPackage> filter, SelectOptions options)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> SelectPackages(string simpleNamePattern)
		{
			string bundleFile = simpleNamePattern + ".dll";
			foreach (string rootDirectory in this.rootDirectories)
			{
				if (File.Exists(Path.Combine(rootDirectory, bundleFile)))
				{
					Assembly assembly = Assembly.LoadFrom(Path.Combine(rootDirectory, bundleFile));
					yield return new DictionaryPackageManager.DictionaryPackage(assembly);
				}
			}
			yield break;
		}

		public IEnumerable<IPackage> SelectPackages(string simpleNamePattern, SelectOptions options)
		{
			if (options == 1)
			{
				IPackage pkg;
				this.Packages.TryGetValue(simpleNamePattern, out pkg);
				yield return pkg;
				yield break;
			}
			throw new NotImplementedException();
		}

		public long RegisterDeploymentCallback(Action<DeploymentNotificationAction> callback, Predicate<IPackage> filter)
		{
			throw new NotImplementedException();
		}

		public long RegisterDeploymentCallback(Action<DeploymentNotificationAction> callback, AssemblyName partialPackageName, Predicate<IPackage> filter)
		{
			throw new NotImplementedException();
		}

		public long RegisterDeploymentCallback(Action<DeploymentNotificationAction> callback, string simpleNamePattern, Predicate<IPackage> filter)
		{
			return -1L;
		}

		public bool UnRegisterDeploymentCallback(long deploymentCallbackId)
		{
			return true;
		}

		public void Deploy(AssemblyName name, string path)
		{
			throw new NotImplementedException();
		}

		public void Deploy(AssemblyName name, Stream stream)
		{
			throw new NotImplementedException();
		}

		public void Deploy(IPackage packageWrapper)
		{
			throw new NotImplementedException();
		}

		public void UnDeploy(AssemblyName name)
		{
			throw new NotImplementedException();
		}

		public void UnDeploy(IPackage packageWrapper)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<string, IPackage>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(KeyValuePair<string, IPackage> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<string, IPackage> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<string, IPackage>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<string, IPackage> item)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		public void Add(string key, IPackage value)
		{
			throw new NotImplementedException();
		}

		public bool Remove(string key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out IPackage value)
		{
			value = this.Get(key);
			return true;
		}

		private IPackage Get(string bundleIdFullName)
		{
			string path = bundleIdFullName + ".dll";
			foreach (string path2 in this.rootDirectories)
			{
				string text = Path.Combine(path2, path);
				if (File.Exists(text))
				{
					Assembly assembly = Assembly.LoadFrom(text);
					return new DictionaryPackageManager.DictionaryPackage(assembly);
				}
			}
			throw new ArgumentException(string.Concat(new string[]
			{
				"Bundle: ",
				bundleIdFullName,
				" could not be found (Search Paths: ",
				string.Join(";", this.rootDirectories),
				")"
			}));
		}

		private readonly string[] rootDirectories;

		private class DictionaryPackage : IPackage
		{
			public DictionaryPackage(Assembly assembly)
			{
				this.assembly = assembly;
			}

			public int PropertyCount
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<string> Resources
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<IPackage> ReferencedPackages
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<IPackage> AllReferencedPackages
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<AssemblyName> References
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<IPackage> Dependencies
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<IPackage> AllDependencies
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IEnumerable<AssemblyName> DependencyIdentifiers
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public int DependencyCount
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public AssemblyName Name
			{
				get
				{
					return this.assembly.GetName();
				}
			}

			public string Description
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IDictionary<string, string> Properties
			{
				get
				{
					this.GetPropertiesIfNeeded();
					return this.properties;
				}
			}

			public string this[string propertyName]
			{
				get
				{
					this.GetPropertiesIfNeeded();
					string result;
					this.properties.TryGetValue(propertyName, out result);
					return result;
				}
			}

			public bool HasProperty(string propertyName)
			{
				return this.Properties.ContainsKey(propertyName);
			}

			public bool HasPropertyWithValue(string propertyName, string propertyValue)
			{
				throw new NotImplementedException();
			}

			public Stream OpenResource(string resourceName)
			{
				return this.assembly.GetManifestResourceStream(resourceName);
			}

			public bool HasResource(string resourceName)
			{
				this.GetResourcesIfNeeded();
				return this.resourceNames.Contains(resourceName);
			}

			public void Unpack(string path)
			{
				throw new NotImplementedException();
			}

			public string UnpackedPath()
			{
				throw new NotImplementedException();
			}

			public string UnpackedPath(string resourceName)
			{
				throw new NotImplementedException();
			}

			private void GetResourcesIfNeeded()
			{
				if (this.resourceNames != null)
				{
					return;
				}
				this.resourceNames = this.assembly.GetManifestResourceNames().ToList<string>();
			}

			private void GetPropertiesIfNeeded()
			{
				if (this.properties != null)
				{
					return;
				}
				IDictionary<string, string> dictionary = new Dictionary<string, string>();
				IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(this.assembly);
				foreach (CustomAttributeData customAttributeData in customAttributes)
				{
					if (customAttributeData.Constructor.DeclaringType.FullName.Equals(typeof(PackagePropertyAttribute).FullName))
					{
						string value = (customAttributeData.ConstructorArguments[1].Value == null) ? null : customAttributeData.ConstructorArguments[1].Value.ToString();
						dictionary.Add(customAttributeData.ConstructorArguments[0].Value.ToString(), value);
					}
				}
				this.properties = dictionary;
			}

			private readonly Assembly assembly;

			private IDictionary<string, string> properties;

			private IList<string> resourceNames;
		}
	}
}
