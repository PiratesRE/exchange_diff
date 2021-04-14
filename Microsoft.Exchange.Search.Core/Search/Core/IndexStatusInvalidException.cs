using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IndexStatusInvalidException : IndexStatusException
	{
		public IndexStatusInvalidException(string value) : base(Strings.IndexStatusInvalid(value))
		{
			this.value = value;
		}

		public IndexStatusInvalidException(string value, Exception innerException) : base(Strings.IndexStatusInvalid(value), innerException)
		{
			this.value = value;
		}

		protected IndexStatusInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string value;
	}
}
