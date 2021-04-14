using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Exchange.Data.Common
{
	[Serializable]
	public struct LocalizedString : ISerializable, ILocalizedString, IFormattable, IEquatable<LocalizedString>
	{
		public LocalizedString(string id, ExchangeResourceManager resourceManager, params object[] inserts)
		{
			this = new LocalizedString(id, null, false, false, resourceManager, inserts);
		}

		public LocalizedString(string id, string stringId, bool showStringIdIfError, bool showAssistanceInfoIfError, ExchangeResourceManager resourceManager, params object[] inserts)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (resourceManager == null)
			{
				throw new ArgumentNullException("resourceManager");
			}
			this.Id = id;
			this.stringId = stringId;
			this.showStringIdInUIIfError = showStringIdIfError;
			this.showAssistanceInfoInUIIfError = showAssistanceInfoIfError;
			this.ResourceManager = resourceManager;
			this.DeserializedFallback = null;
			this.Inserts = ((inserts != null && inserts.Length > 0) ? inserts : null);
			this.formatParameters = ((this.Inserts != null) ? new ReadOnlyCollection<object>(this.Inserts) : null);
		}

		public LocalizedString(string value)
		{
			this.Id = value;
			this.stringId = null;
			this.showStringIdInUIIfError = false;
			this.showAssistanceInfoInUIIfError = false;
			this.Inserts = null;
			this.ResourceManager = null;
			this.DeserializedFallback = null;
			this.formatParameters = null;
		}

		private LocalizedString(string format, object[] inserts)
		{
			this.Id = format;
			this.stringId = null;
			this.showStringIdInUIIfError = false;
			this.showAssistanceInfoInUIIfError = false;
			this.Inserts = inserts;
			this.ResourceManager = null;
			this.DeserializedFallback = null;
			this.formatParameters = ((this.Inserts != null) ? new ReadOnlyCollection<object>(this.Inserts) : null);
		}

		private LocalizedString(SerializationInfo info, StreamingContext context)
		{
			this.Inserts = (object[])info.GetValue("inserts", typeof(object[]));
			this.formatParameters = ((this.Inserts != null) ? new ReadOnlyCollection<object>(this.Inserts) : null);
			this.ResourceManager = null;
			this.Id = null;
			this.stringId = null;
			this.showStringIdInUIIfError = false;
			this.showAssistanceInfoInUIIfError = false;
			this.DeserializedFallback = null;
			string text = null;
			try
			{
				string @string = info.GetString("baseName");
				text = info.GetString("fallback");
				if (!string.IsNullOrEmpty(@string))
				{
					string string2 = info.GetString("assemblyName");
					Assembly assembly = Assembly.Load(new AssemblyName(string2));
					this.ResourceManager = ExchangeResourceManager.GetResourceManager(@string, assembly);
					this.Id = info.GetString("id");
					if (this.ResourceManager.GetString(this.Id) == null)
					{
						this.ResourceManager = null;
					}
					else
					{
						this.DeserializedFallback = text;
						try
						{
							this.stringId = info.GetString("stringId");
							this.showStringIdInUIIfError = info.GetBoolean("showStringIdInUIIfError");
							this.showAssistanceInfoInUIIfError = info.GetBoolean("showAssistanceInfoInUIIfError");
						}
						catch (SerializationException)
						{
							this.stringId = null;
							this.showStringIdInUIIfError = false;
							this.showAssistanceInfoInUIIfError = false;
						}
					}
				}
			}
			catch (SerializationException)
			{
				this.ResourceManager = null;
			}
			catch (FileNotFoundException)
			{
				this.ResourceManager = null;
			}
			catch (FileLoadException)
			{
				this.ResourceManager = null;
			}
			catch (MissingManifestResourceException)
			{
				this.ResourceManager = null;
			}
			if (this.ResourceManager == null)
			{
				this.Id = text;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return null == this.Id;
			}
		}

		public string FullId
		{
			get
			{
				return ((this.ResourceManager != null) ? this.ResourceManager.BaseName : string.Empty) + this.Id;
			}
		}

		public int BaseId
		{
			get
			{
				return this.FullId.GetHashCode();
			}
		}

		public string StringId
		{
			get
			{
				if (this.stringId != null)
				{
					return this.stringId;
				}
				return string.Empty;
			}
		}

		public bool ShowStringIdInUIIfError
		{
			get
			{
				return this.showStringIdInUIIfError;
			}
		}

		public bool ShowAssistanceInfoInUIIfError
		{
			get
			{
				return this.showAssistanceInfoInUIIfError;
			}
		}

		LocalizedString ILocalizedString.LocalizedString
		{
			get
			{
				return this;
			}
		}

		public ReadOnlyCollection<object> FormatParameters
		{
			get
			{
				return this.formatParameters;
			}
		}

		public static bool operator ==(LocalizedString s1, LocalizedString s2)
		{
			return s1.Equals(s2);
		}

		public static bool operator !=(LocalizedString s1, LocalizedString s2)
		{
			return !s1.Equals(s2);
		}

		public static implicit operator string(LocalizedString value)
		{
			return value.ToString();
		}

		public static LocalizedString Join(object separator, object[] value)
		{
			if (value == null || value.Length == 0)
			{
				return LocalizedString.Empty;
			}
			if (separator == null)
			{
				separator = string.Empty;
			}
			object[] array = new object[value.Length + 1];
			array[0] = separator;
			Array.Copy(value, 0, array, 1, value.Length);
			StringBuilder stringBuilder = new StringBuilder(6 * value.Length);
			stringBuilder.Append("{");
			for (int i = 1; i < value.Length; i++)
			{
				stringBuilder.Append(i);
				stringBuilder.Append("}{0}{");
			}
			stringBuilder.Append(value.Length + "}");
			return new LocalizedString(stringBuilder.ToString(), array);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			object[] array = null;
			if (this.Inserts != null && this.Inserts.Length > 0)
			{
				array = new object[this.Inserts.Length];
				for (int i = 0; i < this.Inserts.Length; i++)
				{
					object obj = this.Inserts[i];
					if (obj != null)
					{
						if (obj is ILocalizedString)
						{
							obj = ((ILocalizedString)obj).LocalizedString;
						}
						else if (!obj.GetType().GetTypeInfo().IsSerializable && !(obj is ISerializable))
						{
							object obj2 = LocalizedString.TranslateObject(obj, CultureInfo.InvariantCulture);
							if (obj2 == obj)
							{
								obj = obj.ToString();
							}
							else
							{
								obj = obj2;
							}
						}
					}
					array[i] = obj;
				}
			}
			info.AddValue("inserts", array);
			if (this.ResourceManager == null)
			{
				if (this.DeserializedFallback == null)
				{
					info.AddValue("fallback", this.Id);
				}
				else
				{
					info.AddValue("fallback", this.DeserializedFallback);
				}
				info.AddValue("baseName", string.Empty);
				return;
			}
			info.AddValue("baseName", this.ResourceManager.BaseName);
			info.AddValue("assemblyName", this.ResourceManager.AssemblyName);
			info.AddValue("id", this.Id);
			info.AddValue("stringId", this.stringId);
			info.AddValue("showStringIdInUIIfError", this.showStringIdInUIIfError);
			info.AddValue("showAssistanceInfoInUIIfError", this.showAssistanceInfoInUIIfError);
			if (this.DeserializedFallback == null)
			{
				info.AddValue("fallback", this.ResourceManager.GetString(this.Id, CultureInfo.InvariantCulture));
				return;
			}
			info.AddValue("fallback", this.DeserializedFallback);
		}

		public LocalizedString RecreateWithNewParams(params object[] inserts)
		{
			return new LocalizedString(this.Id, this.StringId, this.ShowStringIdInUIIfError, this.ShowAssistanceInfoInUIIfError, this.ResourceManager, inserts);
		}

		public override string ToString()
		{
			return ((IFormattable)this).ToString(null, null);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return ((IFormattable)this).ToString(null, formatProvider);
		}

		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			if (this.IsEmpty)
			{
				return string.Empty;
			}
			format = ((this.ResourceManager != null) ? this.ResourceManager.GetString(this.Id, formatProvider as CultureInfo) : this.Id);
			if (this.Inserts != null && this.Inserts.Length > 0)
			{
				object[] array = new object[this.Inserts.Length];
				for (int i = 0; i < this.Inserts.Length; i++)
				{
					object obj = this.Inserts[i];
					if (obj is ILocalizedString)
					{
						obj = ((ILocalizedString)obj).LocalizedString;
					}
					else
					{
						obj = LocalizedString.TranslateObject(obj, formatProvider);
					}
					array[i] = obj;
				}
				try
				{
					return string.Format(formatProvider, format, array);
				}
				catch (FormatException)
				{
					if (this.DeserializedFallback == null)
					{
						throw;
					}
					return string.Format(formatProvider, this.DeserializedFallback, array);
				}
				return format;
			}
			return format;
		}

		public override int GetHashCode()
		{
			int num = (this.Id != null) ? this.Id.GetHashCode() : 0;
			int num2 = (this.ResourceManager != null) ? this.ResourceManager.GetHashCode() : 0;
			int num3 = num ^ num2;
			if (this.Inserts != null)
			{
				for (int i = 0; i < this.Inserts.Length; i++)
				{
					num3 = (num3 ^ i ^ ((this.Inserts[i] != null) ? this.Inserts[i].GetHashCode() : 0));
				}
			}
			return num3;
		}

		public override bool Equals(object obj)
		{
			return obj is LocalizedString && this.Equals((LocalizedString)obj);
		}

		public bool Equals(LocalizedString that)
		{
			if (!string.Equals(this.Id, that.Id, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!string.Equals(this.stringId, that.stringId, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (this.showStringIdInUIIfError != that.showStringIdInUIIfError)
			{
				return false;
			}
			if (this.showAssistanceInfoInUIIfError != that.showAssistanceInfoInUIIfError)
			{
				return false;
			}
			if (null != this.ResourceManager ^ null != that.ResourceManager)
			{
				return false;
			}
			if (this.ResourceManager != null && !this.ResourceManager.Equals(that.ResourceManager))
			{
				return false;
			}
			if (null != this.Inserts ^ null != that.Inserts)
			{
				return false;
			}
			if (this.Inserts != null && that.Inserts != null)
			{
				if (this.Inserts.Length != that.Inserts.Length)
				{
					return false;
				}
				for (int i = 0; i < this.Inserts.Length; i++)
				{
					if (null != this.Inserts[i] ^ null != that.Inserts[i])
					{
						return false;
					}
					if (this.Inserts[i] != null && that.Inserts[i] != null && !this.Inserts[i].Equals(that.Inserts[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static object TranslateObject(object badObject, IFormatProvider formatProvider)
		{
			if (badObject is Exception)
			{
				return ((Exception)badObject).Message;
			}
			return badObject;
		}

		public static readonly LocalizedString Empty = default(LocalizedString);

		internal readonly string Id;

		private readonly object[] Inserts;

		private readonly string stringId;

		private readonly bool showStringIdInUIIfError;

		private readonly bool showAssistanceInfoInUIIfError;

		private readonly ExchangeResourceManager ResourceManager;

		private readonly string DeserializedFallback;

		private ReadOnlyCollection<object> formatParameters;
	}
}
