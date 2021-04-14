using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotResolveCurrentKeyException : LocalizedException
	{
		public CannotResolveCurrentKeyException(bool currentKey) : base(Strings.CannotResolveCurrentKeyException(currentKey))
		{
			this.currentKey = currentKey;
		}

		public CannotResolveCurrentKeyException(bool currentKey, Exception innerException) : base(Strings.CannotResolveCurrentKeyException(currentKey), innerException)
		{
			this.currentKey = currentKey;
		}

		protected CannotResolveCurrentKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.currentKey = (bool)info.GetValue("currentKey", typeof(bool));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("currentKey", this.currentKey);
		}

		public bool CurrentKey
		{
			get
			{
				return this.currentKey;
			}
		}

		private readonly bool currentKey;
	}
}
