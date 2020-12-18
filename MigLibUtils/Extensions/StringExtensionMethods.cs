using APIBase.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MigLibUtils.Extensions
{
    public static class StringExtensionMethods
    {
        public static Dictionary<string, object> JSONDic(this string s)
        {
            var ret = JsonConvert.DeserializeObject<Dictionary<string, object>>(s);
            RecurseDeserialize(ret);
            return ret;
        }

        public static string JSONString(this object obj, bool formatted = true)
        {
            if (obj == null)
                return null;

            Newtonsoft.Json.Formatting form = default;
            if (formatted)
                form = Newtonsoft.Json.Formatting.Indented;

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(obj, form);

            return ret;
        }

        private static void RecurseDeserialize(Dictionary<string, object> result)
        {
            //Iterate throgh key/value pairs
            if (result == null)
                return;

            foreach (var keyValuePair in result.ToArray())
            {
                //Check to see if Newtonsoft thinks this is a JArray
                var jarray = keyValuePair.Value as JArray;
                var jobject = keyValuePair.Value as JObject;
                var jvalue = keyValuePair.Value as JValue;

                if (jarray != null)
                {
                    //We have a JArray
                    if (jarray.Count > 0 && jarray.First.GetType() == typeof(JObject))
                    {
                        //Convert JArray back to json and deserialize to a list of dictionaries
                        var dictionaries = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarray.ToString());

                        //Set the result as the dictionary
                        result[keyValuePair.Key] = dictionaries;

                        //Iterate throught the dictionaries
                        foreach (var dictionary in dictionaries)
                        {
                            //Recurse
                            RecurseDeserialize(dictionary);
                        }
                    }
                    else
                    {
                        result[keyValuePair.Key] = JsonConvert.DeserializeObject<List<object>>(jarray.ToString());
                    }


                } else if (jobject != null)
                {
                    var dictionar = JsonConvert.DeserializeObject<Dictionary<string, object>>(jobject.ToString());

                    //Set the result as the dictionary
                    RecurseDeserialize(dictionar);

                    result[keyValuePair.Key] = dictionar;

               
                }
                else if (jvalue != null)
                {
                    result[keyValuePair.Key] = jvalue.Value;
                }
            }
        }



        public static void MergeWith<T>(this IList<T> list, IEnumerable<T> other, bool distinct = true)
        {
            if (other == null) { return; }


            if (!distinct)
            {
                foreach (var o in other)
                {
                    list.Add(o);

                }
            }
            else
            {
                foreach (T item in other)
                {
                    if (!list.Contains(item))
                    {

                        list.Add(item);
                    }
                }
            }

        }


        public static string OuterXMLFormatted(this System.Xml.XmlDocument document)
        {
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);


            writer.Formatting = System.Xml.Formatting.Indented;

            // Write the XML into a formatting XmlTextWriter
            document.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;

            // Read MemoryStream contents into a StreamReader.
            StreamReader sReader = new StreamReader(mStream);

            // Extract the text from the StreamReader.
            string formattedXml = sReader.ReadToEnd();

            return formattedXml;
        }



        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            if (format == null)
                throw new ArgumentNullException("format");


            string regExString = @"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+";

            Regex r = new Regex(regExString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate (Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                if (propertyGroup.Value != "0")
                {
                    //busca o valor até achar um TypeValue ou String
                    var prop = propertyGroup.Value;
                    var val = source;
                    while (prop != "")
                    {
                        var propName = prop.Substring(0, prop.Contains(".") ? prop.IndexOf(".") : prop.Length);

                        val = ReflectionInfo.GetValue(val, propName);

                        if (prop.Contains("."))
                            prop = prop.Substring(prop.IndexOf(".") + 1);
                        else
                            prop = "";

                    }
                    values.Add(val);
                }
                else
                {
                    values.Add(source);
                }

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            if (values.Count == 0)
            {
                return rewrittenFormat;
            }

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }


        public static string FormatWith(this string format, IFormatProvider provider, object[] arPlainValues, params KeyValuePair<string, object>[] objects)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            var DicObj = new Dictionary<string, object>();
            if (objects != null && objects.Length > 0)
            {
                DicObj = objects.ToDictionaryStringObject();
            }

            List<object> values = new List<object>();

            //busca numeros em {n} ou objetos direto
            string regExStringPlain = @"(?<start>\{)+(?<property>[0-9A-Z]+)(?<format>:[^}]+)?(?<end>\})+";
            Regex rPlain = new Regex(regExStringPlain, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            string rewrittenFormatNumber = rPlain.Replace(format, delegate (Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                int index;
                if (int.TryParse(propertyGroup.Value, out index))
                {
                    if (arPlainValues != null && arPlainValues.Length > index)
                    {
                        values.Add(arPlainValues[index]);
                    }
                    else
                    {
                        values.Add("");
                    }
                }
                else if (DicObj.ContainsKey(propertyGroup.Value))
                {
                    values.Add(DicObj[propertyGroup.Value]);
                }
                else
                {
                    values.Add("");
                }

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            format = rewrittenFormatNumber;

            string regExString = @"(?<start>\{)+(?<property>{[REPL]}[\w\.\[\]]*)(?<format>:[^}]+)?(?<end>\})+";

            foreach (var key in DicObj.Keys)
            {

                var regExStringObj = regExString.Replace("{[REPL]}", key);

                Regex r = new Regex(regExStringObj, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                string rewrittenFormat = r.Replace(format, delegate (Match m)
                {
                    Group startGroup = m.Groups["start"];
                    Group propertyGroup = m.Groups["property"];
                    Group formatGroup = m.Groups["format"];
                    Group endGroup = m.Groups["end"];


                    //busca o valor até achar um TypeValue ou String
                    var prop = propertyGroup.Value;

                    if (prop.Contains("."))
                        prop = prop.Substring((key + ".").Length);
                    else
                        prop = "";

                    var val = DicObj[key];
                    while (prop != "")
                    {
                        var propName = prop.Substring(0, prop.Contains(".") ? prop.IndexOf(".") : prop.Length);

                        val = ReflectionInfo.GetValue(val, propName);

                        if (prop.Contains("."))
                            prop = prop.Substring(prop.IndexOf(".") + 1);
                        else
                            prop = "";

                    }
                    values.Add(val);


                    return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                      + new string('}', endGroup.Captures.Count);
                });
                format = rewrittenFormat;

            }

            if (values.Count == 0)
            {
                return format;
            }

            return string.Format(provider, format, values.ToArray());
        }

    }

    class ReflectionInfo
    {
        enum COMPTYPE { PROPERTY, METHOD, FIELD };

        private static Dictionary<Type, ReflectionInfo> DicInfos = new Dictionary<Type, ReflectionInfo>();

        Dictionary<string, KeyValuePair<COMPTYPE, object>> Elements = new Dictionary<string, KeyValuePair<COMPTYPE, object>>();

        internal static object GetValue(object obj, string propName)
        {
            if (obj == null) return null;

            object retVal = null;

            //tratar quando o objeto for um dicionario <string,object> pois retorna o valor como propriedade
            if (obj.GetType() == typeof(Dictionary<string, object>))
            {
                var objdic = (Dictionary<string, object>)obj;
                if (objdic.ContainsKey(propName))
                {
                    return objdic[propName];
                }
                else
                {
                    return null;
                }
            }
            else if (obj.GetType() == typeof(JObject))
            {
                var objjo = (JObject)obj;
                JToken jtok = null;
                if (objjo.HasValues && objjo.TryGetValue(propName, out jtok))
                {
                    return jtok;
                }
                else
                {
                    return null;
                }

            }
            else if (obj.GetType() == typeof(DataRow))
            {
                var objjo = (DataRow)obj;
                if (objjo.Table.Columns.Contains(propName))
                {
                    return objjo[propName];
                }
                else
                {
                    return null;
                }

            }



            if (!DicInfos.ContainsKey(obj.GetType()))
            {
                DicInfos[obj.GetType()] = new ReflectionInfo();
            }

            if (!DicInfos[obj.GetType()].Elements.ContainsKey(propName))
            {
                var propInfo = obj.GetType().GetProperty(propName);
                if (propInfo != null)
                {
                    retVal = propInfo.GetValue(obj);
                    DicInfos[obj.GetType()].Elements[propName] = new KeyValuePair<COMPTYPE, object>(COMPTYPE.PROPERTY, propInfo);
                }
                else
                {
                    var fldInfo = obj.GetType().GetField(propName);
                    if (fldInfo != null)
                    {
                        retVal = fldInfo.GetValue(obj);
                        DicInfos[obj.GetType()].Elements[propName] = new KeyValuePair<COMPTYPE, object>(COMPTYPE.FIELD, fldInfo);

                    }
                    else
                    {
                        var mtdInfo = obj.GetType().GetMethod(propName);
                        if (mtdInfo != null)
                        {
                            retVal = mtdInfo.Invoke(obj, null);
                            DicInfos[obj.GetType()].Elements[propName] = new KeyValuePair<COMPTYPE, object>(COMPTYPE.METHOD, mtdInfo);

                        }
                        else
                        {
                            return null;
                        }

                    }

                }
            }

            if (retVal != null)
                return retVal;

            switch (DicInfos[obj.GetType()].Elements[propName].Key)
            {
                case COMPTYPE.PROPERTY:
                    retVal = ((PropertyInfo)DicInfos[obj.GetType()].Elements[propName].Value).GetValue(obj);
                    break;

                case COMPTYPE.FIELD:
                    retVal = ((FieldInfo)DicInfos[obj.GetType()].Elements[propName].Value).GetValue(obj);
                    break;

                case COMPTYPE.METHOD:
                    retVal = ((MethodInfo)DicInfos[obj.GetType()].Elements[propName].Value).Invoke(obj, null);
                    break;

            }
            return retVal;


        }





    }

}
