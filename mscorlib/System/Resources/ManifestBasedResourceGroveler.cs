using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System.Resources
{
	internal class ManifestBasedResourceGroveler : IResourceGroveler
	{
		public ManifestBasedResourceGroveler(ResourceManager.ResourceManagerMediator mediator)
		{
			this._mediator = mediator;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public ResourceSet GrovelForResourceSet(CultureInfo culture, Dictionary<string, ResourceSet> localResourceSets, bool tryParents, bool createIfNotExists, ref StackCrawlMark stackMark)
		{
			ResourceSet resourceSet = null;
			Stream stream = null;
			RuntimeAssembly runtimeAssembly = null;
			CultureInfo cultureInfo = this.UltimateFallbackFixup(culture);
			if (cultureInfo.HasInvariantCultureName && this._mediator.FallbackLoc == UltimateResourceFallbackLocation.MainAssembly)
			{
				runtimeAssembly = this._mediator.MainAssembly;
			}
			else if (!cultureInfo.HasInvariantCultureName && !this._mediator.TryLookingForSatellite(cultureInfo))
			{
				runtimeAssembly = null;
			}
			else
			{
				runtimeAssembly = this.GetSatelliteAssembly(cultureInfo, ref stackMark);
				if (runtimeAssembly == null)
				{
					bool flag = culture.HasInvariantCultureName && this._mediator.FallbackLoc == UltimateResourceFallbackLocation.Satellite;
					if (flag)
					{
						this.HandleSatelliteMissing();
					}
				}
			}
			string resourceFileName = this._mediator.GetResourceFileName(cultureInfo);
			if (runtimeAssembly != null)
			{
				lock (localResourceSets)
				{
					if (localResourceSets.TryGetValue(culture.Name, out resourceSet) && FrameworkEventSource.IsInitialized)
					{
						FrameworkEventSource.Log.ResourceManagerFoundResourceSetInCacheUnexpected(this._mediator.BaseName, this._mediator.MainAssembly, culture.Name);
					}
				}
				stream = this.GetManifestResourceStream(runtimeAssembly, resourceFileName, ref stackMark);
			}
			if (FrameworkEventSource.IsInitialized)
			{
				if (stream != null)
				{
					FrameworkEventSource.Log.ResourceManagerStreamFound(this._mediator.BaseName, this._mediator.MainAssembly, culture.Name, runtimeAssembly, resourceFileName);
				}
				else
				{
					FrameworkEventSource.Log.ResourceManagerStreamNotFound(this._mediator.BaseName, this._mediator.MainAssembly, culture.Name, runtimeAssembly, resourceFileName);
				}
			}
			if (createIfNotExists && stream != null && resourceSet == null)
			{
				if (FrameworkEventSource.IsInitialized)
				{
					FrameworkEventSource.Log.ResourceManagerCreatingResourceSet(this._mediator.BaseName, this._mediator.MainAssembly, culture.Name, resourceFileName);
				}
				resourceSet = this.CreateResourceSet(stream, runtimeAssembly);
			}
			else if (stream == null && tryParents)
			{
				bool hasInvariantCultureName = culture.HasInvariantCultureName;
				if (hasInvariantCultureName)
				{
					this.HandleResourceStreamMissing(resourceFileName);
				}
			}
			if (!createIfNotExists && stream != null && resourceSet == null && FrameworkEventSource.IsInitialized)
			{
				FrameworkEventSource.Log.ResourceManagerNotCreatingResourceSet(this._mediator.BaseName, this._mediator.MainAssembly, culture.Name);
			}
			return resourceSet;
		}

		public bool HasNeutralResources(CultureInfo culture, string defaultResName)
		{
			string value = defaultResName;
			if (this._mediator.LocationInfo != null && this._mediator.LocationInfo.Namespace != null)
			{
				value = this._mediator.LocationInfo.Namespace + Type.Delimiter.ToString() + defaultResName;
			}
			string[] manifestResourceNames = this._mediator.MainAssembly.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (text.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		private CultureInfo UltimateFallbackFixup(CultureInfo lookForCulture)
		{
			CultureInfo result = lookForCulture;
			if (lookForCulture.Name == this._mediator.NeutralResourcesCulture.Name && this._mediator.FallbackLoc == UltimateResourceFallbackLocation.MainAssembly)
			{
				if (FrameworkEventSource.IsInitialized)
				{
					FrameworkEventSource.Log.ResourceManagerNeutralResourcesSufficient(this._mediator.BaseName, this._mediator.MainAssembly, lookForCulture.Name);
				}
				result = CultureInfo.InvariantCulture;
			}
			else if (lookForCulture.HasInvariantCultureName && this._mediator.FallbackLoc == UltimateResourceFallbackLocation.Satellite)
			{
				result = this._mediator.NeutralResourcesCulture;
			}
			return result;
		}

		[SecurityCritical]
		internal static CultureInfo GetNeutralResourcesLanguage(Assembly a, ref UltimateResourceFallbackLocation fallbackLocation)
		{
			string text = null;
			short num = 0;
			if (!ManifestBasedResourceGroveler.GetNeutralResourcesLanguageAttribute(((RuntimeAssembly)a).GetNativeHandle(), JitHelpers.GetStringHandleOnStack(ref text), out num))
			{
				if (FrameworkEventSource.IsInitialized)
				{
					FrameworkEventSource.Log.ResourceManagerNeutralResourceAttributeMissing(a);
				}
				fallbackLocation = UltimateResourceFallbackLocation.MainAssembly;
				return CultureInfo.InvariantCulture;
			}
			if (num < 0 || num > 1)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_InvalidNeutralResourcesLanguage_FallbackLoc", new object[]
				{
					num
				}));
			}
			fallbackLocation = (UltimateResourceFallbackLocation)num;
			CultureInfo result;
			try
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(text);
				result = cultureInfo;
			}
			catch (ArgumentException innerException)
			{
				if (!(a == typeof(object).Assembly))
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_InvalidNeutralResourcesLanguage_Asm_Culture", new object[]
					{
						a.ToString(),
						text
					}), innerException);
				}
				result = CultureInfo.InvariantCulture;
			}
			return result;
		}

		[SecurityCritical]
		internal ResourceSet CreateResourceSet(Stream store, Assembly assembly)
		{
			if (store.CanSeek && store.Length > 4L)
			{
				long position = store.Position;
				BinaryReader binaryReader = new BinaryReader(store);
				int num = binaryReader.ReadInt32();
				if (num == ResourceManager.MagicNumber)
				{
					int num2 = binaryReader.ReadInt32();
					string text;
					string text2;
					if (num2 == ResourceManager.HeaderVersionNumber)
					{
						binaryReader.ReadInt32();
						text = binaryReader.ReadString();
						text2 = binaryReader.ReadString();
					}
					else
					{
						if (num2 <= ResourceManager.HeaderVersionNumber)
						{
							throw new NotSupportedException(Environment.GetResourceString("NotSupported_ObsoleteResourcesFile", new object[]
							{
								this._mediator.MainAssembly.GetSimpleName()
							}));
						}
						int num3 = binaryReader.ReadInt32();
						long offset = binaryReader.BaseStream.Position + (long)num3;
						text = binaryReader.ReadString();
						text2 = binaryReader.ReadString();
						binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
					}
					store.Position = position;
					if (this.CanUseDefaultResourceClasses(text, text2))
					{
						return new RuntimeResourceSet(store);
					}
					Type type = Type.GetType(text, true);
					IResourceReader resourceReader = (IResourceReader)Activator.CreateInstance(type, new object[]
					{
						store
					});
					object[] args = new object[]
					{
						resourceReader
					};
					Type type2;
					if (this._mediator.UserResourceSet == null)
					{
						type2 = Type.GetType(text2, true, false);
					}
					else
					{
						type2 = this._mediator.UserResourceSet;
					}
					return (ResourceSet)Activator.CreateInstance(type2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, args, null, null);
				}
				else
				{
					store.Position = position;
				}
			}
			if (this._mediator.UserResourceSet == null)
			{
				return new RuntimeResourceSet(store);
			}
			object[] args2 = new object[]
			{
				store,
				assembly
			};
			ResourceSet result;
			try
			{
				try
				{
					return (ResourceSet)Activator.CreateInstance(this._mediator.UserResourceSet, args2);
				}
				catch (MissingMethodException)
				{
				}
				args2 = new object[]
				{
					store
				};
				ResourceSet resourceSet = (ResourceSet)Activator.CreateInstance(this._mediator.UserResourceSet, args2);
				result = resourceSet;
			}
			catch (MissingMethodException innerException)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResMgrBadResSet_Type", new object[]
				{
					this._mediator.UserResourceSet.AssemblyQualifiedName
				}), innerException);
			}
			return result;
		}

		[SecurityCritical]
		private Stream GetManifestResourceStream(RuntimeAssembly satellite, string fileName, ref StackCrawlMark stackMark)
		{
			bool skipSecurityCheck = this._mediator.MainAssembly == satellite && this._mediator.CallingAssembly == this._mediator.MainAssembly;
			Stream stream = satellite.GetManifestResourceStream(this._mediator.LocationInfo, fileName, skipSecurityCheck, ref stackMark);
			if (stream == null)
			{
				stream = this.CaseInsensitiveManifestResourceStreamLookup(satellite, fileName);
			}
			return stream;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private Stream CaseInsensitiveManifestResourceStreamLookup(RuntimeAssembly satellite, string name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this._mediator.LocationInfo != null)
			{
				string @namespace = this._mediator.LocationInfo.Namespace;
				if (@namespace != null)
				{
					stringBuilder.Append(@namespace);
					if (name != null)
					{
						stringBuilder.Append(Type.Delimiter);
					}
				}
			}
			stringBuilder.Append(name);
			string text = stringBuilder.ToString();
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			string text2 = null;
			foreach (string text3 in satellite.GetManifestResourceNames())
			{
				if (compareInfo.Compare(text3, text, CompareOptions.IgnoreCase) == 0)
				{
					if (text2 != null)
					{
						throw new MissingManifestResourceException(Environment.GetResourceString("MissingManifestResource_MultipleBlobs", new object[]
						{
							text,
							satellite.ToString()
						}));
					}
					text2 = text3;
				}
			}
			if (FrameworkEventSource.IsInitialized)
			{
				if (text2 != null)
				{
					FrameworkEventSource.Log.ResourceManagerCaseInsensitiveResourceStreamLookupSucceeded(this._mediator.BaseName, this._mediator.MainAssembly, satellite.GetSimpleName(), text);
				}
				else
				{
					FrameworkEventSource.Log.ResourceManagerCaseInsensitiveResourceStreamLookupFailed(this._mediator.BaseName, this._mediator.MainAssembly, satellite.GetSimpleName(), text);
				}
			}
			if (text2 == null)
			{
				return null;
			}
			bool skipSecurityCheck = this._mediator.MainAssembly == satellite && this._mediator.CallingAssembly == this._mediator.MainAssembly;
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			Stream manifestResourceStream = satellite.GetManifestResourceStream(text2, ref stackCrawlMark, skipSecurityCheck);
			if (manifestResourceStream != null && FrameworkEventSource.IsInitialized)
			{
				FrameworkEventSource.Log.ResourceManagerManifestResourceAccessDenied(this._mediator.BaseName, this._mediator.MainAssembly, satellite.GetSimpleName(), text2);
			}
			return manifestResourceStream;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private RuntimeAssembly GetSatelliteAssembly(CultureInfo lookForCulture, ref StackCrawlMark stackMark)
		{
			if (!this._mediator.LookedForSatelliteContractVersion)
			{
				this._mediator.SatelliteContractVersion = this._mediator.ObtainSatelliteContractVersion(this._mediator.MainAssembly);
				this._mediator.LookedForSatelliteContractVersion = true;
			}
			RuntimeAssembly runtimeAssembly = null;
			string satelliteAssemblyName = this.GetSatelliteAssemblyName();
			try
			{
				runtimeAssembly = this._mediator.MainAssembly.InternalGetSatelliteAssembly(satelliteAssemblyName, lookForCulture, this._mediator.SatelliteContractVersion, false, ref stackMark);
			}
			catch (FileLoadException ex)
			{
				int hresult = ex._HResult;
				Win32Native.MakeHRFromErrorCode(5);
			}
			catch (BadImageFormatException ex2)
			{
			}
			if (FrameworkEventSource.IsInitialized)
			{
				if (runtimeAssembly != null)
				{
					FrameworkEventSource.Log.ResourceManagerGetSatelliteAssemblySucceeded(this._mediator.BaseName, this._mediator.MainAssembly, lookForCulture.Name, satelliteAssemblyName);
				}
				else
				{
					FrameworkEventSource.Log.ResourceManagerGetSatelliteAssemblyFailed(this._mediator.BaseName, this._mediator.MainAssembly, lookForCulture.Name, satelliteAssemblyName);
				}
			}
			return runtimeAssembly;
		}

		private bool CanUseDefaultResourceClasses(string readerTypeName, string resSetTypeName)
		{
			if (this._mediator.UserResourceSet != null)
			{
				return false;
			}
			AssemblyName asmName = new AssemblyName(ResourceManager.MscorlibName);
			return (readerTypeName == null || ResourceManager.CompareNames(readerTypeName, ResourceManager.ResReaderTypeName, asmName)) && (resSetTypeName == null || ResourceManager.CompareNames(resSetTypeName, ResourceManager.ResSetTypeName, asmName));
		}

		[SecurityCritical]
		private string GetSatelliteAssemblyName()
		{
			string simpleName = this._mediator.MainAssembly.GetSimpleName();
			return simpleName + ".resources";
		}

		[SecurityCritical]
		private void HandleSatelliteMissing()
		{
			string text = this._mediator.MainAssembly.GetSimpleName() + ".resources.dll";
			if (this._mediator.SatelliteContractVersion != null)
			{
				text = text + ", Version=" + this._mediator.SatelliteContractVersion.ToString();
			}
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.SetPublicKey(this._mediator.MainAssembly.GetPublicKey());
			byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
			int num = publicKeyToken.Length;
			StringBuilder stringBuilder = new StringBuilder(num * 2);
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(publicKeyToken[i].ToString("x", CultureInfo.InvariantCulture));
			}
			text = text + ", PublicKeyToken=" + stringBuilder;
			string text2 = this._mediator.NeutralResourcesCulture.Name;
			if (text2.Length == 0)
			{
				text2 = "<invariant>";
			}
			throw new MissingSatelliteAssemblyException(Environment.GetResourceString("MissingSatelliteAssembly_Culture_Name", new object[]
			{
				this._mediator.NeutralResourcesCulture,
				text
			}), text2);
		}

		[SecurityCritical]
		private void HandleResourceStreamMissing(string fileName)
		{
			if (this._mediator.MainAssembly == typeof(object).Assembly && this._mediator.BaseName.Equals("mscorlib"))
			{
				string message = "mscorlib.resources couldn't be found!  Large parts of the BCL won't work!";
				Environment.FailFast(message);
			}
			string text = string.Empty;
			if (this._mediator.LocationInfo != null && this._mediator.LocationInfo.Namespace != null)
			{
				text = this._mediator.LocationInfo.Namespace + Type.Delimiter.ToString();
			}
			text += fileName;
			throw new MissingManifestResourceException(Environment.GetResourceString("MissingManifestResource_NoNeutralAsm", new object[]
			{
				text,
				this._mediator.MainAssembly.GetSimpleName()
			}));
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNeutralResourcesLanguageAttribute(RuntimeAssembly assemblyHandle, StringHandleOnStack cultureName, out short fallbackLocation);

		private ResourceManager.ResourceManagerMediator _mediator;
	}
}
