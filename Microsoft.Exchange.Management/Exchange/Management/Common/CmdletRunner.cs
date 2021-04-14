using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace Microsoft.Exchange.Management.Common
{
	internal class CmdletRunner
	{
		public CmdletRunner(HashSet<string> allowedCommands, Dictionary<string, HashSet<string>> requiredParameters = null, Dictionary<string, HashSet<string>> notAllowedParameters = null)
		{
			if (allowedCommands == null)
			{
				throw new ArgumentNullException(this.NameOf<HashSet<string>>(() => allowedCommands));
			}
			foreach (string text in allowedCommands)
			{
				if (!CmdletRunner.AllAllowedCommands.Contains(text))
				{
					throw new CmdletExecutionException(string.Format("Command passed {0} is not a valid command", text));
				}
			}
			this.allowedCommands = new HashSet<string>(allowedCommands, StringComparer.OrdinalIgnoreCase);
			this.cmdletValidator = new CmdletValidator(allowedCommands, requiredParameters, notAllowedParameters);
		}

		public IEnumerable<PSObject> RunCmdlet(string command, CommandParameterCollection parameters, bool throwExceptionOnError = true)
		{
			if (this.cmdletValidator.Validate(command, parameters))
			{
				return this.ExecuteCmdlet(command, parameters, throwExceptionOnError);
			}
			if (throwExceptionOnError)
			{
				throw new CmdletExecutionException(string.Format("Command passed {0} failed validation", command));
			}
			return new List<PSObject>();
		}

		public IEnumerable<PSObject> RunCmdlet(string cmdlet, bool throwExceptionOnError = true)
		{
			ScriptParseResult scriptParseResult = this.cmdletValidator.ParseCmdletScript(cmdlet);
			if (scriptParseResult.IsSuccessful)
			{
				return this.ExecuteCmdlet(scriptParseResult.Command, scriptParseResult.Parameters, throwExceptionOnError);
			}
			if (throwExceptionOnError)
			{
				throw new CmdletExecutionException(string.Format("Command passed {0} failed validation", cmdlet));
			}
			return new List<PSObject>();
		}

		private IEnumerable<PSObject> ExecuteCmdlet(string command, CommandParameterCollection parameters, bool throwExceptionOnError)
		{
			if (!this.allowedCommands.Contains(command))
			{
				if (throwExceptionOnError)
				{
					throw new CmdletExecutionException(string.Format("Command passed {0} is not allowed", command));
				}
				return new List<PSObject>();
			}
			else
			{
				PSLanguageMode languageMode = Runspace.DefaultRunspace.SessionStateProxy.LanguageMode;
				if (languageMode != PSLanguageMode.NoLanguage)
				{
					Runspace.DefaultRunspace.SessionStateProxy.LanguageMode = PSLanguageMode.NoLanguage;
				}
				List<PSObject> list = new List<PSObject>();
				StringBuilder stringBuilder = new StringBuilder();
				try
				{
					using (Pipeline pipeline = Runspace.DefaultRunspace.CreateNestedPipeline())
					{
						Command command2 = new Command(command);
						if (parameters != null)
						{
							foreach (CommandParameter item in parameters)
							{
								command2.Parameters.Add(item);
							}
						}
						pipeline.Commands.Add(command2);
						IEnumerable<PSObject> collection = pipeline.Invoke();
						list.AddRange(collection);
						IEnumerable<object> enumerable = pipeline.Error.ReadToEnd();
						if (enumerable.Any<object>())
						{
							stringBuilder.AppendLine(command);
							foreach (object obj in enumerable)
							{
								stringBuilder.AppendLine(obj.ToString());
							}
						}
					}
				}
				finally
				{
					Runspace.DefaultRunspace.SessionStateProxy.LanguageMode = languageMode;
				}
				if (stringBuilder.Length > 0 && throwExceptionOnError)
				{
					throw new CmdletExecutionException(stringBuilder.ToString());
				}
				return list;
			}
		}

		private string NameOf<T>(Expression<Func<T>> memberExpression)
		{
			MemberExpression memberExpression2 = (MemberExpression)memberExpression.Body;
			return memberExpression2.Member.Name;
		}

		private static readonly HashSet<string> AllAllowedCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"New-TransportRule"
		};

		private readonly HashSet<string> allowedCommands;

		private readonly CmdletValidator cmdletValidator;
	}
}
