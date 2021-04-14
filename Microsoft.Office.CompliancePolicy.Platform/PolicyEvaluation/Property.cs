using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class Property : Argument
	{
		public Property(string propertyName, Type type) : base(type)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new CompliancePolicyValidationException("Property name must not be empty");
			}
			this.Name = propertyName;
		}

		public string Name { get; set; }

		public string SupplementalInfo { get; set; }

		public static Property CreateProperty(string propertyName, string typeName)
		{
			if (string.IsNullOrWhiteSpace(typeName))
			{
				return new Property(propertyName, typeof(string));
			}
			try
			{
				return new Property(propertyName, Type.GetType(typeName));
			}
			catch (TargetInvocationException)
			{
			}
			catch (ArgumentException)
			{
			}
			catch (TypeLoadException)
			{
			}
			catch (FileLoadException)
			{
			}
			catch (BadImageFormatException)
			{
			}
			return null;
		}

		public override object GetValue(PolicyEvaluationContext context)
		{
			object obj = this.OnGetValue(context);
			if (obj != null)
			{
				Type type = obj.GetType();
				if (type != base.Type && !base.Type.IsAssignableFrom(type))
				{
					if (context.Tracer != null)
					{
						context.Tracer.TraceError("Property value is of the wrong type: {0}", new object[]
						{
							type
						});
					}
					return null;
				}
			}
			return obj;
		}

		protected virtual object OnGetValue(PolicyEvaluationContext context)
		{
			string name;
			switch (name = this.Name)
			{
			case "Item.CreationAgeInDays":
				return (int)(DateTime.UtcNow - context.SourceItem.WhenCreated).TotalDays;
			case "Item.Extension":
				return context.SourceItem.Extension;
			case "Item.DisplayName":
				return context.SourceItem.DisplayName;
			case "Item.WhenCreated":
				return context.SourceItem.WhenCreated;
			case "Item.WhenModified":
				return context.SourceItem.WhenLastModified;
			case "Item.Creator":
				return context.SourceItem.Creator;
			case "Item.LastModifier":
				return context.SourceItem.LastModifier;
			case "Item.ExpiryTime":
				return context.SourceItem.ExpiryTime;
			case "Item.ClassificationDiscovered":
				return context.SourceItem.ClassificationDiscovered;
			case "Item.AccessScope":
				return context.SourceItem.AccessScope;
			case "Item.Metadata":
				return context.SourceItem.Metadata;
			}
			return null;
		}

		public static class PropertyNames
		{
			public const string Extension = "Item.Extension";

			public const string DisplayName = "Item.DisplayName";

			public const string WhenCreated = "Item.WhenCreated";

			public const string WhenModified = "Item.WhenModified";

			public const string Creator = "Item.Creator";

			public const string LastModifier = "Item.LastModifier";

			public const string ExpiryTime = "Item.ExpiryTime";

			public const string CreationAgeInDays = "Item.CreationAgeInDays";

			public const string CreationAgeInMonths = "Item.CreationAgeInMonths";

			public const string CreationAgeInYears = "Item.CreationAgeInYears";

			public const string ModificationAgeInDays = "Item.ModificationAgeInDays";

			public const string ModificationAgeInMonths = "Item.ModificationAgeInMonths";

			public const string ModificationAgeInYears = "Item.ModificationAgeInYears";

			public const string ClassificationDiscovered = "Item.ClassificationDiscovered";

			public const string AccessScope = "Item.AccessScope";

			public const string Metadata = "Item.Metadata";
		}
	}
}
