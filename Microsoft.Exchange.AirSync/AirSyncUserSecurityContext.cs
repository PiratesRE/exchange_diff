using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class AirSyncUserSecurityContext : ISecurityAccessToken
	{
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
			writer.WriteStartElement("r");
			writer.WriteAttributeString("at", this.authenticationType);
			writer.WriteAttributeString("ln", this.logonName);
			AirSyncUserSecurityContext.WriteSid(writer, this.UserSid, 0U, AirSyncUserSecurityContext.SidType.User);
			if (this.GroupSids != null)
			{
				for (int i = 0; i < this.GroupSids.Length; i++)
				{
					AirSyncUserSecurityContext.WriteSid(writer, this.GroupSids[i].SecurityIdentifier, this.GroupSids[i].Attributes, AirSyncUserSecurityContext.SidType.Group);
				}
			}
			if (this.RestrictedGroupSids != null)
			{
				for (int j = 0; j < this.RestrictedGroupSids.Length; j++)
				{
					AirSyncUserSecurityContext.WriteSid(writer, this.RestrictedGroupSids[j].SecurityIdentifier, this.RestrictedGroupSids[j].Attributes, AirSyncUserSecurityContext.SidType.RestrictedGroup);
				}
			}
			writer.WriteEndElement();
		}

		internal static AirSyncUserSecurityContext Deserialize(Stream input)
		{
			XmlTextReader xmlTextReader = null;
			AirSyncUserSecurityContext result;
			try
			{
				xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(input);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				AirSyncUserSecurityContext airSyncUserSecurityContext = AirSyncUserSecurityContext.Deserialize(xmlTextReader);
				result = airSyncUserSecurityContext;
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

		internal static AirSyncUserSecurityContext Deserialize(XmlTextReader reader)
		{
			AirSyncUserSecurityContext airSyncUserSecurityContext = new AirSyncUserSecurityContext();
			airSyncUserSecurityContext.UserSid = null;
			airSyncUserSecurityContext.GroupSids = null;
			airSyncUserSecurityContext.RestrictedGroupSids = null;
			try
			{
				List<SidStringAndAttributes> list = new List<SidStringAndAttributes>();
				List<SidStringAndAttributes> list2 = new List<SidStringAndAttributes>();
				if (!reader.Read() || XmlNodeType.Element != reader.NodeType || StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "r") != 0)
				{
					AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:RootNodeMissing");
				}
				if (reader.MoveToFirstAttribute())
				{
					do
					{
						if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "at") == 0)
						{
							if (airSyncUserSecurityContext.authenticationType != null)
							{
								AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:AuthTypeTwice");
							}
							airSyncUserSecurityContext.authenticationType = reader.Value;
						}
						else if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "ln") == 0)
						{
							if (airSyncUserSecurityContext.logonName != null)
							{
								AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:UserNameTwice");
							}
							airSyncUserSecurityContext.logonName = reader.Value;
						}
						else
						{
							string protocolErrorString = "ProxyRequestError:UnknownElement(" + reader.Name + ")";
							AirSyncUserSecurityContext.ThrowParserException(protocolErrorString);
						}
					}
					while (reader.MoveToNextAttribute());
				}
				if (airSyncUserSecurityContext.authenticationType == null || airSyncUserSecurityContext.logonName == null)
				{
					AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:AuthTypeLogonNameMissing");
				}
				bool flag = false;
				int num = 0;
				while (reader.Read())
				{
					if (XmlNodeType.EndElement == reader.NodeType && StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "r") == 0)
					{
						flag = true;
						break;
					}
					if (XmlNodeType.Element != reader.NodeType || StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "s") != 0)
					{
						AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:NoSID");
					}
					AirSyncUserSecurityContext.SidType sidType = AirSyncUserSecurityContext.SidType.User;
					uint num2 = 0U;
					if (reader.MoveToFirstAttribute())
					{
						do
						{
							if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "t") == 0)
							{
								int num3 = int.Parse(reader.Value, CultureInfo.InvariantCulture);
								if (num3 == 1)
								{
									sidType = AirSyncUserSecurityContext.SidType.Group;
								}
								else if (num3 == 2)
								{
									sidType = AirSyncUserSecurityContext.SidType.RestrictedGroup;
								}
								else
								{
									AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:InvalidSIDType");
								}
							}
							else if (StringComparer.OrdinalIgnoreCase.Compare(reader.Name, "a") == 0)
							{
								num2 = uint.Parse(reader.Value, CultureInfo.InvariantCulture);
							}
							else
							{
								AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:InvalidSIDAttribute");
							}
						}
						while (reader.MoveToNextAttribute());
					}
					if (sidType == AirSyncUserSecurityContext.SidType.User)
					{
						if (num2 != 0U)
						{
							AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:AttributesOnUserSID");
						}
						else if (airSyncUserSecurityContext.UserSid != null)
						{
							AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:MultipleUserSIDs");
						}
					}
					if (!reader.Read() || XmlNodeType.Text != reader.NodeType || string.IsNullOrEmpty(reader.Value))
					{
						AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:BadProxyHeader");
					}
					string value = reader.Value;
					if (sidType == AirSyncUserSecurityContext.SidType.User)
					{
						airSyncUserSecurityContext.UserSid = value;
					}
					else if (sidType == AirSyncUserSecurityContext.SidType.Group)
					{
						SidStringAndAttributes item = new SidStringAndAttributes(value, num2);
						list.Add(item);
					}
					else if (sidType == AirSyncUserSecurityContext.SidType.RestrictedGroup)
					{
						SidStringAndAttributes item2 = new SidStringAndAttributes(value, num2);
						list2.Add(item2);
					}
					if (!reader.Read() || XmlNodeType.EndElement != reader.NodeType)
					{
						AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:BadProxyHeader2");
					}
					num++;
					if (num > 3000)
					{
						AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:TooManySIDs");
					}
				}
				if (airSyncUserSecurityContext.UserSid == null)
				{
					AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:NoUserSID");
				}
				if (!flag)
				{
					AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:BadParsing");
				}
				if (list.Count > 0)
				{
					airSyncUserSecurityContext.GroupSids = list.ToArray();
				}
				if (list2.Count > 0)
				{
					airSyncUserSecurityContext.RestrictedGroupSids = list2.ToArray();
				}
			}
			catch (XmlException)
			{
				AirSyncUserSecurityContext.ThrowParserException("ProxyRequestError:XMLParsingException");
			}
			return airSyncUserSecurityContext;
		}

		internal string Serialize()
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
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

		private static void WriteSid(XmlTextWriter writer, string sid, uint attributes, AirSyncUserSecurityContext.SidType sidType)
		{
			writer.WriteStartElement("s");
			if (attributes != 0U)
			{
				writer.WriteAttributeString("a", attributes.ToString(CultureInfo.InvariantCulture));
			}
			if (sidType != AirSyncUserSecurityContext.SidType.User)
			{
				string localName = "t";
				int num = (int)sidType;
				writer.WriteAttributeString(localName, num.ToString(CultureInfo.InvariantCulture));
			}
			writer.WriteString(sid);
			writer.WriteEndElement();
		}

		private static void ThrowParserException(string protocolErrorString)
		{
			throw new AirSyncPermanentException(false)
			{
				ErrorStringForProtocolLogger = protocolErrorString
			};
		}

		private const int MaximumSidsPerContext = 3000;

		private const string RootElementName = "r";

		private const string AuthenticationTypeAttributeName = "at";

		private const string LogonNameAttributeName = "ln";

		private const string SidElementName = "s";

		private const string SidTypeAttributeName = "t";

		private const string SidAttributesAttributeName = "a";

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
