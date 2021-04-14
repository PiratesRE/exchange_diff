using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsUrlIsInvalidException : LocalizedException
	{
		public RmsUrlIsInvalidException(Uri uri) : base(Strings.RmsUrlIsInvalidException(uri))
		{
			this.uri = uri;
		}

		public RmsUrlIsInvalidException(Uri uri, Exception innerException) : base(Strings.RmsUrlIsInvalidException(uri), innerException)
		{
			this.uri = uri;
		}

		protected RmsUrlIsInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uri = (Uri)info.GetValue("uri", typeof(Uri));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uri", this.uri);
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		private readonly Uri uri;
	}
}
