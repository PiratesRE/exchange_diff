using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidAcceptedDomainException : LocalizedException
	{
		public InvalidAcceptedDomainException(string organizationId) : base(Strings.InvalidAcceptedDomainException(organizationId))
		{
			this.organizationId = organizationId;
		}

		public InvalidAcceptedDomainException(string organizationId, Exception innerException) : base(Strings.InvalidAcceptedDomainException(organizationId), innerException)
		{
			this.organizationId = organizationId;
		}

		protected InvalidAcceptedDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.organizationId = (string)info.GetValue("organizationId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("organizationId", this.organizationId);
		}

		public string OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		private readonly string organizationId;
	}
}
