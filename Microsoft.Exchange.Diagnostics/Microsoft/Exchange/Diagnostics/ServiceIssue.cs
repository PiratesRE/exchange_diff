using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ServiceIssue
	{
		protected ServiceIssue(string error)
		{
			this.Error = error;
			this.Guid = Guid.NewGuid();
			this.CreatedTime = DateTime.UtcNow;
			this.LastUpdatedTime = DateTime.UtcNow;
		}

		public Guid Guid { get; private set; }

		public string Error { get; private set; }

		public DateTime CreatedTime { get; private set; }

		public DateTime LastUpdatedTime { get; private set; }

		public abstract string IdentifierString { get; }

		public virtual void DeriveFromIssue(ServiceIssue issue)
		{
			this.Guid = issue.Guid;
			this.CreatedTime = issue.CreatedTime;
		}

		public virtual XElement GetDiagnosticInfo(SICDiagnosticArgument arguments)
		{
			XElement xelement = new XElement("ServiceIssue");
			xelement.Add(new object[]
			{
				new XElement("Guid", this.Guid),
				new XElement("Error", this.Error),
				new XElement("Type", base.GetType().FullName),
				new XElement("IdentifierString", this.IdentifierString),
				new XElement("CreatedTime", this.CreatedTime),
				new XElement("LastUpdatedTime", this.LastUpdatedTime)
			});
			return xelement;
		}

		private static class ServiceIssueSchema
		{
			public const string ElementName = "ServiceIssue";

			public const string GuidElement = "Guid";

			public const string ErrorElement = "Error";

			public const string TypeElement = "Type";

			public const string IdentifierStringElement = "IdentifierString";

			public const string CreatedTimeElement = "CreatedTime";

			public const string LastUpdatedTimeElement = "LastUpdatedTime";
		}
	}
}
