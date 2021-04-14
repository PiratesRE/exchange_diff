using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal sealed class ResourceIdentifier : IEquatable<ResourceIdentifier>
	{
		internal ResourceIdentifier(string name, string instanceName = "")
		{
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			ArgumentValidator.ThrowIfNull("instanceName", instanceName);
			if (!ResourceIdentifier.NamePattern.IsMatch(name))
			{
				throw new ArgumentException("The resource name should contain only letters and digits", name);
			}
			this.Name = name;
			this.InstanceName = instanceName;
		}

		internal string Name { get; private set; }

		internal string InstanceName { get; private set; }

		public static bool TryParse(string resourceString, out ResourceIdentifier resourceIdentifier)
		{
			resourceIdentifier = null;
			if (string.IsNullOrEmpty(resourceString))
			{
				return false;
			}
			Match match = ResourceIdentifier.ParsePattern.Match(resourceString);
			if (match.Success)
			{
				if (match.Groups["name"].Success && match.Groups["instance"].Success)
				{
					resourceIdentifier = new ResourceIdentifier(match.Groups["name"].Value, match.Groups["instance"].Value);
				}
				else
				{
					resourceIdentifier = new ResourceIdentifier(match.Groups["name"].Value, "");
				}
				return true;
			}
			return false;
		}

		public static bool operator ==(ResourceIdentifier obj1, ResourceIdentifier obj2)
		{
			return object.ReferenceEquals(obj1, obj2) || (!object.ReferenceEquals(obj1, null) && !object.ReferenceEquals(obj2, null) && obj1.Equals(obj2));
		}

		public static bool operator !=(ResourceIdentifier obj1, ResourceIdentifier obj2)
		{
			return !(obj1 == obj2);
		}

		public bool Equals(ResourceIdentifier other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(this, other) || (!(base.GetType() != other.GetType()) && string.Equals(this.InstanceName, other.InstanceName, StringComparison.OrdinalIgnoreCase) && this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase)));
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as ResourceIdentifier);
		}

		public override int GetHashCode()
		{
			int num = 17 + 31 * this.Name.ToLower().GetHashCode();
			if (!string.IsNullOrEmpty(this.InstanceName))
			{
				num += 31 * this.InstanceName.ToLower().GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.InstanceName))
			{
				return this.Name;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", new object[]
			{
				this.Name,
				this.InstanceName
			});
		}

		private static readonly Regex NamePattern = new Regex("^\\w+$");

		private static readonly Regex ParsePattern = new Regex("(?<name>^\\w+)(\\[(?<instance>.+)\\])?$");
	}
}
