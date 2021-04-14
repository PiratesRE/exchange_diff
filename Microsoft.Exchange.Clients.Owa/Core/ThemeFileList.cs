using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class ThemeFileList
	{
		internal static int Add(string themeFileName, bool useCssSprites)
		{
			if (!ThemeFileList.idTable.ContainsKey(themeFileName))
			{
				ThemeFileList.ThemeFile item = new ThemeFileList.ThemeFile((ThemeFileId)ThemeFileList.nameTable.Count, themeFileName, useCssSprites);
				ThemeFileList.idTable[themeFileName] = ThemeFileList.nameTable.Count;
				ThemeFileList.nameTable.Add(item);
			}
			return ThemeFileList.idTable[themeFileName];
		}

		internal static int GetIdFromName(string themeFileName)
		{
			int result = 0;
			ThemeFileList.idTable.TryGetValue(themeFileName, out result);
			return result;
		}

		internal static string GetNameFromId(ThemeFileId themeFileId)
		{
			return ThemeFileList.nameTable[(int)themeFileId].Name;
		}

		internal static string GetClassNameFromId(int themeFileIndex)
		{
			return ThemeFileList.nameTable[themeFileIndex].ClassName;
		}

		internal static bool GetPhaseIIFromId(int themeFileIndex)
		{
			return ThemeFileList.nameTable[themeFileIndex].PhaseII;
		}

		internal static string GetNameFromId(int themeFileIndex)
		{
			return ThemeFileList.nameTable[themeFileIndex].Name;
		}

		internal static bool CanUseCssSprites(ThemeFileId themeFileId)
		{
			return ThemeFileList.CanUseCssSprites((int)themeFileId);
		}

		internal static bool CanUseCssSprites(int themeFileIndex)
		{
			return ThemeFileList.nameTable[themeFileIndex].UseCssSprites;
		}

		internal static bool IsResourceFile(ThemeFileId themeFileId)
		{
			return ThemeFileList.IsResourceFile((int)themeFileId);
		}

		internal static bool IsResourceFile(int themeFileIndex)
		{
			return ThemeFileList.nameTable[themeFileIndex].IsResource;
		}

		internal static int Count
		{
			get
			{
				return ThemeFileList.idTable.Count;
			}
		}

		private static bool Initialize()
		{
			ThemeFileList.nameTable = new List<ThemeFileList.ThemeFile>(601);
			ThemeFileList.idTable = new Dictionary<string, int>(601);
			Type typeFromHandle = typeof(ThemeFileId);
			FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
			foreach (FieldInfo fieldInfo in fields)
			{
				ThemeFileId themeFileId = (ThemeFileId)fieldInfo.GetValue(null);
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ThemeFileInfoAttribute), false);
				if (customAttributes == null || customAttributes.Length == 0)
				{
					ExTraceGlobals.CoreTracer.TraceError<ThemeFileId>(0L, "{0} doesn't define ThemeFileInfoAttribute", themeFileId);
				}
				else
				{
					ThemeFileInfoAttribute themeFileInfoAttribute = (ThemeFileInfoAttribute)customAttributes[0];
					ThemeFileList.idTable[themeFileInfoAttribute.Name] = (int)themeFileId;
					ThemeFileList.nameTable.Add(new ThemeFileList.ThemeFile(themeFileId, themeFileInfoAttribute.Name, themeFileInfoAttribute.UseCssSprites, themeFileInfoAttribute.IsResource, themeFileInfoAttribute.PhaseII));
				}
			}
			return true;
		}

		private static List<ThemeFileList.ThemeFile> nameTable;

		private static Dictionary<string, int> idTable;

		private static bool isInitialized = ThemeFileList.Initialize();

		private struct ThemeFile
		{
			public ThemeFile(ThemeFileId id, string name)
			{
				this = new ThemeFileList.ThemeFile(id, name, true);
			}

			public ThemeFile(ThemeFileId id, string name, bool useCssSprites)
			{
				this = new ThemeFileList.ThemeFile(id, name, useCssSprites, false);
			}

			public ThemeFile(ThemeFileId id, string name, bool useCssSprites, bool isResource)
			{
				this = new ThemeFileList.ThemeFile(id, name, useCssSprites, isResource, false);
			}

			public ThemeFile(ThemeFileId id, string name, bool useCssSprites, bool isResource, bool phaseII)
			{
				this.Id = id;
				this.Name = name;
				this.ClassName = (useCssSprites ? ("sprites-" + name.Replace(".", "-")) : string.Empty);
				this.UseCssSprites = useCssSprites;
				this.IsResource = isResource;
				this.PhaseII = phaseII;
			}

			private const string SpritesClassPrefix = "sprites-";

			public ThemeFileId Id;

			public string Name;

			public string ClassName;

			public bool UseCssSprites;

			public bool IsResource;

			public bool PhaseII;
		}
	}
}
