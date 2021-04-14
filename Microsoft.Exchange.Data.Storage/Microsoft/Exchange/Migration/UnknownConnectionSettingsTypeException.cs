using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnknownConnectionSettingsTypeException : MigrationPermanentException
	{
		public UnknownConnectionSettingsTypeException(string root) : base(Strings.ErrorUnknownConnectionSettingsType(root))
		{
			this.root = root;
		}

		public UnknownConnectionSettingsTypeException(string root, Exception innerException) : base(Strings.ErrorUnknownConnectionSettingsType(root), innerException)
		{
			this.root = root;
		}

		protected UnknownConnectionSettingsTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.root = (string)info.GetValue("root", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("root", this.root);
		}

		public string Root
		{
			get
			{
				return this.root;
			}
		}

		private readonly string root;
	}
}
