using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AceIsUnsupportedTypeException : LocalizedException
	{
		public AceIsUnsupportedTypeException(string aceType) : base(Strings.AceIsUnsupportedTypeException(aceType))
		{
			this.aceType = aceType;
		}

		public AceIsUnsupportedTypeException(string aceType, Exception innerException) : base(Strings.AceIsUnsupportedTypeException(aceType), innerException)
		{
			this.aceType = aceType;
		}

		protected AceIsUnsupportedTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.aceType = (string)info.GetValue("aceType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("aceType", this.aceType);
		}

		public string AceType
		{
			get
			{
				return this.aceType;
			}
		}

		private readonly string aceType;
	}
}
