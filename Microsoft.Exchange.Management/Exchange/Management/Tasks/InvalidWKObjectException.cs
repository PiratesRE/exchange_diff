using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidWKObjectException : LocalizedException
	{
		public InvalidWKObjectException(string wkentry, string container) : base(Strings.InvalidWKObjectException(wkentry, container))
		{
			this.wkentry = wkentry;
			this.container = container;
		}

		public InvalidWKObjectException(string wkentry, string container, Exception innerException) : base(Strings.InvalidWKObjectException(wkentry, container), innerException)
		{
			this.wkentry = wkentry;
			this.container = container;
		}

		protected InvalidWKObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.wkentry = (string)info.GetValue("wkentry", typeof(string));
			this.container = (string)info.GetValue("container", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("wkentry", this.wkentry);
			info.AddValue("container", this.container);
		}

		public string Wkentry
		{
			get
			{
				return this.wkentry;
			}
		}

		public string Container
		{
			get
			{
				return this.container;
			}
		}

		private readonly string wkentry;

		private readonly string container;
	}
}
