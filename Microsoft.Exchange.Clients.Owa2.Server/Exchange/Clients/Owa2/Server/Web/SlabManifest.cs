using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabManifest
	{
		public SlabManifest(BootSlabDefinition bootSlabDefinition, IDictionary<string, SlabDefinition> slabDefinitions)
		{
			if (bootSlabDefinition == null)
			{
				throw new ArgumentNullException("bootSlabDefinition");
			}
			if (slabDefinitions == null)
			{
				throw new ArgumentNullException("slabDefinitions");
			}
			this.bootSlabDefinition = bootSlabDefinition;
			this.slabDefinitions = slabDefinitions;
			this.slabDefinitions.Add("boot", bootSlabDefinition);
		}

		public SlabManifest(IDictionary<string, SlabDefinition> slabDefinitions)
		{
			if (slabDefinitions == null)
			{
				throw new ArgumentNullException("slabDefinitions");
			}
			this.slabDefinitions = slabDefinitions;
		}

		public SlabManifestType Type
		{
			get
			{
				return this.slabManifestType;
			}
			set
			{
				this.slabManifestType = value;
			}
		}

		public IDictionary<string, SlabDefinition> SlabDefinitions
		{
			get
			{
				return this.slabDefinitions;
			}
		}

		public BootSlabDefinition GetBootSlabDefinition()
		{
			if (this.bootSlabDefinition == null)
			{
				throw new SlabManifestFormatException("Manifest does not have a boot slab");
			}
			return this.bootSlabDefinition;
		}

		public string GetSlabsJson(string[] features, LayoutType layout)
		{
			IDictionary<string, Slab> slabs = this.GetSlabs(features, layout);
			return this.GetSlabsJson(slabs);
		}

		public string GetSlabsJson(IDictionary<string, Slab> slabs)
		{
			DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
			{
				UseSimpleDictionaryFormat = true
			};
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, Slab>), settings);
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, slabs);
				memoryStream.Close();
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		public IDictionary<string, Slab> GetSlabs(string[] enabledFeatureNames, LayoutType layout)
		{
			Dictionary<string, Slab> dictionary = new Dictionary<string, Slab>();
			Dictionary<string, IEnumerable<string>> dictionary2 = new Dictionary<string, IEnumerable<string>>();
			foreach (KeyValuePair<string, SlabDefinition> keyValuePair in this.SlabDefinitions)
			{
				string key = keyValuePair.Key;
				if (this.Type != SlabManifestType.SharedHoverCard || !key.Equals("editor"))
				{
					SlabDefinition value = keyValuePair.Value;
					string[] array = enabledFeatureNames.Where(new Func<string, bool>(value.HasBinding)).ToArray<string>();
					IEnumerable<string> enumerable = null;
					if (array.Any<string>())
					{
						dictionary[key] = value.GetSlab(array, layout, out enumerable);
					}
					else if (value.HasDefaultBinding())
					{
						Slab defaultSlab = value.GetDefaultSlab(layout, out enumerable);
						if (this.Type == SlabManifestType.SharedHoverCard && defaultSlab.Dependencies.Contains("editor"))
						{
							IEnumerable<string> second = new string[]
							{
								"editor"
							};
							defaultSlab.Dependencies = enumerable.Except(second).ToArray<string>();
						}
						dictionary[key] = defaultSlab;
					}
					if (enumerable != null)
					{
						if (this.Type == SlabManifestType.SharedHoverCard && enumerable.Contains("editor"))
						{
							IEnumerable<string> second2 = new string[]
							{
								"editor"
							};
							enumerable = enumerable.Except(second2).ToArray<string>();
						}
						dictionary2[key] = enumerable;
					}
				}
			}
			this.IncludeSlabsDependencies(dictionary, dictionary2, enabledFeatureNames, layout);
			return dictionary;
		}

		private void IncludeSlabsDependencies(IDictionary<string, Slab> includedSlabs, Dictionary<string, IEnumerable<string>> slabNameToDependencies, string[] enabledFeatureNames, LayoutType layout)
		{
			if (!slabNameToDependencies.Any<KeyValuePair<string, IEnumerable<string>>>())
			{
				return;
			}
			IEnumerable<string> source = from slabNameToDependency in slabNameToDependencies.Values
			from dependency in slabNameToDependency
			where !includedSlabs.ContainsKey(dependency)
			select dependency;
			ILookup<string, string> lookup = source.ToLookup((string s) => s);
			Dictionary<string, IEnumerable<string>> dictionary = new Dictionary<string, IEnumerable<string>>();
			IEnumerable<KeyValuePair<string, SlabDefinition>> enumerable = from kv in this.SlabDefinitions
			where lookup.Contains(kv.Key)
			select kv;
			foreach (KeyValuePair<string, SlabDefinition> keyValuePair in enumerable)
			{
				string key = keyValuePair.Key;
				SlabDefinition value = keyValuePair.Value;
				string[] array = enabledFeatureNames.Where(new Func<string, bool>(value.HasBinding)).ToArray<string>();
				IEnumerable<string> enumerable2;
				if (array.Any<string>())
				{
					includedSlabs[key] = value.GetSlab(array, layout, out enumerable2);
				}
				else if (value.HasDefaultBinding())
				{
					includedSlabs[key] = value.GetDefaultSlab(layout, out enumerable2);
				}
				else
				{
					string text = value.GetFeatures().First<string>();
					includedSlabs[key] = value.GetSlab(new string[]
					{
						text
					}, layout, out enumerable2);
				}
				if (enumerable2 != null)
				{
					dictionary[key] = enumerable2;
				}
			}
			this.IncludeSlabsDependencies(includedSlabs, dictionary, enabledFeatureNames, layout);
		}

		public const string BootSlabName = "boot";

		private readonly BootSlabDefinition bootSlabDefinition;

		private readonly IDictionary<string, SlabDefinition> slabDefinitions;

		private SlabManifestType slabManifestType;
	}
}
