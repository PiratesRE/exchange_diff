using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	[Serializable]
	public sealed class InterceptorAgentCondition : IComparable<InterceptorAgentCondition>, IEquatable<InterceptorAgentCondition>
	{
		public InterceptorAgentCondition(string header, InterceptorAgentConditionMatchType headerMatchType, string value, InterceptorAgentConditionMatchType valueMatchType) : this(InterceptorAgentConditionType.HeaderValue, value, valueMatchType)
		{
			if (string.IsNullOrEmpty(header))
			{
				throw new ArgumentNullException("header");
			}
			if (this.headerType == InterceptorAgentConditionMatchType.Regex)
			{
				this.headerRegex = new Regex(this.header);
			}
			this.header = header;
			this.headerType = headerMatchType;
		}

		public InterceptorAgentCondition(ServerVersion serverVersion, string str, InterceptorAgentConditionMatchType matchType) : this(InterceptorAgentConditionType.ServerVersion, str, matchType)
		{
			this.serverVersion = serverVersion;
			string message;
			if (!InterceptorAgentCondition.ValidateMatchTypeForServiceVersion(matchType, out message))
			{
				throw new ArgumentException(message);
			}
		}

		public InterceptorAgentCondition(InterceptorAgentConditionType cond, string str, InterceptorAgentConditionMatchType matchType)
		{
			if (string.IsNullOrEmpty(str))
			{
				throw new ArgumentNullException("str");
			}
			switch (cond)
			{
			case InterceptorAgentConditionType.MessageSubject:
			case InterceptorAgentConditionType.EnvelopeFrom:
			case InterceptorAgentConditionType.EnvelopeTo:
			case InterceptorAgentConditionType.MessageId:
			case InterceptorAgentConditionType.HeaderValue:
			case InterceptorAgentConditionType.SmtpClientHostName:
			case InterceptorAgentConditionType.ProcessRole:
			case InterceptorAgentConditionType.ServerVersion:
			case InterceptorAgentConditionType.TenantId:
			case InterceptorAgentConditionType.Directionality:
			case InterceptorAgentConditionType.AccountForest:
				switch (matchType)
				{
				case InterceptorAgentConditionMatchType.CaseInsensitive:
				case InterceptorAgentConditionMatchType.CaseSensitive:
				case InterceptorAgentConditionMatchType.CaseSensitiveEqual:
				case InterceptorAgentConditionMatchType.CaseInsensitiveEqual:
				case InterceptorAgentConditionMatchType.CaseSensitiveNotEqual:
				case InterceptorAgentConditionMatchType.CaseInsensitiveNotEqual:
				case InterceptorAgentConditionMatchType.PatternMatch:
				case InterceptorAgentConditionMatchType.GreaterThan:
				case InterceptorAgentConditionMatchType.GreaterThanOrEqual:
				case InterceptorAgentConditionMatchType.LessThan:
				case InterceptorAgentConditionMatchType.LessThanOrEqual:
					this.matchString = str;
					break;
				case InterceptorAgentConditionMatchType.Regex:
					this.matchRegex = new Regex(str);
					this.matchString = this.matchRegex.ToString();
					break;
				default:
					throw new ArgumentException(string.Format("Unrecognized match type '{0}'", matchType), "matchType");
				}
				this.isMatchStringRedacted = (Util.IsDataRedactionNecessary() && SuppressingPiiData.ContainsRedactedValue(str));
				this.field = cond;
				this.type = matchType;
				return;
			default:
				throw new ArgumentException(string.Format("Unrecognized condition type '{0}'", cond), "cond");
			}
		}

		internal InterceptorAgentCondition(ProcessTransportRole[] processRoles, string str, InterceptorAgentConditionMatchType matchType) : this(InterceptorAgentConditionType.ProcessRole, str, matchType)
		{
			this.processRoles = processRoles;
			string message;
			if (!InterceptorAgentCondition.ValidateMatchTypeForProcessRole(matchType, out message))
			{
				throw new ArgumentException(message);
			}
		}

		private InterceptorAgentCondition()
		{
		}

		[XmlAttribute("property")]
		public InterceptorAgentConditionType Property
		{
			get
			{
				return this.field;
			}
			set
			{
				this.field = value;
			}
		}

		[XmlAttribute("matchType")]
		public InterceptorAgentConditionMatchType MatchType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		[XmlAttribute("value")]
		public string MatchString
		{
			get
			{
				return this.matchString;
			}
			set
			{
				this.matchString = value;
				this.isMatchStringRedacted = (Util.IsDataRedactionNecessary() && SuppressingPiiData.ContainsRedactedValue(value));
			}
		}

		[XmlAttribute("header")]
		public string HeaderName
		{
			get
			{
				return this.header;
			}
			set
			{
				this.header = value;
			}
		}

		[XmlAttribute("headerMatchType")]
		public InterceptorAgentConditionMatchType HeaderMatchType
		{
			get
			{
				return this.headerType;
			}
			set
			{
				this.headerType = value;
			}
		}

		internal static string[] AllConditions
		{
			get
			{
				return InterceptorAgentCondition.allConditionNames;
			}
		}

		internal static string[] AllMailDirectionalities
		{
			get
			{
				return InterceptorAgentCondition.allMailDirectionalityNames;
			}
		}

		private Regex MatchRegex
		{
			get
			{
				if (this.matchRegex == null && this.MatchType == InterceptorAgentConditionMatchType.Regex)
				{
					this.matchRegex = new Regex(this.matchString);
				}
				return this.matchRegex;
			}
		}

		private Regex HeaderRegex
		{
			get
			{
				if (this.headerRegex == null && this.HeaderMatchType == InterceptorAgentConditionMatchType.Regex)
				{
					this.headerRegex = new Regex(this.HeaderName);
				}
				return this.headerRegex;
			}
		}

		public bool Equals(InterceptorAgentCondition other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as InterceptorAgentCondition);
		}

		public override int GetHashCode()
		{
			int num = (int)this.Property;
			num ^= (int)this.MatchType;
			num ^= ((this.MatchString != null) ? this.MatchString.GetHashCode() : 0);
			if (this.Property == InterceptorAgentConditionType.HeaderValue)
			{
				num ^= ((this.HeaderName != null) ? this.HeaderName.GetHashCode() : 0);
				num ^= (int)this.HeaderMatchType;
			}
			return num;
		}

		public int CompareTo(InterceptorAgentCondition other)
		{
			if (other == null)
			{
				return 1;
			}
			if (object.ReferenceEquals(this, other))
			{
				return 0;
			}
			if (this.Property != other.Property)
			{
				return this.Property - other.Property;
			}
			if (this.MatchType != other.MatchType)
			{
				return this.MatchType - other.MatchType;
			}
			if (this.MatchString != other.MatchString)
			{
				return string.Compare(this.MatchString, other.MatchString, StringComparison.InvariantCulture);
			}
			if (this.Property == InterceptorAgentConditionType.HeaderValue && this.HeaderName != other.HeaderName)
			{
				return string.Compare(this.HeaderName, other.HeaderName, StringComparison.InvariantCulture);
			}
			if (this.Property == InterceptorAgentConditionType.HeaderValue && this.HeaderMatchType != other.HeaderMatchType)
			{
				return this.HeaderMatchType - other.HeaderMatchType;
			}
			return 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal static bool PatternMatch(string pattern, string value)
		{
			if (!pattern.Contains("*"))
			{
				return value.Equals(pattern);
			}
			if (pattern.StartsWith("*") && pattern.EndsWith("*"))
			{
				return InterceptorAgentCondition.InnerPatternMatch(pattern.Trim(new char[]
				{
					'*'
				}), value);
			}
			if (!pattern.StartsWith("*"))
			{
				int num = pattern.IndexOf('*');
				return value.StartsWith(pattern.Substring(0, num), StringComparison.InvariantCultureIgnoreCase) && InterceptorAgentCondition.PatternMatch(pattern.Substring(num, pattern.Length - num), value.Substring(num));
			}
			int num2 = pattern.LastIndexOf('*');
			return value.EndsWith(pattern.Substring(num2 + 1), StringComparison.InvariantCultureIgnoreCase) && InterceptorAgentCondition.PatternMatch(pattern.Substring(0, num2 + 1), value.Substring(0, value.Length - pattern.Length + num2 + 1));
		}

		internal static bool ValidateProcessRole(InterceptorAgentConditionMatchType type, string matchString, out ProcessTransportRole[] roles, out string error)
		{
			roles = null;
			if (!InterceptorAgentCondition.ValidateMatchTypeForProcessRole(type, out error))
			{
				return false;
			}
			string[] array = matchString.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				error = "ProcessRole not specified in condition";
				return false;
			}
			List<ProcessTransportRole> list = new List<ProcessTransportRole>();
			foreach (string text in array)
			{
				ProcessTransportRole item;
				if (!EnumValidator.TryParse<ProcessTransportRole>(text, EnumParseOptions.IgnoreCase, out item))
				{
					error = string.Format("Invalid ProcessRole {0} in condition. Allowed values are {1}", text, string.Join(",", Enum.GetNames(typeof(ProcessTransportRole))));
					return false;
				}
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			roles = list.ToArray();
			error = null;
			return true;
		}

		internal static bool ValidateServerVersion(InterceptorAgentConditionMatchType type, string matchString, out ServerVersion serverVersion, out string error)
		{
			serverVersion = null;
			if (!InterceptorAgentCondition.ValidateMatchTypeForServiceVersion(type, out error))
			{
				return false;
			}
			if (!ServerVersion.TryParseFromSerialNumber(matchString, out serverVersion))
			{
				Version version;
				if (!Version.TryParse(matchString, out version))
				{
					error = string.Format("Invalid serial number {0} in condition. Serial Number should be a valid ServerVersion like 15.00.0649.000 or Version 15.0 (Build 649.0)", matchString);
					return false;
				}
				serverVersion = new ServerVersion(version.Major, version.Minor, version.Build, version.Revision);
			}
			error = null;
			return true;
		}

		internal static bool ValidateCondition(InterceptorAgentConditionType conditionType, InterceptorAgentConditionMatchType matchType, out string error)
		{
			if (conditionType != InterceptorAgentConditionType.ServerVersion)
			{
				switch (matchType)
				{
				case InterceptorAgentConditionMatchType.GreaterThan:
				case InterceptorAgentConditionMatchType.GreaterThanOrEqual:
				case InterceptorAgentConditionMatchType.LessThan:
				case InterceptorAgentConditionMatchType.LessThanOrEqual:
					error = "GreaterThan, GreaterThanOrEqual, LessThan and LessThanOrEqual are only allowed for Condition ServerVersion";
					return false;
				}
			}
			error = null;
			return true;
		}

		internal static bool ValidateConditionTypeValue(InterceptorAgentConditionType conditionType, string value, out string error)
		{
			if (string.IsNullOrEmpty(value))
			{
				error = InterceptorAgentStrings.ConditionTypeValueCannotBeNullOrEmpty;
				return false;
			}
			switch (conditionType)
			{
			case InterceptorAgentConditionType.TenantId:
			{
				Guid guid;
				if (!Guid.TryParse(value, out guid))
				{
					error = InterceptorAgentStrings.ConditionTypeValueInvalidTenantIdGuid;
					return false;
				}
				break;
			}
			case InterceptorAgentConditionType.Directionality:
			{
				MailDirectionality mailDirectionality;
				if (!InterceptorAgentCondition.ValidateDirectionality(value, out mailDirectionality))
				{
					error = InterceptorAgentStrings.ConditionTypeValueInvalidDirectionalityType(string.Join(",", InterceptorAgentCondition.AllMailDirectionalities));
					return false;
				}
				break;
			}
			}
			error = null;
			return true;
		}

		internal static bool ValidateDirectionality(string value, out MailDirectionality directionality)
		{
			return EnumValidator.TryParse<MailDirectionality>(value, EnumParseOptions.AllowNumericConstants | EnumParseOptions.IgnoreCase, out directionality);
		}

		internal bool MatchRoleAndServerVersion(ProcessTransportRole role, ServerVersion version)
		{
			if (this.field == InterceptorAgentConditionType.ProcessRole)
			{
				if (this.processRoles == null)
				{
					throw new InvalidOperationException("ProcessRoles is not initialized in InterceptorAgentCondition");
				}
				foreach (ProcessTransportRole processTransportRole in this.processRoles)
				{
					if (role == processTransportRole)
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				if (this.field != InterceptorAgentConditionType.ServerVersion)
				{
					return true;
				}
				if (this.serverVersion == null)
				{
					throw new InvalidOperationException("ServerVersion is not initialized in InterceptorAgentCondition");
				}
				int num = ServerVersion.Compare(version, this.serverVersion);
				switch (this.type)
				{
				case InterceptorAgentConditionMatchType.CaseSensitiveEqual:
				case InterceptorAgentConditionMatchType.CaseInsensitiveEqual:
					return num == 0;
				case InterceptorAgentConditionMatchType.GreaterThan:
					return num > 0;
				case InterceptorAgentConditionMatchType.GreaterThanOrEqual:
					return num >= 0;
				case InterceptorAgentConditionMatchType.LessThan:
					return num < 0;
				case InterceptorAgentConditionMatchType.LessThanOrEqual:
					return num <= 0;
				}
				return false;
			}
		}

		internal bool IsMatch(string subject, string envelopeFrom, string messageId, EnvelopeRecipientCollection recipients, RoutingAddress envelopeTo, HeaderList headers, string smtpClientHostName, Guid tenantId, MailDirectionality directionality, string accountForest)
		{
			switch (this.field)
			{
			case InterceptorAgentConditionType.MessageSubject:
				return this.Match(this.isMatchStringRedacted ? Util.Redact(subject.ToUpperInvariant()) : subject);
			case InterceptorAgentConditionType.EnvelopeFrom:
				return this.Match(this.isMatchStringRedacted ? Util.Redact(envelopeFrom) : envelopeFrom);
			case InterceptorAgentConditionType.EnvelopeTo:
				if (envelopeTo.IsValid)
				{
					return this.Match(this.isMatchStringRedacted ? Util.Redact(envelopeTo) : envelopeTo.ToString());
				}
				foreach (EnvelopeRecipient envelopeRecipient in recipients)
				{
					if (this.Match(this.isMatchStringRedacted ? Util.Redact(envelopeRecipient.Address) : envelopeRecipient.Address.ToString()))
					{
						return true;
					}
				}
				return false;
			case InterceptorAgentConditionType.MessageId:
				return this.Match(messageId);
			case InterceptorAgentConditionType.HeaderValue:
				return this.MatchHeader(headers);
			case InterceptorAgentConditionType.SmtpClientHostName:
				return this.Match(smtpClientHostName);
			case InterceptorAgentConditionType.ProcessRole:
			case InterceptorAgentConditionType.ServerVersion:
				return true;
			case InterceptorAgentConditionType.TenantId:
				return this.Match(tenantId.ToString());
			case InterceptorAgentConditionType.Directionality:
				return this.Match(directionality.ToString());
			case InterceptorAgentConditionType.AccountForest:
				return this.Match(accountForest);
			}
			return false;
		}

		internal void Verify()
		{
			try
			{
				if (this.headerType == InterceptorAgentConditionMatchType.Regex && this.HeaderRegex == null)
				{
					throw new InvalidOperationException("Missing header regex for regex matchType");
				}
				if (this.type == InterceptorAgentConditionMatchType.Regex && this.MatchRegex == null)
				{
					throw new InvalidOperationException("Missing value regex for regex matchType");
				}
				string message;
				if (this.field == InterceptorAgentConditionType.ProcessRole && !InterceptorAgentCondition.ValidateProcessRole(this.type, this.matchString, out this.processRoles, out message))
				{
					throw new InvalidOperationException(message);
				}
				if (this.field == InterceptorAgentConditionType.ServerVersion && !InterceptorAgentCondition.ValidateServerVersion(this.type, this.matchString, out this.serverVersion, out message))
				{
					throw new InvalidOperationException(message);
				}
				if (!InterceptorAgentCondition.ValidateCondition(this.field, this.type, out message))
				{
					throw new InvalidOperationException(message);
				}
			}
			catch (ArgumentException)
			{
				throw new InvalidOperationException("Value is not a valid regex");
			}
		}

		internal void ToString(StringBuilder result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			result.AppendFormat("property=\"{0}\" ", this.field);
			if (this.field == InterceptorAgentConditionType.HeaderValue)
			{
				result.AppendFormat("header=\"{0}\" headerMatchType=\"{1}\"", this.header, this.HeaderMatchType);
			}
			result.AppendFormat("value=\"{0}\" matchType=\"{1}\" ", this.matchString, this.type.ToString());
		}

		private static bool InnerPatternMatch(string pattern, string value)
		{
			if (!pattern.Contains("*"))
			{
				return value.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) != -1;
			}
			int num = pattern.IndexOf('*');
			int num2 = value.IndexOf(pattern.Substring(0, num), StringComparison.InvariantCultureIgnoreCase);
			return num2 != -1 && InterceptorAgentCondition.InnerPatternMatch(pattern.Substring(num + 1, pattern.Length - num - 1), value.Substring(num2 + num));
		}

		private static bool ValidateMatchTypeForProcessRole(InterceptorAgentConditionMatchType type, out string error)
		{
			if (type != InterceptorAgentConditionMatchType.CaseSensitiveEqual && type != InterceptorAgentConditionMatchType.CaseInsensitiveEqual)
			{
				error = "CaseSensitiveEqual and CaseInsensitiveEqual are the only match types allowed for condition ProcessRole";
				return false;
			}
			error = null;
			return true;
		}

		private static bool ValidateMatchTypeForServiceVersion(InterceptorAgentConditionMatchType type, out string error)
		{
			switch (type)
			{
			case InterceptorAgentConditionMatchType.CaseSensitiveEqual:
			case InterceptorAgentConditionMatchType.CaseInsensitiveEqual:
			case InterceptorAgentConditionMatchType.GreaterThan:
			case InterceptorAgentConditionMatchType.GreaterThanOrEqual:
			case InterceptorAgentConditionMatchType.LessThan:
			case InterceptorAgentConditionMatchType.LessThanOrEqual:
				error = null;
				return true;
			}
			error = "CaseSensitiveEqual, CaseInsensitiveEqual, GreaterThan, GreaterThanOrEqual, LessThan and LessThanOrEqual are the only match types allowed for condition ServerVersion";
			return false;
		}

		private bool MatchHeaderValue(Header header)
		{
			if (!string.IsNullOrEmpty(header.Value))
			{
				return this.Match(header.Value);
			}
			AddressHeader addressHeader = header as AddressHeader;
			if (addressHeader != null)
			{
				foreach (AddressItem addressItem in addressHeader)
				{
					MimeRecipient mimeRecipient = addressItem as MimeRecipient;
					if (mimeRecipient != null)
					{
						if (this.Match(((RoutingAddress)mimeRecipient.Email).ToString()))
						{
							return true;
						}
					}
					else
					{
						MimeGroup mimeGroup = addressItem as MimeGroup;
						if (mimeGroup != null)
						{
							foreach (MimeRecipient mimeRecipient2 in mimeGroup)
							{
								if (this.Match(((RoutingAddress)mimeRecipient2.Email).ToString()))
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		private bool Match(string toCompare)
		{
			if (toCompare == null)
			{
				return false;
			}
			bool flag;
			switch (this.type)
			{
			case InterceptorAgentConditionMatchType.CaseInsensitive:
			case InterceptorAgentConditionMatchType.CaseInsensitiveEqual:
			case InterceptorAgentConditionMatchType.CaseInsensitiveNotEqual:
				flag = string.Equals(this.matchString, toCompare, StringComparison.InvariantCultureIgnoreCase);
				break;
			case InterceptorAgentConditionMatchType.CaseSensitive:
			case InterceptorAgentConditionMatchType.CaseSensitiveEqual:
			case InterceptorAgentConditionMatchType.CaseSensitiveNotEqual:
				flag = string.Equals(this.matchString, toCompare, StringComparison.InvariantCulture);
				break;
			case InterceptorAgentConditionMatchType.Regex:
				return this.matchRegex.IsMatch(toCompare);
			case InterceptorAgentConditionMatchType.PatternMatch:
				return InterceptorAgentCondition.PatternMatch(this.matchString, toCompare);
			default:
				throw new InvalidOperationException(string.Format("Unrecognized MatchType '{0}'", this.type));
			}
			switch (this.type)
			{
			case InterceptorAgentConditionMatchType.CaseInsensitive:
			case InterceptorAgentConditionMatchType.CaseSensitive:
			case InterceptorAgentConditionMatchType.CaseSensitiveEqual:
			case InterceptorAgentConditionMatchType.CaseInsensitiveEqual:
				return flag;
			case InterceptorAgentConditionMatchType.CaseSensitiveNotEqual:
			case InterceptorAgentConditionMatchType.CaseInsensitiveNotEqual:
				return !flag;
			default:
				return false;
			}
		}

		private bool MatchHeader(HeaderList headers)
		{
			if (headers == null)
			{
				return false;
			}
			if (this.HeaderMatchType == InterceptorAgentConditionMatchType.Regex)
			{
				using (MimeNode.Enumerator<Header> enumerator = headers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Header header = enumerator.Current;
						if (this.headerRegex.IsMatch(header.Name) && this.MatchHeaderValue(header))
						{
							return true;
						}
					}
					return false;
				}
			}
			if (this.HeaderMatchType == InterceptorAgentConditionMatchType.PatternMatch)
			{
				using (MimeNode.Enumerator<Header> enumerator2 = headers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Header header2 = enumerator2.Current;
						if (InterceptorAgentCondition.PatternMatch(this.header, header2.Name) && this.MatchHeaderValue(header2))
						{
							return true;
						}
					}
					return false;
				}
			}
			Header[] array = headers.FindAll(this.header);
			foreach (Header header3 in array)
			{
				if (this.MatchHeaderValue(header3))
				{
					return true;
				}
			}
			return false;
		}

		private static string[] allConditionNames = Enum.GetNames(typeof(InterceptorAgentConditionType));

		private static string[] allMailDirectionalityNames = Enum.GetNames(typeof(MailDirectionality));

		private Regex matchRegex;

		private string header;

		private Regex headerRegex;

		private InterceptorAgentConditionType field;

		private InterceptorAgentConditionMatchType type;

		private InterceptorAgentConditionMatchType headerType;

		private string matchString;

		private bool isMatchStringRedacted;

		private ServerVersion serverVersion;

		private ProcessTransportRole[] processRoles;
	}
}
