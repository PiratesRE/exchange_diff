using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IndexStatusException : LocalizedException
	{
		public IndexStatusException(string error) : base(Strings.IndexStatusException(error))
		{
			this.error = error;
		}

		public IndexStatusException(string error, Exception innerException) : base(Strings.IndexStatusException(error), innerException)
		{
			this.error = error;
		}

		protected IndexStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
