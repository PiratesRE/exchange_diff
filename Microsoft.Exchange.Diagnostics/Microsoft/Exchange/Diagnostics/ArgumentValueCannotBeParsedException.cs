using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ArgumentValueCannotBeParsedException : DiagnosticArgumentException
	{
		public ArgumentValueCannotBeParsedException(string key, string value, string typeName) : base(DiagnosticsResources.ArgumentValueCannotBeParsed(key, value, typeName))
		{
			this.key = key;
			this.value = value;
			this.typeName = typeName;
		}

		public ArgumentValueCannotBeParsedException(string key, string value, string typeName, Exception innerException) : base(DiagnosticsResources.ArgumentValueCannotBeParsed(key, value, typeName), innerException)
		{
			this.key = key;
			this.value = value;
			this.typeName = typeName;
		}

		protected ArgumentValueCannotBeParsedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.key = (string)info.GetValue("key", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
			this.typeName = (string)info.GetValue("typeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("key", this.key);
			info.AddValue("value", this.value);
			info.AddValue("typeName", this.typeName);
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		private readonly string key;

		private readonly string value;

		private readonly string typeName;
	}
}
