using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConflictSavingException : LocalizedException
	{
		public ConflictSavingException(string identiy) : base(Strings.ErrorConflictSaving(identiy))
		{
			this.identiy = identiy;
		}

		public ConflictSavingException(string identiy, Exception innerException) : base(Strings.ErrorConflictSaving(identiy), innerException)
		{
			this.identiy = identiy;
		}

		protected ConflictSavingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identiy = (string)info.GetValue("identiy", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identiy", this.identiy);
		}

		public string Identiy
		{
			get
			{
				return this.identiy;
			}
		}

		private readonly string identiy;
	}
}
