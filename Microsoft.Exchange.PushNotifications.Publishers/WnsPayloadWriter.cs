using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsPayloadWriter
	{
		public WnsPayloadWriter()
		{
			this.stringBuilder = new StringBuilder();
			this.elementNames = new Stack<string>();
			this.ValidationErrors = new List<LocalizedString>();
		}

		public bool IsValid
		{
			get
			{
				return this.ValidationErrors.Count == 0;
			}
		}

		public List<LocalizedString> ValidationErrors { get; private set; }

		public void WriteElementStart(string name, bool hasContent = false)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("name", name);
			this.stringBuilder.Append("<").Append(name);
			this.elementNames.Push(hasContent ? name : "/>");
		}

		public void WriteAttribute(string name, int value)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("name", name);
			this.InternalWriteAttribute(name, value.ToString());
		}

		public void WriteAttribute(string name, string value, bool isOptional = false)
		{
			if (!this.CanSkipAttributeWriting(name, string.IsNullOrWhiteSpace(value), isOptional))
			{
				this.InternalWriteAttribute(name, value);
			}
		}

		public void WriteAttribute<T>(string name, T? nullableValue, bool isOptional = false) where T : struct
		{
			string value;
			if (nullableValue == null)
			{
				value = null;
			}
			else
			{
				T value2 = nullableValue.Value;
				value = value2.ToString();
			}
			this.WriteAttribute(name, value, isOptional);
		}

		public void WriteUriAttribute(string name, string serializedUri, bool isOptional = false)
		{
			if (this.CanSkipAttributeWriting(name, string.IsNullOrWhiteSpace(serializedUri), isOptional))
			{
				return;
			}
			try
			{
				Uri uri = new Uri(serializedUri, UriKind.RelativeOrAbsolute);
				StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
				if (uri.IsAbsoluteUri && !ordinalIgnoreCase.Equals(uri.Scheme, "https") && !ordinalIgnoreCase.Equals(uri.Scheme, "http") && !ordinalIgnoreCase.Equals(uri.Scheme, "ms-appx") && !ordinalIgnoreCase.Equals(uri.Scheme, "ms-appdata"))
				{
					this.ValidationErrors.Add(Strings.InvalidWnsUriScheme(serializedUri));
				}
				else
				{
					this.InternalWriteAttribute(name, serializedUri);
				}
			}
			catch (UriFormatException ex)
			{
				this.ValidationErrors.Add(Strings.InvalidWnsUri(serializedUri, ex.Message));
			}
		}

		public void WriteLanguageAttribute(string name, string lang, bool isOptional = false)
		{
			if (this.CanSkipAttributeWriting(name, string.IsNullOrWhiteSpace(lang), isOptional))
			{
				return;
			}
			try
			{
				CultureInfo.GetCultureInfo(lang);
				this.InternalWriteAttribute(name, lang);
			}
			catch (ArgumentException ex)
			{
				this.ValidationErrors.Add(Strings.InvalidWnsLanguage(lang, ex.Message));
			}
		}

		public void WriteTemplateAttribute(string name, WnsTemplateDescription templateDescription, bool isOptional = false)
		{
			if (!this.CanSkipAttributeWriting(name, templateDescription == null, isOptional))
			{
				this.InternalWriteAttribute(name, templateDescription.Name);
			}
		}

		public void WriteSoundAttribute(string name, WnsSound? sound, bool isOptional = false)
		{
			if (this.CanSkipAttributeWriting(name, sound == null, isOptional))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder("ms-winsoundevent:Notification.", 16);
			if (sound >= WnsSound.Alarm)
			{
				stringBuilder.Append("Looping.");
			}
			stringBuilder.Append(sound);
			this.InternalWriteAttribute(name, stringBuilder.ToString());
		}

		public void WriteAttributesEnd()
		{
			this.stringBuilder.Append(">");
		}

		public void WriteContent(string content)
		{
			if (!string.IsNullOrWhiteSpace(content))
			{
				this.stringBuilder.Append(this.XmlEscape(content));
			}
		}

		public void WriteElementEnd()
		{
			if (this.elementNames.Count == 0)
			{
				return;
			}
			string text = this.elementNames.Pop();
			if (text == "/>")
			{
				this.stringBuilder.Append(" ").Append(text);
				return;
			}
			this.stringBuilder.Append("</").Append(text).Append(">");
		}

		public string Dump()
		{
			if (this.IsValid)
			{
				return this.ToString();
			}
			throw new InvalidPushNotificationException(this.ValidationErrors[0]);
		}

		public override string ToString()
		{
			return this.stringBuilder.ToString();
		}

		private bool CanSkipAttributeWriting(string name, bool isEmpty, bool isOptional)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("name", name);
			if (isEmpty && !isOptional)
			{
				this.ValidationErrors.Add(Strings.InvalidWnsAttributeIsMandatory(name));
			}
			return isEmpty;
		}

		private void InternalWriteAttribute(string name, string value)
		{
			this.stringBuilder.Append(" ").Append(name).Append("=\"").Append(this.XmlEscape(value)).Append("\"");
		}

		private string XmlEscape(string unescapedString)
		{
			return SecurityElement.Escape(unescapedString);
		}

		private const string Opener = "<";

		private const string ElementCloserNoContent = "/>";

		private const string ElementCloserContent = "</";

		private const string Closer = ">";

		private const string Space = " ";

		private const string AttributeOpener = "=\"";

		private const string AttributeCloser = "\"";

		private StringBuilder stringBuilder;

		private Stack<string> elementNames;
	}
}
