using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotCreateOscFolderBecauseOfConflictException : LocalizedException
	{
		public CannotCreateOscFolderBecauseOfConflictException(string provider) : base(ServerStrings.CannotCreateOscFolderBecauseOfConflict(provider))
		{
			this.provider = provider;
		}

		public CannotCreateOscFolderBecauseOfConflictException(string provider, Exception innerException) : base(ServerStrings.CannotCreateOscFolderBecauseOfConflict(provider), innerException)
		{
			this.provider = provider;
		}

		protected CannotCreateOscFolderBecauseOfConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.provider = (string)info.GetValue("provider", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("provider", this.provider);
		}

		public string Provider
		{
			get
			{
				return this.provider;
			}
		}

		private readonly string provider;
	}
}
