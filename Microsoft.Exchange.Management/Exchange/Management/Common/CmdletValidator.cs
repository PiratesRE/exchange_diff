using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal class CmdletValidator
	{
		public HashSet<string> AllowedCommands
		{
			get
			{
				return this.allowedCommands;
			}
			set
			{
				this.allowedCommands = value;
			}
		}

		public Dictionary<string, HashSet<string>> RequiredParameters
		{
			get
			{
				return this.requiredParameters;
			}
			set
			{
				this.requiredParameters = value;
			}
		}

		public Dictionary<string, HashSet<string>> NotAllowedParameters
		{
			get
			{
				return this.notAllowedParameters;
			}
			set
			{
				this.notAllowedParameters = value;
			}
		}

		public CmdletValidator(HashSet<string> allowedCommands, Dictionary<string, HashSet<string>> requiredParameters = null, Dictionary<string, HashSet<string>> notAllowedParameters = null)
		{
			if (allowedCommands == null)
			{
				throw new ArgumentNullException(this.NameOf<HashSet<string>>(() => allowedCommands));
			}
			this.AllowedCommands = new HashSet<string>(allowedCommands, StringComparer.OrdinalIgnoreCase);
			if (requiredParameters != null)
			{
				this.RequiredParameters = new Dictionary<string, HashSet<string>>(requiredParameters, StringComparer.OrdinalIgnoreCase);
			}
			if (notAllowedParameters != null)
			{
				this.NotAllowedParameters = new Dictionary<string, HashSet<string>>(notAllowedParameters, StringComparer.OrdinalIgnoreCase);
			}
		}

		public ScriptParseResult ParseCmdletScript(string cmdlet)
		{
			ScriptParseResult scriptParseResult = new ScriptParseResult();
			Token[] array = null;
			ParseError[] array2 = null;
			Parser.ParseInput(cmdlet, out array, out array2);
			if (array2 != null && array2.Count<ParseError>() > 0)
			{
				TaskLogger.Log(Strings.CmdletValidatorParseErrors(string.Join(", ", from error in array2
				select error.Message)));
				return scriptParseResult;
			}
			int num = 0;
			if (array == null || array[0].TokenFlags != TokenFlags.CommandName)
			{
				TaskLogger.Log(Strings.CmdletValidatorParseErrors("No valid command specified"));
				return scriptParseResult;
			}
			if (array.Any((Token token) => (token.TokenFlags & TokenFlags.SpecialOperator) != TokenFlags.None || (token.TokenFlags & TokenFlags.TypeName) != TokenFlags.None))
			{
				TaskLogger.Log(Strings.CmdletValidatorParseErrors("Special Operator and types not supported"));
				return scriptParseResult;
			}
			scriptParseResult.Command = array[num].Text;
			if (!this.AllowedCommands.Contains(scriptParseResult.Command))
			{
				TaskLogger.Log(Strings.CmdletValidatorParseErrors(string.Format("Command {0} not allowed", scriptParseResult.Command)));
				return scriptParseResult;
			}
			List<Token> list = null;
			string text = null;
			int num2 = array.Count<Token>();
			try
			{
				while (++num < num2 && array[num].Kind != TokenKind.EndOfInput)
				{
					if (array[num].Kind == TokenKind.Parameter)
					{
						CmdletValidator.AddParameter(text, list, scriptParseResult.Parameters);
						text = array[num].Text;
						list = null;
					}
					else
					{
						if (string.IsNullOrWhiteSpace(text))
						{
							TaskLogger.Log(Strings.CmdletValidatorParseErrors(string.Format("Invalid token {0} of kind {1}, {2} encountered without a parameter", array[num].Text, array[num].Kind, array[num].TokenFlags)));
							return scriptParseResult;
						}
						if (list == null)
						{
							list = new List<Token>();
						}
						list.Add(array[num]);
					}
				}
				CmdletValidator.AddParameter(text, list, scriptParseResult.Parameters);
				scriptParseResult.IsSuccessful = this.Validate(scriptParseResult.Command, scriptParseResult.Parameters);
			}
			catch (InvalidOperationException ex)
			{
				TaskLogger.Log(Strings.CmdletValidatorParseErrors(ex.ToString()));
			}
			return scriptParseResult;
		}

		public bool Validate(string command, CommandParameterCollection parameters)
		{
			if (!this.AllowedCommands.Contains(command))
			{
				TaskLogger.Log(Strings.CmdletValidatorParseErrors(string.Format("Command {0} not allowed", command)));
				return false;
			}
			if (this.NotAllowedParameters != null && this.NotAllowedParameters.ContainsKey(command))
			{
				HashSet<string> hashSet = this.NotAllowedParameters[command];
				foreach (CommandParameter commandParameter in parameters)
				{
					if (hashSet.Contains(commandParameter.Name))
					{
						TaskLogger.Log(Strings.CmdletValidatorParseErrors(string.Format("Invalid parameter {0}", commandParameter.Name)));
						return false;
					}
				}
			}
			if (this.RequiredParameters != null && this.RequiredParameters.ContainsKey(command))
			{
				HashSet<string> hashSet2 = new HashSet<string>(this.RequiredParameters[command]);
				foreach (CommandParameter commandParameter2 in parameters)
				{
					if (hashSet2.Contains(commandParameter2.Name))
					{
						hashSet2.Remove(commandParameter2.Name);
					}
				}
				if (hashSet2.Count > 0)
				{
					TaskLogger.Log(Strings.CmdletValidatorParseErrors(string.Format("Required parameters {0} not specified", string.Join(",", hashSet2))));
					return false;
				}
			}
			return true;
		}

		public static bool IsParameterPresent(string command, string parameterName)
		{
			if (string.IsNullOrWhiteSpace(parameterName))
			{
				throw new ArgumentException("Invalid parameterName passed");
			}
			Token[] array = null;
			ParseError[] array2 = null;
			Parser.ParseInput(command, out array, out array2);
			return array != null && array.Any((Token token) => CmdletValidator.IsTokenParameter(parameterName, token));
		}

		private static bool IsTokenParameter(string parameterName, Token token)
		{
			return token.Kind == TokenKind.Parameter && parameterName.Equals(token.Text, StringComparison.OrdinalIgnoreCase);
		}

		private static void AddParameter(string parameterName, List<Token> parameterTokens, CommandParameterCollection parameters)
		{
			if (string.IsNullOrWhiteSpace(parameterName))
			{
				return;
			}
			if (parameterTokens == null || parameterTokens.Count == 0)
			{
				parameters.Add(new CommandParameter(parameterName));
				return;
			}
			int i = 0;
			object obj = null;
			i = CmdletValidator.GetParameterValue(parameterTokens, i, out obj);
			if (obj != null && i >= parameterTokens.Count)
			{
				parameters.Add(new CommandParameter(parameterName, obj));
				return;
			}
			List<object> list = new List<object>();
			if (obj != null)
			{
				list.Add(obj);
			}
			while (i < parameterTokens.Count)
			{
				i = CmdletValidator.GetParameterValue(parameterTokens, i, out obj);
				if (obj != null)
				{
					list.Add(obj);
				}
			}
			parameters.Add(new CommandParameter(parameterName, list.ToArray()));
		}

		private static int GetParameterValue(List<Token> parameterValueTokens, int tokenIndex, out object parameterValue)
		{
			int count = parameterValueTokens.Count;
			parameterValue = null;
			if (tokenIndex >= count)
			{
				return tokenIndex;
			}
			Token token = parameterValueTokens[tokenIndex];
			TokenKind kind = token.Kind;
			if (kind <= TokenKind.Number)
			{
				if (kind != TokenKind.Variable)
				{
					if (kind == TokenKind.Number)
					{
						parameterValue = ((NumberToken)token).Value;
						goto IL_23B;
					}
				}
				else
				{
					bool flag;
					if (bool.TryParse(((VariableToken)token).Name, out flag))
					{
						parameterValue = flag;
						goto IL_23B;
					}
					parameterValue = token.Text;
					goto IL_23B;
				}
			}
			else
			{
				switch (kind)
				{
				case TokenKind.StringLiteral:
					parameterValue = ((StringToken)token).Value;
					goto IL_23B;
				case TokenKind.StringExpandable:
					parameterValue = ((StringExpandableToken)token).Value;
					goto IL_23B;
				default:
					switch (kind)
					{
					case TokenKind.LBracket:
					case TokenKind.RBracket:
					case TokenKind.Comma:
						parameterValue = null;
						goto IL_23B;
					case TokenKind.AtParen:
					{
						List<object> list = new List<object>();
						tokenIndex++;
						while (tokenIndex < count && parameterValueTokens[tokenIndex].Kind != TokenKind.RParen)
						{
							if (parameterValueTokens[tokenIndex].Kind == TokenKind.Comma)
							{
								tokenIndex++;
							}
							else
							{
								object obj = null;
								tokenIndex = CmdletValidator.GetParameterValue(parameterValueTokens, tokenIndex, out obj);
								if (obj != null)
								{
									list.Add(obj);
								}
							}
						}
						parameterValue = list.ToArray();
						goto IL_23B;
					}
					case TokenKind.AtCurly:
					{
						Dictionary<object, object> dictionary = new Dictionary<object, object>();
						tokenIndex++;
						while (tokenIndex < count && parameterValueTokens[tokenIndex].Kind != TokenKind.RCurly)
						{
							if (parameterValueTokens[tokenIndex].Kind == TokenKind.Semi)
							{
								tokenIndex++;
							}
							else
							{
								object obj2 = null;
								tokenIndex = CmdletValidator.GetParameterValue(parameterValueTokens, tokenIndex, out obj2);
								if (tokenIndex >= count || parameterValueTokens[tokenIndex].Kind != TokenKind.Equals)
								{
									throw new InvalidOperationException(string.Format("Cmdlet parsing failed at index {0}. Expected Equals. Actual {1}", tokenIndex, parameterValueTokens[tokenIndex].Text));
								}
								if (++tokenIndex >= count)
								{
									throw new InvalidOperationException(string.Format("Cmdlet parsing failed. No value for key {0}", obj2));
								}
								object value = null;
								tokenIndex = CmdletValidator.GetParameterValue(parameterValueTokens, tokenIndex, out value);
								dictionary.Add(obj2, value);
							}
						}
						parameterValue = dictionary;
						goto IL_23B;
					}
					case TokenKind.Semi:
					case TokenKind.Pipe:
						throw new InvalidOperationException(string.Format("Cmdlet parsing failed at index {0}. Encountered unexpected {1}", tokenIndex, token.Kind));
					}
					break;
				}
			}
			parameterValue = token.Text;
			IL_23B:
			return ++tokenIndex;
		}

		private string NameOf<T>(Expression<Func<T>> memberExpression)
		{
			MemberExpression memberExpression2 = (MemberExpression)memberExpression.Body;
			return memberExpression2.Member.Name;
		}

		private HashSet<string> allowedCommands;

		private Dictionary<string, HashSet<string>> requiredParameters;

		private Dictionary<string, HashSet<string>> notAllowedParameters;
	}
}
