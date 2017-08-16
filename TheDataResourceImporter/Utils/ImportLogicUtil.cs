using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace TheDataResourceExporter.Utils
{
    class ImportLogicUtil
    {
        public static string  getSingleBrandRelatedName(System.Xml.Linq.XElement ele)
        {
            var XKR_NAME_ZHInner = MiscUtil.getXElementSingleValueByXPath(ele, "./NAME-ZH");
            var XKR_NAME_ENInner = MiscUtil.getXElementSingleValueByXPath(ele, "./NAME-EN");
            var XKR_NAMEInner = "";

            if (string.IsNullOrEmpty(XKR_NAME_ZHInner) || string.IsNullOrEmpty(XKR_NAME_ENInner))
            {
                XKR_NAMEInner = XKR_NAME_ZHInner + XKR_NAME_ENInner;
            }
            else//CN, EN都不为空
            {
                XKR_NAMEInner = XKR_NAME_ZHInner + "，" + XKR_NAME_ENInner;
            }
            return XKR_NAMEInner;
        }


        public static string getMultiBrandRelatedZHandENNamesByXPath(XElement currentNode, string xPath, IXmlNamespaceResolver resolver = null)
        {
            IEnumerable<XElement> targets = null;

            if (null != resolver)
            {
                targets = currentNode.XPathSelectElements(xPath, resolver);
            }
            else
            {
                targets = currentNode.XPathSelectElements(xPath);
            }

            return  string.Join("；；", (from ele in targets
                                       select getSingleBrandRelatedName(ele)).ToArray());
        }
    }
}
