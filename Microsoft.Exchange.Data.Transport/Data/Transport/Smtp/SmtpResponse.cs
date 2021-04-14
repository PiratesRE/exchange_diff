using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public struct SmtpResponse : IEquatable<SmtpResponse>
	{
		public SmtpResponse(string statusCode, string enhancedStatusCode, params string[] statusText)
		{
			this = default(SmtpResponse);
			if (!SmtpResponseParser.IsValidStatusCode(statusCode))
			{
				throw new FormatException(string.Format("The SMTP status code must have three digits - statusCode: {0} statusText: {1}", statusCode, (statusText == null) ? "null" : string.Join(";", statusText)));
			}
			if (!string.IsNullOrEmpty(enhancedStatusCode) && !EnhancedStatusCodeImpl.IsValid(enhancedStatusCode))
			{
				throw new FormatException(string.Format("The SMTP enhanced status code must be in the form 2.yyy.zzz, 4.yyy.zzz, 5.yyy.zzz - {0}statusText: {1}", enhancedStatusCode, (statusText == null) ? "null" : string.Join(";", statusText)));
			}
			this.DsnExplanation = string.Empty;
			this.bytes = null;
			List<string> list = new List<string>();
			if (statusText != null)
			{
				foreach (string text in statusText)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = SmtpResponse.SplitLines(text);
						for (int j = 0; j < array.Length; j++)
						{
							list.Add(array[j]);
						}
					}
				}
				foreach (string text2 in list)
				{
					for (int k = 0; k < text2.Length; k++)
					{
						if (text2[k] < '\u0001' || text2[k] > '\u007f')
						{
							throw new FormatException(string.Format("Text for the response must contain only US-ASCII characters - {0}", text2));
						}
					}
				}
			}
			this.data = new string[2 + list.Count];
			this.data[0] = statusCode;
			this.data[1] = enhancedStatusCode;
			list.CopyTo(this.data, 2);
			this.statusText = null;
		}

		public SmtpResponse(string statusCode, string enhancedStatusCode, string dsnExplanation, bool overloadDifferentiator, params string[] statusText)
		{
			this = new SmtpResponse(statusCode, enhancedStatusCode, statusText);
			this.DsnExplanation = dsnExplanation;
		}

		private SmtpResponse(bool isEmpty)
		{
			this = default(SmtpResponse);
			this.empty = isEmpty;
			this.data = null;
			this.bytes = null;
			this.statusText = null;
			this.DsnExplanation = string.Empty;
		}

		public bool IsEmpty
		{
			get
			{
				return this.empty;
			}
		}

		public string DsnExplanation { get; private set; }

		public SmtpResponseType SmtpResponseType
		{
			get
			{
				if (string.IsNullOrEmpty(this.StatusCode))
				{
					return SmtpResponseType.Unknown;
				}
				switch (this.StatusCode[0])
				{
				case '2':
					return SmtpResponseType.Success;
				case '3':
					return SmtpResponseType.IntermediateSuccess;
				case '4':
					return SmtpResponseType.TransientError;
				case '5':
					return SmtpResponseType.PermanentError;
				default:
					return SmtpResponseType.Unknown;
				}
			}
		}

		public string StatusCode
		{
			get
			{
				if (this.data != null && this.data.Length >= 2)
				{
					return this.data[0];
				}
				return string.Empty;
			}
		}

		public string EnhancedStatusCode
		{
			get
			{
				if (this.data != null && this.data.Length >= 2)
				{
					return this.data[1];
				}
				return string.Empty;
			}
		}

		public string[] StatusText
		{
			get
			{
				if (this.statusText == null)
				{
					if (this.data == null || this.data.Length < 2)
					{
						return null;
					}
					string[] array = new string[this.data.Length - 2];
					int num = 0;
					for (int i = 2; i < this.data.Length; i++)
					{
						array[num++] = this.data[i];
					}
					this.statusText = array;
				}
				return this.statusText;
			}
		}

		public static SmtpResponse QueuedMailForDelivery(string messageId)
		{
			return new SmtpResponse("250", "2.6.0", new string[]
			{
				messageId + " Queued mail for delivery"
			});
		}

		public static SmtpResponse ConnectionDroppedDueTo(string reason)
		{
			if (string.IsNullOrEmpty(reason))
			{
				return SmtpResponse.ConnectionDropped;
			}
			return new SmtpResponse("421", "4.4.2", new string[]
			{
				"Connection dropped due to " + reason
			});
		}

		public static SmtpResponse QueuedMailForDelivery(string messageId, string recordId, string hostname, string smtpCustomDataResponse)
		{
			return new SmtpResponse("250", "2.6.0", new string[]
			{
				string.Format("{0} [InternalId={1}, Hostname={2}] {3}", new object[]
				{
					messageId,
					recordId,
					hostname,
					smtpCustomDataResponse
				})
			});
		}

		public static SmtpResponse QueuedMailForRedundancy(string messageId)
		{
			if (string.IsNullOrEmpty(messageId))
			{
				throw new ArgumentNullException("messageId");
			}
			return new SmtpResponse("250", "2.6.0", new string[]
			{
				messageId + " Queued mail for redundancy"
			});
		}

		public static SmtpResponse QueuedMailForRedundancy(string messageId, string recordId, string hostname)
		{
			if (string.IsNullOrEmpty(messageId))
			{
				throw new ArgumentNullException("messageId");
			}
			if (string.IsNullOrEmpty(recordId))
			{
				throw new ArgumentNullException("recordId");
			}
			return SmtpResponse.QueuedMailForRedundancy(string.Format("{0} [InternalId={1}, Hostname={2}]", messageId, hostname, recordId));
		}

		public static SmtpResponse ResourceForestMismatch(string expectedForest, string actualForest)
		{
			string text = string.Format("Temporary Server Network Configuration error detected: Expected={0}, actual={1}", expectedForest, actualForest);
			return new SmtpResponse("421", "4.3.2", new string[]
			{
				text
			});
		}

		public static bool TryParse(string text, out SmtpResponse response)
		{
			if (string.IsNullOrEmpty(text) || text.Length < 3)
			{
				response = SmtpResponse.Empty;
				return false;
			}
			string[] array;
			if (!SmtpResponseParser.Split(text, out array))
			{
				response = SmtpResponse.Empty;
				return false;
			}
			response = new SmtpResponse(false)
			{
				data = array
			};
			return true;
		}

		internal static bool TryParse(List<string> lines, out SmtpResponse response)
		{
			string[] array;
			if (!SmtpResponseParser.Split(lines, out array))
			{
				response = SmtpResponse.Empty;
				return false;
			}
			response = new SmtpResponse(false)
			{
				data = array
			};
			return true;
		}

		internal static SmtpResponse Parse(string text)
		{
			SmtpResponse result;
			if (!SmtpResponse.TryParse(text, out result))
			{
				throw new ArgumentException(string.Format("Text '{0}' could not be parsed into an SMTP response", text), "text");
			}
			return result;
		}

		public static bool operator ==(SmtpResponse response1, SmtpResponse response2)
		{
			return string.Equals(response1.StatusCode, response2.StatusCode, StringComparison.Ordinal) && string.Equals(response1.EnhancedStatusCode, response2.EnhancedStatusCode, StringComparison.Ordinal);
		}

		public static bool operator !=(SmtpResponse response1, SmtpResponse response2)
		{
			return !(response1 == response2);
		}

		public bool Equals(SmtpResponse comparand)
		{
			return this == comparand;
		}

		public bool Equals(SmtpResponse other, SmtpResponseCompareOptions compareOptions)
		{
			if (!this.Equals(other))
			{
				return false;
			}
			if (compareOptions != SmtpResponseCompareOptions.IncludeTextComponent)
			{
				return true;
			}
			if (this.StatusText != null && other.StatusText != null)
			{
				return this.statusText.SequenceEqual(other.statusText, StringComparer.OrdinalIgnoreCase);
			}
			return this.StatusText == null && other.StatusText == null;
		}

		public override bool Equals(object obj)
		{
			return obj is SmtpResponse && this == (SmtpResponse)obj;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (!string.IsNullOrEmpty(this.StatusCode))
			{
				num = this.StatusCode.GetHashCode();
			}
			if (!string.IsNullOrEmpty(this.EnhancedStatusCode))
			{
				num ^= this.EnhancedStatusCode.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			byte[] array = this.ToByteArray();
			return CTSGlobals.AsciiEncoding.GetString(array, 0, array.Length - 2);
		}

		internal static SmtpResponse AuthBlob(byte[] blob)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			return new SmtpResponse("334", string.Empty, new string[]
			{
				Encoding.ASCII.GetString(blob)
			});
		}

		internal static SmtpResponse ExchangeAuthSuccessful(byte[] blob)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			return new SmtpResponse("235", string.Empty, new string[]
			{
				Encoding.ASCII.GetString(blob)
			});
		}

		internal static SmtpResponse Banner(string serverName, string serverVersion, string date, bool isModernServer = false)
		{
			return new SmtpResponse("220", string.Empty, new string[]
			{
				string.Format("{0} {1} {2}", serverName, isModernServer ? "MICROSOFT ESMTP MAIL SERVICE READY AT" : "Microsoft ESMTP MAIL Service ready at", date)
			});
		}

		internal static SmtpResponse Ehlo(string serverName, IPAddress ipAddress, long maxMessageSize, bool advertiseStartTls, bool advertiseAuth)
		{
			return new SmtpResponse("250", string.Empty, new string[]
			{
				string.Format("{0} Hello [{1}]", serverName, ipAddress),
				string.Format("SIZE {0}", maxMessageSize),
				"PIPELINING",
				"DSN",
				"ENHANCEDSTATUSCODES",
				advertiseStartTls ? "STARTTLS" : null,
				advertiseAuth ? "AUTH NTLM LOGIN" : null
			});
		}

		internal static SmtpResponse Helo(string serverName, IPAddress ipAddress)
		{
			return new SmtpResponse("250", string.Empty, new string[]
			{
				string.Concat(new object[]
				{
					serverName,
					" Hello [",
					ipAddress,
					"]"
				})
			});
		}

		internal static SmtpResponse OctetsReceived(long octets)
		{
			return new SmtpResponse("250", "2.6.0", new string[]
			{
				"CHUNK received OK, " + octets.ToString(CultureInfo.InvariantCulture) + " octets"
			});
		}

		internal byte[] ToByteArray()
		{
			if (this.bytes == null)
			{
				string s;
				if (this.data != null && this.data.Length > 2)
				{
					string text;
					if (!string.IsNullOrEmpty(this.EnhancedStatusCode))
					{
						text = this.StatusCode + "-" + this.EnhancedStatusCode + " ";
					}
					else
					{
						text = this.StatusCode + "-";
					}
					int num = 0;
					for (int i = 2; i < this.data.Length; i++)
					{
						num += text.Length + this.data[i].Length + "\r\n".Length;
					}
					StringBuilder stringBuilder = new StringBuilder(num);
					int num2 = 0;
					for (int j = 2; j < this.data.Length; j++)
					{
						num2 = stringBuilder.Length;
						stringBuilder.Append(text);
						stringBuilder.Append(this.data[j]);
						stringBuilder.Append("\r\n");
					}
					if (stringBuilder.Length > 0)
					{
						num2 += this.StatusCode.Length;
						stringBuilder[num2] = ' ';
					}
					s = stringBuilder.ToString();
				}
				else if (!string.IsNullOrEmpty(this.EnhancedStatusCode))
				{
					s = this.StatusCode + " " + this.EnhancedStatusCode + "\r\n";
				}
				else if (!string.IsNullOrEmpty(this.StatusCode))
				{
					s = this.StatusCode + " " + "\r\n";
				}
				else
				{
					s = "\r\n";
				}
				this.bytes = CTSGlobals.AsciiEncoding.GetBytes(s);
			}
			return this.bytes;
		}

		private static string[] SplitLines(string response)
		{
			List<string> list = new List<string>();
			int num = 0;
			int num2;
			while ((num2 = response.IndexOf("\r\n", num, StringComparison.Ordinal)) != -1)
			{
				list.Add(response.Substring(num, num2 - num));
				num = num2 + "\r\n".Length;
				if (num >= response.Length)
				{
					break;
				}
			}
			if (num < response.Length)
			{
				list.Add(response.Substring(num));
			}
			return list.ToArray();
		}

		internal const int MaxResponseLineLength = 2000;

		internal const int MaxNumResponseLines = 50;

		internal const string ProxySessionProtocolFailurePrefixString = "Proxy session setup failed on Frontend with ";

		private const string CRLF = "\r\n";

		public static readonly SmtpResponse Empty = new SmtpResponse(true);

		public static readonly SmtpResponse BadCommandSequence = new SmtpResponse("503", "5.5.1", new string[]
		{
			"Bad sequence of commands"
		});

		public static readonly SmtpResponse ConnectionDroppedByAgentError = new SmtpResponse("421", "4.3.2", new string[]
		{
			"System not accepting network messages"
		});

		public static readonly SmtpResponse ConnectionTimedOut = new SmtpResponse("421", "4.4.1", new string[]
		{
			"Connection timed out"
		});

		public static readonly SmtpResponse DataTransactionFailed = new SmtpResponse("451", "4.3.2", new string[]
		{
			"System not accepting network messages"
		});

		public static readonly SmtpResponse RecipientAddressExpanded = new SmtpResponse("250", "2.0.0", new string[]
		{
			"Recipient address expanded"
		});

		public static readonly SmtpResponse RecipientAddressExpandedByRedirectionAgent = new SmtpResponse("250", "2.0.0", new string[]
		{
			"Recipient address expanded by Redirection Agent"
		});

		public static readonly SmtpResponse InsufficientResource = new SmtpResponse("452", "4.3.1", new string[]
		{
			"Insufficient system resources"
		});

		public static readonly SmtpResponse CTSParseError = new SmtpResponse("452", "4.6.0", new string[]
		{
			"Insufficient system resources (I/O)"
		});

		public static readonly SmtpResponse InvalidAddress = new SmtpResponse("501", "5.1.3", new string[]
		{
			"Invalid address"
		});

		public static readonly SmtpResponse InvalidSenderAddress = new SmtpResponse("501", "5.1.7", new string[]
		{
			"Invalid address"
		});

		public static readonly SmtpResponse SendAsDenied = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Client does not have permissions to send as this sender"
		});

		public static readonly SmtpResponse AnonymousSendAsDenied = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Anonymous client does not have permissions to send as this sender"
		});

		public static readonly SmtpResponse SendOnBehalfOfDenied = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Client does not have permissions to send on behalf of the from address"
		});

		public static readonly SmtpResponse SubmitDenied = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Client does not have permissions to submit to this server"
		});

		public static readonly SmtpResponse InvalidArguments = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Invalid arguments"
		});

		public static readonly SmtpResponse TransientInvalidArguments = new SmtpResponse("401", "4.5.4", new string[]
		{
			"Invalid arguments - possible version mismatch"
		});

		public static readonly SmtpResponse InvalidContent = new SmtpResponse("554", "5.6.0", new string[]
		{
			"Invalid message content"
		});

		public static readonly SmtpResponse InvalidContentBareLinefeeds = new SmtpResponse("554", "5.6.0", new string[]
		{
			"Invalid message content, contains bare linefeeds"
		});

		public static readonly SmtpResponse MessagePartialNotSupported = new SmtpResponse("554", "5.6.1", new string[]
		{
			"Messages of type message/partial are not supported"
		});

		public static readonly SmtpResponse NoopOk = new SmtpResponse("250", "2.0.0", new string[]
		{
			"OK"
		});

		public static readonly SmtpResponse InvalidResponse = new SmtpResponse("421", "4.4.0", new string[]
		{
			"Remote server response was not RFC conformant"
		});

		public static readonly SmtpResponse MessageTooLarge = new SmtpResponse("552", "5.3.4", new string[]
		{
			"Message size exceeds fixed maximum message size"
		});

		public static readonly SmtpResponse ServiceUnavailable = new SmtpResponse("421", "4.3.2", new string[]
		{
			"Service not available"
		});

		public static readonly SmtpResponse RcptNotFound = new SmtpResponse("550", "5.1.1", new string[]
		{
			"User unknown"
		});

		public static readonly SmtpResponse SuccessfulConnection = new SmtpResponse("250", string.Empty, new string[]
		{
			"Success"
		});

		public static readonly SmtpResponse SystemMisconfiguration = new SmtpResponse("550", "5.3.5", new string[]
		{
			"System incorrectly configured"
		});

		public static readonly SmtpResponse UnableToRoute = new SmtpResponse("554", "5.4.4", new string[]
		{
			"Unable to route"
		});

		public static readonly SmtpResponse InvalidRecipientAddress = new SmtpResponse("554", "5.4.4", new string[]
		{
			"Unable to route due to invalid recipient address"
		});

		public static readonly SmtpResponse TimeoutOccurred = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Timeout waiting for client input"
		});

		public static readonly SmtpResponse UnsupportedCommand = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Command not allowed"
		});

		public static readonly SmtpResponse UnrecognizedCommand = new SmtpResponse("500", "5.3.3", new string[]
		{
			"Unrecognized command"
		});

		public static readonly SmtpResponse UnrecognizedParameter = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Unrecognized parameter"
		});

		public static readonly SmtpResponse MailFromOk = new SmtpResponse("250", "2.1.0", new string[]
		{
			"Sender OK"
		});

		public static readonly SmtpResponse RcptToOk = new SmtpResponse("250", "2.1.5", new string[]
		{
			"Recipient OK"
		});

		internal static readonly SmtpResponse Rcpt2ToOk = new SmtpResponse("250", "2.1.5", new string[]
		{
			"Recipient2 OK"
		});

		internal static readonly SmtpResponse Rcpt2ToOkButInvalidAddress = new SmtpResponse("249", "2.1.5", new string[]
		{
			"Recipient2 OK but invalid address"
		});

		internal static readonly SmtpResponse Rcpt2ToOkButInvalidArguments = new SmtpResponse("247", "2.1.5", new string[]
		{
			"Recipient2 OK but invalid parameters"
		});

		internal static readonly SmtpResponse Rcpt2ToOkButRcpt2AddressDifferentFromRcptAddress = new SmtpResponse("248", "2.1.5", new string[]
		{
			"Recipient2 OK but unknown address"
		});

		internal static readonly SmtpResponse Rcpt2AlreadyReceived = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Recipient2 already received"
		});

		public static readonly SmtpResponse TlsDomainRequired = new SmtpResponse("421", string.Empty, new string[]
		{
			"TLSAuthLevel cannot be DomainValidation with out a valid TLS Domain."
		});

		public static readonly SmtpResponse IncorrectTlsAuthLevel = new SmtpResponse("421", string.Empty, new string[]
		{
			"TLS Domain can only be specified with TLSAuthLevel set to DomainValidation."
		});

		public static readonly SmtpResponse ServiceInactive = new SmtpResponse("421", "4.3.2", new string[]
		{
			"Service not active"
		});

		public static readonly SmtpResponse SpamFilterNotReady = new SmtpResponse("421", "4.3.2", new string[]
		{
			"Temporary local error initializing data. System not accepting network messages. (SF1)"
		});

		public static readonly SmtpResponse InvalidMailboxSpamFilterRule = new SmtpResponse("421", "4.3.2", new string[]
		{
			"Temporary local error initializing data. System not accepting network messages. (SF2)"
		});

		public static readonly SmtpResponse EnvelopeFilterNotReady = new SmtpResponse("421", "4.3.2", new string[]
		{
			"Temporary local error initializing data. System not accepting IPv6 network messages. (EV1)"
		});

		public static readonly SmtpResponse OrgQueueQuotaExceeded = new SmtpResponse("450", "4.7.0", new string[]
		{
			"Organization queue quota exceeded."
		});

		internal static readonly SmtpResponse AuthAlreadySpecified = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Auth command already specified"
		});

		internal static readonly SmtpResponse AuthCancelled = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Authentication cancelled"
		});

		internal static readonly SmtpResponse AuthPassword = new SmtpResponse("334", string.Empty, new string[]
		{
			"UGFzc3dvcmQ6"
		});

		internal static readonly SmtpResponse AuthSuccessful = new SmtpResponse("235", "2.7.0", new string[]
		{
			"Authentication successful"
		});

		internal static readonly SmtpResponse AuthTempFailure = new SmtpResponse("454", "4.7.0", new string[]
		{
			"Temporary authentication failure"
		});

		internal static readonly SmtpResponse AuthorizationTempFailure = new SmtpResponse("454", "4.7.0", new string[]
		{
			"Temporary authorization failure"
		});

		internal static readonly SmtpResponse AuthTempFailureTLSCipherTooWeak = new SmtpResponse("454", "4.7.0", new string[]
		{
			"Temporary authentication failure because negotiated Tls cipher strength is too weak"
		});

		internal static readonly SmtpResponse AuthUnrecognized = new SmtpResponse("504", "5.7.4", new string[]
		{
			"Unrecognized authentication type"
		});

		internal static readonly SmtpResponse AuthUnsuccessful = new SmtpResponse("535", "5.7.3", new string[]
		{
			"Authentication unsuccessful"
		});

		internal static readonly SmtpResponse SubmissionDisabledBySendQuota = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Submission has been disabled for this account"
		});

		internal static readonly SmtpResponse CryptographicFailure = new SmtpResponse("554", "5.7.5", new string[]
		{
			"Cryptographic failure"
		});

		internal static readonly SmtpResponse CertificateValidationFailure = new SmtpResponse("454", "4.7.5", new string[]
		{
			"Certificate validation failure"
		});

		internal static readonly SmtpResponse AuthUsername = new SmtpResponse("334", string.Empty, new string[]
		{
			"VXNlcm5hbWU6"
		});

		internal static readonly SmtpResponse CommandTooLong = new SmtpResponse("500", "5.5.2", new string[]
		{
			"Command too long"
		});

		internal static readonly SmtpResponse ConnectionDropped = new SmtpResponse("421", "4.4.2", new string[]
		{
			"Connection dropped"
		});

		internal static readonly SmtpResponse CommandNotImplemented = new SmtpResponse("502", "5.3.3", new string[]
		{
			"Command not implemented"
		});

		internal static readonly SmtpResponse MessageSizeLimitReached = new SmtpResponse("421", "4.4.2", new string[]
		{
			"Message size limit for this connection reached"
		});

		internal static readonly SmtpResponse MessageRateLimitExceeded = new SmtpResponse("421", "4.4.2", new string[]
		{
			"Message submission rate for this client has exceeded the configured limit"
		});

		internal static readonly SmtpResponse MessageDeletedByAdmin = new SmtpResponse("421", "4.3.2", new string[]
		{
			"System not accepting network messages"
		});

		internal static readonly SmtpResponse RetryDataSend = new SmtpResponse("451", "4.3.0", new string[]
		{
			"I/O error reading message body"
		});

		internal static readonly SmtpResponse ShadowRedundancyFailed = new SmtpResponse("451", "4.4.0", new string[]
		{
			"Message failed to be made redundant"
		});

		internal static readonly SmtpResponse HeadersTooLarge = new SmtpResponse("552", "5.3.4", new string[]
		{
			"Header size exceeds fixed maximum size"
		});

		internal static readonly SmtpResponse Help = new SmtpResponse("214", string.Empty, new string[]
		{
			"This server supports the following commands:",
			"HELO EHLO STARTTLS RCPT DATA RSET MAIL QUIT HELP AUTH BDAT"
		});

		internal static readonly SmtpResponse HopCountExceeded = new SmtpResponse("554", "5.4.6", new string[]
		{
			"Hop count exceeded - possible mail loop"
		});

		internal static readonly SmtpResponse ProxyHopCountExceeded = new SmtpResponse("554", "5.4.6", new string[]
		{
			"Proxy Hop count exceeded"
		});

		internal static readonly SmtpResponse AgentGeneratedMessageDepthExceeded = new SmtpResponse("554", "5.3.5", new string[]
		{
			"Agent generated message depth exceeded"
		});

		internal static readonly SmtpResponse InvalidEhloDomain = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Invalid domain name"
		});

		internal static readonly SmtpResponse InvalidHeloDomain = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Invalid domain name"
		});

		internal static readonly SmtpResponse EhloDomainRequired = new SmtpResponse("501", "5.0.0", new string[]
		{
			"EHLO requires domain address"
		});

		internal static readonly SmtpResponse HeloDomainRequired = new SmtpResponse("501", "5.0.0", new string[]
		{
			"HELO requires domain address"
		});

		internal static readonly SmtpResponse XShadowContextRequired = new SmtpResponse("501", "5.0.0", new string[]
		{
			"XSHADOW requires Shadow server's context"
		});

		internal static readonly SmtpResponse XQDiscardMaxDiscardCountRequired = new SmtpResponse("501", "5.0.0", new string[]
		{
			"XQDISCARD requires maximum discard events count"
		});

		internal static readonly SmtpResponse MailFromAlreadySpecified = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Sender already specified"
		});

		internal static readonly SmtpResponse MailingListExpansionProblem = new SmtpResponse("550", "5.2.4", new string[]
		{
			"Mailing list expansion problem"
		});

		internal static readonly SmtpResponse MailboxDisabled = new SmtpResponse("550", "5.2.1", new string[]
		{
			"Mailbox disabled, not accepting messages"
		});

		internal static readonly SmtpResponse MailboxOffline = new SmtpResponse("550", "5.2.1", new string[]
		{
			"Mailbox cannot be accessed"
		});

		internal static readonly SmtpResponse MailboxOverQuota = new SmtpResponse("550", "5.2.2", new string[]
		{
			"Mailbox full"
		});

		internal static readonly SmtpResponse SubmissionQuotaExceeded = new SmtpResponse("550", "5.2.2", new string[]
		{
			"Submission quota exceeded"
		});

		internal static readonly SmtpResponse NeedStartTls = new SmtpResponse("530", "5.7.0", new string[]
		{
			"Must issue a STARTTLS command first"
		});

		internal static readonly SmtpResponse NeedHello = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Send hello first"
		});

		internal static readonly SmtpResponse NeedEhlo = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Send EHLO first"
		});

		internal static readonly SmtpResponse NeedMailFrom = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Need mail command"
		});

		internal static readonly SmtpResponse NeedRcptTo = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Need rcpt command"
		});

		internal static readonly SmtpResponse NoRecipientSucceeded = new SmtpResponse("450", "4.5.0", new string[]
		{
			"No recipient succeeded"
		});

		internal static readonly SmtpResponse NotAuthenticated = new SmtpResponse("530", "5.7.1", new string[]
		{
			"Not authenticated"
		});

		internal static readonly SmtpResponse NotAuthorized = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Not authorized"
		});

		internal static readonly SmtpResponse Quit = new SmtpResponse("221", "2.0.0", new string[]
		{
			"Service closing transmission channel"
		});

		internal static readonly SmtpResponse RcptRelayNotPermitted = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to relay"
		});

		internal static readonly SmtpResponse Reset = new SmtpResponse("250", "2.0.0", new string[]
		{
			"Resetting"
		});

		internal static readonly SmtpResponse XSessionParamsOk = new SmtpResponse("250", "2.0.0", new string[]
		{
			"XSESSIONPARAMS OK"
		});

		internal static readonly SmtpResponse DomainSecureDisabled = new SmtpResponse("451", "4.7.3", new string[]
		{
			"The admin has temporarily disallowed this secure domain"
		});

		internal static readonly SmtpResponse RequireTLSToSendMail = new SmtpResponse("451", "5.7.3", new string[]
		{
			"STARTTLS is required to send mail"
		});

		internal static readonly SmtpResponse RequireBasicAuthentication = new SmtpResponse("451", "5.7.3", new string[]
		{
			"Require basic authentication"
		});

		internal static readonly SmtpResponse RequireSTARTTLSToAuth = new SmtpResponse("451", "5.7.3", new string[]
		{
			"Require STARTTLS to authenticate"
		});

		internal static readonly SmtpResponse RequireSTARTTLSToBasicAuth = new SmtpResponse("451", "5.7.3", new string[]
		{
			"Require STARTTLS to do basic authentication"
		});

		internal static readonly SmtpResponse RequireXOorgToSendMail = new SmtpResponse("451", "5.7.3", new string[]
		{
			"Require XOORG extension to send mail"
		});

		internal static readonly SmtpResponse CannotExchangeAuthenticate = new SmtpResponse("451", "5.7.3", new string[]
		{
			"Cannot achieve Exchange Server authentication"
		});

		internal static readonly SmtpResponse InternalTransportCertificateNotAvailable = new SmtpResponse("451", "5.7.3", new string[]
		{
			"Cannot load Internal Transport Certificate. Exchange Server authentication failed."
		});

		internal static readonly SmtpResponse StartData = new SmtpResponse("354", string.Empty, new string[]
		{
			"Start mail input; end with <CRLF>.<CRLF>"
		});

		internal static readonly SmtpResponse StartTlsAlreadyNegotiated = new SmtpResponse("503", "5.5.2", new string[]
		{
			"Only one STARTTLS command can be issued per session"
		});

		internal static readonly SmtpResponse StartTlsNegotiationFailure = new SmtpResponse("554", "5.7.3", new string[]
		{
			"Unable to initialize security subsystem"
		});

		internal static readonly SmtpResponse StartTlsReadyToNegotiate = new SmtpResponse("220", "2.0.0", new string[]
		{
			"SMTP server ready"
		});

		internal static readonly SmtpResponse StartTlsTempReject = new SmtpResponse("454", "4.7.0", new string[]
		{
			"Cannot accept TLS as maximum TLS rate exceeded for server"
		});

		internal static readonly SmtpResponse TooManyAuthenticationErrors = new SmtpResponse("421", "4.7.0", new string[]
		{
			"Too many errors on this connection, closing transmission channel"
		});

		internal static readonly SmtpResponse TooManyProtocolErrors = new SmtpResponse("421", "4.7.0", new string[]
		{
			"Too many errors on this connection, closing transmission channel"
		});

		internal static readonly SmtpResponse TooManyConnections = new SmtpResponse("421", "4.3.2", new string[]
		{
			"The maximum number of concurrent server connections has exceeded a limit, closing transmission channel"
		});

		internal static readonly SmtpResponse TooManyConnectionsPerSource = new SmtpResponse("421", "4.3.2", new string[]
		{
			"The maximum number of concurrent connections has exceeded a limit, closing transmission channel"
		});

		internal static readonly SmtpResponse TooManyRecipients = new SmtpResponse("452", "4.5.3", new string[]
		{
			"Too many recipients"
		});

		internal static readonly SmtpResponse MessageExpired = new SmtpResponse("500", "4.4.7", new string[]
		{
			"Message expired"
		});

		internal static readonly SmtpResponse MessageDelayed = new SmtpResponse("400", "4.4.7", new string[]
		{
			"Message delayed"
		});

		internal static readonly SmtpResponse MessageDelayedConcurrentDeliveries = new SmtpResponse("400", "4.4.7", new string[]
		{
			"Message delayed, too many concurrent deliveries at this time"
		});

		internal static readonly SmtpResponse UnableToConnect = new SmtpResponse("421", "4.2.1", new string[]
		{
			"Unable to connect"
		});

		internal static readonly SmtpResponse UnsupportedBodyType = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Unsupported BODY type"
		});

		internal static readonly SmtpResponse Xexch50Success = new SmtpResponse("250", "2.0.0", new string[]
		{
			"XEXCH50 OK"
		});

		internal static readonly SmtpResponse Xexch50SendBlob = new SmtpResponse("354", string.Empty, new string[]
		{
			"Send binary data"
		});

		internal static readonly SmtpResponse Xexch50Error = new SmtpResponse("500", "5.5.2", new string[]
		{
			"Error processing XEXCH50 command"
		});

		internal static readonly SmtpResponse XMessageContextError = new SmtpResponse("400", "4.5.1", new string[]
		{
			"Error processing BDAT blob with Message context information"
		});

		internal static readonly SmtpResponse XMessageEPropNotFoundInMailCommand = new SmtpResponse("400", "4.5.2", new string[]
		{
			"Did not receive mandatory Extended Properties XMESSAGECONTEXT blob in MAIL command"
		});

		internal static readonly SmtpResponse Xexch50NotEnabled = new SmtpResponse("500", "5.5.1", new string[]
		{
			"Not enabled"
		});

		internal static readonly SmtpResponse Xexch50InvalidCommand = new SmtpResponse("501", "5.5.4", new string[]
		{
			"XEXCH50 Invalid command format"
		});

		internal static readonly SmtpResponse Xexch50NotAuthorized = new SmtpResponse("504", "5.7.1", new string[]
		{
			"Not authorized"
		});

		internal static readonly SmtpResponse OrarNotAuthorized = new SmtpResponse("504", "5.7.1", new string[]
		{
			"Not authorized to send ORAR"
		});

		internal static readonly SmtpResponse RDstNotAuthorized = new SmtpResponse("504", "5.7.1", new string[]
		{
			"Not authorized to send RDST"
		});

		internal static readonly SmtpResponse UnableToAcceptAnonymousSession = new SmtpResponse("530", "5.7.1", new string[]
		{
			"Client was not authenticated"
		});

		internal static readonly SmtpResponse RoutingLoopDetected = new SmtpResponse("500", "5.4.6", new string[]
		{
			"Routing Loop Detected"
		});

		internal static readonly SmtpResponse UnableToVrfyUser = new SmtpResponse("252", "2.1.5", new string[]
		{
			"Cannot VRFY user"
		});

		internal static readonly SmtpResponse LongAddress = new SmtpResponse("501", "5.1.3", new string[]
		{
			"Long addresses not supported"
		});

		internal static readonly SmtpResponse SmtpUtf8ArgumentNotProvided = new SmtpResponse("501", "5.1.6", new string[]
		{
			"SMTPUTF8 argument required."
		});

		internal static readonly SmtpResponse LongSenderAddress = new SmtpResponse("501", "5.1.7", new string[]
		{
			"Long addresses not supported"
		});

		internal static readonly SmtpResponse Utf8Address = new SmtpResponse("501", "5.1.8", new string[]
		{
			"UTF-8 addresses not supported"
		});

		internal static readonly SmtpResponse Utf8SenderAddress = new SmtpResponse("501", "5.1.9", new string[]
		{
			"UTF-8 addresses not supported"
		});

		internal static readonly SmtpResponse NtlmSupported = new SmtpResponse("334", string.Empty, new string[]
		{
			"NTLM supported"
		});

		internal static readonly SmtpResponse GssapiSupported = new SmtpResponse("334", string.Empty, new string[]
		{
			"GSSAPI supported"
		});

		internal static readonly SmtpResponse QueueSuspended = new SmtpResponse("400", "4.0.0", new string[]
		{
			"Message delivery delayed due to a suspended delivery queue"
		});

		internal static readonly SmtpResponse QueueLarge = new SmtpResponse("400", "4.0.0", new string[]
		{
			"Message delivery delayed due to a large queue"
		});

		internal static readonly SmtpResponse InterceptorPermanentlyRejectedMessage = new SmtpResponse("530", "5.3.1", new string[]
		{
			"Intercepted and rejected by administrative action. PRJCT"
		});

		internal static readonly SmtpResponse InterceptorTransientlyRejectedMessage = new SmtpResponse("430", "4.3.1", new string[]
		{
			"Intercepted and rejected by administrative action. TRJCT"
		});

		internal static readonly SmtpResponse AgentDiscardedMessage = new SmtpResponse("550", "4.3.2", new string[]
		{
			"Discarded by administrative action. DROP"
		});

		internal static readonly SmtpResponse TooManyRelatedErrors = new SmtpResponse("530", "5.3.0", new string[]
		{
			"Too many related errors"
		});

		internal static readonly SmtpResponse RecipientDeferredNoMdb = new SmtpResponse("420", "4.2.0", new string[]
		{
			"Recipient deferred because there is no Mdb"
		});

		internal static readonly SmtpResponse ProbeMessageDropped = new SmtpResponse("250", string.Empty, new string[]
		{
			"Probe message accepted and dropped"
		});

		internal static readonly SmtpResponse SocketError = new SmtpResponse("441", "4.4.1", new string[]
		{
			"Socket error"
		});

		internal static readonly SmtpResponse ConnectionFailover = new SmtpResponse("451", "4.4.0", new string[]
		{
			"Connection failover"
		});

		internal static readonly SmtpResponse HubAttributionFailureInEOH = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to Relay. ATTR11"
		});

		internal static readonly SmtpResponse HubAttributionTransientFailureInEOH = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to fetch attribution data. ATTR11"
		});

		internal static readonly SmtpResponse ProxyAttributionFailureInEOH = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to Relay. ATTR21"
		});

		internal static readonly SmtpResponse ProxyAttributionTransientFailureinEOH = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to fetch attribution data. ATTR21"
		});

		internal static readonly SmtpResponse HubAttributionTransientFailureInEOHFallback = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to fetch attribution data. ATTR13"
		});

		internal static readonly SmtpResponse ProxyAttributionTransientFailureInEOHFallback = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to fetch attribution data. ATTR23"
		});

		internal static readonly SmtpResponse InboundProxyDestinationTrackerReject = new SmtpResponse("421", "4.3.2", new string[]
		{
			"The maximum number of concurrent server connections has exceeded a limit, closing transmission channel PRX7"
		});

		internal static readonly SmtpResponse HubRecipientCacheCreationFailureInEOH = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to Relay. ATTR12"
		});

		internal static readonly SmtpResponse HubRecipientCacheCreationTransientFailureInEOH = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to Relay. ATTR12"
		});

		internal static readonly SmtpResponse ProxyRecipientCacheCreationFailureInEOH = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to Relay. ATTR22"
		});

		internal static readonly SmtpResponse ProxyRecipientCacheCreationTransientFailureInEOH = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to Relay. ATTR22"
		});

		internal static readonly SmtpResponse HubAttributionFailureInMailFrom = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to Relay. ATTR14"
		});

		internal static readonly SmtpResponse HubAttributionTransientFailureInMailFrom = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to Relay. ATTR14"
		});

		internal static readonly SmtpResponse HubAttributionFailureInCreateTmi = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Unable to Relay. ATTR15"
		});

		internal static readonly SmtpResponse HubAttributionTransientFailureInCreateTmi = new SmtpResponse("450", "4.7.1", new string[]
		{
			"Unable to Relay. ATTR15"
		});

		internal static readonly SmtpResponse RequiredArgumentsNotPresent = new SmtpResponse("501", "5.5.4", new string[]
		{
			"Required arguments not present"
		});

		internal static readonly SmtpResponse UserLookupFailed = new SmtpResponse("250", null, new string[]
		{
			"XProxy accepted but user name could not be resolved"
		});

		internal static readonly SmtpResponse UnableToObtainIdentity = new SmtpResponse("250", null, new string[]
		{
			"XProxy accepted but user identity could not be obtained"
		});

		internal static readonly SmtpResponse XProxyAcceptedAuthenticated = new SmtpResponse("250", null, new string[]
		{
			"XProxy accepted and authenticated"
		});

		internal static readonly SmtpResponse XProxyAccepted = new SmtpResponse("250", null, new string[]
		{
			"XProxy accepted"
		});

		internal static readonly SmtpResponse NoDestinationsReceivedResponse = new SmtpResponse("451", "4.5.0", new string[]
		{
			"Destinations need to be sent for proxying"
		});

		internal static readonly SmtpResponse SuccessfulResponse = new SmtpResponse("250", null, new string[]
		{
			"XProxyTo accepted"
		});

		internal static readonly SmtpResponse GenericProxyFailure = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later"
		});

		internal static readonly SmtpResponse ProxyDiscardingMessage = new SmtpResponse("451", "4.7.0", new string[]
		{
			"The proxy layer is discarding data"
		});

		internal static readonly SmtpResponse XProxyAlreadySpecified = new SmtpResponse("503", "5.5.0", new string[]
		{
			"XProxy already sent"
		});

		internal static readonly SmtpResponse UnableToProxyIntegratedAuthResponse = new SmtpResponse("535", "5.7.3", new string[]
		{
			"Unable to proxy authenticated session because either the backend does not support it or failed to resolve the user"
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseNoDestinations = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. PRX1 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseDnsError = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. PRX2 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseConnectionFailure = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. PRX3 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseProtocolError = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. PRX4 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseSocketError = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. PRX5 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseShutdown = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. PRX6 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseBackEndLocatorFailure = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. CPRX1 "
		});

		internal static readonly SmtpResponse EncodedProxyFailureResponseUserLookupFailure = new SmtpResponse("451", "4.7.0", new string[]
		{
			"Temporary server error. Please try again later. CPRX2 "
		});

		internal static readonly SmtpResponse AuthenticationFailureTenantLockedOut = new SmtpResponse("535", "5.7.3", new string[]
		{
			"Authentication unsuccessful. Tenant locked out"
		});

		internal static readonly SmtpResponse AuthenticationFailureUserNotFound = new SmtpResponse("535", "5.7.3", new string[]
		{
			"Authentication unsuccessful. User not found"
		});

		internal static readonly SmtpResponse NoProxyDestinationsResponse = new SmtpResponse("451", "4.7.0", new string[]
		{
			"No proxy destinations could be obtained"
		});

		internal static readonly SmtpResponse SuccessNoNewConnectionResponse = new SmtpResponse("250", null, new string[]
		{
			"Success; no new connection requested"
		});

		internal static readonly SmtpResponse MessageNotProxiedResponse = new SmtpResponse("450", "4.7.0", new string[]
		{
			"Message not proxied"
		});

		internal static readonly SmtpResponse ProxySessionProtocolSetupPermanentFailure = new SmtpResponse("550", "5.7.0", new string[]
		{
			"Proxy session setup failed on Frontend with "
		});

		internal static readonly SmtpResponse ProxySessionProtocolSetupTransientFailure = new SmtpResponse("450", "4.7.0", new string[]
		{
			"Proxy session setup failed on Frontend with "
		});

		internal static readonly SmtpResponse GssapiSmtpResponseToLog334 = new SmtpResponse("334", null, new string[]
		{
			"<authentication information>"
		});

		internal static readonly SmtpResponse GssapiSmtpResponseToLog235 = new SmtpResponse("235", null, new string[]
		{
			"<authentication information>"
		});

		internal static readonly SmtpResponse RequireXProxy = new SmtpResponse("451", "4.5.0", new string[]
		{
			"Require XPROXY"
		});

		internal static readonly SmtpResponse RequireXProxyTo = new SmtpResponse("451", "4.5.0", new string[]
		{
			"Require XPROXYTO"
		});

		internal static readonly SmtpResponse RequireMatchingEhloOptions = new SmtpResponse("451", "4.5.0", new string[]
		{
			"Require EHLO options to match"
		});

		internal static readonly SmtpResponse RequireEhloToSendMail = new SmtpResponse("451", "4.5.0", new string[]
		{
			"Require EHLO to send mail"
		});

		internal static readonly SmtpResponse RequireAnonymousTlsToSendMail = new SmtpResponse("451", "4.5.0", new string[]
		{
			"Require XAnonymousTls to send mail"
		});

		internal static readonly SmtpResponse EhloOptionsDoNotMatchForProxy = new SmtpResponse("451", "4.7.0", new string[]
		{
			"EHLO Options do not match for proxy"
		});

		internal static readonly SmtpResponse XProxyFromAccepted = new SmtpResponse("250", null, new string[]
		{
			"XProxyFrom accepted"
		});

		internal static readonly SmtpResponse DnsQueryFailedResponseDefault = new SmtpResponse("451", "4.4.0", new string[]
		{
			"DNS query failed"
		});

		internal static readonly SmtpResponse DnsConfigChangedSmtpResponse = new SmtpResponse("554", "5.4.4", new string[]
		{
			"Configuration changed"
		});

		internal static readonly SmtpResponse UnexpectedExceptionHandlingNewOutboundConnection = new SmtpResponse("421", null, new string[]
		{
			"Unexpected exception occurred when trying to handle a new SMTP outbound connection."
		});

		internal static readonly SmtpResponse XProxyToRequired = new SmtpResponse("451", "4.7.0", new string[]
		{
			"XPROXYTO is required to send mail "
		});

		internal static readonly SmtpResponse AuthenticationFailedTemporary = new SmtpResponse("451", "4.7.3", new string[]
		{
			"Authentication unsuccessful due to temporary server error. Please try again later."
		});

		internal static readonly SmtpResponse AuthenticationFailedPermanent = new SmtpResponse("535", "5.7.3", new string[]
		{
			"Authentication unsuccessful due to permanent server error."
		});

		internal static readonly SmtpResponse CheckTenantLockedOutFailedTemporary = new SmtpResponse("451", "4.7.3", new string[]
		{
			"Checking tenant locked out state unsuccessful due to temporary server error. Please try again later."
		});

		internal static readonly SmtpResponse CheckTenantLockedOutFailedPermanent = new SmtpResponse("535", "5.7.3", new string[]
		{
			"Checking tenant locked out state unsuccessful due to permanent server error."
		});

		private readonly bool empty;

		private string[] data;

		private string[] statusText;

		private byte[] bytes;

		internal struct TenantAttribution
		{
			public static SmtpResponse GetInvalidChannelRejectResponse(string failureReason)
			{
				string text;
				if (string.IsNullOrEmpty(failureReason))
				{
					text = "Failed to establish appropriate TLS channel: Access Denied";
				}
				else
				{
					text = string.Format("Failed to establish appropriate TLS channel: {0}: Access Denied", failureReason);
				}
				return new SmtpResponse("454", "4.7.0", new string[]
				{
					text
				});
			}

			public static readonly SmtpResponse UnattributableMailRejectSmtpResponse = new SmtpResponse("550", "5.7.0", new string[]
			{
				"Relay Access Denied"
			});

			public static readonly SmtpResponse RelayNotAllowedRejectSmtpResponse = new SmtpResponse("550", "5.7.1", new string[]
			{
				"Relay Access Denied ATTR1"
			});

			public static readonly SmtpResponse UnAuthorizedMessageOverIPv6 = new SmtpResponse("550", "5.7.1", new string[]
			{
				"Unable to accept unauthorized message over IPv6 ATTR2"
			});

			public static readonly SmtpResponse RecipientBelongsToDifferentDomainThanPreviouslyAttributedRejectSmtpResponse = new SmtpResponse("452", "4.5.3", new string[]
			{
				"Too many recipients"
			});

			public static readonly SmtpResponse MissingAttributionHeaderSmtpResponse = new SmtpResponse("451", "4.4.0", new string[]
			{
				"Temporary server error. Please try again later"
			});

			public static readonly SmtpResponse AppConfigFfoHubMissingSmtpResponse = new SmtpResponse("451", "4.4.4", new string[]
			{
				"Temporary server error. Please try again later ATTR1"
			});

			public static readonly SmtpResponse ExoSmtpNextHopDomainMissingForHostedCustomerSmtpResponse = new SmtpResponse("451", "4.4.4", new string[]
			{
				"Temporary server error. Please try again later ATTR2"
			});

			public static readonly SmtpResponse GlsMissingTenantPropertiesSmtpResponse = new SmtpResponse("451", "4.4.4", new string[]
			{
				"Temporary server error. Please try again later ATTR3"
			});

			public static readonly SmtpResponse UnknownCustomerTypeSmtpResponse = new SmtpResponse("451", "4.4.4", new string[]
			{
				"Temporary server error. Please try again later ATTR5"
			});

			public static readonly SmtpResponse DirectoryRequestOverThresholdSmtpResponse = new SmtpResponse("451", "4.3.2", new string[]
			{
				"Temporary server error. Please try again later ATTR1"
			});

			public static readonly SmtpResponse DirectoryRequestFailureSmtpResponse = new SmtpResponse("451", "4.4.3", new string[]
			{
				"Temporary server error. Please try again later ATTR1"
			});

			public static readonly SmtpResponse GlsRequestOverThresholdSmtpResponse = new SmtpResponse("451", "4.3.2", new string[]
			{
				"Temporary server error. Please try again later ATTR2"
			});

			public static readonly SmtpResponse GlsRequestErrorSmtpResponse = new SmtpResponse("451", "4.4.3", new string[]
			{
				"Temporary server error. Please try again later ATTR2"
			});

			public static readonly SmtpResponse DefaultDomainQueryErrorSmtpResponse = new SmtpResponse("451", "4.4.3", new string[]
			{
				"Temporary server error. Please try again later ATTR3.1"
			});

			public static readonly SmtpResponse DefaultDomainQueryErrorInEOHSmtpResponse = new SmtpResponse("451", "4.4.3", new string[]
			{
				"Temporary server error. Please try again later ATTR3.2"
			});

			public static readonly SmtpResponse UpdateScopeAndDirectoryFailureSmtpResponse = new SmtpResponse("451", "4.4.3", new string[]
			{
				"Temporary server error. Please try again later ATTR6"
			});

			public static readonly SmtpResponse HopCountExceededAttribution = new SmtpResponse("554", "5.4.6", new string[]
			{
				"Hop count exceeded - possible mail loop ATTR1"
			});
		}
	}
}
