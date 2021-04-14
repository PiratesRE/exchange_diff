using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsVersionMismatchException : LocalizedException
	{
		public RmsVersionMismatchException(Uri uri) : base(Strings.RmsVersionMismatchException(uri))
		{
			this.uri = uri;
		}

		public RmsVersionMismatchException(Uri uri, Exception innerException) : base(Strings.RmsVersionMismatchException(uri), innerException)
		{
			this.uri = uri;
		}

		protected RmsVersionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
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
