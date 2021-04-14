using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TooManyActiveManagerClientRPCsException : LocalizedException
	{
		public TooManyActiveManagerClientRPCsException(int maximum) : base(ServerStrings.TooManyActiveManagerClientRPCs(maximum))
		{
			this.maximum = maximum;
		}

		public TooManyActiveManagerClientRPCsException(int maximum, Exception innerException) : base(ServerStrings.TooManyActiveManagerClientRPCs(maximum), innerException)
		{
			this.maximum = maximum;
		}

		protected TooManyActiveManagerClientRPCsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maximum = (int)info.GetValue("maximum", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maximum", this.maximum);
		}

		public int Maximum
		{
			get
			{
				return this.maximum;
			}
		}

		private readonly int maximum;
	}
}
