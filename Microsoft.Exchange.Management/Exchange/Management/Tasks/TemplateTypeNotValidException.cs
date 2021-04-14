using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TemplateTypeNotValidException : LocalizedException
	{
		public TemplateTypeNotValidException(string type) : base(Strings.TemplateTypeNotValid(type))
		{
			this.type = type;
		}

		public TemplateTypeNotValidException(string type, Exception innerException) : base(Strings.TemplateTypeNotValid(type), innerException)
		{
			this.type = type;
		}

		protected TemplateTypeNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string type;
	}
}
