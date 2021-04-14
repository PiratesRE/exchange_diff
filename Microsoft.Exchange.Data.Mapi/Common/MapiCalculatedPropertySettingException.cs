using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiCalculatedPropertySettingException : MapiConvertingException
	{
		public MapiCalculatedPropertySettingException(string name, string value, string details) : base(Strings.MapiCalculatedPropertySettingExceptionError(name, value, details))
		{
			this.name = name;
			this.value = value;
			this.details = details;
		}

		public MapiCalculatedPropertySettingException(string name, string value, string details, Exception innerException) : base(Strings.MapiCalculatedPropertySettingExceptionError(name, value, details), innerException)
		{
			this.name = name;
			this.value = value;
			this.details = details;
		}

		protected MapiCalculatedPropertySettingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("value", this.value);
			info.AddValue("details", this.details);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly string name;

		private readonly string value;

		private readonly string details;
	}
}
