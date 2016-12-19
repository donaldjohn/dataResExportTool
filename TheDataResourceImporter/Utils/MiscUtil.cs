using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpCompress;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Xml;

namespace TheDataResourceExporter.Utils
{
    class MiscUtil
    {
        public static IMPORT_ERROR getImpErrorInstance(string sessionId, string isZip, string zipOrDirPath, string zipPath = "", string errorMessage = "", string errorDetail = "")
        {
            var innerImpError = new IMPORT_ERROR() { ID = System.Guid.NewGuid().ToString(), IGNORED = "N", OCURREDTIME = DateTime.Now, REIMPORTED = "N", ISZIP = isZip, POINTOR = 0, SESSION_ID = sessionId, ZIP_OR_DIR_PATH = zipOrDirPath, ZIP_PATH = zipPath, ERROR_MESSAGE = errorMessage, ERROR_DETAIL = errorDetail };
            return innerImpError;
        }

        public static S_IMPORT_BATH getNewImportBathObject(string fileType)
        {
            return new S_IMPORT_BATH() { ID = System.Guid.NewGuid().ToString(), HANDLED_ITEM_COUNT = 0, ISCOMPLETED = "N", IS_DIR_MODE = "N", RES_TYPE = fileType, START_TIME = System.DateTime.Now };
        }

        public static IMPORT_SESSION getNewImportSession(string fileType, string filePath, S_IMPORT_BATH bath, string IS_ZIP = "Y")
        {
            return new IMPORT_SESSION() { SESSION_ID = System.Guid.NewGuid().ToString(), ROLLED_BACK = "N", BATCH_ID = bath.ID, DATA_RES_TYPE = fileType, START_TIME = System.DateTime.Now, ZIP_OR_DIR_PATH = filePath, HAS_ERROR = "N", FAILED_COUNT = 0, COMPLETED = "N", LAST_TIME = 0, ZIP_ENTRIES_COUNT = 0, ZIP_ENTRY_POINTOR = 0, IS_ZIP = IS_ZIP };
        }

        public static Type getTypeByFullName(string typeFullName)
        {
            return Type.GetType(typeFullName, true);
        }

        public static void setProperityByName(Type type, string propName, Object obj, Object value)
        {
            try
            {
                type.GetProperty(propName).SetValue(obj, value);
            }
            catch (Exception ex)
            {

            }
        }

        /***
         * 导入异常处理，添加错误信息
         * **/
        public static void exceptionHandler(DataSourceEntities entiesContext, string sessionId, string isZip, string zipOrDirPath, string zipPath = "", string errorMessage = "", string errorDetail = "")
        {
            var importError = MiscUtil.getImpErrorInstance(sessionId, isZip, zipOrDirPath, zipPath, errorMessage, errorDetail);
            entiesContext.IMPORT_ERROR.Add(importError);
            //输出错误信息
            MessageUtil.DoAppendTBDetail($"{errorMessage}：{errorDetail}");
        }


        /***
         *获取指定XPath的单值 
         ***/
        public static string getXElementSingleValueByXPath(XElement currentNode, string xPath, string attribute = "", IXmlNamespaceResolver resolver = null)
        {
            string value = "";
            try
            {
                XElement target = null;
                if (null != resolver)
                {
                    target = currentNode.XPathSelectElement(xPath, resolver);
                }
                else
                {
                    target = currentNode.XPathSelectElement(xPath);
                }

                if (null != target)
                {
                    if (String.IsNullOrEmpty(attribute))
                    {
                        value = target.Value;
                    }
                    else
                    {
                        var targetAttr = target.Attribute(attribute);
                        if (null != targetAttr)
                        {
                            value = targetAttr.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
            return value;
        }




        public static string getXElementMultiValueByXPathSepratedByDoubleColon(XElement currentNode, string xPath, IXmlNamespaceResolver resolver = null)
        {
            string value = "";


            try
            {
                if (null != resolver)
                {
                    var targetsEles = from ele in currentNode.XPathSelectElements(xPath, resolver)
                                      select ele.Value;
                    if (0 == targetsEles.Count())
                    {
                        value = "";
                    }
                    else
                    {
                        value = string.Join(";;", targetsEles.ToArray());
                    }
                }
                else
                {
                    var targetsEles = from ele in currentNode.XPathSelectElements(xPath)
                                      select ele.Value;
                    if (0 == targetsEles.Count())
                    {
                        value = "";
                    }
                    else
                    {
                        value = string.Join(";;", targetsEles.ToArray());
                    }
                }
            }
            catch (Exception)
            {

            }


            return value;
        }


        public static string getXElementMultiValueByXPathSepratedByDoubleColon(XElement currentNode, string xPath, string attriName, IXmlNamespaceResolver resolver = null)
        {
            string value = "";

            if (string.IsNullOrEmpty(attriName))
            {
                value = getXElementMultiValueByXPathSepratedByDoubleColon(currentNode, xPath, resolver);
                return value;
            }
            else
            {
                if (null == resolver)
                {
                    //var targets = currentNode.XPathSelectElements(xPath);

                    var targets = currentNode.XPathSelectElements(xPath);

                    var targetsAttr = from ele in targets
                                      where null != ele
                                      select ele.Attribute(attriName);

                    if (0 == targetsAttr.Count())
                    {
                        value = "";
                    }
                    else
                    {
                        var targetsAttrValue = from attr in targetsAttr
                                               where null != attr && null != attr.Value 
                                               select attr.Value;
                        value = string.Join(";;", targetsAttrValue.ToArray());
                    }

                    //var targetValues = ((from ele in targets
                    //                    select ele.Attribute(attriName).Value).Distinct()).ToArray();
                    //value = string.Join(";;", targetValues);
                }
                else
                {

                    var targets = currentNode.XPathSelectElements(xPath, resolver);

                    var targetsAttr = from ele in targets
                                      where null != ele
                                      select ele.Attribute(attriName);

                    if (0 == targetsAttr.Count())
                    {
                        value = "";
                    }
                    else
                    {
                        var targetsAttrValue = from attr in targetsAttr
                                               where null != attr && null != attr.Value
                                               select attr.Value;
                        value = string.Join(";;", targetsAttrValue.ToArray());
                    }

                    //var targets = currentNode.XPathSelectElements(xPath, resolver);
                    //var targetValues = ((from ele in targets
                    //                    select ele.Attribute(attriName).Value).Distinct()).ToArray();
                    //value = string.Join(";;", targetValues);
                }


                //if (string.IsNullOrEmpty(value) || value.Replace(";;", "").Trim().Length == 0)
                //{
                //    value = "";
                //}

                return value;
            }
        }


        public static string getSingleXElementInnerXMLByXPath(XElement currentNode, string xPath, IXmlNamespaceResolver resolver = null)
        {
            string value = "";

            XElement target = null;
            if (null != resolver)
            {
                target = currentNode.XPathSelectElement(xPath, resolver);
            }
            else
            {
                target = currentNode.XPathSelectElement(xPath);
            }

            if (null != target)
            {
                value = target.ToString();
            }

            return value;
        }


        public static string getMultiXElementsInnerXMLByXPath(XElement currentNode, string xPath, IXmlNamespaceResolver resolver = null)
        {
            string value = "";
            IEnumerable<XElement> targets = null;

            if (null != resolver)
            {
                targets = currentNode.XPathSelectElements(xPath, resolver);
            }
            else
            {
                targets = currentNode.XPathSelectElements(xPath);
            }

            value = string.Join(Environment.NewLine, (from ele in targets
                                                      where null != ele
                                                      select MiscUtil.getSingleXElementInnerXMLByXPath(ele, ".")).ToArray());
            return value;
        }


        /***
         *在currentNode中搜索tagName的元素, 可以指定tagName的直接子元素标签, 子元素标签可选
         ***/
        public static string getXElementValueByTagNameaAndChildTabName(XElement currentNode, string tagName, params string[] subTags)
        {
            string value = "";

            var targetNode = currentNode.Descendants(tagName).FirstOrDefault();

            if (subTags.Length > 0)
            {
                foreach (string tag in subTags)
                {
                    if (null != targetNode)
                    {
                        targetNode = targetNode.Element(tag);
                    }
                }
            }
            if (null != targetNode)
            {
                value = targetNode.Value;
            }

            return value;
        }


        public static string jsonSerilizeObject(Object source)
        {
            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
            IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };
            jSetting.Converters.Add(dtConverter);
            string json = JsonConvert.SerializeObject(source, jSetting);
            return json;
        }



        public static DateTime? pareseDateTimeExactUseCurrentCultureInfo(string dataStr, string format = "yyyyMMdd")
        {
            if(!string.IsNullOrEmpty(dataStr))
            {
                dataStr = dataStr.Trim();
            }
            DateTime? resultDate = null;
            try
            {
                resultDate = DateTime.ParseExact(dataStr, format, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {

            }

            return resultDate;
        }

        /// <summary>
        /// 获取祖先目录的路径信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="depth">从当前路径网上数的层数</param>
        /// <returns></returns>
        public static string getRelativeFilePathInclude(string path, int depth)
        {
            string relativeFilePath = "";
            FileInfo fileInfoTmp = new FileInfo(path);
            if (fileInfoTmp.Exists)
            {
                var parentDir = fileInfoTmp.Directory;
                for (int index = 0; index < depth - 1; index++)
                {
                    parentDir = parentDir.Parent;
                }
                relativeFilePath = parentDir.Name + fileInfoTmp.FullName.Substring(parentDir.FullName.Length);
            }
            return relativeFilePath;
        }


        public static string getDictValueOrDefaultByKey(Dictionary<string, string> dict, string key)
        {
            string value = "";//默认值

            dict.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// 嵌套的获取指定目录内符合条件的文件 只有一个搜索条件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<FileInfo> getFileInfosByDirPathRecuriouslyWithSingleSearchPattern(string dirPath, string searchPattern)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

            var directChildFiles = dirInfo.GetFiles(searchPattern);
            fileInfos.AddRange(directChildFiles);

            var directChildDirs = dirInfo.GetDirectories();
            foreach (var dirChild in directChildDirs)
            {
                var descentants = getFileInfosByDirPathRecuriouslyWithSingleSearchPattern(dirChild.FullName, searchPattern);
                fileInfos.AddRange(descentants);
            }
            return fileInfos;
        }

        /// <summary>
        /// 嵌套的获取指定目录内符合条件的文件 多个搜索条件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="searchPatterns"></param>
        /// <returns></returns>
        public static List<FileInfo> getFileInfosByDirPathRecuriouslyWithMultiSearchPattern(string dirPath, string[] searchPatterns)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();

            foreach (var searchPattern in searchPatterns)
            {
                fileInfos.AddRange(getFileInfosByDirPathRecuriouslyWithSingleSearchPattern(dirPath, searchPattern));
            }
            return fileInfos;
        }


        /// <summary>
        /// 尝试解析字符串为decimal?类型
        /// </summary>
        /// <param name="valueStr"></param>
        /// <returns></returns>
        public static decimal? tryParseDecimalNullable(string valueStr)
        {
            decimal? result = null;

            try
            {
                result = new decimal?(new decimal(double.Parse(valueStr)));
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public static int? tryParseIntNullable(string valueStr)
        {
            int? result = null;

            try
            {
                result = new int?(int.Parse(valueStr));
            }
            catch (Exception ex)
            {

            }
            return result;
        }


    }
}
