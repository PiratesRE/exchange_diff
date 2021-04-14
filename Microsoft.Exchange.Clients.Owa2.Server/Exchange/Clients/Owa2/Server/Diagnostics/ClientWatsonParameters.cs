using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class ClientWatsonParameters : DisposeTrackableBase, IClientWatsonParameters
	{
		public ClientWatsonParameters(string owaVersion)
		{
			this.symbolsFolderPath = AppConfigLoader.GetConfigStringValue("ClientWatsonSymbolsFolderPath", Path.Combine(ExchangeSetupContext.InstallPath, string.Format("ClientAccess\\Owa\\{0}\\ScriptSymbols", ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(owaVersion))));
			this.maxNumberOfWatsonsPerError = AppConfigLoader.GetConfigIntValue("ClientWatsonMaxReportsPerError", 1, int.MaxValue, 5);
			TimeSpan configTimeSpanValue = AppConfigLoader.GetConfigTimeSpanValue("ClientWatsonResetErrorCountInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromHours(1.0));
			this.resetErrorsReportedTimer = new Timer(delegate(object param0)
			{
				this.clientErrorsReported = new ConcurrentDictionary<int, int>();
			}, null, configTimeSpanValue, configTimeSpanValue);
			Version installedVersion = ExchangeSetupContext.InstalledVersion;
			this.ExchangeSourcesPath = string.Format("\\\\exsrc\\SOURCES\\ALL\\{0:D2}.{1:D2}.{2:D4}.{3:D3}\\", new object[]
			{
				installedVersion.Major,
				installedVersion.Minor,
				installedVersion.Build,
				installedVersion.Revision
			});
			this.owaVersion = owaVersion;
		}

		public IConsolidationSymbolsMap ConsolidationSymbolsMap
		{
			get
			{
				this.AssureSymbolsAreLoaded();
				return this.bootSlabSymbolsMap;
			}
		}

		public IJavaScriptSymbolsMap<AjaxMinSymbolForJavaScript> MinificationSymbolsMapForJavaScript
		{
			get
			{
				this.AssureSymbolsAreLoaded();
				return this.minificationSymbolsMapForJavaScript;
			}
		}

		public IJavaScriptSymbolsMap<AjaxMinSymbolForScriptSharp> MinificationSymbolsMapForScriptSharp
		{
			get
			{
				this.AssureSymbolsAreLoaded();
				return this.minificationSymbolsMapForScriptSharp;
			}
		}

		public IJavaScriptSymbolsMap<ScriptSharpSymbolWrapper> ObfuscationSymbolsMap
		{
			get
			{
				this.AssureSymbolsAreLoaded();
				return this.obfuscationSymbolsMap;
			}
		}

		public SendClientWatsonReportAction ReportAction
		{
			get
			{
				return new SendClientWatsonReportAction(ExWatson.SendClientWatsonReport);
			}
		}

		public string OwaVersion
		{
			get
			{
				return this.owaVersion;
			}
		}

		public string ExchangeSourcesPath { get; private set; }

		private void AssureSymbolsAreLoaded()
		{
			if (this.isLoaded)
			{
				return;
			}
			lock (this.loadLock)
			{
				if (!this.isLoaded)
				{
					Exception ex = null;
					try
					{
						this.LoadSymbols();
					}
					catch (IOException ex2)
					{
						ex = ex2;
					}
					catch (UnauthorizedAccessException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						OwaServerLogger.AppendToLog(SymbolMapLoadLogEvent.CreateForError(ex));
					}
					this.isLoaded = true;
				}
			}
		}

		public bool IsErrorOverReportQuota(int hashCode)
		{
			int num = this.clientErrorsReported.AddOrUpdate(hashCode, 1, (int key, int oldValue) => oldValue + 1);
			return num > this.maxNumberOfWatsonsPerError;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ClientWatsonParameters>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && !base.IsDisposed)
			{
				this.resetErrorsReportedTimer.Dispose();
			}
		}

		private void LoadSymbols()
		{
			this.bootSlabSymbolsMap = new ConsolidationSymbolsMap(this.symbolsFolderPath, this.OwaVersion);
			string[] files = Directory.GetFiles(this.symbolsFolderPath, "*_obfuscate.xml");
			ScriptSharpSourceMapLoader scriptSharpSourceMapLoader = new ScriptSharpSourceMapLoader(files);
			this.obfuscationSymbolsMap = scriptSharpSourceMapLoader.Load();
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string[] files2 = Directory.GetFiles(this.symbolsFolderPath, "**_minify.xml");
			int length = "*_minify.xml".Length;
			foreach (string text in files2)
			{
				string fileName = Path.GetFileName(text);
				string scriptName = fileName.Substring(0, fileName.Length - length + 1);
				if (this.obfuscationSymbolsMap.HasSymbolsLoadedForScript(scriptName))
				{
					list.Add(text);
				}
				else
				{
					list2.Add(text);
				}
			}
			AjaxMinSourceMapLoader<AjaxMinSymbolForJavaScript> ajaxMinSourceMapLoader = new AjaxMinSourceMapLoader<AjaxMinSymbolForJavaScript>(list2, new AjaxMinSymbolParserForJavaScript());
			AjaxMinSourceMapLoader<AjaxMinSymbolForScriptSharp> ajaxMinSourceMapLoader2 = new AjaxMinSourceMapLoader<AjaxMinSymbolForScriptSharp>(list, new AjaxMinSymbolParserForScriptSharp());
			this.minificationSymbolsMapForJavaScript = ajaxMinSourceMapLoader.Load();
			this.minificationSymbolsMapForScriptSharp = ajaxMinSourceMapLoader2.Load();
		}

		private readonly int maxNumberOfWatsonsPerError;

		private readonly Timer resetErrorsReportedTimer;

		private readonly string symbolsFolderPath;

		private ConcurrentDictionary<int, int> clientErrorsReported = new ConcurrentDictionary<int, int>();

		private readonly object loadLock = new object();

		private readonly string owaVersion;

		private volatile bool isLoaded;

		public IConsolidationSymbolsMap bootSlabSymbolsMap;

		public IJavaScriptSymbolsMap<AjaxMinSymbolForJavaScript> minificationSymbolsMapForJavaScript;

		public IJavaScriptSymbolsMap<AjaxMinSymbolForScriptSharp> minificationSymbolsMapForScriptSharp;

		public IJavaScriptSymbolsMap<ScriptSharpSymbolWrapper> obfuscationSymbolsMap;
	}
}
