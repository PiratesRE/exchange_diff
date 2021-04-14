using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MissingExpectedCapabilityException : MigrationPermanentException
	{
		public MissingExpectedCapabilityException(string user, string capability) : base(Strings.ErrorMissingExpectedCapability(user, capability))
		{
			this.user = user;
			this.capability = capability;
		}

		public MissingExpectedCapabilityException(string user, string capability, Exception innerException) : base(Strings.ErrorMissingExpectedCapability(user, capability), innerException)
		{
			this.user = user;
			this.capability = capability;
		}

		protected MissingExpectedCapabilityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.capability = (string)info.GetValue("capability", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("capability", this.capability);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string Capability
		{
			get
			{
				return this.capability;
			}
		}

		private readonly string user;

		private readonly string capability;
	}
}
