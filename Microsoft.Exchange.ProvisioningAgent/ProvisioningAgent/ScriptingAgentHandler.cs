using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class ScriptingAgentHandler : ProvisioningHandlerBase, IDisposable
	{
		internal ScriptingAgentHandler(ScriptingAgentConfiguration agentConfiguration)
		{
			if (agentConfiguration == null)
			{
				throw new ArgumentNullException("agentConfiguration");
			}
			this.agentConfiguration = agentConfiguration;
			this.monadConnection = new MonadConnection("pooled=false");
			this.monadConnection.Open();
		}

		public override IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable)
		{
			IConfigurable result;
			try
			{
				string scriptForAPI = this.GetScriptForAPI(this.agentConfiguration.ProvisionDefaultPropertiesDictionary, "$provisioningHandler = $args[1];\r\n$readOnlyIConfigurable = $args[3];");
				if (scriptForAPI == null)
				{
					result = null;
				}
				else
				{
					base.LogMessage(Strings.InvokedProvisionDefaultProperties);
					MonadCommand monadCommand = new MonadCommand(scriptForAPI, this.monadConnection);
					monadCommand.Parameters.AddWithValue("provisioningHandler", this);
					monadCommand.Parameters.AddWithValue("readOnlyIConfigurable", readOnlyIConfigurable);
					monadCommand.CommandType = CommandType.Text;
					base.LogMessage(Strings.ExecutingScript(monadCommand.CommandText));
					object[] array = monadCommand.Execute();
					base.LogMessage(Strings.ScriptReturned(array.ToString()));
					if (array == null || array.Length == 0)
					{
						result = null;
					}
					else
					{
						result = (array[0] as IConfigurable);
					}
				}
			}
			catch (Exception ex)
			{
				Exception innerException = ex.InnerException;
				if (innerException != null)
				{
					throw new ProvisioningException(Strings.ErrorInProvisionDefaultProperties(innerException.Message), innerException);
				}
				throw;
			}
			return result;
		}

		public override bool UpdateAffectedIConfigurable(IConfigurable writableIConfigurable)
		{
			bool result;
			try
			{
				string scriptForAPI = this.GetScriptForAPI(this.agentConfiguration.UpdateAffectedIConfigurableDictionary, "$provisioningHandler = $args[1];\r\n$writableIConfigurable = $args[3];");
				if (scriptForAPI == null)
				{
					result = false;
				}
				else
				{
					base.LogMessage(Strings.InvokedUpdateAffectedIConfigurable);
					MonadCommand monadCommand = new MonadCommand(scriptForAPI, this.monadConnection);
					monadCommand.Parameters.AddWithValue("provisioningHandler", this);
					monadCommand.Parameters.AddWithValue("writableIConfigurable", writableIConfigurable);
					monadCommand.CommandType = CommandType.Text;
					base.LogMessage(Strings.ExecutingScript(monadCommand.CommandText));
					object[] array = monadCommand.Execute();
					base.LogMessage(Strings.ScriptReturned(array.ToString()));
					if (array == null || array.Length == 0)
					{
						throw new ProvisioningException(Strings.ErrorUpdateAffectedIConfigurableBadRetObject);
					}
					result = (bool)array[0];
				}
			}
			catch (Exception ex)
			{
				Exception innerException = ex.InnerException;
				if (innerException != null)
				{
					throw new ProvisioningException(Strings.ErrorInUpdateAffectedIConfigurable(innerException.Message), innerException);
				}
				throw;
			}
			return result;
		}

		public override ProvisioningValidationError[] Validate(IConfigurable readOnlyIConfigurable)
		{
			ProvisioningValidationError[] result;
			try
			{
				string scriptForAPI = this.GetScriptForAPI(this.agentConfiguration.ValidateDictionary, "$provisioningHandler = $args[1];\r\n$readOnlyIConfigurable = $args[3];\r\n");
				if (scriptForAPI == null)
				{
					result = null;
				}
				else
				{
					base.LogMessage(Strings.InvokedValidate);
					MonadCommand monadCommand = new MonadCommand(scriptForAPI, this.monadConnection);
					monadCommand.Parameters.AddWithValue("provisioningHandler", this);
					monadCommand.Parameters.AddWithValue("readOnlyIConfigurable", readOnlyIConfigurable);
					monadCommand.CommandType = CommandType.Text;
					base.LogMessage(Strings.ExecutingScript(monadCommand.CommandText));
					object[] array = monadCommand.Execute();
					base.LogMessage(Strings.ScriptReturned(array.ToString()));
					if (array == null || array.Length == 0)
					{
						result = new ProvisioningValidationError[0];
					}
					else
					{
						ProvisioningValidationError[] array2 = new ProvisioningValidationError[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array2[i] = (array[i] as ProvisioningValidationError);
							if (array2[i] == null)
							{
								throw new ProvisioningException(Strings.ErrorUnexpectedReturnTypeInValidate);
							}
						}
						result = array2;
					}
				}
			}
			catch (Exception ex)
			{
				Exception innerException = ex.InnerException;
				if (innerException != null)
				{
					throw new ProvisioningException(Strings.ErrorInValidate(innerException.Message), innerException);
				}
				throw;
			}
			return result;
		}

		public override void OnComplete(bool succeeded, Exception e)
		{
			try
			{
				string scriptForAPI = this.GetScriptForAPI(this.agentConfiguration.OnCompleteDictionary, "$provisioningHandler = $args[1];\r\n$succeeded = $args[3];\r\n$exception = $args[5];\r\n");
				if (scriptForAPI != null)
				{
					base.LogMessage(Strings.InvokedOnComplete(succeeded.ToString(), (e == null) ? "<null>" : e.Message));
					MonadCommand monadCommand = new MonadCommand(scriptForAPI, this.monadConnection);
					monadCommand.Parameters.AddWithValue("provisioningHandler", this);
					monadCommand.Parameters.AddWithValue("succeeded", succeeded);
					monadCommand.Parameters.AddWithValue("exception", e);
					monadCommand.CommandType = CommandType.Text;
					base.LogMessage(Strings.ExecutingScript(monadCommand.CommandText));
					monadCommand.Execute();
				}
			}
			catch (Exception ex)
			{
				Exception innerException = ex.InnerException;
				if (innerException != null)
				{
					throw new ProvisioningException(Strings.ErrorInOnComplete(innerException.Message), innerException);
				}
				throw;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.monadConnection != null && disposing)
			{
				this.monadConnection.Close();
				this.monadConnection = null;
			}
			GC.SuppressFinalize(this);
		}

		private string GetScriptForAPI(Dictionary<string, string> scriptDictionary, string prefix)
		{
			string text;
			if (!scriptDictionary.TryGetValue(base.TaskName, out text))
			{
				return null;
			}
			if (this.agentConfiguration.CommonFunctions != null && this.agentConfiguration.CommonFunctions.Length > 0)
			{
				text = string.Format("{0}\r\n{1}\r\n{2}", this.agentConfiguration.CommonFunctions, prefix, text);
			}
			else
			{
				text = string.Format("{0}\r\n{1}", prefix, text);
			}
			return ScriptingAgentHandler.UnescapeHelper.Unescape(text);
		}

		private MonadConnection monadConnection;

		private ScriptingAgentConfiguration agentConfiguration;

		private static class UnescapeHelper
		{
			public static string Unescape(string str)
			{
				if (str == null)
				{
					return null;
				}
				StringBuilder stringBuilder = null;
				string text = str;
				for (;;)
				{
					int num = text.IndexOf('&');
					if (num == -1)
					{
						break;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(text.Substring(0, num));
					int startIndex;
					stringBuilder.Append(ScriptingAgentHandler.UnescapeHelper.GetUnescapeSequence(text, num, out startIndex));
					text = text.Substring(startIndex);
				}
				if (stringBuilder == null)
				{
					return str;
				}
				stringBuilder.Append(text);
				return stringBuilder.ToString();
			}

			private static string GetUnescapeSequence(string str, int index, out int newIndex)
			{
				int num = str.Length - index;
				int num2 = ScriptingAgentHandler.UnescapeHelper.s_escapeStringPairs.Length;
				for (int i = 0; i < num2; i += 2)
				{
					string result = ScriptingAgentHandler.UnescapeHelper.s_escapeStringPairs[i];
					string text = ScriptingAgentHandler.UnescapeHelper.s_escapeStringPairs[i + 1];
					int length = text.Length;
					if (length <= num && string.Compare(text, 0, str, index, length, StringComparison.Ordinal) == 0)
					{
						newIndex = index + text.Length;
						return result;
					}
				}
				newIndex = index + 1;
				return str[index].ToString();
			}

			private static readonly string[] s_escapeStringPairs = new string[]
			{
				"<",
				"&lt;",
				">",
				"&gt;",
				"\"",
				"&quot;",
				"'",
				"&apos;",
				"&",
				"&amp;"
			};
		}
	}
}
