using System;
using System.Collections;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class RightsManagementLicenseData : INestedData
	{
		public RightsManagementLicenseData()
		{
			this.subProperties = new Hashtable();
		}

		public string TemplateID
		{
			get
			{
				return this.subProperties["TemplateID"] as string;
			}
			set
			{
				this.subProperties["TemplateID"] = value;
			}
		}

		public string TemplateName
		{
			get
			{
				return this.subProperties["TemplateName"] as string;
			}
			set
			{
				this.subProperties["TemplateName"] = value;
			}
		}

		public string TemplateDescription
		{
			get
			{
				return this.subProperties["TemplateDescription"] as string;
			}
			set
			{
				this.subProperties["TemplateDescription"] = value;
			}
		}

		public bool? EditAllowed
		{
			get
			{
				string text = this.subProperties["EditAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["EditAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ReplyAllowed
		{
			get
			{
				string text = this.subProperties["ReplyAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ReplyAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ReplyAllAllowed
		{
			get
			{
				string text = this.subProperties["ReplyAllAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ReplyAllAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ForwardAllowed
		{
			get
			{
				string text = this.subProperties["ForwardAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ForwardAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ModifyRecipientsAllowed
		{
			get
			{
				string text = this.subProperties["ModifyRecipientsAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ModifyRecipientsAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ExtractAllowed
		{
			get
			{
				string text = this.subProperties["ExtractAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ExtractAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? PrintAllowed
		{
			get
			{
				string text = this.subProperties["PrintAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["PrintAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ExportAllowed
		{
			get
			{
				string text = this.subProperties["ExportAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ExportAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? ProgrammaticAccessAllowed
		{
			get
			{
				string text = this.subProperties["ProgrammaticAccessAllowed"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ProgrammaticAccessAllowed"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public bool? Owner
		{
			get
			{
				string text = this.subProperties["Owner"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["Owner"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public ExDateTime? ContentExpiryDate
		{
			get
			{
				string text = this.subProperties["ContentExpiryDate"] as string;
				if (text == null)
				{
					return null;
				}
				ExDateTime value;
				if (!ExDateTime.TryParseExact(text, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out value))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidDateTimeInContentExpiryDate"
					};
				}
				return new ExDateTime?(value);
			}
			set
			{
				this.subProperties["ContentExpiryDate"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public string ContentOwner
		{
			get
			{
				return this.subProperties["ContentOwner"] as string;
			}
			set
			{
				this.subProperties["ContentOwner"] = value;
			}
		}

		public IDictionary SubProperties
		{
			get
			{
				return this.subProperties;
			}
		}

		public void Clear()
		{
			this.subProperties.Clear();
		}

		public bool ContainsValidData()
		{
			return this.subProperties.Count > 0;
		}

		public void InitNoRightsTemplate()
		{
			this.EditAllowed = new bool?(false);
			this.ReplyAllowed = new bool?(false);
			this.ReplyAllAllowed = new bool?(false);
			this.ForwardAllowed = new bool?(false);
			this.PrintAllowed = new bool?(false);
			this.ExtractAllowed = new bool?(false);
			this.ProgrammaticAccessAllowed = new bool?(false);
			this.Owner = new bool?(false);
			this.ExportAllowed = new bool?(false);
			this.ModifyRecipientsAllowed = new bool?(false);
			this.ContentOwner = " ";
			this.ContentExpiryDate = new ExDateTime?(new ExDateTime(ExTimeZone.TimeZoneFromKind(DateTimeKind.Utc), DateTime.UtcNow.AddDays(30.0)));
			this.TemplateName = " ";
			this.TemplateDescription = " ";
			this.TemplateID = Guid.Empty.ToString();
		}

		private IDictionary subProperties;
	}
}
