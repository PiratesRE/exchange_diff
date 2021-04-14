using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class SerializedClientSecurityContext : ISecurityAccessToken
	{
		public static SerializedClientSecurityContext CreateFromOwaIdentity(OwaIdentity owaIdentity)
		{
			SerializedClientSecurityContext serializedClientSecurityContext = new SerializedClientSecurityContext();
			owaIdentity.ClientSecurityContext.SetSecurityAccessToken(serializedClientSecurityContext);
			serializedClientSecurityContext.LogonName = owaIdentity.GetLogonName();
			serializedClientSecurityContext.AuthenticationType = owaIdentity.AuthenticationType;
			return serializedClientSecurityContext;
		}

		public string UserSid
		{
			get
			{
				return this.userSid;
			}
			set
			{
				this.userSid = value;
			}
		}

		public SidStringAndAttributes[] GroupSids
		{
			get
			{
				return this.groupSids;
			}
			set
			{
				this.groupSids = value;
			}
		}

		public SidStringAndAttributes[] RestrictedGroupSids
		{
			get
			{
				return this.restrictedGroupSids;
			}
			set
			{
				this.restrictedGroupSids = value;
			}
		}

		internal string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
			set
			{
				this.authenticationType = value;
			}
		}

		internal string LogonName
		{
			get
			{
				return this.logonName;
			}
			set
			{
				this.logonName = value;
			}
		}

		public void Serialize(XmlTextWriter writer)
		{
			writer.WriteStartElement(SerializedClientSecurityContext.rootElementName);
			writer.WriteAttributeString(SerializedClientSecurityContext.authenticationTypeAttributeName, this.authenticationType);
			writer.WriteAttributeString(SerializedClientSecurityContext.logonNameAttributeName, this.logonName);
			SerializedClientSecurityContext.WriteSid(writer, this.UserSid, 0U, SerializedClientSecurityContext.SidType.User);
			if (this.GroupSids != null)
			{
				for (int i = 0; i < this.GroupSids.Length; i++)
				{
					SerializedClientSecurityContext.WriteSid(writer, this.GroupSids[i].SecurityIdentifier, this.GroupSids[i].Attributes, SerializedClientSecurityContext.SidType.Group);
				}
			}
			if (this.RestrictedGroupSids != null)
			{
				for (int j = 0; j < this.RestrictedGroupSids.Length; j++)
				{
					SerializedClientSecurityContext.WriteSid(writer, this.RestrictedGroupSids[j].SecurityIdentifier, this.RestrictedGroupSids[j].Attributes, SerializedClientSecurityContext.SidType.RestrictedGroup);
				}
			}
			writer.WriteEndElement();
		}

		private static void WriteSid(XmlTextWriter writer, string sid, uint attributes, SerializedClientSecurityContext.SidType sidType)
		{
			writer.WriteStartElement(SerializedClientSecurityContext.sidElementName);
			if (attributes != 0U)
			{
				writer.WriteAttributeString(SerializedClientSecurityContext.sidAttributesAttributeName, attributes.ToString());
			}
			if (sidType != SerializedClientSecurityContext.SidType.User)
			{
				string localName = SerializedClientSecurityContext.sidTypeAttributeName;
				int num = (int)sidType;
				writer.WriteAttributeString(localName, num.ToString());
			}
			writer.WriteString(sid);
			writer.WriteEndElement();
		}

		internal string Serialize()
		{
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			string result;
			try
			{
				this.Serialize(xmlTextWriter);
				stringWriter.Flush();
				result = stringWriter.ToString();
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
				if (stringWriter != null)
				{
					stringWriter.Close();
				}
			}
			return result;
		}

		internal static SerializedClientSecurityContext Deserialize(Stream input)
		{
			XmlTextReader xmlTextReader = null;
			SerializedClientSecurityContext result;
			try
			{
				xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(input);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				SerializedClientSecurityContext serializedClientSecurityContext = SerializedClientSecurityContext.Deserialize(xmlTextReader);
				result = serializedClientSecurityContext;
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			return result;
		}

		internal static SerializedClientSecurityContext Deserialize(XmlTextReader reader)
		{
			SerializedClientSecurityContext serializedClientSecurityContext = new SerializedClientSecurityContext();
			serializedClientSecurityContext.UserSid = null;
			serializedClientSecurityContext.GroupSids = null;
			serializedClientSecurityContext.RestrictedGroupSids = null;
			try
			{
				List<SidStringAndAttributes> list = new List<SidStringAndAttributes>();
				List<SidStringAndAttributes> list2 = new List<SidStringAndAttributes>();
				if (!reader.Read() || XmlNodeType.Element != reader.NodeType || StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.rootElementName) != 0)
				{
					SerializedClientSecurityContext.ThrowParserException(reader, "Missing or invalid root node");
				}
				if (reader.MoveToFirstAttribute())
				{
					do
					{
						if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.authenticationTypeAttributeName) == 0)
						{
							if (serializedClientSecurityContext.authenticationType != null)
							{
								SerializedClientSecurityContext.ThrowParserException(reader, string.Format("Duplicated attribute {0}", SerializedClientSecurityContext.authenticationTypeAttributeName));
							}
							serializedClientSecurityContext.authenticationType = reader.Value;
						}
						else if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.logonNameAttributeName) == 0)
						{
							if (serializedClientSecurityContext.logonName != null)
							{
								SerializedClientSecurityContext.ThrowParserException(reader, string.Format("Duplicated attribute {0}", SerializedClientSecurityContext.logonNameAttributeName));
							}
							serializedClientSecurityContext.logonName = reader.Value;
						}
						else
						{
							SerializedClientSecurityContext.ThrowParserException(reader, "Found invalid attribute in root element");
						}
					}
					while (reader.MoveToNextAttribute());
				}
				if (serializedClientSecurityContext.authenticationType == null || serializedClientSecurityContext.logonName == null)
				{
					SerializedClientSecurityContext.ThrowParserException(reader, "Auth type or logon name attributes are missing");
				}
				bool flag = false;
				int num = 0;
				while (reader.Read())
				{
					if (XmlNodeType.EndElement == reader.NodeType && StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.rootElementName) == 0)
					{
						flag = true;
						break;
					}
					if (XmlNodeType.Element != reader.NodeType || StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.sidElementName) != 0)
					{
						SerializedClientSecurityContext.ThrowParserException(reader, "Expecting SID node");
					}
					SerializedClientSecurityContext.SidType sidType = SerializedClientSecurityContext.SidType.User;
					uint num2 = 0U;
					if (reader.MoveToFirstAttribute())
					{
						do
						{
							if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.sidTypeAttributeName) == 0)
							{
								int num3 = int.Parse(reader.Value);
								if (num3 == 1)
								{
									sidType = SerializedClientSecurityContext.SidType.Group;
								}
								else if (num3 == 2)
								{
									sidType = SerializedClientSecurityContext.SidType.RestrictedGroup;
								}
								else
								{
									SerializedClientSecurityContext.ThrowParserException(reader, "Invalid SID type");
								}
							}
							else if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, SerializedClientSecurityContext.sidAttributesAttributeName) == 0)
							{
								num2 = uint.Parse(reader.Value);
							}
							else
							{
								SerializedClientSecurityContext.ThrowParserException(reader, "Found invalid attribute in SID element");
							}
						}
						while (reader.MoveToNextAttribute());
					}
					if (sidType == SerializedClientSecurityContext.SidType.User)
					{
						if (num2 != 0U)
						{
							SerializedClientSecurityContext.ThrowParserException(reader, "'Attributes' shouldn't be set in an user SID");
						}
						else if (serializedClientSecurityContext.UserSid != null)
						{
							SerializedClientSecurityContext.ThrowParserException(reader, "There can only be one user SID in the XML document");
						}
					}
					if (!reader.Read() || XmlNodeType.Text != reader.NodeType || string.IsNullOrEmpty(reader.Value))
					{
						SerializedClientSecurityContext.ThrowParserException(reader, "Expecting SID value in SDDL format");
					}
					string value = reader.Value;
					if (sidType == SerializedClientSecurityContext.SidType.User)
					{
						serializedClientSecurityContext.UserSid = value;
					}
					else if (sidType == SerializedClientSecurityContext.SidType.Group)
					{
						SidStringAndAttributes item = new SidStringAndAttributes(value, num2);
						list.Add(item);
					}
					else if (sidType == SerializedClientSecurityContext.SidType.RestrictedGroup)
					{
						SidStringAndAttributes item2 = new SidStringAndAttributes(value, num2);
						list2.Add(item2);
					}
					if (!reader.Read() || XmlNodeType.EndElement != reader.NodeType)
					{
						SerializedClientSecurityContext.ThrowParserException(reader, "Expected end of SID node");
					}
					num++;
					if (num > SerializedClientSecurityContext.MaximumSidsPerContext)
					{
						throw new OwaSecurityContextSidLimitException(string.Format("Too many SID nodes in the request, maximum is {0}", SerializedClientSecurityContext.MaximumSidsPerContext), serializedClientSecurityContext.logonName, serializedClientSecurityContext.authenticationType);
					}
				}
				if (serializedClientSecurityContext.UserSid == null)
				{
					SerializedClientSecurityContext.ThrowParserException(reader, "Serialized context should at least contain an user SID");
				}
				if (!flag)
				{
					SerializedClientSecurityContext.ThrowParserException(reader, "Parsing error");
				}
				if (list.Count > 0)
				{
					serializedClientSecurityContext.GroupSids = list.ToArray();
				}
				if (list2.Count > 0)
				{
					serializedClientSecurityContext.RestrictedGroupSids = list2.ToArray();
				}
			}
			catch (XmlException ex)
			{
				SerializedClientSecurityContext.ThrowParserException(reader, string.Format("Parser threw an XML exception: {0}", ex.Message));
			}
			return serializedClientSecurityContext;
		}

		private static void ThrowParserException(XmlTextReader reader, string description)
		{
			throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Invalid serialized client context. Line number: {0} Position: {1}.{2}", new object[]
			{
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(description != null) ? (" " + description) : string.Empty
			}));
		}

		public static int MaximumSidsPerContext = 3000;

		private static readonly string rootElementName = "r";

		private static readonly string authenticationTypeAttributeName = "at";

		private static readonly string logonNameAttributeName = "ln";

		private static readonly string sidElementName = "s";

		private static readonly string sidTypeAttributeName = "t";

		private static readonly string sidAttributesAttributeName = "a";

		private string userSid;

		private SidStringAndAttributes[] groupSids;

		private SidStringAndAttributes[] restrictedGroupSids;

		private string authenticationType;

		private string logonName;

		private enum SidType
		{
			User,
			Group,
			RestrictedGroup
		}
	}
}
