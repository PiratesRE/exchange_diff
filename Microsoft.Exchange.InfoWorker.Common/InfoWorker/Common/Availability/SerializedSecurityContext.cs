using System;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	public class SerializedSecurityContext : SoapHeader
	{
		public SerializedSecurityContext()
		{
		}

		internal SerializedSecurityContext(SecurityAccessToken securityContext)
		{
			if (securityContext == null)
			{
				throw new ArgumentNullException("securityContext");
			}
			this.userSid = securityContext.UserSid;
			this.groupSids = this.GroupSidArrayFromSidStringAndAttributeArray(securityContext.GroupSids);
			this.restrictedGroupSids = this.GroupSidArrayFromSidStringAndAttributeArray(securityContext.RestrictedGroupSids);
			this.groupSAArray = securityContext.GroupSids;
			this.restrictedGroupSAArray = securityContext.RestrictedGroupSids;
		}

		[XmlElement]
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

		[XmlArrayItem("GroupIdentifier")]
		public GroupSid[] GroupSids
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

		[XmlArrayItem("RestrictedGroupIdentifier")]
		public GroupSid[] RestrictedGroupSids
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

		public override string ToString()
		{
			return this.UserSid;
		}

		internal SecurityAccessToken GetSecurityAccessToken()
		{
			SecurityAccessToken securityAccessToken = new SecurityAccessToken();
			securityAccessToken.UserSid = this.UserSid;
			if (this.groupSAArray == null)
			{
				this.groupSAArray = this.SidStringAndAttributeArrayFromGroupSidArray(this.groupSids);
			}
			if (this.restrictedGroupSAArray == null)
			{
				this.restrictedGroupSAArray = this.SidStringAndAttributeArrayFromGroupSidArray(this.restrictedGroupSids);
			}
			securityAccessToken.GroupSids = this.groupSAArray;
			securityAccessToken.RestrictedGroupSids = this.restrictedGroupSAArray;
			return securityAccessToken;
		}

		public string GetDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("\nUserSid: {0}", this.UserSid);
			if (this.GroupSids != null)
			{
				stringBuilder.AppendFormat("\nGroup SIDs and Attributes:", new object[0]);
				for (int i = 0; i < this.GroupSids.Length; i++)
				{
					stringBuilder.AppendFormat("\n{0}", this.GroupSids[i]);
				}
			}
			if (this.RestrictedGroupSids != null)
			{
				stringBuilder.AppendFormat("\nRestricted Group SIDs and Attributes:", new object[0]);
				for (int j = 0; j < this.GroupSids.Length; j++)
				{
					stringBuilder.AppendFormat("\n{0}", this.RestrictedGroupSids[j]);
				}
			}
			return stringBuilder.ToString();
		}

		private GroupSid[] GroupSidArrayFromSidStringAndAttributeArray(SidStringAndAttributes[] saArray)
		{
			if (saArray == null || saArray.Length == 0)
			{
				return null;
			}
			GroupSid[] array = new GroupSid[saArray.Length];
			for (int i = 0; i < saArray.Length; i++)
			{
				array[i] = new GroupSid();
				array[i].SecurityIdentifier = saArray[i].SecurityIdentifier;
				array[i].Attributes = saArray[i].Attributes;
			}
			return array;
		}

		private SidStringAndAttributes[] SidStringAndAttributeArrayFromGroupSidArray(GroupSid[] gsArray)
		{
			if (gsArray == null || gsArray.Length == 0)
			{
				return null;
			}
			SidStringAndAttributes[] array = new SidStringAndAttributes[gsArray.Length];
			for (int i = 0; i < gsArray.Length; i++)
			{
				array[i] = new SidStringAndAttributes(gsArray[i].SecurityIdentifier, gsArray[i].Attributes);
			}
			return array;
		}

		private string userSid;

		private GroupSid[] groupSids;

		private GroupSid[] restrictedGroupSids;

		private SidStringAndAttributes[] groupSAArray;

		private SidStringAndAttributes[] restrictedGroupSAArray;
	}
}
