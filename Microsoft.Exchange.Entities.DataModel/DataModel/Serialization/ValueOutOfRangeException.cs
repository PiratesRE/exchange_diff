using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Entities.DataModel.Serialization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ValueOutOfRangeException : ArgumentOutOfRangeException
	{
		public ValueOutOfRangeException(string name, object value) : base(Strings.ValueIsOutOfRange(name, value))
		{
			this.name = name;
			this.value = value;
		}

		public ValueOutOfRangeException(string name, object value, Exception innerException) : base(Strings.ValueIsOutOfRange(name, value), innerException)
		{
			this.name = name;
			this.value = value;
		}

		protected ValueOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.value = info.GetValue("value", typeof(object));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("value", this.value);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string name;

		private readonly object value;
	}
}
