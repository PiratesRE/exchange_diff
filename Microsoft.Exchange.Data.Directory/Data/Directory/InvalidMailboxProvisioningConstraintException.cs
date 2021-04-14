using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidMailboxProvisioningConstraintException : DataSourceOperationException
	{
		public InvalidMailboxProvisioningConstraintException(string parserErrorString) : base(DirectoryStrings.ErrorInvalidMailboxProvisioningConstraint(parserErrorString))
		{
			this.parserErrorString = parserErrorString;
		}

		public InvalidMailboxProvisioningConstraintException(string parserErrorString, Exception innerException) : base(DirectoryStrings.ErrorInvalidMailboxProvisioningConstraint(parserErrorString), innerException)
		{
			this.parserErrorString = parserErrorString;
		}

		protected InvalidMailboxProvisioningConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parserErrorString = (string)info.GetValue("parserErrorString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parserErrorString", this.parserErrorString);
		}

		public string ParserErrorString
		{
			get
			{
				return this.parserErrorString;
			}
		}

		private readonly string parserErrorString;
	}
}
