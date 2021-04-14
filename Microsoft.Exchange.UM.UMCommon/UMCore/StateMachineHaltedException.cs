using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class StateMachineHaltedException : LocalizedException
	{
		public StateMachineHaltedException(string id) : base(Strings.StateMachineHalted(id))
		{
			this.id = id;
		}

		public StateMachineHaltedException(string id, Exception innerException) : base(Strings.StateMachineHalted(id), innerException)
		{
			this.id = id;
		}

		protected StateMachineHaltedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
