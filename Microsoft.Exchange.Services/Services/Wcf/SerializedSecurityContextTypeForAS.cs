using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlRoot(ElementName = "SerializedSecurityContext", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SerializedSecurityContextTypeForAS
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

		[XmlArrayItem("GroupIdentifier", IsNullable = false)]
		[XmlArray(Order = 1)]
		public SidAndAttributesTypeForAS[] GroupSids
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

		[XmlArrayItem("RestrictedGroupIdentifier", IsNullable = false)]
		[XmlArray(Order = 2)]
		public SidAndAttributesTypeForAS[] RestrictedGroupSids
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
				GroupSids = SerializedSecurityContextTypeForAS.ToSidStringAndAttributesArray(this.GroupSids),
				RestrictedGroupSids = SerializedSecurityContextTypeForAS.ToSidStringAndAttributesArray(this.RestrictedGroupSids),
				SmtpAddress = this.PrimarySmtpAddress
			};
		}

		private static SidStringAndAttributes[] ToSidStringAndAttributesArray(SidAndAttributesTypeForAS[] sidAndAttributes)
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

		private SidAndAttributesTypeForAS[] groupSidsField;

		private SidAndAttributesTypeForAS[] restrictedGroupSidsField;

		private string primarySmtpAddressField;
	}
}
