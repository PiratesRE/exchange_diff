using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackFileId : IEquatable<ApnsFeedbackFileId>
	{
		internal ApnsFeedbackFileId(ExDateTime date, string appId, string extension, string directory)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNullOrEmpty("extension", extension);
			this.Date = date;
			this.Directory = (directory ?? string.Empty);
			foreach (string text in ApnsFeedbackFileId.WellKnownExtensions)
			{
				if (extension.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					this.Extension = text;
					break;
				}
			}
			if (this.Extension == null)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackFileIdInvalidExtension(extension));
			}
			if (appId.Equals("feedback", StringComparison.OrdinalIgnoreCase))
			{
				if (!(this.Extension == "zip") && !(this.Extension == "metadata"))
				{
					throw new ApnsFeedbackException(Strings.ApnsFeedbackFileIdInvalidPseudoAppId(this.Extension));
				}
				this.AppId = "feedback";
			}
			else
			{
				this.AppId = appId;
			}
			if (this.Extension != "zip")
			{
				string packageExtractionFolderName = this.GetPackageExtractionFolderName();
				if (!this.Directory.EndsWith(packageExtractionFolderName, StringComparison.OrdinalIgnoreCase))
				{
					throw new ApnsFeedbackException(Strings.ApnsFeedbackFileIdInvalidDirectory(packageExtractionFolderName));
				}
			}
		}

		internal ExDateTime Date { get; private set; }

		internal string AppId { get; private set; }

		internal string Extension { get; private set; }

		internal string Directory { get; private set; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ApnsFeedbackFileId);
		}

		public bool Equals(ApnsFeedbackFileId other)
		{
			return other != null && this.Date.Equals(other.Date) && this.AppId.Equals(other.AppId, StringComparison.OrdinalIgnoreCase) && this.Extension == other.Extension;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				string path = string.Format("{0}.{1}.{2}", this.Date.ToString("yyyy_MM_dd_H_mm"), this.AppId, this.Extension);
				this.toString = Path.Combine(this.Directory, path);
			}
			return this.toString;
		}

		internal static ApnsFeedbackFileId Parse(string serializedId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("serializedId", serializedId);
			string directory = null;
			string text = null;
			string text2 = null;
			try
			{
				directory = Path.GetDirectoryName(serializedId);
				text = Path.GetFileNameWithoutExtension(serializedId);
				text2 = Path.GetExtension(serializedId);
			}
			catch (ArgumentException ex)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackFileIdInvalidCharacters(serializedId, ex.Message), ex);
			}
			if (!string.IsNullOrEmpty(text2) && text2.StartsWith("."))
			{
				text2 = text2.Substring(1);
			}
			int num = (text == null) ? -1 : text.IndexOf('.');
			if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text) || num <= 0 || num + 1 >= text.Length)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackFileIdInsufficientComponents(serializedId));
			}
			DateTime dateTime = default(DateTime);
			try
			{
				dateTime = DateTime.ParseExact(text.Substring(0, num), "yyyy_MM_dd_H_mm", DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeUniversal).ToUniversalTime();
			}
			catch (FormatException ex2)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackFileIdInvalidDate(serializedId, ex2.Message), ex2);
			}
			string appId = text.Substring(num + 1, text.Length - num - 1);
			return new ApnsFeedbackFileId((ExDateTime)dateTime, appId, text2, directory)
			{
				toString = serializedId
			};
		}

		internal string GetFeedbackFileSearchPattern()
		{
			return string.Format("{0}*.txt", this.Date.ToString("yyyy_MM_dd_H_mm"));
		}

		internal ApnsFeedbackFileId GetPackageId()
		{
			if (this.Extension == "zip")
			{
				return this;
			}
			return new ApnsFeedbackFileId(this.Date, "feedback", "zip", Path.GetDirectoryName(this.Directory));
		}

		internal string GetPackageExtractionFolder()
		{
			if (this.packageExtractionFolder == null)
			{
				this.packageExtractionFolder = ((this.Extension == "zip") ? Path.Combine(this.Directory, this.GetPackageExtractionFolderName()) : this.Directory);
			}
			return this.packageExtractionFolder;
		}

		private string GetPackageExtractionFolderName()
		{
			return this.Date.ToString("yyyy_MM_dd_H_mm");
		}

		internal const string PackageExtension = "zip";

		internal const string MetadataExtension = "metadata";

		internal const string FeedbackExtension = "txt";

		internal const string DateFormat = "yyyy_MM_dd_H_mm";

		internal const string AllPackageSearchPattern = "*.zip";

		internal const string AllMetadataSearchPattern = "*.metadata";

		private const string FeedbackSearchPatternTemplate = "{0}*.txt";

		private const string StarDot = "*.";

		private const string FileNameFormat = "{0}.{1}.{2}";

		private const string FeedbackPseudoAppId = "feedback";

		private static readonly string[] WellKnownExtensions = new string[]
		{
			"txt",
			"metadata",
			"zip"
		};

		private string toString;

		private string packageExtractionFolder;
	}
}
