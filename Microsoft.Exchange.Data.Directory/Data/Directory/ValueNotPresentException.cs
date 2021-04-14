using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ValueNotPresentException : ADOperationException
	{
		public ValueNotPresentException(string propertyName, string objectName) : base(DirectoryStrings.ExceptionValueNotPresent(propertyName, objectName))
		{
			this.propertyName = propertyName;
			this.objectName = objectName;
		}

		public ValueNotPresentException(string propertyName, string objectName, Exception innerException) : base(DirectoryStrings.ExceptionValueNotPresent(propertyName, objectName), innerException)
		{
			this.propertyName = propertyName;
			this.objectName = objectName;
		}

		protected ValueNotPresentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
			this.objectName = (string)info.GetValue("objectName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
			info.AddValue("objectName", this.objectName);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public string ObjectName
		{
			get
			{
				return this.objectName;
			}
		}

		private readonly string propertyName;

		private readonly string objectName;
	}
}
