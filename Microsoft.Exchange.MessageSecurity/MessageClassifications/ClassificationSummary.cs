using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessageSecurity.MessageClassifications
{
	internal class ClassificationSummary
	{
		private ClassificationSummary()
		{
		}

		public ClassificationSummary(string name, Guid classificationID, string locale, string displayName, string senderDescription, string recipientDescription, ClassificationDisplayPrecedenceLevel displayPrecedence, bool permissionMenuVisible, bool retainClassificationEnabled)
		{
			this.name = name;
			this.classificationID = classificationID;
			this.locale = locale;
			this.displayName = displayName;
			this.senderDescription = senderDescription;
			this.recipientDescription = recipientDescription;
			this.displayPrecedence = displayPrecedence;
			this.permissionMenuVisible = permissionMenuVisible;
			this.retainClassificationEnabled = retainClassificationEnabled;
			this.isClassified = true;
		}

		public ClassificationSummary(ClassificationSummary classificationSummary)
		{
			this.name = classificationSummary.Name;
			this.classificationID = classificationSummary.ClassificationID;
			this.locale = classificationSummary.Locale;
			this.displayName = classificationSummary.DisplayName;
			this.senderDescription = classificationSummary.SenderDescription;
			this.recipientDescription = classificationSummary.RecipientDescription;
			this.displayPrecedence = classificationSummary.DisplayPrecedence;
			this.permissionMenuVisible = classificationSummary.PermissionMenuVisible;
			this.retainClassificationEnabled = classificationSummary.RetainClassificationEnabled;
			this.isClassified = true;
		}

		public ClassificationSummary(MessageClassification messageClassification)
		{
			this.name = messageClassification.Name;
			this.classificationID = messageClassification.ClassificationID;
			this.locale = messageClassification.Locale;
			this.displayName = messageClassification.DisplayName;
			this.senderDescription = messageClassification.SenderDescription;
			this.recipientDescription = messageClassification.RecipientDescription;
			this.displayPrecedence = messageClassification.DisplayPrecedence;
			this.permissionMenuVisible = messageClassification.PermissionMenuVisible;
			this.retainClassificationEnabled = messageClassification.RetainClassificationEnabled;
			this.isClassified = true;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Guid ClassificationID
		{
			get
			{
				return this.classificationID;
			}
		}

		public string Locale
		{
			get
			{
				return this.locale;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string SenderDescription
		{
			get
			{
				return this.senderDescription;
			}
		}

		public string RecipientDescription
		{
			get
			{
				return this.recipientDescription;
			}
			set
			{
				this.recipientDescription = value;
			}
		}

		public ClassificationDisplayPrecedenceLevel DisplayPrecedence
		{
			get
			{
				return this.displayPrecedence;
			}
		}

		public bool PermissionMenuVisible
		{
			get
			{
				return this.permissionMenuVisible;
			}
		}

		public bool RetainClassificationEnabled
		{
			get
			{
				return this.retainClassificationEnabled;
			}
		}

		public bool IsClassified
		{
			get
			{
				return this.isClassified;
			}
		}

		public bool IsValid
		{
			get
			{
				return this != ClassificationSummary.Invalid;
			}
		}

		public int Size
		{
			get
			{
				return ClassificationSummary.GetSize(this.name) + Marshal.SizeOf(typeof(Guid)) + ClassificationSummary.GetSize(this.locale) + ClassificationSummary.GetSize(this.displayName) + ClassificationSummary.GetSize(this.senderDescription) + ClassificationSummary.GetSize(this.recipientDescription) + 4 + 1 + 1 + 1;
			}
		}

		private static int GetSize(string str)
		{
			return (string.IsNullOrEmpty(str) ? 0 : str.Length) * 2;
		}

		public static readonly ClassificationSummary Empty = new ClassificationSummary();

		public static readonly ClassificationSummary Invalid = new ClassificationSummary();

		private string name;

		private Guid classificationID;

		private string locale;

		private string displayName = string.Empty;

		private string senderDescription = string.Empty;

		private string recipientDescription = string.Empty;

		private ClassificationDisplayPrecedenceLevel displayPrecedence;

		private bool permissionMenuVisible;

		private bool retainClassificationEnabled;

		private bool isClassified;
	}
}
