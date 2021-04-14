using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidTenantGuidException : LocalizedException
	{
		public InvalidTenantGuidException(string id) : base(Strings.InvalidTenantGuidError(id))
		{
			this.id = id;
		}

		public InvalidTenantGuidException(string id, Exception innerException) : base(Strings.InvalidTenantGuidError(id), innerException)
		{
			this.id = id;
		}

		protected InvalidTenantGuidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
