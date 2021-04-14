using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiPackingException : MapiConvertingException
	{
		public MapiPackingException(string name, string value, string type, string isMultiValued, string propTag, string propType, string details) : base(Strings.MapiPackingExceptionError(name, value, type, isMultiValued, propTag, propType, details))
		{
			this.name = name;
			this.value = value;
			this.type = type;
			this.isMultiValued = isMultiValued;
			this.propTag = propTag;
			this.propType = propType;
			this.details = details;
		}

		public MapiPackingException(string name, string value, string type, string isMultiValued, string propTag, string propType, string details, Exception innerException) : base(Strings.MapiPackingExceptionError(name, value, type, isMultiValued, propTag, propType, details), innerException)
		{
			this.name = name;
			this.value = value;
			this.type = type;
			this.isMultiValued = isMultiValued;
			this.propTag = propTag;
			this.propType = propType;
			this.details = details;
		}

		protected MapiPackingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
			this.isMultiValued = (string)info.GetValue("isMultiValued", typeof(string));
			this.propTag = (string)info.GetValue("propTag", typeof(string));
			this.propType = (string)info.GetValue("propType", typeof(string));
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("value", this.value);
			info.AddValue("type", this.type);
			info.AddValue("isMultiValued", this.isMultiValued);
			info.AddValue("propTag", this.propTag);
			info.AddValue("propType", this.propType);
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

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public string IsMultiValued
		{
			get
			{
				return this.isMultiValued;
			}
		}

		public string PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public string PropType
		{
			get
			{
				return this.propType;
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

		private readonly string type;

		private readonly string isMultiValued;

		private readonly string propTag;

		private readonly string propType;

		private readonly string details;
	}
}
