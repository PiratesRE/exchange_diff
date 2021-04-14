using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Attachment : IDisposeTrackable, IAttachment, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal Attachment(CoreAttachment coreAttachment)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.coreAttachment = coreAttachment;
		}

		public static bool TryFindFileExtension(string filename, out string extension, out string rest)
		{
			if (string.IsNullOrEmpty(filename))
			{
				extension = null;
				rest = null;
				return false;
			}
			int length = filename.Length;
			for (int i = length - 1; i >= 0; i--)
			{
				char c = filename[i];
				if (c == '.')
				{
					extension = filename.Substring(i, length - i);
					rest = filename.Substring(0, i);
					return true;
				}
				if (Util.IsAttachmentSeparator(c))
				{
					break;
				}
			}
			extension = null;
			rest = filename;
			return false;
		}

		public static string TrimFilename(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return string.Empty;
			}
			char[] array = filename.ToCharArray();
			for (int num = 0; num != array.Length; num++)
			{
				if (char.IsSeparator(array[num]))
				{
					array[num] = ' ';
				}
			}
			return filename.TrimStart(new char[0]).TrimEnd(new char[]
			{
				'.',
				' '
			});
		}

		public abstract DisposeTracker GetDisposeTracker();

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override string ToString()
		{
			return this.coreAttachment.ToString();
		}

		private static CoreAttachment CopyAttachment(CoreAttachment sourceAttachment, ICollection<NativeStorePropertyDefinition> excludeProperties)
		{
			CoreAttachment coreAttachment = null;
			if (sourceAttachment != null)
			{
				coreAttachment = sourceAttachment.ParentCollection.Create(AttachmentType.Stream);
				sourceAttachment.PropertyBag.Load(InternalSchema.ContentConversionProperties);
				foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in sourceAttachment.PropertyBag.AllNativeProperties)
				{
					if (excludeProperties == null || !excludeProperties.Contains(nativeStorePropertyDefinition))
					{
						PersistablePropertyBag.CopyProperty(sourceAttachment.PropertyBag, nativeStorePropertyDefinition, coreAttachment.PropertyBag);
					}
				}
			}
			return coreAttachment;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.coreAttachment != null)
				{
					this.coreAttachment.Dispose();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		internal bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		internal CoreAttachment CoreAttachment
		{
			get
			{
				return this.coreAttachment;
			}
		}

		public AttachmentId Id
		{
			get
			{
				this.CheckDisposed("Id::get");
				return this.coreAttachment.Id;
			}
		}

		public virtual AttachmentType AttachmentType
		{
			get
			{
				this.CheckDisposed("AttachmentType::get");
				return AttachmentType.Stream;
			}
		}

		public string FileName
		{
			get
			{
				this.CheckDisposed("FileName::get");
				string valueOrDefault = this.GetValueOrDefault<string>(InternalSchema.AttachLongFileName);
				if (valueOrDefault == null)
				{
					valueOrDefault = this.GetValueOrDefault<string>(InternalSchema.AttachFileName, string.Empty);
				}
				return Attachment.TrimFilename(valueOrDefault);
			}
			set
			{
				this.CheckDisposed("FileName::set");
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				string text = Attachment.TrimFilename(value);
				string text2;
				string text3;
				Attachment.TryFindFileExtension(text, out text2, out text3);
				if (string.IsNullOrEmpty(text3))
				{
					text3 = Attachment.GenerateFilename();
					text = text3 + text2;
				}
				this.PropertyBag[InternalSchema.AttachLongFileName] = text;
				this.PropertyBag[InternalSchema.DisplayName] = text;
				this.PropertyBag[InternalSchema.AttachFileName] = Attachment.Make8x3FileName(text, this.Session != null && this.Session.IsMoveUser);
				this.PropertyBag[InternalSchema.AttachExtension] = (text2 ?? string.Empty);
			}
		}

		public bool IsContactPhoto
		{
			get
			{
				this.CheckDisposed("IsContactPhoto::get");
				return this.GetValueOrDefault<bool>(InternalSchema.IsContactPhoto);
			}
		}

		public string FileExtension
		{
			get
			{
				this.CheckDisposed("FileExtension::get");
				string fileName = this.FileName;
				string text;
				string text2;
				Attachment.TryFindFileExtension(fileName, out text, out text2);
				return text ?? string.Empty;
			}
		}

		public Uri ContentBase
		{
			get
			{
				this.CheckDisposed("ContentBase::get");
				return CoreAttachment.GetUriProperty(this.coreAttachment, InternalSchema.AttachContentBase);
			}
		}

		public string ContentId
		{
			get
			{
				this.CheckDisposed("ContentId::get");
				return this.GetValueOrDefault<string>(InternalSchema.AttachContentId, string.Empty);
			}
			set
			{
				this.CheckDisposed("ContentId::set");
				this.PropertyBag.SetOrDeleteProperty(InternalSchema.AttachContentId, value);
			}
		}

		public int RenderingPosition
		{
			get
			{
				this.CheckDisposed("RenderingPosition::get");
				return this.GetValueOrDefault<int>(InternalSchema.RenderingPosition, -1);
			}
			set
			{
				this.CheckDisposed("RenderingPosition::set");
				this.PropertyBag.SetOrDeleteProperty(InternalSchema.RenderingPosition, value);
			}
		}

		public Uri ContentLocation
		{
			get
			{
				this.CheckDisposed("ContentLocation::get");
				return CoreAttachment.GetUriProperty(this.coreAttachment, InternalSchema.AttachContentLocation);
			}
		}

		public virtual string ContentType
		{
			get
			{
				this.CheckDisposed("ContentType::get");
				return this.GetValueOrDefault<string>(InternalSchema.AttachMimeTag);
			}
			set
			{
				this.CheckDisposed("ContentType::set");
				this.PropertyBag.SetOrDeleteProperty(InternalSchema.AttachMimeTag, value);
			}
		}

		public string CalculatedContentType
		{
			get
			{
				this.CheckDisposed("CalculatedContentType");
				return this.CalculateContentType();
			}
		}

		public bool IsInline
		{
			get
			{
				this.CheckDisposed("IsInline::get");
				return this.coreAttachment.IsInline;
			}
			set
			{
				this.CheckDisposed("IsInline::set");
				this.coreAttachment.IsInline = value;
			}
		}

		public string DisplayName
		{
			get
			{
				this.CheckDisposed("DisplayName::get");
				return this.GetValueOrDefault<string>(InternalSchema.DisplayName, string.Empty);
			}
		}

		public long Size
		{
			get
			{
				this.CheckDisposed("Size::get");
				return (long)this.GetValueOrDefault<int>(InternalSchema.AttachSize);
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				this.CheckDisposed("CreationTime::get");
				return this.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.Now);
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				this.CheckDisposed("LastModifiedTime::get");
				return this.GetValueOrDefault<ExDateTime>(InternalSchema.LastModifiedTime, ExDateTime.Now);
			}
		}

		public StoreObjectValidationError[] Validate()
		{
			this.CheckDisposed("Validate");
			ValidationContext context = new ValidationContext(this.Session);
			return Validation.CreateStoreObjectValiationErrorArray(this.coreAttachment, context);
		}

		public void Save()
		{
			this.CheckDisposed("Save");
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(ServerStrings.CannotSaveReadOnlyAttachment);
			}
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Attachment::Save. HashCode = {0}", this.GetHashCode());
			this.OnBeforeSave();
			this.coreAttachment.Save();
			this.OnAfterSave();
		}

		public Charset TextCharset
		{
			get
			{
				string valueOrDefault = this.GetValueOrDefault<string>(InternalSchema.TextAttachmentCharset);
				Charset charset;
				if (valueOrDefault != null && Charset.TryGetCharset(valueOrDefault, out charset) && charset.IsAvailable)
				{
					return charset;
				}
				return null;
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			this.CheckDisposed("GetProperties");
			if (propertyDefinitionArray == null)
			{
				throw new ArgumentNullException("propertyDefinitionArray");
			}
			object[] array = new object[propertyDefinitionArray.Count];
			int num = 0;
			foreach (PropertyDefinition property in propertyDefinitionArray)
			{
				array[num++] = this.TryGetProperty(property);
			}
			return array;
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			this.CheckDisposed("SetProperties");
			if (propertyDefinitionArray == null || propertyValuesArray == null || propertyDefinitionArray.Count != propertyValuesArray.Length)
			{
				throw new ArgumentException(ServerStrings.PropertyDefinitionsValuesNotMatch);
			}
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
			{
				this[propertyDefinition] = propertyValuesArray[num++];
			}
		}

		public virtual Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			this.CheckDisposed("OpenProperyStream");
			EnumValidator.ThrowIfInvalid<PropertyOpenMode>(openMode, "openMode");
			InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			if (!(propertyDefinition is NativeStorePropertyDefinition))
			{
				throw new NotSupportedException(ServerStrings.ExCalculatedPropertyStreamAccessNotSupported(propertyDefinition.Name));
			}
			return this.PropertyBag.OpenPropertyStream(propertyDefinition, openMode);
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				this.CheckDisposed("this::get[PropertyDefinition]");
				object obj = this.TryGetProperty(propertyDefinition);
				PropertyError propertyError = obj as PropertyError;
				if (propertyError != null)
				{
					throw PropertyError.ToException(new PropertyError[]
					{
						propertyError
					});
				}
				return obj;
			}
			set
			{
				this.CheckDisposed("this::set[PropertyDefinition]");
				this.SetProperty(propertyDefinition, value);
			}
		}

		public void Delete(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("Delete");
			this.PropertyBag.Delete(propertyDefinition);
		}

		public bool IsDirty
		{
			get
			{
				this.CheckDisposed("IsDirty");
				return this.PropertyBag.IsDirty;
			}
		}

		public bool IsPropertyDirty(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("IsPropertyDirty");
			return this.PropertyBag.IsPropertyDirty(propertyDefinition);
		}

		public object TryGetProperty(PropertyDefinition property)
		{
			this.CheckDisposed("TryGetProperty");
			return this.PropertyBag.TryGetProperty(property);
		}

		public void Load()
		{
			this.Load(null);
		}

		public void Load(params PropertyDefinition[] properties)
		{
			this.Load((ICollection<PropertyDefinition>)properties);
		}

		public void Load(ICollection<PropertyDefinition> properties)
		{
			this.CheckDisposed("Load");
			this.PropertyBag.Load(properties);
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			this.CheckDisposed("GetValueOrDefault");
			return this.PropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		public void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			this.CheckDisposed("SetOrDeleteProperty");
			this.PropertyBag.SetOrDeleteProperty(propertyDefinition, propertyValue);
		}

		protected OpenPropertyFlags CalculateOpenPropertyFlags()
		{
			OpenPropertyFlags result;
			if (this.IsNew)
			{
				result = OpenPropertyFlags.Create;
			}
			else if (!this.IsReadOnly)
			{
				result = OpenPropertyFlags.Modify;
			}
			else
			{
				result = OpenPropertyFlags.None;
			}
			return result;
		}

		protected PropertyOpenMode CalculateOpenMode()
		{
			PropertyOpenMode result;
			if (this.IsNew)
			{
				result = PropertyOpenMode.Create;
			}
			else if (!this.IsReadOnly)
			{
				result = PropertyOpenMode.Modify;
			}
			else
			{
				result = PropertyOpenMode.ReadOnly;
			}
			return result;
		}

		internal virtual Attachment CreateCopy(AttachmentCollection collection, BodyFormat? targetBodyFormat)
		{
			return (Attachment)Attachment.CreateCopy(this, collection, new AttachmentType?(this.AttachmentType));
		}

		protected static IAttachment CreateCopy(Attachment attachment, AttachmentCollection collection, AttachmentType? newAttachmentType)
		{
			return collection.Create(newAttachmentType, attachment);
		}

		internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return this.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			this.CheckDisposed("GetValueOrDefault");
			return this.PropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		internal T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct
		{
			return this.PropertyBag.GetValueAsNullable<T>(propertyDefinition);
		}

		internal PersistablePropertyBag PropertyBag
		{
			get
			{
				this.CheckDisposed("Attachment.PropertyBag.get");
				return this.coreAttachment.PropertyBag;
			}
		}

		internal PropertyBagSaveFlags SaveFlags
		{
			get
			{
				this.CheckDisposed("Attachment.SaveFlags.get");
				return this.PropertyBag.SaveFlags;
			}
			set
			{
				this.CheckDisposed("Attachment.SaveFlags.set");
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.PropertyBag.SaveFlags = value;
			}
		}

		internal bool IsNew
		{
			get
			{
				return this.coreAttachment.IsNew;
			}
		}

		public bool IsCalendarException
		{
			get
			{
				this.CheckDisposed("IsCalendarException::get");
				return this.coreAttachment.IsCalendarException;
			}
		}

		internal MapiAttach MapiAttach
		{
			get
			{
				return (MapiAttach)this.PropertyBag.MapiProp;
			}
		}

		internal ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				return this.PropertyBag.AllNativeProperties;
			}
		}

		protected StoreSession Session
		{
			get
			{
				return this.coreAttachment.Session;
			}
		}

		protected virtual Schema Schema
		{
			get
			{
				return AttachmentSchema.Instance;
			}
		}

		internal static object Make8x3FileName(object value, bool isMoveUser)
		{
			Util.ThrowOnNullArgument(value, "value");
			string text = (string)value;
			if (!isMoveUser && text.Length > 256)
			{
				throw new PropertyTooBigException(InternalSchema.AttachLongFileName);
			}
			int num = text.LastIndexOf('.');
			string text2;
			string text3;
			if (num == 0)
			{
				text2 = string.Empty;
				text3 = text.Remove(0, 1);
			}
			else if (num >= 0)
			{
				num++;
				char[] array = new char[text.Length - num];
				char[] array2 = new char[num - 1];
				text.CopyTo(num, array, 0, text.Length - num);
				text2 = new string(array);
				text.CopyTo(0, array2, 0, num - 1);
				text3 = new string(array2);
			}
			else
			{
				text2 = string.Empty;
				text3 = text;
			}
			bool flag = false;
			int num2 = 0;
			while (num2 < 3 && num2 < text2.Length)
			{
				if ("+,;=[]".IndexOf(text2[num2]) >= 0)
				{
					text2 = text2.Remove(num2, 1);
					text2 = text2.Insert(num2, "_");
					flag = true;
					num2++;
				}
				else if (" .'\"*<>?:|".IndexOf(text2[num2]) >= 0 || text2[num2] > '\u007f')
				{
					text2 = text2.Remove(num2, 1);
					flag = true;
				}
				else
				{
					num2++;
				}
			}
			if (text2.Length > 3)
			{
				text2 = text2.Substring(0, 3);
				flag = true;
			}
			int num3 = 0;
			while (num3 < 8 && num3 < text3.Length)
			{
				if ("+,;=[]".IndexOf(text3[num3]) >= 0)
				{
					text3 = text3.Remove(num3, 1);
					text3 = text3.Insert(num3, "_");
					flag = true;
					num3++;
				}
				else if (" .'\"*<>?:|".IndexOf(text3[num3]) >= 0 || text3[num3] > '\u007f')
				{
					text3 = text3.Remove(num3, 1);
					flag = true;
				}
				else
				{
					num3++;
				}
			}
			if (text3.Length == 0)
			{
				text3 = "DEF0";
			}
			if (text3.Length > 8 || flag)
			{
				if (text3.Length > 6)
				{
					text3 = text3.Substring(0, 6);
				}
				text3 += "~1";
			}
			if (text2.Length > 0)
			{
				text3 = text3 + "." + text2;
			}
			return text3;
		}

		internal static string GenerateFilename()
		{
			if (Attachment.staticRandom == null)
			{
				Attachment.staticRandom = new Random((int)ExDateTime.Now.UtcTicks);
			}
			int num = Attachment.staticRandom.Next() % 100000;
			return EmailMessageHelpers.GenerateFileName(ref num);
		}

		protected virtual void OnBeforeSave()
		{
		}

		protected virtual void OnAfterSave()
		{
			CalendarItemBase calendarItemBase = this.coreAttachment.ParentCollection.ContainerItem as CalendarItemBase;
			if (calendarItemBase != null)
			{
				if (this.IsNew)
				{
					calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(51061U, LastChangeAction.AttachmentAdded);
					return;
				}
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(47989U, LastChangeAction.AttachmentChange);
			}
		}

		private void SetProperty(PropertyDefinition propertyDefinition, object value)
		{
			this.PropertyBag.SetProperty(propertyDefinition, value);
		}

		private string CalculateContentType()
		{
			return Attachment.CalculateContentType(this.FileExtension);
		}

		internal static string CalculateContentType(string extension)
		{
			if (!StandaloneFuzzing.IsEnabled)
			{
				if (Attachment.contentTypeMapper == null)
				{
					Attachment.contentTypeMapper = ExtensionToContentTypeMapper.Instance;
				}
				if (extension != null)
				{
					if (extension.StartsWith("."))
					{
						extension = extension.Substring(1);
					}
					return Attachment.contentTypeMapper.GetContentTypeByExtension(extension);
				}
			}
			return "application/octet-stream";
		}

		private bool IsReadOnly
		{
			get
			{
				this.CheckDisposed("IsReadOnly::get");
				return this.coreAttachment.IsReadOnly;
			}
		}

		private const string ConvertToUnder = "+,;=[]";

		private const string IllegalDos = " .'\"*<>?:|";

		private const string DefaultFileName = "DEF0";

		private readonly CoreAttachment coreAttachment;

		private bool isDisposed;

		private readonly DisposeTracker disposeTracker;

		private static ExtensionToContentTypeMapper contentTypeMapper = null;

		[ThreadStatic]
		private static Random staticRandom;
	}
}
