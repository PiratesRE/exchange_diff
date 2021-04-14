using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Monad;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadDataReader : DbDataReader
	{
		internal MonadDataReader(MonadCommand command, CommandBehavior behavior, MonadAsyncResult asyncResult, string preservedObjectProperty) : this(command, behavior, asyncResult)
		{
			this.preservedObjectProperty = preservedObjectProperty;
		}

		internal MonadDataReader(MonadCommand command, CommandBehavior behavior, MonadAsyncResult asyncResult)
		{
			ExTraceGlobals.IntegrationTracer.Information<string, CommandBehavior>((long)this.GetHashCode(), "--> new MonadDataReader({0}, {1})", command.CommandText, behavior);
			this.command = command;
			this.commandBehavior = behavior;
			this.connection = command.Connection;
			this.isRemote = command.Connection.IsRemote;
			this.asyncResult = asyncResult;
			PSDataCollection<PSObject> output = asyncResult.Output;
			this.command.ActivePipeline.InvocationStateChanged += this.PipelineStateChanged;
			output.DataAdded += this.PipelineDataAdded;
			if (this.WaitOne(output))
			{
				ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "\tFirst result ready.");
				PSObject psobject = output[0];
				PagedPositionInfo pagedPositionInfo = this.UnWrappPSObject(psobject) as PagedPositionInfo;
				if (pagedPositionInfo != null)
				{
					ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "\tPagedPositionInfo object found.");
					output.RemoveAt(0);
					this.positionInfo = pagedPositionInfo;
					psobject = null;
					if (this.WaitOne(output))
					{
						psobject = output[0];
					}
				}
				if (psobject != null)
				{
					ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "\tFirst object returned, generating schema.");
					this.GenerateSchema(psobject);
					this.firstResult = this.UnWrappPSObject(psobject);
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<-- new MonadDataReader()");
		}

		public override bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		public override bool HasRows
		{
			get
			{
				ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "-->MonadDataReader.HasRows");
				this.AssertReaderIsOpen();
				if (this.command.ActivePipeline == null)
				{
					return false;
				}
				bool flag = 0 != this.asyncResult.Output.Count;
				ExTraceGlobals.VerboseTracer.Information<bool>((long)this.GetHashCode(), "<--MonadDataReader.HasRows, {0}", flag);
				return flag;
			}
		}

		public override int FieldCount
		{
			get
			{
				if (!this.HasSchema)
				{
					return -1;
				}
				return this.properties.Count;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				int num = 1;
				if (this.recordsAffected != 0)
				{
					num = this.recordsAffected;
				}
				ExTraceGlobals.VerboseTracer.Information<int>((long)this.GetHashCode(), "MonadDataReader.RecordsAffected, {0}", num);
				return num;
			}
		}

		public override int Depth
		{
			get
			{
				return 0;
			}
		}

		public PagedPositionInfo PositionInfo
		{
			get
			{
				return this.positionInfo;
			}
		}

		public object FirstResult
		{
			get
			{
				return this.firstResult;
			}
		}

		public object LastResult
		{
			get
			{
				return this.lastResult;
			}
		}

		private bool HasSchema
		{
			get
			{
				return null != this.properties;
			}
		}

		public override object this[int ordinal]
		{
			get
			{
				return this.GetValue(ordinal);
			}
		}

		public override object this[string name]
		{
			get
			{
				ExTraceGlobals.DataTracer.Information<string>((long)this.GetHashCode(), "MonadDataReader.this[{0}]", name);
				int ordinal = this.GetOrdinal(name);
				if (ordinal >= 0)
				{
					return this.GetValue(ordinal);
				}
				return null;
			}
		}

		public override void Close()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadDataReader.Close()");
			if (!this.IsClosed)
			{
				try
				{
					this.recordsAffected += this.command.EndExecute(this.asyncResult).Length;
					if (this.command.ActivePipeline != null)
					{
						this.command.ActivePipeline.InvocationStateChanged -= this.PipelineStateChanged;
						PSDataCollection<PSObject> output = this.asyncResult.Output;
						if (output != null)
						{
							output.DataAdded -= this.PipelineDataAdded;
						}
						this.UnblockWaitOne();
					}
				}
				finally
				{
					this.isClosed = true;
					if (this.commandBehavior == CommandBehavior.CloseConnection)
					{
						this.connection.Close();
					}
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadDataReader.Close()");
		}

		public override bool Read()
		{
			ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "MonadDataReader.Read()");
			this.AssertReaderIsOpen();
			if (this.command.ActivePipeline == null)
			{
				throw new InvalidOperationException("The command has been canceled.");
			}
			this.currentRecord = null;
			PSDataCollection<PSObject> output = this.asyncResult.Output;
			if (this.WaitOne(output))
			{
				PSObject psobject = output[0];
				output.RemoveAt(0);
				this.lastResult = this.UnWrappPSObject(psobject);
				if (this.isRemote && psobject.BaseObject is PSCustomObject && this.lastResult != psobject.BaseObject)
				{
					psobject = new PSObject(this.lastResult);
				}
				this.currentRecord = psobject;
				this.recordsAffected++;
			}
			ExTraceGlobals.VerboseTracer.Information<bool>((long)this.GetHashCode(), "<--MonadDataReader.Read(), {0}", null != this.currentRecord);
			return null != this.currentRecord;
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this, this.commandBehavior == CommandBehavior.CloseConnection);
		}

		public override bool NextResult()
		{
			this.AssertReaderIsOpen();
			return false;
		}

		public override object GetValue(int ordinal)
		{
			ExTraceGlobals.DataTracer.Information<int>((long)this.GetHashCode(), "-->MonadDataReader.GetValue({0})", ordinal);
			this.AssertReaderIsOpen();
			object result;
			try
			{
				object obj = null;
				if (this.HasSchema && this.currentRecord != null)
				{
					if (this.useBaseObject)
					{
						if (ordinal == 0)
						{
							obj = this.currentRecord.BaseObject;
						}
					}
					else
					{
						COPropertyInfo copropertyInfo = this.properties[ordinal];
						if (copropertyInfo != null)
						{
							if (string.Equals(copropertyInfo.Name, this.preservedObjectProperty))
							{
								obj = this.currentRecord.BaseObject;
							}
							else
							{
								PSPropertyInfo pspropertyInfo = this.currentRecord.Properties[copropertyInfo.Name];
								if (pspropertyInfo != null)
								{
									obj = pspropertyInfo.Value;
									if (obj is PSObject)
									{
										object obj2 = this.UnWrappPSObject(obj as PSObject);
										if (obj2 is ArrayList)
										{
											obj = obj2;
										}
									}
									if (copropertyInfo.Type == typeof(string) && obj is IList)
									{
										obj = this.FormatListToString((IList)obj);
									}
									else if (copropertyInfo.Type == typeof(string) && obj != null && obj.GetType().IsEnum)
									{
										obj = LocalizedDescriptionAttribute.FromEnum(obj.GetType(), obj);
									}
									else if (copropertyInfo.Type == typeof(EnumObject))
									{
										obj = new EnumObject(obj as Enum);
									}
									else if (copropertyInfo.Type == typeof(string) && obj != null && obj.GetType() == typeof(bool))
									{
										bool flag = (bool)obj;
										if (flag)
										{
											obj = Strings.TrueString;
										}
										else
										{
											obj = Strings.FalseString;
										}
									}
									else if (obj != null && obj.GetType() == typeof(string))
									{
										if (copropertyInfo.Type.IsEnum)
										{
											obj = Enum.Parse(copropertyInfo.Type, obj as string);
										}
										else if (copropertyInfo.Type == typeof(Guid))
										{
											obj = new Guid(obj as string);
										}
									}
								}
							}
						}
					}
				}
				ExTraceGlobals.DataTracer.Information((long)this.GetHashCode(), "<--MonadDataReader.GetValue(), {0}", new object[]
				{
					obj
				});
				result = obj;
			}
			catch (GetValueException ex)
			{
				Exception innerException = ex.InnerException;
				while (innerException is TargetInvocationException)
				{
					innerException = innerException.InnerException;
				}
				if (innerException != null)
				{
					result = innerException.Message;
				}
				else
				{
					result = ex.Message;
				}
			}
			return result;
		}

		public override int GetValues(object[] values)
		{
			this.AssertReaderIsOpen();
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (this.currentRecord == null)
			{
				throw new InvalidOperationException();
			}
			int num = Math.Min(values.Length, this.FieldCount);
			for (int i = 0; i < num; i++)
			{
				values[i] = this.GetValue(i);
			}
			return num;
		}

		public override Type GetFieldType(int ordinal)
		{
			ExTraceGlobals.VerboseTracer.Information<int>((long)this.GetHashCode(), "-->MonadDataReader.GetFieldType({0})", ordinal);
			Type type = typeof(object);
			if (this.HasSchema)
			{
				COPropertyInfo copropertyInfo = this.properties[ordinal];
				if (copropertyInfo != null)
				{
					type = copropertyInfo.Type;
					if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
					{
						type = Nullable.GetUnderlyingType(copropertyInfo.Type);
					}
				}
			}
			ExTraceGlobals.VerboseTracer.Information<Type>((long)this.GetHashCode(), "<--MonadDataReader.GetFieldType(), {0}", type);
			return type;
		}

		public override string GetName(int ordinal)
		{
			ExTraceGlobals.VerboseTracer.Information<int>((long)this.GetHashCode(), "-->MonadDataReader.GetName({0})", ordinal);
			string text = null;
			if (this.HasSchema)
			{
				COPropertyInfo copropertyInfo = this.properties[ordinal];
				if (copropertyInfo != null)
				{
					text = copropertyInfo.Name;
				}
			}
			ExTraceGlobals.VerboseTracer.Information<string>((long)this.GetHashCode(), "<--MonadDataReader.GetName(), {0}", text);
			return text;
		}

		public override int GetOrdinal(string name)
		{
			ExTraceGlobals.VerboseTracer.Information<string>((long)this.GetHashCode(), "-->MonadDataReader.GetOrdinal({0})", name);
			int num = -1;
			if (this.HasSchema)
			{
				num = this.properties.IndexOf(this.properties[name]);
			}
			ExTraceGlobals.VerboseTracer.Information<int>((long)this.GetHashCode(), "<--MonadDataReader.GetOrdinal(), {0}", num);
			return num;
		}

		public override string GetDataTypeName(int ordinal)
		{
			ExTraceGlobals.VerboseTracer.Information<int>((long)this.GetHashCode(), "-->MonadDataReader.GetDataTypeName({0})", ordinal);
			string text = null;
			Type fieldType = this.GetFieldType(ordinal);
			if (null != fieldType)
			{
				text = fieldType.FullName;
			}
			ExTraceGlobals.VerboseTracer.Information<string>((long)this.GetHashCode(), "<--MonadDataReader.GetDataTypeName(), {0}", text);
			return text;
		}

		public void EnforceSchema(DataColumnCollection columnSet, DataColumnMappingCollection mappings)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadDataReader.EnforceSchema()");
			if (columnSet == null || columnSet.Count == 0)
			{
				throw new ArgumentException("Parameter cannot be null or empty.", "columnSet");
			}
			this.useBaseObject = false;
			this.properties = new COPropertyInfoCollection();
			if (mappings != null)
			{
				for (int i = 0; i < mappings.Count; i++)
				{
					this.properties.Add(new COPropertyInfo(mappings[i].SourceColumn, columnSet[mappings[i].DataSetColumn].DataType));
					ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tAdding mapped column {0}.", mappings[i].SourceColumn);
				}
			}
			else
			{
				for (int j = 0; j < columnSet.Count; j++)
				{
					if (string.IsNullOrEmpty(columnSet[j].Expression))
					{
						this.properties.Add(new COPropertyInfo(columnSet[j].ColumnName, columnSet[j].DataType));
					}
					ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tAdding column {0}.", columnSet[j].ColumnName);
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadDataReader.EnforceSchema()");
		}

		public override DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		public override bool GetBoolean(int ordinal)
		{
			return (bool)this.GetValue(ordinal);
		}

		public override byte GetByte(int ordinal)
		{
			return (byte)this.GetValue(ordinal);
		}

		public override long GetBytes(int ordinal, long fieldoffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public override char GetChar(int ordinal)
		{
			return (char)this.GetValue(ordinal);
		}

		public override long GetChars(int ordinal, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public override DateTime GetDateTime(int ordinal)
		{
			return (DateTime)this.GetValue(ordinal);
		}

		public override decimal GetDecimal(int ordinal)
		{
			return (decimal)this.GetValue(ordinal);
		}

		public override double GetDouble(int ordinal)
		{
			return (double)this.GetValue(ordinal);
		}

		public override float GetFloat(int ordinal)
		{
			return (float)this.GetValue(ordinal);
		}

		public override Guid GetGuid(int ordinal)
		{
			return (Guid)this.GetValue(ordinal);
		}

		public override short GetInt16(int ordinal)
		{
			return (short)this.GetValue(ordinal);
		}

		public override int GetInt32(int ordinal)
		{
			return (int)this.GetValue(ordinal);
		}

		public override long GetInt64(int ordinal)
		{
			return (long)this.GetValue(ordinal);
		}

		public override string GetString(int ordinal)
		{
			return (string)this.GetValue(ordinal);
		}

		public override bool IsDBNull(int ordinal)
		{
			return false;
		}

		internal void PipelineDataAdded(object sender, DataAddedEventArgs e)
		{
			this.UnblockWaitOne();
		}

		internal void PipelineStateChanged(object sender, PSInvocationStateChangedEventArgs e)
		{
			if (e.InvocationStateInfo.State == PSInvocationState.Failed || e.InvocationStateInfo.State == PSInvocationState.Stopped || e.InvocationStateInfo.State == PSInvocationState.Completed)
			{
				this.UnblockWaitOne();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
			base.Dispose(disposing);
		}

		private void AssertReaderIsOpen()
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("Reader is already closed.");
			}
		}

		private void GenerateSchema(PSObject template)
		{
			ExTraceGlobals.IntegrationTracer.Information<PSObject>((long)this.GetHashCode(), "-->MonadDataReader.GenerateSchema({0})", template);
			this.properties = new COPropertyInfoCollection();
			IConfigurable configurable = this.UnWrappPSObject(template) as IConfigurable;
			if (configurable != null)
			{
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "\tTemplate is a IConfigurable");
				if (template.Properties["Identity"] != null)
				{
					Type typeFromHandle = typeof(string);
					this.properties.Add(new COPropertyInfo("Identity", typeFromHandle));
					ExTraceGlobals.IntegrationTracer.Information<string, Type>((long)this.GetHashCode(), "\tProperty added to schema: {0}, {1}", "Identity", typeFromHandle);
				}
				foreach (PSPropertyInfo pspropertyInfo in template.Properties)
				{
					if (!("Fields" == pspropertyInfo.Name) && !("Identity" == pspropertyInfo.Name))
					{
						Type type = Type.GetType(pspropertyInfo.TypeNameOfValue);
						if (null == type)
						{
							PropertyInfo property = configurable.GetType().GetProperty(pspropertyInfo.Name);
							if (!(null != property))
							{
								ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tProperty skipped: {0}", pspropertyInfo.Name);
								continue;
							}
							type = property.PropertyType;
						}
						this.properties.Add(new COPropertyInfo(pspropertyInfo.Name, type));
						ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tProperty added to schema: {0}", pspropertyInfo.Name);
					}
				}
				if (!string.IsNullOrEmpty(this.preservedObjectProperty))
				{
					this.properties.Add(new COPropertyInfo(this.preservedObjectProperty, configurable.GetType()));
					ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tProperty added to schema: {0}", this.preservedObjectProperty);
				}
			}
			else
			{
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "\tTemplate is NOT a IConfigurable.");
				this.properties.Add(new COPropertyInfo("Value", this.UnWrappPSObject(template).GetType()));
				this.useBaseObject = true;
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadDataReader.GenerateSchema()");
		}

		private string FormatListToString(IList list)
		{
			string[] array = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null)
				{
					array[i] = list[i].ToString();
				}
			}
			return string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, array);
		}

		private object UnWrappPSObject(PSObject element)
		{
			if (this.isRemote && element.BaseObject != null && element.BaseObject is PSCustomObject && MonadCommand.CanDeserialize(element))
			{
				return MonadCommand.Deserialize(element);
			}
			return element.BaseObject;
		}

		private bool WaitOne(PSDataCollection<PSObject> output)
		{
			if (this.command.ActivePipeline == null)
			{
				return false;
			}
			lock (this.syncObject)
			{
				while (output.IsOpen && output.Count == 0 && (this.command.ActivePipeline.InvocationStateInfo.State == PSInvocationState.NotStarted || this.command.ActivePipeline.InvocationStateInfo.State == PSInvocationState.Running))
				{
					Monitor.Wait(this.syncObject);
				}
			}
			return output.Count > 0;
		}

		private void UnblockWaitOne()
		{
			lock (this.syncObject)
			{
				Monitor.PulseAll(this.syncObject);
			}
		}

		private MonadCommand command;

		private MonadConnection connection;

		private int recordsAffected;

		private PSObject currentRecord;

		private COPropertyInfoCollection properties;

		private MonadAsyncResult asyncResult;

		private CommandBehavior commandBehavior;

		private bool isClosed;

		private bool useBaseObject;

		private bool isRemote;

		private string preservedObjectProperty = string.Empty;

		private object syncObject = new object();

		private PagedPositionInfo positionInfo;

		private object firstResult;

		private object lastResult;
	}
}
