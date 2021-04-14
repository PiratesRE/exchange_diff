using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class Operation
	{
		internal TimeSpan Sla
		{
			get
			{
				return this.sla;
			}
			set
			{
				this.sla = value;
			}
		}

		internal int MaxRetryAttempts
		{
			get
			{
				return this.maxRetryAttempts;
			}
			set
			{
				this.maxRetryAttempts = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal List<Parameter> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		internal Type ReturnType
		{
			get
			{
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}

		internal List<ResultItem> ExpectedResultItems
		{
			get
			{
				return this.expectedResultItems;
			}
			set
			{
				this.expectedResultItems = value;
			}
		}

		internal object Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		internal TimeSpan PauseTime
		{
			get
			{
				return this.pauseTime;
			}
			set
			{
				this.pauseTime = value;
			}
		}

		internal string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal TimeSpan Latency { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Operation: {0}", this.Name));
			foreach (Parameter parameter in this.Parameters)
			{
				stringBuilder.AppendLine(string.Format("Parameter: {0}", parameter.ToString()));
			}
			foreach (ResultItem resultItem in this.ExpectedResultItems)
			{
				stringBuilder.AppendLine(string.Format("ResultItem: {0}", resultItem.ToString()));
			}
			stringBuilder.AppendLine(string.Format("SLA: {0}", this.Sla));
			stringBuilder.AppendLine(string.Format("ID: {0}", this.Id));
			stringBuilder.AppendLine(string.Format("PauseTime: {0}", this.PauseTime));
			string arg = (this.ReturnType == null) ? string.Empty : this.ReturnType.ToString();
			string arg2 = (this.Result == null) ? string.Empty : Utils.SerializeToXml(this.Result);
			stringBuilder.AppendLine(string.Format("Return type: {0}", arg));
			stringBuilder.AppendLine(string.Format("Return Value: {0}", arg2));
			return stringBuilder.ToString();
		}

		internal bool Invoke(WebServiceClient client, List<Operation> operations, bool throwIfSlaMissed = true)
		{
			if (client == null || operations == null || operations.Count == 0)
			{
				throw new ArgumentNullException();
			}
			object proxy = client.Proxy;
			MethodInfo method = proxy.GetType().GetMethod(this.Name);
			if (method == null)
			{
				throw new ArgumentException(string.Format("Operation '{0}' not found in proxy class type '{1}'.", this.Name, proxy.GetType()));
			}
			object[] array = this.GetParameters(client.Assembly, operations);
			int num = (array == null) ? 0 : array.Length;
			int num2 = method.GetParameters().Count<ParameterInfo>();
			if (num != num2)
			{
				throw new Exception(string.Format("Work definition error - incorrect number of parameters '{0}' in Operation '{1}'; expecting '{2}'", num, this.Name, num2));
			}
			DateTime utcNow = DateTime.UtcNow;
			object obj;
			try
			{
				obj = method.Invoke(proxy, array);
			}
			catch (CommunicationException)
			{
				client.Abort();
				throw;
			}
			catch (TimeoutException)
			{
				client.Abort();
				throw;
			}
			catch (TargetInvocationException)
			{
				client.Abort();
				throw;
			}
			this.ReturnType = method.ReturnType;
			this.Result = obj;
			this.Latency = DateTime.UtcNow.Subtract(utcNow);
			bool flag = true;
			if (this.Latency.CompareTo(this.Sla) > 0)
			{
				flag = false;
				if (throwIfSlaMissed)
				{
					throw new Exception(string.Format("Operation {0} response time ({1}) exceeded SLA ({2})", this.Name, this.Latency, this.Sla));
				}
			}
			return flag;
		}

		internal bool ValidateResult(WebServiceClient client, bool throwIfFailed = true)
		{
			bool flag = true;
			foreach (ResultItem resultItem in this.ExpectedResultItems)
			{
				StringBuilder stringBuilder = this.CreateErrorMessage(resultItem);
				string text;
				XElement n;
				this.PreProcessResult(resultItem, out text, out n);
				switch (resultItem.VerifyMethod)
				{
				case ResultVerifyMethod.ReturnType:
					if (!this.ReturnType.Name.Equals(resultItem.Value, StringComparison.OrdinalIgnoreCase))
					{
						flag = false;
						if (throwIfFailed)
						{
							stringBuilder.AppendLine("Type returned:");
							stringBuilder.AppendLine(this.ReturnType.Name);
							stringBuilder.AppendLine("Expected:");
							stringBuilder.AppendLine(resultItem.Value);
							throw new Exception(stringBuilder.ToString());
						}
					}
					break;
				case ResultVerifyMethod.ReturnValue:
					if (resultItem.Index < 0)
					{
						flag = this.Result.ToString().Equals(resultItem.Value, StringComparison.OrdinalIgnoreCase);
					}
					else
					{
						object obj = ((object[])this.Result)[resultItem.Index];
						flag = obj.ToString().Equals(resultItem.Value, StringComparison.OrdinalIgnoreCase);
					}
					if (!flag && throwIfFailed)
					{
						stringBuilder.AppendLine("Value returned:");
						stringBuilder.AppendLine(text);
						stringBuilder.AppendLine("Expected:");
						stringBuilder.AppendLine(resultItem.Value);
						throw new Exception(stringBuilder.ToString());
					}
					break;
				case ResultVerifyMethod.ReturnValueRegex:
				case ResultVerifyMethod.PropertyValueRegex:
				{
					MatchCollection matchCollection = Regex.Matches(text, resultItem.Value, RegexOptions.IgnoreCase);
					if (matchCollection.Count == 0)
					{
						flag = false;
						if (throwIfFailed)
						{
							stringBuilder.AppendLine("Value returned:");
							stringBuilder.AppendLine(text);
							stringBuilder.AppendLine("Regex:");
							stringBuilder.AppendLine(resultItem.Value);
							throw new Exception(stringBuilder.ToString());
						}
					}
					break;
				}
				case ResultVerifyMethod.ReturnValueContains:
				case ResultVerifyMethod.PropertyValueContains:
					if (!text.ToLower().Contains(resultItem.Value.ToLower()))
					{
						flag = false;
						if (throwIfFailed)
						{
							stringBuilder.AppendLine("Value returned:");
							stringBuilder.AppendLine(text);
							stringBuilder.AppendLine("Substring expected:");
							stringBuilder.AppendLine(resultItem.Value);
							throw new Exception(stringBuilder.ToString());
						}
					}
					break;
				case ResultVerifyMethod.ReturnValueXml:
				case ResultVerifyMethod.PropertyValueXml:
				{
					XElement xelement = resultItem.UseFile ? XElement.Load(resultItem.Value) : XElement.Parse(resultItem.Value);
					if (!XNode.DeepEquals(xelement, n))
					{
						flag = false;
						if (throwIfFailed)
						{
							stringBuilder.AppendLine("Value returned:");
							stringBuilder.AppendLine(text);
							stringBuilder.AppendLine("Expected:");
							stringBuilder.AppendLine(xelement.ToString());
							throw new Exception(stringBuilder.ToString());
						}
					}
					break;
				}
				case ResultVerifyMethod.ReturnValueUseValidator:
				{
					object obj2 = Utils.DeserializeFromXml(resultItem.Value, this.Result.GetType());
					MethodInfo validatorMethod = client.GetValidatorMethod(this.Result.GetType());
					object[] array = new object[3];
					array[0] = obj2;
					array[1] = this.Result;
					object[] array2 = array;
					flag = (bool)validatorMethod.Invoke(null, array2);
					if (!flag && throwIfFailed)
					{
						string value = array2[2] as string;
						stringBuilder.AppendLine(value);
						throw new Exception(stringBuilder.ToString());
					}
					break;
				}
				case ResultVerifyMethod.PropertyValue:
					if (resultItem.Index < 0)
					{
						flag = this.ReturnType.GetProperty(resultItem.PropertyName).GetValue(this.Result, null).ToString().Equals(resultItem.Value, StringComparison.OrdinalIgnoreCase);
					}
					else
					{
						object value2 = this.ReturnType.GetProperty(resultItem.PropertyName).GetValue(this.Result, null);
						flag = ((object[])value2)[resultItem.Index].ToString().Equals(resultItem.Value, StringComparison.OrdinalIgnoreCase);
					}
					if (!flag && throwIfFailed)
					{
						stringBuilder.AppendLine("Value returned:");
						stringBuilder.AppendLine(text);
						stringBuilder.AppendLine("Expected:");
						stringBuilder.AppendLine(resultItem.Value);
						throw new Exception(stringBuilder.ToString());
					}
					break;
				}
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		internal string GetDiagnosticsInfo(WebServiceClient client)
		{
			MethodInfo diagnosticsInfoMethod = client.GetDiagnosticsInfoMethod();
			object[] array = new object[]
			{
				this.Result
			};
			return string.Format("{0}:{1}\n", this.Name, (string)diagnosticsInfoMethod.Invoke(null, array));
		}

		private object[] GetParameters(Assembly assembly, List<Operation> operations)
		{
			if (this.Parameters.Count == 0)
			{
				return null;
			}
			List<object> list = new List<object>();
			using (List<Parameter>.Enumerator enumerator = this.Parameters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Parameter p = enumerator.Current;
					object item2 = null;
					if (!p.IsNull)
					{
						if (string.IsNullOrWhiteSpace(p.UseResultFromOperationId))
						{
							if (string.IsNullOrWhiteSpace(p.Type))
							{
								item2 = p.Value.InnerText;
							}
							else
							{
								Type type;
								try
								{
									type = Type.GetType(p.Type, true, true);
								}
								catch
								{
									type = assembly.GetType(p.Type, true, true);
								}
								if (p.UseFile)
								{
									XmlDocument xmlDocument = new XmlDocument();
									xmlDocument.Load(p.Value.InnerText);
									item2 = Utils.DeserializeFromXml(xmlDocument.DocumentElement.OuterXml, type);
								}
								else
								{
									item2 = Utils.DeserializeFromXml(p.Value.InnerXml, type);
								}
							}
						}
						else
						{
							Operation operation = operations.Find((Operation item) => item.Id.Equals(p.UseResultFromOperationId, StringComparison.OrdinalIgnoreCase));
							if (string.IsNullOrWhiteSpace(p.PropertyName))
							{
								if (p.Index < 0)
								{
									item2 = operation.Result;
								}
								else
								{
									string elementName = string.Format("The return value of previous Operation '{0}'", operation.Name);
									item2 = this.GetIndexedObject(operation.Result, p.Index, elementName);
								}
							}
							else
							{
								PropertyInfo property = operation.Result.GetType().GetProperty(p.PropertyName);
								if (property == null)
								{
									throw new ArgumentException(string.Format("The result of Operation '{0}' does not have a Property called '{1}'", operation.Name, p.PropertyName));
								}
								object value = property.GetValue(operation.Result, null);
								if (p.Index < 0)
								{
									item2 = value;
								}
								else
								{
									string elementName = string.Format("The value of Property '{0}' in the return value of previous Operation '{1}'", p.PropertyName, operation.Name);
									item2 = this.GetIndexedObject(value, p.Index, elementName);
								}
							}
						}
					}
					list.Add(item2);
				}
			}
			return list.ToArray();
		}

		private void PreProcessResult(ResultItem r, out string resultXml, out XElement resultXElement)
		{
			resultXml = null;
			resultXElement = null;
			switch (r.VerifyMethod)
			{
			case ResultVerifyMethod.ReturnValue:
			case ResultVerifyMethod.ReturnValueRegex:
			case ResultVerifyMethod.ReturnValueContains:
			case ResultVerifyMethod.ReturnValueXml:
			case ResultVerifyMethod.ReturnValueIsNotNull:
			case ResultVerifyMethod.ReturnValueUseValidator:
			{
				if (this.Result == null)
				{
					throw new Exception(string.Format("The return value from Operation '{0}' is null; expected non-null result", this.Name));
				}
				if (r.Index < 0)
				{
					resultXml = Utils.SerializeToXml(this.Result);
					resultXElement = Utils.SerializeToXmlElement(this.Result);
					return;
				}
				string elementName = string.Format("The return value of Operation '{0}'", this.Name);
				object indexedObject = this.GetIndexedObject(this.Result, r.Index, elementName);
				resultXml = Utils.SerializeToXml(indexedObject);
				resultXElement = Utils.SerializeToXmlElement(indexedObject);
				return;
			}
			case ResultVerifyMethod.ReturnValueIsNull:
				if (this.Result != null)
				{
					throw new Exception(string.Format("The return value of Operation '{0}' is not null; expected null result", this.Name));
				}
				break;
			case ResultVerifyMethod.PropertyValue:
			case ResultVerifyMethod.PropertyValueRegex:
			case ResultVerifyMethod.PropertyValueContains:
			case ResultVerifyMethod.PropertyValueXml:
			{
				if (this.Result == null)
				{
					throw new Exception(string.Format("The return value of Operation '{0}' is null; expected non-null result", this.Name));
				}
				PropertyInfo property = this.ReturnType.GetProperty(r.PropertyName);
				if (property == null)
				{
					throw new Exception(string.Format("The return value of Operation '{0}' does not have a property called '{1}':\r\n{2}", this.Name, r.PropertyName, Utils.SerializeToXml(this.Result)));
				}
				object value = property.GetValue(this.Result, null);
				if (value == null)
				{
					throw new Exception(string.Format("The value of Property '{0}' in the return value of Operation '{1}' is null; expected non-null value '{2}'", r.PropertyName, this.Name, r.Value));
				}
				if (r.Index < 0)
				{
					resultXml = Utils.SerializeToXml(value);
					resultXElement = Utils.SerializeToXmlElement(value);
					return;
				}
				string elementName = string.Format("The value of Property '{0}' in the return value of Operation '{1}'", r.PropertyName, this.Name);
				object indexedObject2 = this.GetIndexedObject(value, r.Index, elementName);
				resultXml = Utils.SerializeToXml(indexedObject2);
				resultXElement = Utils.SerializeToXmlElement(indexedObject2);
				break;
			}
			default:
				return;
			}
		}

		private object GetIndexedObject(object o, int index, string elementName)
		{
			if (!(o is Array))
			{
				throw new Exception(string.Format("{0} is not an arry, required index={1}.", elementName, index));
			}
			object[] array = (object[])o;
			int num = array.Length;
			if (index >= num)
			{
				throw new Exception(string.Format("{0} has array length={1}, index '{2}' is out of range.", elementName, num, index));
			}
			return array[index];
		}

		private StringBuilder CreateErrorMessage(ResultItem r)
		{
			string text = (r.Index < 0) ? "n/a" : r.Index.ToString();
			string text2 = string.IsNullOrWhiteSpace(r.PropertyName) ? "n/a" : r.PropertyName;
			string value = string.Format("'{0}' validation of operation '{1}' failed - index='{2}', propertyName='{3}'.", new object[]
			{
				r.VerifyMethod.ToString(),
				this.Name,
				text,
				text2
			});
			return new StringBuilder(value);
		}

		private TimeSpan sla;

		private string name;

		private List<Parameter> parameters;

		private Type returnType;

		private List<ResultItem> expectedResultItems;

		private object result;

		private TimeSpan pauseTime;

		private string id;

		private int maxRetryAttempts;
	}
}
