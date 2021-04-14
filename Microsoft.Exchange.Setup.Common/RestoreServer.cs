using System;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RestoreServer
	{
		public void RestoreServerOnPrereqFailure()
		{
			string path = "RestoreServerOnPrereqFailure.ps1";
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string text = Path.Combine(directoryName, path);
			SetupLogger.Log(new LocalizedString("Exchange Server installation failed during prereq check. Trying to restore the server state back to active."));
			SetupLogger.Log(new LocalizedString("RestoreServer Script Path: " + text));
			try
			{
				this.RunScript(text);
			}
			catch (Exception e)
			{
				SetupLogger.LogError(e);
				throw;
			}
		}

		private void RunScript(string scriptPath)
		{
			string scriptContents = string.Empty;
			if (File.Exists(scriptPath))
			{
				scriptContents = File.ReadAllText(scriptPath);
				RunspaceConfiguration runspaceConfiguration = MonadRunspaceConfiguration.Create();
				using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
				{
					runspace.Open();
					Pipeline pipeline = runspace.CreatePipeline();
					pipeline.Commands.AddScript(scriptContents);
					pipeline.Invoke();
					runspace.Close();
				}
				return;
			}
			throw new FileNotFoundException("Couldn't find file: " + scriptPath);
		}
	}
}
