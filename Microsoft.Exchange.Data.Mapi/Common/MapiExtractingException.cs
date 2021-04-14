using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiExtractingException : MapiConvertingException
	{
		public MapiExtractingException(string name, string propTag, string propType, string rawValue, string rawValueType, string type, string isMultiValued, string details) : base(Strings.MapiExtractingExceptionError(name, propTag, propType, rawValue, rawValueType, type, isMultiValued, details))
		{
			this.name = name;
			this.propTag = propTag;
			this.propType = propType;
			this.rawValue = rawValue;
			this.rawValueType = rawValueType;
			this.type = type;
			this.isMultiValued = isMultiValued;
			this.details = details;
		}

		public MapiExtractingException(string name, string propTag, string propType, string rawValue, string rawValueType, string type, string isMultiValued, string details, Exception innerException) : base(Strings.MapiExtractingExceptionError(name, propTag, propType, rawValue, rawValueType, type, isMultiValued, details), innerException)
		{
			this.name = name;
			this.propTag = propTag;
			this.propType = propType;
			this.rawValue = rawValue;
			this.rawValueType = rawValueType;
			this.type = type;
			this.isMultiValued = isMultiValued;
			this.details = details;
		}

		protected MapiExtractingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.propTag = (string)info.GetValue("propTag", typeof(string));
			this.propType = (string)info.GetValue("propType", typeof(string));
			this.rawValue = (string)info.GetValue("rawValue", typeof(string));
			this.rawValueType = (string)info.GetValue("rawValueType", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
			this.isMultiValued = (string)info.GetValue("isMultiValued", typeof(string));
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("propTag", this.propTag);
			info.AddValue("propType", this.propType);
			info.AddValue("rawValue", this.rawValue);
			info.AddValue("rawValueType", this.rawValueType);
			info.AddValue("type", this.type);
			info.AddValue("isMultiValued", this.isMultiValued);
			info.AddValue("details", this.details);
		}

		public string Name
		{
			get
			{
				return this.name;
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

		public string RawValue
		{
			get
			{
				return this.rawValue;
			}
		}

		public string RawValueType
		{
			get
			{
				return this.rawValueType;
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

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly string name;

		private readonly string propTag;

		private readonly string propType;

		private readonly string rawValue;

		private readonly string rawValueType;

		private readonly string type;

		private readonly string isMultiValued;

		private readonly string details;
	}
}
