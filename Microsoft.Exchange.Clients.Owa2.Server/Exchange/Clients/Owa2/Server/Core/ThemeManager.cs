using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class ThemeManager
	{
		public ThemeManager(string owaVersion)
		{
			this.owaVersion = owaVersion;
			this.storageIdToIdMap = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
			string path = Path.Combine(new string[]
			{
				FolderConfiguration.Instance.RootPath,
				ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(owaVersion),
				"resources",
				"styles",
				"0"
			});
			this.shouldSkipThemeFolder = Directory.Exists(path);
		}

		public Theme[] Themes
		{
			get
			{
				if (this.themes == null)
				{
					this.LoadThemeFiles(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.owaVersion));
				}
				return this.themes;
			}
		}

		public Theme BaseTheme
		{
			get
			{
				if (this.baseTheme == null)
				{
					this.LoadThemeFiles(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.owaVersion));
				}
				return this.baseTheme;
			}
		}

		public bool ShouldSkipThemeFolder
		{
			get
			{
				return this.shouldSkipThemeFolder;
			}
		}

		internal uint GetIdFromStorageId(string storageId)
		{
			if (storageId == null)
			{
				throw new ArgumentNullException("storageId");
			}
			uint maxValue = uint.MaxValue;
			if (this.storageIdToIdMap.ContainsKey(storageId))
			{
				this.storageIdToIdMap.TryGetValue(storageId, out maxValue);
			}
			return maxValue;
		}

		internal string GetThemeFolderName(UserAgent agent, HttpContext httpContext)
		{
			Theme theme = this.BaseTheme;
			if (agent != null && httpContext != null && !UserAgentUtilities.IsMonitoringRequest(agent.RawString) && agent.Layout == LayoutType.Mouse && !Globals.IsAnonymousCalendarApp)
			{
				UserContext userContext = UserContextManager.GetUserContext(httpContext);
				if (userContext != null)
				{
					theme = userContext.Theme;
				}
			}
			return theme.FolderName;
		}

		public Theme GetDefaultTheme(string defaultThemeStorageId)
		{
			Theme result;
			if (string.IsNullOrEmpty(defaultThemeStorageId))
			{
				result = this.baseTheme;
			}
			else
			{
				uint idFromStorageId = this.GetIdFromStorageId(defaultThemeStorageId);
				if (idFromStorageId == 4294967295U)
				{
					result = this.baseTheme;
				}
				else
				{
					result = this.themes[(int)((UIntPtr)idFromStorageId)];
				}
			}
			return result;
		}

		private void LoadThemeFiles(string resourcesDiskRelativeFolderPath)
		{
			ExTraceGlobals.ThemesCallTracer.TraceDebug(0L, "LoadThemeFiles");
			string text = Path.Combine(FolderConfiguration.Instance.RootPath, resourcesDiskRelativeFolderPath, ThemeManager.themesFolderSubPath);
			if (!Directory.Exists(text))
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_NoThemesFolder, string.Empty, new object[]
				{
					text
				});
				throw new OwaThemeManagerInitializationException("Themes folder not found ('" + text + "')");
			}
			string[] directories = Directory.GetDirectories(text);
			List<Theme> list = new List<Theme>();
			foreach (string text2 in directories)
			{
				Path.GetFileNameWithoutExtension(text2);
				ExTraceGlobals.ThemesTracer.TraceDebug<string>(0L, "Inspecting theme folder '{0}'", text2);
				Theme theme = Theme.Create(text2);
				if (theme == null)
				{
					ExTraceGlobals.ThemesTracer.TraceWarning<string>(0L, "Theme folder '{0}' is invalid", text2);
				}
				else
				{
					list.Add(theme);
					if (theme.IsBase)
					{
						this.baseTheme = theme;
					}
					if (this.storageIdToIdMap.ContainsKey(theme.StorageId))
					{
						throw new OwaThemeManagerInitializationException(string.Format("Duplicated theme found (folder name={0})", theme.FolderName));
					}
					this.storageIdToIdMap.Add(theme.StorageId, uint.MaxValue);
					ExTraceGlobals.ThemesTracer.TraceDebug<string>(0L, "Successfully added theme. Name={0}", theme.DisplayName);
				}
			}
			list.Sort();
			this.themes = list.ToArray();
			for (int j = 0; j < this.themes.Length; j++)
			{
				this.storageIdToIdMap[this.themes[j].StorageId] = (uint)j;
			}
			if (this.baseTheme == null)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_NoBaseTheme, string.Empty, new object[]
				{
					ThemeManager.BaseThemeFolderName
				});
				throw new OwaThemeManagerInitializationException(string.Format("Couldn't find a base theme (folder name={0})", ThemeManager.BaseThemeFolderName));
			}
		}

		public static readonly string BaseThemeFolderName = "base";

		private static readonly string themesFolderSubPath = "resources\\themes";

		private readonly bool shouldSkipThemeFolder;

		private readonly string owaVersion;

		private Theme baseTheme;

		private Theme[] themes;

		private Dictionary<string, uint> storageIdToIdMap;
	}
}
