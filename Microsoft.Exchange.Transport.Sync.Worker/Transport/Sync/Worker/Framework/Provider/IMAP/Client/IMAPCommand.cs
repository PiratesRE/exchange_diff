using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPCommand
	{
		internal IMAPCommand()
		{
			this.cachedStringBuilder = new StringBuilder(128);
			this.commandParameters = new List<object>(5);
		}

		internal string CommandId
		{
			get
			{
				return this.commandId;
			}
		}

		internal IMAPCommandType CommandType
		{
			get
			{
				return this.commandType;
			}
		}

		internal IList<object> CommandParameters
		{
			get
			{
				return this.commandParameters;
			}
		}

		private StringBuilder CachedStringBuilder
		{
			get
			{
				return this.cachedStringBuilder;
			}
		}

		internal void ResetAsLogin(string newCommandId, string user, SecureString password)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("user", user);
			SyncUtilities.ThrowIfArgumentNull("password", password);
			this.Reset(IMAPCommandType.Login, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} LOGIN \"{1}\" \"{2}\"\r\n", new object[]
			{
				cmd.CommandId,
				cmd.ConvertToQuotableString((string)cmd.CommandParameters[0]),
				cmd.ConvertToQuotableString(SyncUtilities.SecureStringToString((SecureString)cmd.CommandParameters[1]))
			}), new object[]
			{
				user,
				password
			});
		}

		internal void ResetAsId(string newCommandId, SecureString clientToken)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNull("clientToken", clientToken);
			this.Reset(IMAPCommandType.Id, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} ID {1}\r\n", new object[]
			{
				cmd.CommandId,
				SyncUtilities.SecureStringToString((SecureString)cmd.CommandParameters[0])
			}), new object[]
			{
				clientToken
			});
		}

		internal void ResetAsLogout(string newCommandId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			this.Reset(IMAPCommandType.Logout, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} LOGOUT\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsStarttls(string newCommandId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			this.Reset(IMAPCommandType.Starttls, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} STARTTLS\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsAuthenticate(string newCommandId, IMAPAuthenticationMechanism authMechanism, string user, SecureString password, AuthenticationContext authContext)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNull("user", user);
			SyncUtilities.ThrowIfArgumentNull("password", password);
			this.Reset(IMAPCommandType.Authenticate, newCommandId, delegate(IMAPCommand cmd)
			{
				StringBuilder stringBuilder = cmd.cachedStringBuilder;
				stringBuilder.Length = 0;
				stringBuilder.Append(cmd.CommandId);
				stringBuilder.Append(" AUTHENTICATE");
				IMAPAuthenticationMechanism authMechanism2 = authMechanism;
				if (authMechanism2 != IMAPAuthenticationMechanism.Basic)
				{
					if (authMechanism2 != IMAPAuthenticationMechanism.Ntlm)
					{
						throw new InvalidOperationException("Unexpected authentication mechanism " + authMechanism);
					}
					stringBuilder.Append(" NTLM\r\n");
				}
				else
				{
					stringBuilder.Append(" PLAIN\r\n");
				}
				return stringBuilder.ToString();
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
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			this.Reset(IMAPCommandType.Capability, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} CAPABILITY\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsExpunge(string newCommandId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			this.Reset(IMAPCommandType.Expunge, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} EXPUNGE\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsSelect(string newCommandId, IMAPMailbox imapMailbox)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNull("imapMailbox", imapMailbox);
			this.Reset(IMAPCommandType.Select, newCommandId, delegate(IMAPCommand cmd)
			{
				string text = cmd.ConvertToQuotableString(((IMAPMailbox)cmd.CommandParameters[0]).NameOnTheWire);
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

		internal void ResetAsStatus(string newCommandId, IMAPMailbox imapMailbox)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNull("imapMailbox", imapMailbox);
			this.Reset(IMAPCommandType.Status, newCommandId, delegate(IMAPCommand cmd)
			{
				string text = cmd.ConvertToQuotableString(((IMAPMailbox)cmd.CommandParameters[0]).NameOnTheWire);
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
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("start", start);
			SyncUtilities.ThrowIfArgumentNull("dataItemSet", dataItemSet);
			if (dataItemSet.Count == 0)
			{
				throw new ArgumentException("dataItemSet cannot be empty");
			}
			this.Reset(IMAPCommandType.Fetch, newCommandId, delegate(IMAPCommand cmd)
			{
				bool flag = (bool)cmd.CommandParameters[2];
				StringBuilder stringBuilder = cmd.cachedStringBuilder;
				stringBuilder.Length = 0;
				stringBuilder.Append(cmd.CommandId);
				if (flag)
				{
					stringBuilder.Append(" UID");
				}
				stringBuilder.Append(" FETCH ");
				stringBuilder.Append(cmd.CommandParameters[0]);
				if (cmd.CommandParameters[1] != null)
				{
					stringBuilder.Append(':');
					stringBuilder.Append(cmd.CommandParameters[1]);
				}
				string value = " (";
				IList<string> list = (IList<string>)cmd.CommandParameters[3];
				foreach (string value2 in list)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(value2);
					value = " ";
				}
				stringBuilder.Append(')');
				stringBuilder.Append("\r\n");
				return stringBuilder.ToString();
			}, new object[]
			{
				start,
				end,
				uidFetch,
				dataItemSet
			});
		}

		internal void ResetAsAppend(string newCommandId, string mailboxName, IMAPMailFlags messageFlags, Stream messageBody)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxName", mailboxName);
			SyncUtilities.ThrowIfArgumentNull("messageFlags", messageFlags);
			SyncUtilities.ThrowIfArgumentNull("messageBody", messageBody);
			this.Reset(IMAPCommandType.Append, newCommandId, delegate(IMAPCommand cmd)
			{
				string value = cmd.ConvertToQuotableString((string)cmd.CommandParameters[0]);
				StringBuilder stringBuilder = cmd.CachedStringBuilder;
				stringBuilder.Length = 0;
				stringBuilder.Append(cmd.CommandId);
				stringBuilder.Append(" APPEND \"");
				stringBuilder.Append(value);
				stringBuilder.Append("\" ");
				IMAPUtils.AppendStringBuilderIMAPFlags((IMAPMailFlags)cmd.CommandParameters[1], stringBuilder);
				stringBuilder.Append(" {");
				stringBuilder.Append((long)cmd.CommandParameters[3]);
				stringBuilder.Append("}\r\n");
				return stringBuilder.ToString();
			}, new object[]
			{
				mailboxName,
				messageFlags,
				messageBody,
				messageBody.Length
			});
		}

		internal void ResetAsUidStore(string newCommandId, string uid, IMAPMailFlags flags, bool addFlags)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("uid", uid);
			this.Reset(IMAPCommandType.Store, newCommandId, delegate(IMAPCommand cmd)
			{
				StringBuilder stringBuilder = cmd.cachedStringBuilder;
				stringBuilder.Length = 0;
				stringBuilder.Append(cmd.CommandId);
				stringBuilder.Append(" UID STORE ");
				stringBuilder.Append((string)cmd.CommandParameters[0]);
				if (addFlags)
				{
					stringBuilder.Append(" +FLAGS.SILENT ");
				}
				else
				{
					stringBuilder.Append(" -FLAGS.SILENT ");
				}
				IMAPUtils.AppendStringBuilderIMAPFlags((IMAPMailFlags)cmd.CommandParameters[1], stringBuilder);
				stringBuilder.Append("\r\n");
				return stringBuilder.ToString();
			}, new object[]
			{
				uid,
				flags
			});
		}

		internal void ResetAsNoop(string newCommandId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			this.Reset(IMAPCommandType.Noop, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} NOOP\r\n", new object[]
			{
				cmd.CommandId
			}), new object[0]);
		}

		internal void ResetAsSearch(string newCommandId, params string[] queryTerms)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNull("queryTerms", queryTerms);
			this.Reset(IMAPCommandType.Search, newCommandId, delegate(IMAPCommand cmd)
			{
				StringBuilder stringBuilder = cmd.CachedStringBuilder;
				stringBuilder.Length = 0;
				stringBuilder.Append(cmd.CommandId);
				stringBuilder.Append(" UID SEARCH");
				foreach (object value in cmd.CommandParameters)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(value);
				}
				stringBuilder.Append("\r\n");
				return stringBuilder.ToString();
			}, queryTerms);
		}

		internal void ResetAsList(string newCommandId, char separator, int? level, string rootFolderPath)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			this.Reset(IMAPCommandType.List, newCommandId, delegate(IMAPCommand cmd)
			{
				int? num = (int?)cmd.CommandParameters[0];
				string text = "*";
				if (num != null)
				{
					StringBuilder stringBuilder = cmd.CachedStringBuilder;
					stringBuilder.Length = 0;
					stringBuilder.Append("%");
					for (int i = 1; i < num.Value; i++)
					{
						stringBuilder.Append(separator);
						stringBuilder.Append('%');
					}
					text = stringBuilder.ToString();
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
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxName", mailboxName);
			this.Reset(IMAPCommandType.CreateMailbox, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} CREATE \"{1}\"\r\n", new object[]
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
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxName", mailboxName);
			this.Reset(IMAPCommandType.DeleteMailbox, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} DELETE \"{1}\"\r\n", new object[]
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
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newCommandId", newCommandId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("oldMailboxName", oldMailboxName);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("newMailboxName", newMailboxName);
			this.Reset(IMAPCommandType.RenameMailbox, newCommandId, (IMAPCommand cmd) => string.Format(CultureInfo.InvariantCulture, "{0} RENAME \"{1}\" \"{2}\"\r\n", new object[]
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
			return Encoding.ASCII.GetBytes(this.builder(this));
		}

		internal string ToPiiCleanString()
		{
			IMAPCommandType imapcommandType = this.commandType;
			if (imapcommandType == IMAPCommandType.Login)
			{
				return this.commandId + " LOGIN ...";
			}
			if (imapcommandType != IMAPCommandType.Id)
			{
				return this.builder(this);
			}
			return this.commandId + " ID ...";
		}

		internal Stream GetCommandParameterStream(Fqdn targetHost, string responseLine, out Exception failureException)
		{
			failureException = null;
			if (this.commandType == IMAPCommandType.Append)
			{
				return this.CommandParameters[2] as Stream;
			}
			if (this.commandType == IMAPCommandType.Authenticate)
			{
				byte[] inputBuffer = null;
				MemoryStream result = null;
				IMAPAuthenticationMechanism imapauthenticationMechanism = (IMAPAuthenticationMechanism)this.CommandParameters[0];
				string text = (string)this.CommandParameters[1];
				SecureString password = (SecureString)this.CommandParameters[2];
				AuthenticationContext authenticationContext = (AuthenticationContext)this.CommandParameters[3];
				string text2 = null;
				if (responseLine != null && responseLine.Length > 2)
				{
					inputBuffer = Encoding.ASCII.GetBytes(responseLine.Substring(2));
				}
				byte[] buffer = null;
				IMAPAuthenticationMechanism imapauthenticationMechanism2 = imapauthenticationMechanism;
				if (imapauthenticationMechanism2 != IMAPAuthenticationMechanism.Basic)
				{
					if (imapauthenticationMechanism2 == IMAPAuthenticationMechanism.Ntlm)
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
								failureException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new IMAPException("Failure in NTLM Authentication"), true);
								return null;
							}
						}
						securityStatus = authenticationContext.NegotiateSecurityContext(inputBuffer, out buffer);
						SecurityStatus securityStatus2 = securityStatus;
						if (securityStatus2 != SecurityStatus.OK && securityStatus2 != SecurityStatus.ContinueNeeded)
						{
							failureException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new IMAPException("Failure in NTLM Authentication"), true);
							return null;
						}
						result = new MemoryStream(buffer);
					}
					else
					{
						failureException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, new IMAPException("Unsupported Authentication Mechanism"), true);
					}
				}
				else
				{
					SecurityStatus securityStatus;
					if (authenticationContext == null)
					{
						authenticationContext = new AuthenticationContext();
						this.CommandParameters[3] = authenticationContext;
						Match match = IMAPCommand.UserNameWithAuthorizationId.Match(text);
						if (match != null && match.Success && match.Groups.Count == 3)
						{
							text2 = match.Groups[1].Value;
							text = match.Groups[2].Value;
						}
						securityStatus = authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Plain, null, text, password);
						if (securityStatus != SecurityStatus.OK)
						{
							failureException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new IMAPException("Failure in PLAIN Authentication"), true);
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
						failureException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new IMAPException("Failure in PLAIN Authentication"), true);
						return null;
					}
					result = new MemoryStream(buffer);
				}
				return result;
			}
			return null;
		}

		private void Reset(IMAPCommandType incomingCommandType, string incomingCommandId, IMAPCommand.CommandStringBuilder incomingCommandBuilder, params object[] incomingCommandParameters)
		{
			this.commandType = incomingCommandType;
			this.commandId = incomingCommandId;
			this.commandParameters.Clear();
			this.cachedStringBuilder.Length = 0;
			this.builder = incomingCommandBuilder;
			if (incomingCommandParameters != null)
			{
				foreach (object item in incomingCommandParameters)
				{
					this.commandParameters.Add(item);
				}
			}
		}

		private string ConvertToQuotableString(string incoming)
		{
			StringBuilder stringBuilder = this.cachedStringBuilder;
			stringBuilder.Length = 0;
			for (int i = 0; i < incoming.Length; i++)
			{
				if (incoming[i] == '\\' || incoming[i] == '"')
				{
					stringBuilder.Append('\\');
				}
				stringBuilder.Append(incoming[i]);
			}
			return stringBuilder.ToString();
		}

		internal const int MailboxToSelect = 0;

		internal const int MailboxToCreate = 0;

		internal const int FetchStartIndex = 0;

		internal const int FetchEndIndex = 1;

		internal const int FetchByUid = 2;

		internal const int FetchDataItems = 3;

		internal const int StoreUidIndex = 0;

		internal const string PrefixTag = "E";

		private const string ImapSpnPrefix = "IMAP/";

		private const int DefaultCommandLength = 128;

		private const int DefaultNumberOfCommandParameters = 5;

		private const int MessageBodyIndex = 2;

		private const int AuthMechanismIndex = 0;

		private const int UsernameIndex = 1;

		private const int PasswordIndex = 2;

		private const int AuthContextIndex = 3;

		private static readonly Regex UserNameWithAuthorizationId = new Regex("^#(.+)#([^#\n]+)#$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private IMAPCommandType commandType;

		private string commandId;

		private IList<object> commandParameters;

		private IMAPCommand.CommandStringBuilder builder;

		private StringBuilder cachedStringBuilder;

		private delegate string CommandStringBuilder(IMAPCommand command);
	}
}
