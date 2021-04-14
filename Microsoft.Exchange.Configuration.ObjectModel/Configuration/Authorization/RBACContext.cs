using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[Serializable]
	internal sealed class RBACContext : IEquatable<RBACContext>
	{
		private RBACContext(SerializedAccessToken impersonatedUser, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, bool callerCheckedAccess)
		{
			if (impersonatedUser != null)
			{
				this.roleTypeFilter = roleTypeFilter;
				this.sortedRoleEntryFilter = sortedRoleEntryFilter;
				this.logonUserRequiredRoleTypes = logonUserRequiredRoleTypes;
				this.callerCheckedAccess = callerCheckedAccess;
				this.impersonatedUserSddl = impersonatedUser.UserSid;
				this.impersonatedAuthenticationType = impersonatedUser.AuthenticationType;
			}
		}

		public RBACContext(SerializedAccessToken executingUser, SerializedAccessToken impersonatedUser, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, bool callerCheckedAccess) : this(impersonatedUser, roleTypeFilter, sortedRoleEntryFilter, logonUserRequiredRoleTypes, callerCheckedAccess)
		{
			if (executingUser == null)
			{
				throw new ArgumentNullException("executingUser");
			}
			this.serializedExecutingUser = executingUser.ToString();
			this.ExecutingUserName = executingUser.LogonName;
			this.AuthenticationType = executingUser.AuthenticationType;
			this.contextType = RBACContext.RBACContextType.Windows;
		}

		public RBACContext(DelegatedPrincipal executingUser, SerializedAccessToken impersonatedUser, IList<RoleType> roleTypeFilter, List<RoleEntry> sortedRoleEntryFilter, IList<RoleType> logonUserRequiredRoleTypes, bool callerCheckedAccess) : this(impersonatedUser, roleTypeFilter, sortedRoleEntryFilter, logonUserRequiredRoleTypes, callerCheckedAccess)
		{
			if (executingUser == null)
			{
				throw new ArgumentNullException("executingUser");
			}
			this.AuthenticationType = DelegatedPrincipal.DelegatedAuthenticationType;
			this.ExecutingUserName = executingUser.DisplayName;
			this.serializedExecutingUser = executingUser.Identity.Name;
			this.contextType = RBACContext.RBACContextType.Delegated;
		}

		public RBACContext(SerializedAccessToken executingUser) : this(executingUser, null, null, null, null, false)
		{
		}

		public RBACContext(DelegatedPrincipal executingUser) : this(executingUser, null, null, null, null, false)
		{
		}

		public RBACContext(Stream inputStream)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(inputStream))
			{
				this.Deserialize(xmlTextReader);
			}
		}

		public ExchangeRunspaceConfiguration CreateExchangeRunspaceConfiguration()
		{
			IIdentity identity = this.GetExecutingUserIdentity();
			IIdentity identity2 = null;
			if (!string.IsNullOrEmpty(this.impersonatedUserSddl))
			{
				identity2 = new GenericSidIdentity(this.impersonatedUserSddl, this.impersonatedAuthenticationType, new SecurityIdentifier(this.impersonatedUserSddl));
			}
			ExchangeRunspaceConfiguration result;
			if (identity2 == null)
			{
				result = new ExchangeRunspaceConfiguration(identity);
			}
			else
			{
				result = new ExchangeRunspaceConfiguration(identity, identity2, ExchangeRunspaceConfigurationSettings.GetDefaultInstance(), this.roleTypeFilter, this.sortedRoleEntryFilter, this.logonUserRequiredRoleTypes, this.callerCheckedAccess);
			}
			return result;
		}

		public string ExecutingUserName { get; private set; }

		public string AuthenticationType { get; private set; }

		private IIdentity GetExecutingUserIdentity()
		{
			if (this.executingUserIdentity == null)
			{
				switch (this.contextType)
				{
				case RBACContext.RBACContextType.Delegated:
					break;
				case (RBACContext.RBACContextType)3:
					goto IL_69;
				case RBACContext.RBACContextType.Windows:
					using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(this.serializedExecutingUser)))
					{
						this.executingUserIdentity = new SerializedIdentity(new SerializedAccessToken(memoryStream));
						goto IL_69;
					}
					break;
				default:
					goto IL_69;
				}
				this.executingUserIdentity = DelegatedPrincipal.GetDelegatedIdentity(this.serializedExecutingUser);
			}
			IL_69:
			return this.executingUserIdentity;
		}

		public bool Equals(RBACContext other)
		{
			if (other == null)
			{
				return false;
			}
			if (!string.Equals(this.impersonatedUserSddl, other.impersonatedUserSddl, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (this.roleTypeFilter != null && other.roleTypeFilter != null)
			{
				if (this.roleTypeFilter.Count != other.roleTypeFilter.Count)
				{
					return false;
				}
				using (IEnumerator<RoleType> enumerator = this.roleTypeFilter.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RoleType item = enumerator.Current;
						if (!other.roleTypeFilter.Contains(item))
						{
							return false;
						}
					}
					goto IL_99;
				}
			}
			if (this.roleTypeFilter != other.roleTypeFilter)
			{
				return false;
			}
			IL_99:
			if (this.sortedRoleEntryFilter != null && other.sortedRoleEntryFilter != null)
			{
				if (this.sortedRoleEntryFilter.Count != other.sortedRoleEntryFilter.Count)
				{
					return false;
				}
				using (List<RoleEntry>.Enumerator enumerator2 = this.sortedRoleEntryFilter.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						RoleEntry element = enumerator2.Current;
						if (null == other.sortedRoleEntryFilter.FirstOrDefault((RoleEntry x) => x.Equals(element)))
						{
							return false;
						}
					}
					goto IL_142;
				}
			}
			if (this.sortedRoleEntryFilter != other.sortedRoleEntryFilter)
			{
				return false;
			}
			IL_142:
			if (this.logonUserRequiredRoleTypes != null && other.logonUserRequiredRoleTypes != null)
			{
				if (this.logonUserRequiredRoleTypes.Count != other.logonUserRequiredRoleTypes.Count)
				{
					return false;
				}
				using (IEnumerator<RoleType> enumerator3 = this.logonUserRequiredRoleTypes.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						RoleType item2 = enumerator3.Current;
						if (!other.logonUserRequiredRoleTypes.Contains(item2))
						{
							return false;
						}
					}
					goto IL_1BD;
				}
			}
			if (this.logonUserRequiredRoleTypes != other.logonUserRequiredRoleTypes)
			{
				return false;
			}
			IL_1BD:
			return this.callerCheckedAccess == other.callerCheckedAccess && string.Equals(this.serializedExecutingUser, other.serializedExecutingUser, StringComparison.OrdinalIgnoreCase) && string.Equals(this.AuthenticationType, other.AuthenticationType, StringComparison.OrdinalIgnoreCase) && string.Equals(this.ExecutingUserName, other.ExecutingUserName, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					this.Serialize(xmlTextWriter);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		public static bool TryParseRbacContextString(string inputString, out RBACContext context)
		{
			context = null;
			bool result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)))
				{
					context = new RBACContext(memoryStream);
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private void Serialize(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("erccontext");
			xmlWriter.WriteAttributeString("at", this.AuthenticationType);
			xmlWriter.WriteAttributeString("un", this.ExecutingUserName);
			string localName = "t";
			int num = (int)this.contextType;
			xmlWriter.WriteAttributeString(localName, num.ToString());
			this.WriteExecutingUser(xmlWriter);
			this.WriteImpersonatedUser(xmlWriter);
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}

		private void WriteExecutingUser(XmlWriter writer)
		{
			writer.WriteElementString("ex", this.serializedExecutingUser);
		}

		private void WriteImpersonatedUser(XmlWriter writer)
		{
			if (string.IsNullOrEmpty(this.impersonatedUserSddl))
			{
				return;
			}
			writer.WriteStartElement("im");
			writer.WriteAttributeString("sddl", this.impersonatedUserSddl);
			writer.WriteAttributeString("at", this.impersonatedAuthenticationType);
			writer.WriteAttributeString("cca", this.callerCheckedAccess.ToString());
			this.WriteList<RoleType>(writer, "rtf", this.roleTypeFilter, delegate(RoleType x)
			{
				int num = (int)x;
				return num.ToString();
			});
			this.WriteList<RoleType>(writer, "lrr", this.logonUserRequiredRoleTypes, delegate(RoleType x)
			{
				int num = (int)x;
				return num.ToString();
			});
			this.WriteList<RoleEntry>(writer, "sref", this.sortedRoleEntryFilter, (RoleEntry x) => x.ToADString());
			writer.WriteEndElement();
		}

		private void WriteList<T>(XmlWriter writer, string elementName, IList<T> list, Func<T, string> convertToStringFunc)
		{
			if (list == null || list.Count == 0)
			{
				return;
			}
			writer.WriteStartElement(elementName);
			foreach (T arg in list)
			{
				writer.WriteElementString("le", convertToStringFunc(arg));
			}
			writer.WriteEndElement();
		}

		private void Deserialize(XmlTextReader reader)
		{
			try
			{
				this.ReadStartOfNodeElement(reader, "erccontext");
				if (reader.MoveToFirstAttribute())
				{
					for (;;)
					{
						if (!StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "t"))
						{
							goto IL_88;
						}
						if (!string.IsNullOrEmpty(reader.Value))
						{
							try
							{
								this.contextType = (RBACContext.RBACContextType)int.Parse(reader.Value);
								goto IL_E4;
							}
							catch (FormatException)
							{
								this.ThrowParserException(reader, Strings.InvalidAttributeValue(reader.Value, "t"));
								goto IL_E4;
							}
							catch (OverflowException)
							{
								this.ThrowParserException(reader, Strings.InvalidAttributeValue(reader.Value, "t"));
								goto IL_E4;
							}
							goto IL_88;
						}
						IL_E4:
						if (!reader.MoveToNextAttribute())
						{
							break;
						}
						continue;
						IL_88:
						if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "at"))
						{
							this.AuthenticationType = reader.Value;
							goto IL_E4;
						}
						if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "un"))
						{
							this.ExecutingUserName = reader.Value;
							goto IL_E4;
						}
						this.ThrowParserException(reader, Strings.InvalidAttribute(reader.Name));
						goto IL_E4;
					}
				}
				if (RBACContext.RBACContextType.Windows != this.contextType && RBACContext.RBACContextType.Delegated != this.contextType)
				{
					this.ThrowParserException(reader, Strings.InvalidRBACContextType(this.contextType.ToString()));
				}
				if (this.AuthenticationType == null)
				{
					this.ThrowParserException(reader, AuthorizationStrings.AuthenticationTypeIsMissing);
				}
				if (string.IsNullOrEmpty(this.ExecutingUserName))
				{
					this.ThrowParserException(reader, Strings.ExecutingUserNameIsMissing);
				}
				this.ReadExecutingUser(reader);
				this.ReadImpersonatedUser(reader);
			}
			catch (XmlException ex)
			{
				this.ThrowParserException(reader, new LocalizedString(AuthorizationStrings.InvalidXml + " : " + ex.Message));
			}
		}

		private void ReadExecutingUser(XmlTextReader reader)
		{
			this.ReadStartOfNodeElement(reader, "ex");
			if (this.ElementContainsAttributes(reader))
			{
				this.ThrowParserException(reader, Strings.ElementMustNotHaveAttributes("ex"));
			}
			if (!reader.Read() || XmlNodeType.Text != reader.NodeType || string.IsNullOrEmpty(reader.Value))
			{
				this.ThrowParserException(reader, Strings.InvalidElementValue(reader.Value, "ex"));
			}
			this.serializedExecutingUser = reader.Value;
			this.ReadEndOfNodeElement(reader, "ex");
		}

		private void ReadImpersonatedUser(XmlTextReader reader)
		{
			if (reader.Read() && XmlNodeType.EndElement == reader.NodeType && StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "erccontext"))
			{
				return;
			}
			if (XmlNodeType.Element != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "im"))
			{
				this.ThrowParserException(reader, Strings.UnExpectedElement("im", reader.Name));
			}
			bool flag = false;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "sddl"))
					{
						this.impersonatedUserSddl = reader.Value;
					}
					else if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "at"))
					{
						this.impersonatedAuthenticationType = reader.Value;
					}
					else
					{
						if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "cca"))
						{
							if (string.IsNullOrEmpty(reader.Value))
							{
								goto IL_120;
							}
							try
							{
								this.callerCheckedAccess = bool.Parse(reader.Value);
								flag = true;
								goto IL_120;
							}
							catch (FormatException)
							{
								this.ThrowParserException(reader, Strings.InvalidAttributeValue(reader.Value, "cca"));
								goto IL_120;
							}
						}
						this.ThrowParserException(reader, Strings.InvalidAttribute(reader.Name));
					}
					IL_120:;
				}
				while (reader.MoveToNextAttribute());
			}
			if (string.IsNullOrEmpty(this.impersonatedUserSddl))
			{
				this.ThrowParserException(reader, Strings.MissingImpersonatedUserSid);
			}
			if (this.impersonatedAuthenticationType == null)
			{
				this.ThrowParserException(reader, AuthorizationStrings.AuthenticationTypeIsMissing);
			}
			if (!flag)
			{
				this.ThrowParserException(reader, Strings.MissingAttribute("cca", "im"));
			}
			while (reader.Read())
			{
				if (XmlNodeType.Element != reader.NodeType)
				{
					return;
				}
				string name;
				if ((name = reader.Name) != null)
				{
					if (name == "rtf")
					{
						this.roleTypeFilter = this.ReadListElements<RoleType>(reader, "rtf", new Func<XmlTextReader, RoleType>(this.ParseListElementToRoleType));
						continue;
					}
					if (name == "lrr")
					{
						this.logonUserRequiredRoleTypes = this.ReadListElements<RoleType>(reader, "lrr", new Func<XmlTextReader, RoleType>(this.ParseListElementToRoleType));
						continue;
					}
					if (name == "sref")
					{
						this.sortedRoleEntryFilter = this.ReadListElements<RoleEntry>(reader, "sref", new Func<XmlTextReader, RoleEntry>(this.ParseListElementToRoleEntry));
						continue;
					}
				}
				this.ThrowParserException(reader, Strings.InvalidElement(reader.Name));
			}
		}

		private List<T> ReadListElements<T>(XmlTextReader reader, string listElementName, Func<XmlTextReader, T> parseFunction)
		{
			List<T> list = new List<T>();
			foreach (T item in this.EnumerateListElements<T>(reader, listElementName, parseFunction))
			{
				list.Add(item);
			}
			return list;
		}

		private IEnumerable<T> EnumerateListElements<T>(XmlTextReader reader, string listElementName, Func<XmlTextReader, T> parseFunction)
		{
			if (this.ElementContainsAttributes(reader))
			{
				this.ThrowParserException(reader, Strings.ElementMustNotHaveAttributes(listElementName));
			}
			while (reader.Read() && (XmlNodeType.EndElement != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, listElementName)))
			{
				if (XmlNodeType.Element != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "le"))
				{
					this.ThrowParserException(reader, Strings.UnExpectedElement("le", reader.Name));
				}
				if (this.ElementContainsAttributes(reader))
				{
					this.ThrowParserException(reader, Strings.ElementMustNotHaveAttributes("le"));
				}
				if (!reader.Read() || XmlNodeType.Text != reader.NodeType || string.IsNullOrEmpty(reader.Value))
				{
					this.ThrowParserException(reader, Strings.InvalidElementValue(reader.Value, "le"));
				}
				yield return parseFunction(reader);
				this.ReadEndOfNodeElement(reader, "le");
			}
			yield break;
		}

		private void ReadStartOfNodeElement(XmlTextReader reader, string elementName)
		{
			if (!reader.Read() || XmlNodeType.Element != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, elementName))
			{
				this.ThrowParserException(reader, Strings.UnExpectedElement(elementName, reader.Name));
			}
		}

		private void ReadEndOfNodeElement(XmlTextReader reader, string elementName)
		{
			if (!reader.Read() || XmlNodeType.EndElement != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, elementName))
			{
				this.ThrowParserException(reader, Strings.UnExpectedElement(elementName, reader.Name));
			}
		}

		private bool ElementContainsAttributes(XmlTextReader reader)
		{
			int num = 0;
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					num++;
				}
				while (reader.MoveToNextAttribute());
			}
			return 0 != num;
		}

		private RoleType ParseListElementToRoleType(XmlTextReader reader)
		{
			int result = int.MaxValue;
			try
			{
				result = int.Parse(reader.Value);
			}
			catch (FormatException)
			{
				this.ThrowParserException(reader, Strings.InvalidElementValue(reader.Value, "le"));
			}
			catch (OverflowException)
			{
				this.ThrowParserException(reader, Strings.InvalidElementValue(reader.Value, "le"));
			}
			return (RoleType)result;
		}

		private RoleEntry ParseListElementToRoleEntry(XmlTextReader reader)
		{
			RoleEntry result = null;
			try
			{
				result = RoleEntry.Parse(reader.Value);
			}
			catch (FormatException ex)
			{
				this.ThrowParserException(reader, new LocalizedString(ex.Message));
			}
			return result;
		}

		private void ThrowParserException(XmlTextReader reader, LocalizedString description)
		{
			throw new RBACContextParserException(reader.LineNumber, reader.LinePosition, description);
		}

		private const string rootNode = "erccontext";

		private const string contextTypeAttributeName = "t";

		private const string authenticationTypeAttributeName = "at";

		private const string executingUserNameAttributeName = "un";

		private const string serializedExecutingUserElementName = "ex";

		private const string serializedImpersonatedUserElementName = "im";

		private const string listElement = "le";

		private const string impersonatedUserSddlSidAttributeName = "sddl";

		private const string impersonatedCallerCheckAccessAttributeName = "cca";

		private const string impersonatedRoleTypeFilterElement = "rtf";

		private const string impersonatedSortedEntryFilterElement = "sref";

		private const string impersonatedLogonRequiredRolesTypesElement = "lrr";

		private RBACContext.RBACContextType contextType;

		private string serializedExecutingUser;

		[NonSerialized]
		private IIdentity executingUserIdentity;

		private IList<RoleType> roleTypeFilter;

		private List<RoleEntry> sortedRoleEntryFilter;

		private IList<RoleType> logonUserRequiredRoleTypes;

		private bool callerCheckedAccess;

		private string impersonatedUserSddl;

		private string impersonatedAuthenticationType;

		private enum RBACContextType
		{
			None,
			Delegated = 2,
			Windows = 4
		}
	}
}
