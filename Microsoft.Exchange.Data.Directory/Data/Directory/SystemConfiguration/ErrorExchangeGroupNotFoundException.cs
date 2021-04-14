using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorExchangeGroupNotFoundException : LocalizedException
	{
		public ErrorExchangeGroupNotFoundException(Guid idStringValue) : base(DirectoryStrings.ErrorExchangeGroupNotFound(idStringValue))
		{
			this.idStringValue = idStringValue;
		}

		public ErrorExchangeGroupNotFoundException(Guid idStringValue, Exception innerException) : base(DirectoryStrings.ErrorExchangeGroupNotFound(idStringValue), innerException)
		{
			this.idStringValue = idStringValue;
		}

		protected ErrorExchangeGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.idStringValue = (Guid)info.GetValue("idStringValue", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("idStringValue", this.idStringValue);
		}

		public Guid IdStringValue
		{
			get
			{
				return this.idStringValue;
			}
		}

		private readonly Guid idStringValue;
	}
}
