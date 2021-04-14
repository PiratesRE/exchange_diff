using System;
using System.Text;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class SyncChangeInfo
	{
		public SyncChangeInfo(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input");
			}
			string[] array = input.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (text.StartsWith("ObjectType:", StringComparison.OrdinalIgnoreCase))
				{
					if (text.Length <= "ObjectType:".Length)
					{
						throw new FormatException("ObjectType is null");
					}
					string text2 = text.Substring("ObjectType:".Length, text.Length - "ObjectType:".Length);
					this.ObjectType = (ConfigurationObjectType)Enum.Parse(typeof(ConfigurationObjectType), text2);
				}
				else if (text.StartsWith("ChangeType:", StringComparison.OrdinalIgnoreCase))
				{
					if (text.Length <= "ChangeType:".Length)
					{
						throw new FormatException("ChangeType is null");
					}
					string text2 = text.Substring("ChangeType:".Length, text.Length - "ChangeType:".Length);
					ChangeType changeType;
					if (!Enum.TryParse<ChangeType>(text2, true, out changeType))
					{
						throw new FormatException("ChangeType is invalid");
					}
					this.ChangeType = changeType;
				}
				else if (text.StartsWith("Version:", StringComparison.OrdinalIgnoreCase))
				{
					if (text.Length <= "Version:".Length)
					{
						throw new FormatException("Version is null");
					}
					string text2 = text.Substring("Version:".Length, text.Length - "Version:".Length);
					this.Version = PolicyVersion.Create(Guid.Parse(text2));
				}
				else
				{
					if (!text.StartsWith("ObjectId:", StringComparison.OrdinalIgnoreCase))
					{
						throw new FormatException("invalid token");
					}
					if (text.Length <= "ObjectId:".Length)
					{
						throw new FormatException("ObjectId is null");
					}
					string text2 = text.Substring("ObjectId:".Length, text.Length - "ObjectId:".Length);
					this.ObjectId = new Guid?(Guid.Parse(text2));
				}
			}
		}

		public SyncChangeInfo(ConfigurationObjectType objectType) : this(objectType, ChangeType.Update, null, null)
		{
		}

		public SyncChangeInfo(ConfigurationObjectType objectType, ChangeType changeType) : this(objectType, changeType, null, null)
		{
		}

		public SyncChangeInfo(ConfigurationObjectType objectType, ChangeType changeType, PolicyVersion version) : this(objectType, changeType, version, null)
		{
		}

		public SyncChangeInfo(ConfigurationObjectType objectType, ChangeType changeType, PolicyVersion version, Guid? objectId)
		{
			this.ObjectType = objectType;
			this.ChangeType = changeType;
			this.Version = version;
			this.ObjectId = objectId;
		}

		public ConfigurationObjectType ObjectType { get; set; }

		public ChangeType ChangeType { get; set; }

		public PolicyVersion Version { get; set; }

		public Guid? ObjectId { get; set; }

		public static SyncChangeInfo Parse(string input)
		{
			return new SyncChangeInfo(input);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ObjectType:");
			stringBuilder.Append(this.ObjectType.ToString());
			stringBuilder.Append(";");
			stringBuilder.Append("ChangeType:");
			stringBuilder.Append(this.ChangeType.ToString());
			stringBuilder.Append(";");
			if (this.Version != null)
			{
				stringBuilder.Append("Version:");
				stringBuilder.Append(this.Version.InternalStorage.ToString());
				stringBuilder.Append(";");
			}
			if (this.ObjectId != null && this.ObjectId != null)
			{
				stringBuilder.Append("ObjectId:");
				stringBuilder.Append(this.ObjectId.Value);
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		private const string ObjectTypeToken = "ObjectType:";

		private const string ChangeTypeToken = "ChangeType:";

		private const string VersionToken = "Version:";

		private const string ObjectIdToken = "ObjectId:";
	}
}
