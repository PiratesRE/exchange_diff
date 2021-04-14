using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AceTypeHasUnsupportedValueException : LocalizedException
	{
		public AceTypeHasUnsupportedValueException(string aceType) : base(Strings.AceTypeHasUnsupportedValueException(aceType))
		{
			this.aceType = aceType;
		}

		public AceTypeHasUnsupportedValueException(string aceType, Exception innerException) : base(Strings.AceTypeHasUnsupportedValueException(aceType), innerException)
		{
			this.aceType = aceType;
		}

		protected AceTypeHasUnsupportedValueException(SerializationInfo info, StreamingContext context) : base(info, context)
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
