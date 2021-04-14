using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AnonymousPublishingUrlValidationException : StoragePermanentException
	{
		public AnonymousPublishingUrlValidationException(string url) : base(ServerStrings.AnonymousPublishingUrlValidationException(url))
		{
			this.url = url;
		}

		public AnonymousPublishingUrlValidationException(string url, Exception innerException) : base(ServerStrings.AnonymousPublishingUrlValidationException(url), innerException)
		{
			this.url = url;
		}

		protected AnonymousPublishingUrlValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.url = (string)info.GetValue("url", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("url", this.url);
		}

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		private readonly string url;
	}
}
