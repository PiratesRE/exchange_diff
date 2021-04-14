using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MinimizedPropertyException : LocalizedException
	{
		public MinimizedPropertyException(string propertyName) : base(Strings.MinimizedProperty(propertyName))
		{
			this.propertyName = propertyName;
		}

		public MinimizedPropertyException(string propertyName, Exception innerException) : base(Strings.MinimizedProperty(propertyName), innerException)
		{
			this.propertyName = propertyName;
		}

		protected MinimizedPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
