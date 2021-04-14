using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSharingDataException : StoragePermanentException
	{
		public InvalidSharingDataException(string name, string value) : base(ServerStrings.InvalidSharingDataException(name, value))
		{
			this.name = name;
			this.value = value;
		}

		public InvalidSharingDataException(string name, string value, Exception innerException) : base(ServerStrings.InvalidSharingDataException(name, value), innerException)
		{
			this.name = name;
			this.value = value;
		}

		protected InvalidSharingDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("value", this.value);
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

		private readonly string name;

		private readonly string value;
	}
}
