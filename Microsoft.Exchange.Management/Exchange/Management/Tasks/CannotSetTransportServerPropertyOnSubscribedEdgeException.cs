using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetTransportServerPropertyOnSubscribedEdgeException : LocalizedException
	{
		public CannotSetTransportServerPropertyOnSubscribedEdgeException(string propertyName) : base(Strings.ErrorCannotSetTransportServerPropertyOnSubscribedEdge(propertyName))
		{
			this.propertyName = propertyName;
		}

		public CannotSetTransportServerPropertyOnSubscribedEdgeException(string propertyName, Exception innerException) : base(Strings.ErrorCannotSetTransportServerPropertyOnSubscribedEdge(propertyName), innerException)
		{
			this.propertyName = propertyName;
		}

		protected CannotSetTransportServerPropertyOnSubscribedEdgeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string propertyName;
	}
}
