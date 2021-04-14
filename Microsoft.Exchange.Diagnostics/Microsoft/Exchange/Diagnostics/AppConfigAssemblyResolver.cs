using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AppConfigAssemblyResolver : AssemblyResolver
	{
		public AppConfigAssemblyResolver(string appConfigFilePath)
		{
			IEnumerable<KeyValuePair<AssemblyName, string>> source = XDocument.Load(appConfigFilePath).Elements("configuration").Elements("runtime").Elements(XName.Get("assemblyBinding", "urn:schemas-microsoft-com:asm.v1")).Elements(XName.Get("dependentAssembly", "urn:schemas-microsoft-com:asm.v1")).Select(delegate(XElement asm)
			{
				IEnumerable<XElement> source2 = asm.Elements(XName.Get("assemblyIdentity", "urn:schemas-microsoft-com:asm.v1"));
				AssemblyName assemblyName = new AssemblyName
				{
					Name = (string)source2.Attributes("name").SingleOrDefault<XAttribute>(),
					CultureInfo = source2.Attributes("culture").Select(new Func<XAttribute, CultureInfo>(this.ParseCultureAttribute)).SingleOrDefault<CultureInfo>()
				};
				assemblyName.SetPublicKeyToken(source2.Attributes("publicKeyToken").Select(new Func<XAttribute, byte[]>(this.ParsePublicKeyToken)).SingleOrDefault<byte[]>());
				return new KeyValuePair<AssemblyName, string>(assemblyName, (from name in asm.Elements(XName.Get("codeBase", "urn:schemas-microsoft-com:asm.v1")).Attributes("href")
				select name.Value.Replace("file:///", string.Empty)).FirstOrDefault<string>());
			});
			this.assemblyNameToFilePaths = (from nameToFileName in source
			where !string.IsNullOrEmpty(nameToFileName.Key.Name) && !string.IsNullOrEmpty(nameToFileName.Value)
			select nameToFileName).ToLookup((KeyValuePair<AssemblyName, string> mapping) => mapping.Key, (KeyValuePair<AssemblyName, string> pair) => pair.Value, AssemblyResolver.NameEqualityComparer.Identity);
		}

		protected override IEnumerable<string> GetCandidateAssemblyPaths(AssemblyName nameToResolve)
		{
			return this.assemblyNameToFilePaths[nameToResolve];
		}

		private CultureInfo ParseCultureAttribute(XAttribute cultureAttribute)
		{
			string text = (string)cultureAttribute;
			if (text == null || !(text != "neutral"))
			{
				return null;
			}
			return CultureInfo.GetCultureInfo(text);
		}

		private byte[] ParsePublicKeyToken(XAttribute binHexPublicKeyTokenAttribute)
		{
			string text = (string)binHexPublicKeyTokenAttribute;
			if (text == null || text.Length % 2 != 0)
			{
				throw new SettingsPropertyWrongTypeException("publicKeyToken");
			}
			byte[] array = new byte[text.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				if (!byte.TryParse(text.Substring(i * 2, 2), NumberStyles.HexNumber, null, out array[i]))
				{
					throw new SettingsPropertyWrongTypeException("publicKeyToken");
				}
			}
			return array;
		}

		private ILookup<AssemblyName, string> assemblyNameToFilePaths;
	}
}
