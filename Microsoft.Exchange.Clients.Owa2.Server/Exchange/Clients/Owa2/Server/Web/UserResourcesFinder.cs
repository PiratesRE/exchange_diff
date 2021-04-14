using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class UserResourcesFinder
	{
		internal static Slab CreateUserBootSlab(SlabManifestType manifestType, LayoutType layout, string owaVersion)
		{
			BootSlabDefinition bootSlabDefinition = SlabManifestCollectionFactory.GetInstance(owaVersion).GetSlabManifest(manifestType, layout).GetBootSlabDefinition();
			UserContext userContext = (manifestType == SlabManifestType.Anonymous || manifestType == SlabManifestType.GenericMail) ? null : UserContextManager.GetUserContext(HttpContext.Current);
			string[] enabledFeatures = UserResourcesFinder.GetEnabledFeatures(manifestType, userContext);
			return bootSlabDefinition.GetSlab(enabledFeatures, layout);
		}

		internal static ResourceBase[] GetNonThemedUserDataEmbededLinks(Slab bootSlab, string owaVersion)
		{
			int num = (bootSlab.PackagedStrings.Length > 0) ? bootSlab.PackagedStrings.Length : bootSlab.Strings.Length;
			ResourceBase[] array = new ResourceBase[num + 1];
			if (bootSlab.PackagedStrings.Any<SlabStringFile>())
			{
				for (int i = 0; i < bootSlab.PackagedStrings.Length; i++)
				{
					array[i] = new LocalizedStringsScriptResource(bootSlab.PackagedStrings[i].Name, ResourceTarget.Any, owaVersion);
				}
			}
			else
			{
				for (int j = 0; j < bootSlab.Strings.Length; j++)
				{
					array[j] = new LocalizedStringsScriptResource(bootSlab.Strings[j].Name, ResourceTarget.Any, owaVersion);
				}
			}
			array[array.Length - 1] = new GlobalizeCultureScriptResource(ResourceTarget.Any, owaVersion);
			return array;
		}

		public static ResourceBase[] GetUserDataEmbeddedLinks(Slab bootSlab, string owaVersion)
		{
			IEnumerable<ResourceBase> nonThemedUserDataEmbededLinks = UserResourcesFinder.GetNonThemedUserDataEmbededLinks(bootSlab, owaVersion);
			ThemeStyleResource[] userDataEmbededStylesLinks = UserResourcesFinder.GetUserDataEmbededStylesLinks(bootSlab, owaVersion);
			return nonThemedUserDataEmbededLinks.Union(userDataEmbededStylesLinks).ToArray<ResourceBase>();
		}

		public static string GetResourcesHash(ResourceBase[] resources, IPageContext context, bool bootResources, string owaVersion)
		{
			List<byte[]> list = new List<byte[]>();
			foreach (ResourceBase resourceBase in resources)
			{
				list.Add(Encoding.ASCII.GetBytes(resourceBase.GetResourcePath(context, bootResources).Replace(owaVersion, string.Empty)));
			}
			return Convert.ToBase64String(AppCacheManifestHandlerBase.CalculateHashOnHashes(list));
		}

		public static ThemeStyleResource[] GetUserDataEmbededStylesLinks(Slab bootSlab, string owaVersion)
		{
			List<ThemeStyleResource> list = new List<ThemeStyleResource>();
			foreach (SlabStyleFile style in bootSlab.Styles)
			{
				list.Add(ThemeStyleResource.FromSlabStyle(style, owaVersion, ThemeManagerFactory.GetInstance(owaVersion).ShouldSkipThemeFolder));
			}
			return list.ToArray();
		}

		public static ResourceBase[] GetUserDataEmbeddedLinks(SlabManifestType manifestType, LayoutType layout, string owaVersion)
		{
			Slab bootSlab = UserResourcesFinder.CreateUserBootSlab(manifestType, layout, owaVersion);
			return UserResourcesFinder.GetUserDataEmbeddedLinks(bootSlab, owaVersion);
		}

		public static string GetEnabledFlightedFeaturesJsonArray(SlabManifestType type, IUserContext userContext, FlightedFeatureScope scope)
		{
			HashSet<string> source = new HashSet<string>();
			if (userContext != null)
			{
				source = UserResourcesFinder.GetEnabledFlightedFeatures(type, userContext, scope);
			}
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(string[]));
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, source.ToArray<string>());
				memoryStream.Close();
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		private static string[] GetEnabledFeatures(SlabManifestType type, UserContext userContext)
		{
			if (type == SlabManifestType.GenericMail || type == SlabManifestType.Anonymous || userContext == null)
			{
				return new string[0];
			}
			return userContext.FeaturesManager.GetClientEnabledFeatures();
		}

		private static HashSet<string> GetEnabledFlightedFeatures(SlabManifestType type, IUserContext userContext, FlightedFeatureScope scope)
		{
			if (type == SlabManifestType.GenericMail || type == SlabManifestType.Anonymous)
			{
				return new HashSet<string>();
			}
			return userContext.FeaturesManager.GetEnabledFlightedFeatures(scope);
		}
	}
}
