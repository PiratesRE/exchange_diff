using System;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADE4eVirtualDirectory : ExchangeWebAppVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADE4eVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchEncryptionVirtualDirectory";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override void Initialize()
		{
			this.LoadSettings();
		}

		public string E4EConfigurationXML
		{
			get
			{
				return (string)this[ADE4eVirtualDirectorySchema.E4EConfigurationXML];
			}
			set
			{
				this[ADE4eVirtualDirectorySchema.E4EConfigurationXML] = value;
			}
		}

		public MultiValuedProperty<string> AllowedFileTypes
		{
			get
			{
				string valueForTag = this.GetValueForTag("AllowedFileTypes");
				return this.StringToArray(valueForTag);
			}
			set
			{
				string text = this.ArrayToString(value);
				if (text != this.GetValueForTag("AllowedFileTypes"))
				{
					this.SetValueForTag("AllowedFileTypes", text);
				}
			}
		}

		public MultiValuedProperty<string> AllowedMimeTypes
		{
			get
			{
				string valueForTag = this.GetValueForTag("AllowedMimeTypes");
				return this.StringToArray(valueForTag);
			}
			set
			{
				string text = this.ArrayToString(value);
				if (text != this.GetValueForTag("AllowedMimeTypes"))
				{
					this.SetValueForTag("AllowedMimeTypes", text);
				}
			}
		}

		public MultiValuedProperty<string> BlockedFileTypes
		{
			get
			{
				string valueForTag = this.GetValueForTag("BlockedFileTypes");
				return this.StringToArray(valueForTag);
			}
			set
			{
				string text = this.ArrayToString(value);
				if (text != this.GetValueForTag("BlockedFileTypes"))
				{
					this.SetValueForTag("BlockedFileTypes", text);
				}
			}
		}

		public MultiValuedProperty<string> BlockedMimeTypes
		{
			get
			{
				string valueForTag = this.GetValueForTag("BlockedMimeTypes");
				return this.StringToArray(valueForTag);
			}
			set
			{
				string text = this.ArrayToString(value);
				if (text != this.GetValueForTag("BlockedMimeTypes"))
				{
					this.SetValueForTag("BlockedMimeTypes", text);
				}
			}
		}

		public MultiValuedProperty<string> ForceSaveFileTypes
		{
			get
			{
				string valueForTag = this.GetValueForTag("ForceSaveFileTypes");
				return this.StringToArray(valueForTag);
			}
			set
			{
				string text = this.ArrayToString(value);
				if (text != this.GetValueForTag("ForceSaveFileTypes"))
				{
					this.SetValueForTag("ForceSaveFileTypes", text);
				}
			}
		}

		public MultiValuedProperty<string> ForceSaveMimeTypes
		{
			get
			{
				string valueForTag = this.GetValueForTag("ForceSaveMimeTypes");
				return this.StringToArray(valueForTag);
			}
			set
			{
				string text = this.ArrayToString(value);
				if (text != this.GetValueForTag("ForceSaveMimeTypes"))
				{
					this.SetValueForTag("ForceSaveMimeTypes", text);
				}
			}
		}

		public bool? AlwaysShowBcc
		{
			get
			{
				string valueForTag = this.GetValueForTag("AlwaysShowBcc");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new bool?(bool.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("AlwaysShowBcc"))
				{
					this.SetValueForTag("AlwaysShowBcc", text);
				}
			}
		}

		public bool? CheckForForgottenAttachments
		{
			get
			{
				string valueForTag = this.GetValueForTag("CheckForForgottenAttachments");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new bool?(bool.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("CheckForForgottenAttachments"))
				{
					this.SetValueForTag("CheckForForgottenAttachments", text);
				}
			}
		}

		public bool? HideMailTipsByDefault
		{
			get
			{
				string valueForTag = this.GetValueForTag("HideMailTipsByDefault");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new bool?(bool.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("HideMailTipsByDefault"))
				{
					this.SetValueForTag("HideMailTipsByDefault", text);
				}
			}
		}

		public uint? MailTipsLargeAudienceThreshold
		{
			get
			{
				string valueForTag = this.GetValueForTag("MailTipsLargeAudienceThreshold");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new uint?(uint.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MailTipsLargeAudienceThreshold"))
				{
					this.SetValueForTag("MailTipsLargeAudienceThreshold", text);
				}
			}
		}

		public int? MaxRecipientsPerMessage
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxRecipientsPerMessage");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxRecipientsPerMessage"))
				{
					this.SetValueForTag("MaxRecipientsPerMessage", text);
				}
			}
		}

		public int? MaxMessageSizeInKb
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxMessageSizeInKb");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxMessageSizeInKb"))
				{
					this.SetValueForTag("MaxMessageSizeInKb", text);
				}
			}
		}

		public string ComposeFontColor
		{
			get
			{
				return this.GetValueForTag("ComposeFontColor");
			}
			set
			{
				if (value != this.GetValueForTag("ComposeFontColor"))
				{
					this.SetValueForTag("ComposeFontColor", value);
				}
			}
		}

		public string ComposeFontName
		{
			get
			{
				return this.GetValueForTag("ComposeFontName");
			}
			set
			{
				if (value != this.GetValueForTag("ComposeFontName"))
				{
					this.SetValueForTag("ComposeFontName", value);
				}
			}
		}

		public int? ComposeFontSize
		{
			get
			{
				string valueForTag = this.GetValueForTag("ComposeFontSize");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("ComposeFontSize"))
				{
					this.SetValueForTag("ComposeFontSize", text);
				}
			}
		}

		public int? MaxImageSizeKB
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxImageSizeKB");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxImageSizeKB"))
				{
					this.SetValueForTag("MaxImageSizeKB", text);
				}
			}
		}

		public int? MaxAttachmentSizeKB
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxAttachmentSizeKB");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxAttachmentSizeKB"))
				{
					this.SetValueForTag("MaxAttachmentSizeKB", text);
				}
			}
		}

		public int? MaxEncryptedContentSizeKB
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxEncryptedContentSizeKB");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxEncryptedContentSizeKB"))
				{
					this.SetValueForTag("MaxEncryptedContentSizeKB", text);
				}
			}
		}

		public int? MaxEmailStringSize
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxEmailStringSize");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxEmailStringSize"))
				{
					this.SetValueForTag("MaxEmailStringSize", text);
				}
			}
		}

		public int? MaxPortalStringSize
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxPortalStringSize");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxPortalStringSize"))
				{
					this.SetValueForTag("MaxPortalStringSize", text);
				}
			}
		}

		public int? MaxFwdAllowed
		{
			get
			{
				string valueForTag = this.GetValueForTag("MaxFwdAllowed");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("MaxFwdAllowed"))
				{
					this.SetValueForTag("MaxFwdAllowed", text);
				}
			}
		}

		public int? PortalInactivityTimeout
		{
			get
			{
				string valueForTag = this.GetValueForTag("PortalInactivityTimeout");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("PortalInactivityTimeout"))
				{
					this.SetValueForTag("PortalInactivityTimeout", text);
				}
			}
		}

		public int? TDSTimeOut
		{
			get
			{
				string valueForTag = this.GetValueForTag("TDSTimeOut");
				if (string.IsNullOrEmpty(valueForTag))
				{
					return null;
				}
				return new int?(int.Parse(valueForTag));
			}
			set
			{
				string text = (value != null) ? value.ToString() : null;
				if (text != this.GetValueForTag("TDSTimeOut"))
				{
					this.SetValueForTag("TDSTimeOut", text);
				}
			}
		}

		public void LoadSettings()
		{
			this.LoadVDirSettings();
		}

		public void SaveSettings()
		{
			this.SaveVDirSettings();
		}

		private string ArrayToString(MultiValuedProperty<string> input)
		{
			if (input == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Count; i++)
			{
				string value = input[i];
				if (!string.IsNullOrWhiteSpace(value))
				{
					if (i != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}

		private MultiValuedProperty<string> StringToArray(string input)
		{
			if (input == null)
			{
				return null;
			}
			string[] array = input.Split(new char[]
			{
				','
			});
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			foreach (string text in array)
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					multiValuedProperty.Add(text);
				}
			}
			return multiValuedProperty;
		}

		private string GetValueForTag(string tag)
		{
			if (this.xmlDoc != null && this.xmlDoc.DocumentElement != null)
			{
				XmlElement documentElement = this.xmlDoc.DocumentElement;
				if (documentElement.HasChildNodes)
				{
					for (int i = 0; i < documentElement.ChildNodes.Count; i++)
					{
						if (documentElement.ChildNodes[i].Name == tag)
						{
							return ((XmlElement)documentElement.ChildNodes[i]).GetAttribute("Value");
						}
					}
				}
			}
			return null;
		}

		private void SetValueForTag(string tag, string value)
		{
			if (this.xmlDoc != null)
			{
				XmlElement xmlElement;
				if (this.xmlDoc.DocumentElement != null)
				{
					xmlElement = this.xmlDoc.DocumentElement;
				}
				else
				{
					xmlElement = this.xmlDoc.CreateElement("E4E");
					this.xmlDoc.AppendChild(xmlElement);
				}
				XmlElement xmlElement2 = null;
				if (xmlElement.HasChildNodes)
				{
					for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
					{
						if (xmlElement.ChildNodes[i].Name == tag)
						{
							xmlElement2 = (XmlElement)xmlElement.ChildNodes[i];
						}
					}
				}
				if (value == null)
				{
					if (xmlElement2 != null)
					{
						xmlElement.RemoveChild(xmlElement2);
						return;
					}
				}
				else
				{
					if (xmlElement2 == null)
					{
						xmlElement2 = this.xmlDoc.CreateElement(tag);
						xmlElement.AppendChild(xmlElement2);
					}
					xmlElement2.SetAttribute("Value", value);
				}
			}
		}

		private void LoadVDirSettings()
		{
			if (this.xmlDoc == null)
			{
				string e4EConfigurationXML = this.E4EConfigurationXML;
				this.xmlDoc = new SafeXmlDocument();
				if (string.IsNullOrEmpty(e4EConfigurationXML))
				{
					XmlElement newChild = this.xmlDoc.CreateElement("E4E");
					this.xmlDoc.AppendChild(newChild);
					return;
				}
				this.xmlDoc.LoadXml(e4EConfigurationXML);
			}
		}

		private void SaveVDirSettings()
		{
			if (this.xmlDoc != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
				{
					this.xmlDoc.Save(xmlWriter);
					xmlWriter.Close();
				}
				this.E4EConfigurationXML = stringBuilder.ToString();
			}
		}

		private const string MostDerivedClassName = "msExchEncryptionVirtualDirectory";

		internal const string VDirName = "Encryption";

		private const string ValueAttribute = "Value";

		private const string RootNodeName = "E4E";

		public const string AllowedFileTypesProperty = "AllowedFileTypes";

		public const string AllowedMimeTypesProperty = "AllowedMimeTypes";

		public const string BlockedFileTypesProperty = "BlockedFileTypes";

		public const string BlockedMimeTypesProperty = "BlockedMimeTypes";

		public const string ForceSaveFileTypesProperty = "ForceSaveFileTypes";

		public const string ForceSaveMimeTypesProperty = "ForceSaveMimeTypes";

		public const string AlwaysShowBccProperty = "AlwaysShowBcc";

		public const string CheckForForgottenAttachmentsProperty = "CheckForForgottenAttachments";

		public const string HideMailTipsByDefaultProperty = "HideMailTipsByDefault";

		public const string MailTipsLargeAudienceThresholdProperty = "MailTipsLargeAudienceThreshold";

		public const string MaxRecipientsPerMessageProperty = "MaxRecipientsPerMessage";

		public const string MaxMessageSizeInKbProperty = "MaxMessageSizeInKb";

		public const string ComposeFontColorProperty = "ComposeFontColor";

		public const string ComposeFontNameProperty = "ComposeFontName";

		public const string ComposeFontSizeProperty = "ComposeFontSize";

		public const string MaxImageSizeKBProperty = "MaxImageSizeKB";

		public const string MaxAttachmentSizeKBProperty = "MaxAttachmentSizeKB";

		public const string MaxEncryptedContentSizeKBProperty = "MaxEncryptedContentSizeKB";

		public const string MaxEmailStringSizeProperty = "MaxEmailStringSize";

		public const string MaxPortalStringSizeProperty = "MaxPortalStringSize";

		public const string MaxFwdAllowedProperty = "MaxFwdAllowed";

		public const string PortalInactivityTimeoutProperty = "PortalInactivityTimeout";

		public const string TDSTimeOutProperty = "TDSTimeOut";

		private static readonly ADE4eVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADE4eVirtualDirectorySchema>();

		private SafeXmlDocument xmlDoc;
	}
}
