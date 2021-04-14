using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotSetPermanentAttributesException : DataSourceOperationException
	{
		public CannotSetPermanentAttributesException(string permanentAttributeNames) : base(DirectoryStrings.ErrorCannotSetPermanentAttributes(permanentAttributeNames))
		{
			this.permanentAttributeNames = permanentAttributeNames;
		}

		public CannotSetPermanentAttributesException(string permanentAttributeNames, Exception innerException) : base(DirectoryStrings.ErrorCannotSetPermanentAttributes(permanentAttributeNames), innerException)
		{
			this.permanentAttributeNames = permanentAttributeNames;
		}

		protected CannotSetPermanentAttributesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.permanentAttributeNames = (string)info.GetValue("permanentAttributeNames", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("permanentAttributeNames", this.permanentAttributeNames);
		}

		public string PermanentAttributeNames
		{
			get
			{
				return this.permanentAttributeNames;
			}
		}

		private readonly string permanentAttributeNames;
	}
}
