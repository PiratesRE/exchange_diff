using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class AttachmentFilterEntrySpecification : IConfigurable
	{
		public AttachmentType Type
		{
			get
			{
				return this.type;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Identity
		{
			get
			{
				return this.ToString();
			}
		}

		public AttachmentFilterEntrySpecification(AttachmentType type, string name)
		{
			this.type = type;
			this.name = name;
		}

		private AttachmentFilterEntrySpecification()
		{
		}

		public override string ToString()
		{
			return this.type.ToString() + ":" + this.name;
		}

		internal static AttachmentFilterEntrySpecification Parse(string storedAttribute)
		{
			AttachmentFilterEntrySpecification attachmentFilterEntrySpecification = new AttachmentFilterEntrySpecification();
			if (storedAttribute.StartsWith(AttachmentType.ContentType.ToString() + ":") && storedAttribute.Length >= AttachmentType.ContentType.ToString().Length + 2)
			{
				attachmentFilterEntrySpecification.type = AttachmentType.ContentType;
				attachmentFilterEntrySpecification.name = storedAttribute.Substring(AttachmentType.ContentType.ToString().Length + 1);
				return attachmentFilterEntrySpecification;
			}
			if (storedAttribute.StartsWith(AttachmentType.FileName.ToString() + ":") && storedAttribute.Length >= AttachmentType.FileName.ToString().Length + 2)
			{
				attachmentFilterEntrySpecification.type = AttachmentType.FileName;
				attachmentFilterEntrySpecification.name = storedAttribute.Substring(AttachmentType.FileName.ToString().Length + 1);
				return attachmentFilterEntrySpecification;
			}
			throw new InvalidDataException(DirectoryStrings.AttachmentFilterEntryInvalid.ToString());
		}

		internal static void ParseFileSpec(string fileSpec, out string blockedExtension, out Regex blockedExpression, out string blockedFileName)
		{
			blockedExtension = null;
			blockedExpression = null;
			blockedFileName = null;
			if (fileSpec.StartsWith("*.") && fileSpec.Length > 2)
			{
				string text = fileSpec.Substring(2);
				if (AttachmentFilterEntrySpecification.Utils.IsRegex(text))
				{
					throw new InvalidDataException(DirectoryStrings.InvalidAttachmentFilterExtension(fileSpec));
				}
				text = AttachmentFilterEntrySpecification.Utils.Unescape(text);
				blockedExtension = "." + text;
				return;
			}
			else
			{
				if (AttachmentFilterEntrySpecification.Utils.IsRegex(fileSpec))
				{
					string pattern = string.Format("^{0}$", fileSpec);
					Regex regex;
					try
					{
						regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
					}
					catch (ArgumentException innerException)
					{
						throw new InvalidDataException(DirectoryStrings.InvalidAttachmentFilterRegex(fileSpec), innerException);
					}
					blockedExpression = regex;
					return;
				}
				blockedFileName = AttachmentFilterEntrySpecification.Utils.Unescape(fileSpec);
				return;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return null;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return ValidationError.None;
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			throw new NotSupportedException();
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotSupportedException();
		}

		private AttachmentType type;

		private string name;

		internal sealed class Utils
		{
			internal static bool IsRegex(string expression)
			{
				for (int i = 0; i < expression.Length; i++)
				{
					if (AttachmentFilterEntrySpecification.Utils.IsSpecial(expression[i]) && !AttachmentFilterEntrySpecification.Utils.IsEscapedCharacter(expression, i))
					{
						return true;
					}
				}
				return false;
			}

			internal static bool IsEscapedCharacter(string expression, int position)
			{
				int num = 0;
				int num2 = position - 1;
				while (num2 >= 0 && expression[num2] == '\\')
				{
					num++;
					num2--;
				}
				return num % 2 != 0;
			}

			internal static bool IsSpecial(char ch)
			{
				for (int i = 0; i < "*+?(|).[]".Length; i++)
				{
					if (ch == "*+?(|).[]"[i])
					{
						return true;
					}
				}
				return false;
			}

			internal static string Unescape(string expression)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int i = 0;
				while (i < expression.Length)
				{
					if (expression[i] == '\\')
					{
						if (i >= expression.Length - 1)
						{
							string message = string.Format("Hit end of string '{0}' and there is no next character to unescape", expression);
							throw new InvalidDataException(message);
						}
						stringBuilder.Append(expression[i + 1]);
						i += 2;
					}
					else
					{
						stringBuilder.Append(expression[i]);
						i++;
					}
				}
				return stringBuilder.ToString();
			}

			private const string SpecialChars = "*+?(|).[]";
		}
	}
}
