using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AutoDiscoverFailedConfigurationErrorException : MigrationPermanentException
	{
		public AutoDiscoverFailedConfigurationErrorException(LocalizedString details) : base(Strings.AutoDiscoverConfigurationError(details))
		{
			this.details = details;
		}

		public AutoDiscoverFailedConfigurationErrorException(LocalizedString details, Exception innerException) : base(Strings.AutoDiscoverConfigurationError(details), innerException)
		{
			this.details = details;
		}

		protected AutoDiscoverFailedConfigurationErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.details = (LocalizedString)info.GetValue("details", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("details", this.details);
		}

		public LocalizedString Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly LocalizedString details;
	}
}
