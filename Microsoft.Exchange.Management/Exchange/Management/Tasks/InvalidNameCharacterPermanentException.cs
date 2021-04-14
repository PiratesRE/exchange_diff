using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidNameCharacterPermanentException : MailboxReplicationPermanentException
	{
		public InvalidNameCharacterPermanentException(string name, string character) : base(Strings.ErrorInvalidNameCharacter(name, character))
		{
			this.name = name;
			this.character = character;
		}

		public InvalidNameCharacterPermanentException(string name, string character, Exception innerException) : base(Strings.ErrorInvalidNameCharacter(name, character), innerException)
		{
			this.name = name;
			this.character = character;
		}

		protected InvalidNameCharacterPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.character = (string)info.GetValue("character", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("character", this.character);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Character
		{
			get
			{
				return this.character;
			}
		}

		private readonly string name;

		private readonly string character;
	}
}
