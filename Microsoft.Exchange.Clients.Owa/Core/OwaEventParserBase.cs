using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class OwaEventParserBase
	{
		internal OwaEventParserBase(OwaEventHandlerBase eventHandler, int parameterTableCapacity)
		{
			this.eventHandler = eventHandler;
			this.parameterTable = new Hashtable(parameterTableCapacity);
		}

		protected Hashtable ParameterTable
		{
			get
			{
				return this.parameterTable;
			}
		}

		protected ulong SetMask
		{
			get
			{
				return this.setMask;
			}
		}

		protected OwaEventHandlerBase EventHandler
		{
			get
			{
				return this.eventHandler;
			}
		}

		internal Hashtable Parse()
		{
			Hashtable result = this.ParseParameters();
			if ((this.EventHandler.EventInfo.RequiredMask & this.SetMask) != this.EventHandler.EventInfo.RequiredMask)
			{
				this.ThrowParserException("A required parameter of the event wasn't set");
			}
			return result;
		}

		protected abstract Hashtable ParseParameters();

		protected abstract void ThrowParserException(string description);

		protected void ThrowParserException()
		{
			this.ThrowParserException(null);
		}

		protected OwaEventParameterAttribute GetParamInfo(string name)
		{
			OwaEventParameterAttribute owaEventParameterAttribute = this.EventHandler.EventInfo.FindParameterInfo(name);
			if (owaEventParameterAttribute == null)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' is unknown.", new object[]
				{
					name
				}));
			}
			return owaEventParameterAttribute;
		}

		protected void AddParameter(OwaEventParameterAttribute paramInfo, object value)
		{
			if (this.parameterTable.Count >= 64)
			{
				this.ThrowParserException("Reached maximum number of parameters");
			}
			if ((this.setMask & paramInfo.ParameterMask) != 0UL)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' is found twice", new object[]
				{
					paramInfo.Name
				}));
			}
			this.setMask |= paramInfo.ParameterMask;
			this.parameterTable.Add(paramInfo.Name, value);
		}

		protected void AddSimpleTypeParameter(OwaEventParameterAttribute paramInfo, string value)
		{
			object value2;
			if (paramInfo.Type == typeof(string))
			{
				value2 = value;
			}
			else
			{
				value2 = this.ConvertToStrongType(paramInfo.Type, value);
			}
			this.AddParameter(paramInfo, value2);
		}

		protected void AddStructParameter(OwaEventParameterAttribute paramInfo, object value)
		{
			this.AddParameter(paramInfo, value);
		}

		protected void AddArrayParameter(OwaEventParameterAttribute paramInfo, ArrayList itemArray)
		{
			Array value = itemArray.ToArray(paramInfo.Type);
			this.AddParameter(paramInfo, value);
		}

		protected void AddEmptyParameter(OwaEventParameterAttribute paramInfo)
		{
			if (paramInfo.IsStruct && !paramInfo.IsArray)
			{
				this.ThrowParserException("Empty structs not supported");
				return;
			}
			if (paramInfo.IsArray)
			{
				this.AddArrayParameter(paramInfo, new ArrayList());
				return;
			}
			this.AddSimpleTypeParameter(paramInfo, string.Empty);
		}

		private void AddItemToArray(ArrayList itemArray, object value)
		{
			if (itemArray.Count >= 2000)
			{
				this.ThrowParserException("Reached maximum number of items in an array");
			}
			itemArray.Add(value);
		}

		protected void AddSimpleTypeToArray(OwaEventParameterAttribute paramInfo, ArrayList itemArray, string value)
		{
			object value2;
			if (paramInfo.Type == typeof(string))
			{
				value2 = value;
			}
			else
			{
				value2 = this.ConvertToStrongType(paramInfo.Type, value);
			}
			this.AddItemToArray(itemArray, value2);
		}

		protected void AddStructToArray(OwaEventParameterAttribute paramInfo, ArrayList itemArray, object value)
		{
			this.AddItemToArray(itemArray, value);
		}

		protected void AddEmptyItemToArray(OwaEventParameterAttribute paramInfo, ArrayList itemArray)
		{
			if (paramInfo.IsStruct)
			{
				this.ThrowParserException("Empty structs not supported");
				return;
			}
			this.AddSimpleTypeParameter(paramInfo, string.Empty);
		}

		protected object ConvertToStrongType(Type paramType, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
				{
					paramType,
					value
				}));
			}
			try
			{
				if (paramType.IsEnum)
				{
					OwaEventEnumAttribute owaEventEnumAttribute = OwaEventRegistry.FindEnumInfo(paramType);
					int intValue = int.Parse(value, CultureInfo.InvariantCulture);
					object obj = owaEventEnumAttribute.FindValueInfo(intValue);
					if (obj == null)
					{
						this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse enum type. Type = {0}, Value = {1}", new object[]
						{
							paramType,
							value
						}));
					}
					return obj;
				}
				if (paramType == typeof(int))
				{
					return int.Parse(value, CultureInfo.InvariantCulture);
				}
				if (paramType == typeof(double))
				{
					return double.Parse(value, CultureInfo.InvariantCulture);
				}
				if (paramType == typeof(ExDateTime))
				{
					return DateTimeUtilities.ParseIsoDate(value, this.EventHandler.OwaContext.SessionContext.TimeZone);
				}
				if (paramType == typeof(bool))
				{
					if (string.Equals(value, "0", StringComparison.Ordinal))
					{
						return false;
					}
					if (string.Equals(value, "1", StringComparison.Ordinal))
					{
						return true;
					}
					this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
					{
						paramType,
						value
					}));
				}
				else
				{
					if (paramType == typeof(StoreObjectId))
					{
						UserContext userContext = this.EventHandler.OwaContext.UserContext;
						return Utilities.CreateStoreObjectId(userContext.MailboxSession, value);
					}
					if (paramType == typeof(ADObjectId))
					{
						ADObjectId adobjectId = DirectoryAssistance.ParseADObjectId(value);
						if (adobjectId == null)
						{
							this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
							{
								paramType,
								value
							}));
						}
						return adobjectId;
					}
					if (paramType == typeof(DocumentLibraryObjectId))
					{
						UserContext userContext2 = this.EventHandler.OwaContext.UserContext;
						Uri uri;
						if (null == (uri = Utilities.TryParseUri(value)))
						{
							return null;
						}
						ClassifyResult[] array = null;
						OwaWindowsIdentity owaWindowsIdentity = userContext2.LogonIdentity as OwaWindowsIdentity;
						if (owaWindowsIdentity != null && owaWindowsIdentity.WindowsPrincipal != null)
						{
							array = LinkClassifier.ClassifyLinks(owaWindowsIdentity.WindowsPrincipal, new Uri[]
							{
								uri
							});
						}
						if (array == null || array.Length == 0)
						{
							return null;
						}
						return array[0].ObjectId;
					}
					else if (paramType == typeof(OwaStoreObjectId))
					{
						UserContext userContext3 = this.EventHandler.OwaContext.UserContext;
						if (OwaStoreObjectId.IsDummyArchiveFolder(value))
						{
							return userContext3.GetArchiveRootFolderId();
						}
						return OwaStoreObjectId.CreateFromString(value);
					}
					else
					{
						this.ThrowParserException("Internal error: unknown type");
					}
				}
			}
			catch (FormatException)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
				{
					paramType,
					value
				}));
			}
			catch (OwaParsingErrorException)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
				{
					paramType,
					value
				}));
			}
			catch (OverflowException)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Type overflow. Type = {0}, Value = {1}", new object[]
				{
					paramType,
					value
				}));
			}
			return null;
		}

		internal const int MaxStructFieldCount = 32;

		internal const int MaxParameterCount = 64;

		internal const int MaxParameterCountGet = 16;

		internal const int MaxArrayItemCount = 2000;

		protected const string BooleanTrue = "1";

		protected const string BooleanFalse = "0";

		private OwaEventHandlerBase eventHandler;

		private Hashtable parameterTable;

		private ulong setMask;
	}
}
