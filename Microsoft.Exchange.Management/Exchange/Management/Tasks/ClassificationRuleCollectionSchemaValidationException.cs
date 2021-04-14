using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionSchemaValidationException : ClassificationRuleCollectionValidationException
	{
		public ClassificationRuleCollectionSchemaValidationException(string schemaError, int lineNumber, int linePosition) : base(Strings.ClassificationRuleCollectionSchemaNonConformance(schemaError, lineNumber, linePosition))
		{
			this.schemaError = schemaError;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		public ClassificationRuleCollectionSchemaValidationException(string schemaError, int lineNumber, int linePosition, Exception innerException) : base(Strings.ClassificationRuleCollectionSchemaNonConformance(schemaError, lineNumber, linePosition), innerException)
		{
			this.schemaError = schemaError;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		protected ClassificationRuleCollectionSchemaValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.schemaError = (string)info.GetValue("schemaError", typeof(string));
			this.lineNumber = (int)info.GetValue("lineNumber", typeof(int));
			this.linePosition = (int)info.GetValue("linePosition", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("schemaError", this.schemaError);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
		}

		public string SchemaError
		{
			get
			{
				return this.schemaError;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		private readonly string schemaError;

		private readonly int lineNumber;

		private readonly int linePosition;
	}
}
