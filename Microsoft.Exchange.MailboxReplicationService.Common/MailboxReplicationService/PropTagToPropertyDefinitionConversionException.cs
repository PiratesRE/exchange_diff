using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PropTagToPropertyDefinitionConversionException : MailboxReplicationPermanentException
	{
		public PropTagToPropertyDefinitionConversionException(int propTag) : base(MrsStrings.PropTagToPropertyDefinitionConversion(propTag))
		{
			this.propTag = propTag;
		}

		public PropTagToPropertyDefinitionConversionException(int propTag, Exception innerException) : base(MrsStrings.PropTagToPropertyDefinitionConversion(propTag), innerException)
		{
			this.propTag = propTag;
		}

		protected PropTagToPropertyDefinitionConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propTag = (int)info.GetValue("propTag", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propTag", this.propTag);
		}

		public int PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		private readonly int propTag;
	}
}
