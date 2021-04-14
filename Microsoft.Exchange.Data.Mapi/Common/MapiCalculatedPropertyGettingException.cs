using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiCalculatedPropertyGettingException : MapiConvertingException
	{
		public MapiCalculatedPropertyGettingException(string name, string details) : base(Strings.MapiCalculatedPropertyGettingExceptionError(name, details))
		{
			this.name = name;
			this.details = details;
		}

		public MapiCalculatedPropertyGettingException(string name, string details, Exception innerException) : base(Strings.MapiCalculatedPropertyGettingExceptionError(name, details), innerException)
		{
			this.name = name;
			this.details = details;
		}

		protected MapiCalculatedPropertyGettingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("details", this.details);
		}

		public string Name
		{
			get
			{
				return this.name;
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

		private readonly string details;
	}
}
