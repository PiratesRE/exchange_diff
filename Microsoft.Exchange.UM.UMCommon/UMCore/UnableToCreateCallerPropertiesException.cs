using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToCreateCallerPropertiesException : LocalizedException
	{
		public UnableToCreateCallerPropertiesException(string typeA) : base(Strings.UnableToCreateCallerPropertiesException(typeA))
		{
			this.typeA = typeA;
		}

		public UnableToCreateCallerPropertiesException(string typeA, Exception innerException) : base(Strings.UnableToCreateCallerPropertiesException(typeA), innerException)
		{
			this.typeA = typeA;
		}

		protected UnableToCreateCallerPropertiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.typeA = (string)info.GetValue("typeA", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typeA", this.typeA);
		}

		public string TypeA
		{
			get
			{
				return this.typeA;
			}
		}

		private readonly string typeA;
	}
}
