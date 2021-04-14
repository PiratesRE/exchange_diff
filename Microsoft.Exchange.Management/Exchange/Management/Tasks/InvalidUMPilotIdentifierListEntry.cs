using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidUMPilotIdentifierListEntry : LocalizedException
	{
		public InvalidUMPilotIdentifierListEntry(string entryValue) : base(Strings.InvalidUMPilotIdentifierListEntry(entryValue))
		{
			this.entryValue = entryValue;
		}

		public InvalidUMPilotIdentifierListEntry(string entryValue, Exception innerException) : base(Strings.InvalidUMPilotIdentifierListEntry(entryValue), innerException)
		{
			this.entryValue = entryValue;
		}

		protected InvalidUMPilotIdentifierListEntry(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.entryValue = (string)info.GetValue("entryValue", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("entryValue", this.entryValue);
		}

		public string EntryValue
		{
			get
			{
				return this.entryValue;
			}
		}

		private readonly string entryValue;
	}
}
