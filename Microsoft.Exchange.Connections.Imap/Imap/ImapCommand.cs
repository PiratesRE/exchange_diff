using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapCommand
	{
		internal ImapCommand()
		{
			this.CachedStringBuilder = new StringBuilder(128);
			this.CommandParameters = new List<object>(5);
		}

		internal string CommandId { get; private set; }

		internal ImapCommandType CommandType { get; private set; }

		internal IList<object> CommandParameters { get; private set; }

		private ImapCommand.CommandStringBuilder Builder { get; set; }

		private StringBuilder CachedStringBuilder { get; set; }

		internal void ResetAsLogin(string newCommandId, string user, SecureString password)
		{
			this.Reset(ImapCommandType.Login, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} LOGIN \"{1}\" \"{2}\"\r\n", new object[]
			{
				cmd.CommandId,
				cmd.ConvertToQuotableString((string)cmd.CommandParameters[0]),
				cmd.ConvertToQuotableString(((SecureString)cmd.CommandParameters[1]).AsUnsecureString())
			}), new object[]
			{
				user,
				password
			});
		}

		internal void ResetAsId(string newCommandId, SecureString clientToken)
		{
			this.Reset(ImapCommandType.Id, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} ID {1}\r\n", new object[]
			{
				cmd.CommandId,
				((SecureString)cmd.CommandParameters[0]).AsUnsecureString()
			}), new object[]
			{
				clientToken
			});
		}

		internal void ResetAsLogout(string newCommandId)
		{
			this.Reset(ImapCommandType.Logout, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} LOGOUT\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsStarttls(string newCommandId)
		{
			this.Reset(ImapCommandType.Starttls, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} STARTTLS\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsAuthenticate(string newCommandId, ImapAuthenticationMechanism authMechanism, string user, SecureString password, AuthenticationContext authContext)
		{
			this.Reset(ImapCommandType.Authenticate, newCommandId, delegate(ImapCommand cmd)
			{
				StringBuilder cachedStringBuilder = cmd.CachedStringBuilder;
				cachedStringBuilder.Length = 0;
				cachedStringBuilder.Append(cmd.CommandId);
				cachedStringBuilder.Append(" AUTHENTICATE");
				ImapAuthenticationMechanism authMechanism2 = authMechanism;
				if (authMechanism2 != ImapAuthenticationMechanism.Basic)
				{
					if (authMechanism2 != ImapAuthenticationMechanism.Ntlm)
					{
						string message = "Unexpected authentication mechanism " + authMechanism;
						throw new InvalidOperationException(message);
					}
					cachedStringBuilder.Append(" NTLM\r\n");
				}
				else
				{
					cachedStringBuilder.Append(" PLAIN\r\n");
				}
				return cachedStringBuilder.ToString();
			}, new object[]
			{
				authMechanism,
				user,
				password,
				authContext
			});
		}

		internal void ResetAsCapability(string newCommandId)
		{
			this.Reset(ImapCommandType.Capability, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} CAPABILITY\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsExpunge(string newCommandId)
		{
			this.Reset(ImapCommandType.Expunge, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} EXPUNGE\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsSelect(string newCommandId, ImapMailbox imapMailbox)
		{
			this.Reset(ImapCommandType.Select, newCommandId, delegate(ImapCommand cmd)
			{
				string text = cmd.ConvertToQuotableString(((ImapMailbox)cmd.CommandParameters[0]).NameOnTheWire);
				return string.Format(CultureInfo.InvariantCulture, "{0} SELECT \"{1}\"\r\n", new object[]
				{
					cmd.CommandId,
					text
				});
			}, new object[]
			{
				imapMailbox
			});
		}

		internal void ResetAsStatus(string newCommandId, ImapMailbox imapMailbox)
		{
			this.Reset(ImapCommandType.Status, newCommandId, delegate(ImapCommand cmd)
			{
				string text = cmd.ConvertToQuotableString(((ImapMailbox)cmd.CommandParameters[0]).NameOnTheWire);
				return string.Format(CultureInfo.InvariantCulture, "{0} STATUS \"{1}\" (UIDNEXT)\r\n", new object[]
				{
					cmd.CommandId,
					text
				});
			}, new object[]
			{
				imapMailbox
			});
		}

		internal void ResetAsFetch(string newCommandId, string start, string end, bool uidFetch, IList<string> dataItemSet)
		{
			if (dataItemSet.Count == 0)
			{
				throw new ArgumentException("dataItemSet cannot be empty");
			}
			this.Reset(ImapCommandType.Fetch, newCommandId, delegate(ImapCommand cmd)
			{
				bool flag = (bool)cmd.CommandParameters[2];
				StringBuilder cachedStringBuilder = cmd.CachedStringBuilder;
				cachedStringBuilder.Length = 0;
				cachedStringBuilder.Append(cmd.CommandId);
				if (flag)
				{
					cachedStringBuilder.Append(" UID");
				}
				cachedStringBuilder.Append(" FETCH ");
				cachedStringBuilder.Append(cmd.CommandParameters[0]);
				if (cmd.CommandParameters[1] != null)
				{
					cachedStringBuilder.Append(':');
					cachedStringBuilder.Append(cmd.CommandParameters[1]);
				}
				string value = " (";
				IList<string> list = (IList<string>)cmd.CommandParameters[3];
				foreach (string value2 in list)
				{
					cachedStringBuilder.Append(value);
					cachedStringBuilder.Append(value2);
					value = " ";
				}
				cachedStringBuilder.Append(')');
				cachedStringBuilder.Append("\r\n");
				return cachedStringBuilder.ToString();
			}, new object[]
			{
				start,
				end,
				uidFetch,
				dataItemSet
			});
		}

		internal void ResetAsAppend(string newCommandId, string mailboxName, ImapMailFlags messageFlags, Stream messageBody)
		{
			this.Reset(ImapCommandType.Append, newCommandId, delegate(ImapCommand cmd)
			{
				string value = cmd.ConvertToQuotableString((string)cmd.CommandParameters[0]);
				StringBuilder cachedStringBuilder = cmd.CachedStringBuilder;
				cachedStringBuilder.Length = 0;
				cachedStringBuilder.Append(cmd.CommandId);
				cachedStringBuilder.Append(" APPEND \"");
				cachedStringBuilder.Append(value);
				cachedStringBuilder.Append("\" ");
				ImapUtilities.AppendStringBuilderImapFlags((ImapMailFlags)cmd.CommandParameters[1], cachedStringBuilder);
				cachedStringBuilder.Append(" {");
				cachedStringBuilder.Append((long)cmd.CommandParameters[3]);
				cachedStringBuilder.Append("}\r\n");
				return cachedStringBuilder.ToString();
			}, new object[]
			{
				mailboxName,
				messageFlags,
				messageBody,
				messageBody.Length
			});
		}

		internal void ResetAsUidStore(string newCommandId, string uid, ImapMailFlags flags, bool addFlags)
		{
			this.Reset(ImapCommandType.Store, newCommandId, delegate(ImapCommand cmd)
			{
				StringBuilder cachedStringBuilder = cmd.CachedStringBuilder;
				cachedStringBuilder.Length = 0;
				cachedStringBuilder.Append(cmd.CommandId);
				cachedStringBuilder.Append(" UID STORE ");
				cachedStringBuilder.Append((string)cmd.CommandParameters[0]);
				if (addFlags)
				{
					cachedStringBuilder.Append(" +FLAGS.SILENT ");
				}
				else
				{
					cachedStringBuilder.Append(" -FLAGS.SILENT ");
				}
				ImapUtilities.AppendStringBuilderImapFlags((ImapMailFlags)cmd.CommandParameters[1], cachedStringBuilder);
				cachedStringBuilder.Append("\r\n");
				return cachedStringBuilder.ToString();
			}, new object[]
			{
				uid,
				flags
			});
		}

		internal void ResetAsNoop(string newCommandId)
		{
			this.Reset(ImapCommandType.Noop, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} NOOP\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsSearch(string newCommandId, params string[] queryTerms)
		{
			this.Reset(ImapCommandType.Search, newCommandId, delegate(ImapCommand cmd)
			{
				StringBuilder cachedStringBuilder = cmd.CachedStringBuilder;
				cachedStringBuilder.Length = 0;
				cachedStringBuilder.Append(cmd.CommandId);
				cachedStringBuilder.Append(" UID SEARCH");
				foreach (object value in cmd.CommandParameters)
				{
					cachedStringBuilder.Append(" ");
					cachedStringBuilder.Append(value);
				}
				cachedStringBuilder.Append("\r\n");
				return cachedStringBuilder.ToString();
			}, queryTerms);
		}

		internal void ResetAsList(string newCommandId, char separator, int? level, string rootFolderPath)
		{
			this.Reset(ImapCommandType.List, newCommandId, delegate(ImapCommand cmd)
			{
				int? num = (int?)cmd.CommandParameters[0];
				string text = "*";
				if (num != null)
				{
					StringBuilder cachedStringBuilder = cmd.CachedStringBuilder;
					cachedStringBuilder.Length = 0;
					cachedStringBuilder.Append("%");
					for (int i = 1; i < num.Value; i++)
					{
						cachedStringBuilder.Append(separator);
						cachedStringBuilder.Append('%');
					}
					text = cachedStringBuilder.ToString();
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} LIST \"{1}\" {2}\r\n", new object[]
				{
					cmd.CommandId,
					string.IsNullOrEmpty(rootFolderPath) ? string.Empty : rootFolderPath,
					text
				});
			}, new object[]
			{
				level,
				separator
			});
		}

		internal void ResetAsCreate(string newCommandId, string mailboxName)
		{
			this.Reset(ImapCommandType.CreateMailbox, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} CREATE \"{1}\"\r\n", new object[]
			{
				cmd.CommandId,
				cmd.ConvertToQuotableString((string)cmd.CommandParameters[0])
			}), new object[]
			{
				mailboxName
			});
		}

		internal void ResetAsDelete(string newCommandId, string mailboxName)
		{
			this.Reset(ImapCommandType.DeleteMailbox, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} DELETE \"{1}\"\r\n", new object[]
			{
				cmd.CommandId,
				cmd.ConvertToQuotableString((string)cmd.CommandParameters[0])
			}), new object[]
			{
				mailboxName
			});
		}

		internal void ResetAsRename(string newCommandId, string oldMailboxName, string newMailboxName)
		{
			this.Reset(ImapCommandType.RenameMailbox, newCommandId, (ImapCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} RENAME \"{1}\" \"{2}\"\r\n", new object[]
			{
				cmd.CommandId,
				cmd.ConvertToQuotableString((string)cmd.CommandParameters[0]),
				cmd.ConvertToQuotableString((string)cmd.CommandParameters[1])
			}), new object[]
			{
				oldMailboxName,
				newMailboxName
			});
		}

		internal byte[] ToBytes()
		{
			return Encoding.ASCII.GetBytes(this.Builder(this));
		}

		internal string ToPiiCleanString()
		{
			ImapCommandType commandType = this.CommandType;
			if (commandType == ImapCommandType.Login)
			{
				return this.CommandId + " LOGIN ...";
			}
			if (commandType != ImapCommandType.Id)
			{
				return this.Builder(this);
			}
			return this.CommandId + " ID ...";
		}

		internal Stream GetCommandParameterStream(string targetHost, string responseLine, out Exception failureException)
		{
			failureException = null;
			if (this.CommandType == ImapCommandType.Append)
			{
				return this.CommandParameters[2] as Stream;
			}
			if (this.CommandType == ImapCommandType.Authenticate)
			{
				byte[] inputBuffer = null;
				MemoryStream result = null;
				ImapAuthenticationMechanism imapAuthenticationMechanism = (ImapAuthenticationMechanism)this.CommandParameters[0];
				string text = (string)this.CommandParameters[1];
				SecureString password = (SecureString)this.CommandParameters[2];
				AuthenticationContext authenticationContext = (AuthenticationContext)this.CommandParameters[3];
				string text2 = null;
				if (responseLine != null && responseLine.Length > 2)
				{
					inputBuffer = Encoding.ASCII.GetBytes(responseLine.Substring(2));
				}
				byte[] buffer = null;
				ImapAuthenticationMechanism imapAuthenticationMechanism2 = imapAuthenticationMechanism;
				if (imapAuthenticationMechanism2 != ImapAuthenticationMechanism.Basic)
				{
					if (imapAuthenticationMechanism2 == ImapAuthenticationMechanism.Ntlm)
					{
						SecurityStatus securityStatus;
						if (authenticationContext == null)
						{
							authenticationContext = new AuthenticationContext();
							this.CommandParameters[3] = authenticationContext;
							string spn = "IMAP/" + targetHost;
							securityStatus = authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Ntlm, spn, text, password);
							if (securityStatus != SecurityStatus.OK)
							{
								failureException = new ImapAuthenticationException(targetHost, imapAuthenticationMechanism.ToString(), RetryPolicy.Backoff);
								return null;
							}
						}
						securityStatus = authenticationContext.NegotiateSecurityContext(inputBuffer, out buffer);
						SecurityStatus securityStatus2 = securityStatus;
						if (securityStatus2 != SecurityStatus.OK && securityStatus2 != SecurityStatus.ContinueNeeded)
						{
							failureException = new ImapAuthenticationException(targetHost, imapAuthenticationMechanism.ToString(), RetryPolicy.Backoff);
							return null;
						}
						result = new MemoryStream(buffer);
					}
					else
					{
						failureException = new ImapUnsupportedAuthenticationException(targetHost, imapAuthenticationMechanism.ToString(), RetryPolicy.Backoff);
					}
				}
				else
				{
					SecurityStatus securityStatus;
					if (authenticationContext == null)
					{
						authenticationContext = new AuthenticationContext();
						this.CommandParameters[3] = authenticationContext;
						Match match = ImapCommand.UserNameWithAuthorizationId.Match(text);
						if (match != null && match.Success && match.Groups.Count == 3)
						{
							text2 = match.Groups[1].Value;
							text = match.Groups[2].Value;
						}
						securityStatus = authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Plain, null, text, password);
						if (securityStatus != SecurityStatus.OK)
						{
							failureException = new ImapAuthenticationException(targetHost, imapAuthenticationMechanism.ToString(), RetryPolicy.Backoff);
							return null;
						}
						if (text2 != null)
						{
							authenticationContext.AuthorizationIdentity = Encoding.ASCII.GetBytes(text2);
						}
					}
					securityStatus = authenticationContext.NegotiateSecurityContext(inputBuffer, out buffer);
					SecurityStatus securityStatus3 = securityStatus;
					if (securityStatus3 != SecurityStatus.OK)
					{
						failureException = new ImapAuthenticationException(targetHost, imapAuthenticationMechanism.ToString(), RetryPolicy.Backoff);
						return null;
					}
					result = new MemoryStream(buffer);
				}
				return result;
			}
			return null;
		}

		private void Reset(ImapCommandType incomingCommandType, string incomingCommandId, ImapCommand.CommandStringBuilder incomingCommandBuilder, params object[] incomingCommandParameters)
		{
			this.CommandType = incomingCommandType;
			this.CommandId = incomingCommandId;
			this.CommandParameters.Clear();
			this.CachedStringBuilder.Length = 0;
			this.Builder = incomingCommandBuilder;
			if (incomingCommandParameters != null)
			{
				foreach (object item in incomingCommandParameters)
				{
					this.CommandParameters.Add(item);
				}
			}
		}

		private string ConvertToQuotableString(string incoming)
		{
			StringBuilder cachedStringBuilder = this.CachedStringBuilder;
			cachedStringBuilder.Length = 0;
			foreach (char c in incoming)
			{
				if (c == '\\' || c == '"')
				{
					cachedStringBuilder.Append('\\');
				}
				cachedStringBuilder.Append(c);
			}
			return cachedStringBuilder.ToString();
		}

		public const int MailboxToSelect = 0;

		public const int MailboxToCreate = 0;

		public const int FetchStartIndex = 0;

		public const int FetchEndIndex = 1;

		public const int FetchByUid = 2;

		public const int FetchDataItems = 3;

		public const int StoreUidIndex = 0;

		public const string PrefixTag = "E";

		private const string ImapSpnPrefix = "IMAP/";

		private const int DefaultCommandLength = 128;

		private const int DefaultNumberOfCommandParameters = 5;

		private const int MessageBodyIndex = 2;

		private const int AuthMechanismIndex = 0;

		private const int UsernameIndex = 1;

		private const int PasswordIndex = 2;

		private const int AuthContextIndex = 3;

		private static readonly Regex UserNameWithAuthorizationId = new Regex("^#(.+)#([^#\n]+)#$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private delegate string CommandStringBuilder(ImapCommand command);
	}
}
