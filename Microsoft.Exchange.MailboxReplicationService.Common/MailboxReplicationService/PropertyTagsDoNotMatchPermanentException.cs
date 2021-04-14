using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PropertyTagsDoNotMatchPermanentException : MailboxReplicationPermanentException
	{
		public PropertyTagsDoNotMatchPermanentException(uint propTagFromSource, uint propTagFromDestination) : base(MrsStrings.PropertyTagsDoNotMatch(propTagFromSource, propTagFromDestination))
		{
			this.propTagFromSource = propTagFromSource;
			this.propTagFromDestination = propTagFromDestination;
		}

		public PropertyTagsDoNotMatchPermanentException(uint propTagFromSource, uint propTagFromDestination, Exception innerException) : base(MrsStrings.PropertyTagsDoNotMatch(propTagFromSource, propTagFromDestination), innerException)
		{
			this.propTagFromSource = propTagFromSource;
			this.propTagFromDestination = propTagFromDestination;
		}

		protected PropertyTagsDoNotMatchPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propTagFromSource = (uint)info.GetValue("propTagFromSource", typeof(uint));
			this.propTagFromDestination = (uint)info.GetValue("propTagFromDestination", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propTagFromSource", this.propTagFromSource);
			info.AddValue("propTagFromDestination", this.propTagFromDestination);
		}

		public uint PropTagFromSource
		{
			get
			{
				return this.propTagFromSource;
			}
		}

		public uint PropTagFromDestination
		{
			get
			{
				return this.propTagFromDestination;
			}
		}

		private readonly uint propTagFromSource;

		private readonly uint propTagFromDestination;
	}
}
