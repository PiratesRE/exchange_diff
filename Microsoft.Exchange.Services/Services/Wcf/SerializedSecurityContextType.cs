using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlRoot(ElementName = "SerializedSecurityContext", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class SerializedSecurityContextType
	{
		[XmlElement(Order = 0)]
		public string UserSid
		{
			get
			{
				return this.userSidField;
			}
			set
			{
				this.userSidField = value;
			}
		}

		[XmlArray(Order = 1)]
		[XmlArrayItem("GroupIdentifier", IsNullable = false)]
		public SidAndAttributesType[] GroupSids
		{
			get
			{
				return this.groupSidsField;
			}
			set
			{
				this.groupSidsField = value;
			}
		}

		[XmlArray(Order = 2)]
		[XmlArrayItem("RestrictedGroupIdentifier", IsNullable = false)]
		public SidAndAttributesType[] RestrictedGroupSids
		{
			get
			{
				return this.restrictedGroupSidsField;
			}
			set
			{
				this.restrictedGroupSidsField = value;
			}
		}

		[XmlElement(Order = 3)]
		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddressField;
			}
			set
			{
				this.primarySmtpAddressField = value;
			}
		}

		internal AuthZClientInfo ToAuthZClientInfo()
		{
			return AuthZClientInfo.FromSecurityAccessToken(this.ToSecurityAccessToken());
		}

		internal SerializedSecurityAccessToken ToSecurityAccessToken()
		{
			return new SerializedSecurityAccessToken
			{
				UserSid = this.UserSid,
				GroupSids = SerializedSecurityContextType.ToSidStringAndAttributesArray(this.GroupSids),
				RestrictedGroupSids = SerializedSecurityContextType.ToSidStringAndAttributesArray(this.RestrictedGroupSids),
				SmtpAddress = this.PrimarySmtpAddress
			};
		}

		private static SidStringAndAttributes[] ToSidStringAndAttributesArray(SidAndAttributesType[] sidAndAttributes)
		{
			if (sidAndAttributes == null || sidAndAttributes.Length == 0)
			{
				return null;
			}
			SidStringAndAttributes[] array = new SidStringAndAttributes[sidAndAttributes.Length];
			for (int i = 0; i < sidAndAttributes.Length; i++)
			{
				array[i] = new SidStringAndAttributes(sidAndAttributes[i].SecurityIdentifier, sidAndAttributes[i].Attributes);
			}
			return array;
		}

		private string userSidField;

		private SidAndAttributesType[] groupSidsField;

		private SidAndAttributesType[] restrictedGroupSidsField;

		private string primarySmtpAddressField;
	}
}
