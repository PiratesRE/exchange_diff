using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IndexStatusInvalidPropertyException : IndexStatusException
	{
		public IndexStatusInvalidPropertyException(string property, string value) : base(Strings.IndexStatusInvalidProperty(property, value))
		{
			this.property = property;
			this.value = value;
		}

		public IndexStatusInvalidPropertyException(string property, string value, Exception innerException) : base(Strings.IndexStatusInvalidProperty(property, value), innerException)
		{
			this.property = property;
			this.value = value;
		}

		protected IndexStatusInvalidPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
			info.AddValue("value", this.value);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string property;

		private readonly string value;
	}
}
