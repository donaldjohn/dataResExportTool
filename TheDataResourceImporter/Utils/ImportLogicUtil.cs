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


        public  static string importS_China_Patent_TextImage(DataSourceEntities entitiesContext,string filePath,string session_id, string app_type, DateTime? pub_date, string entryKey, string hasTif)
        {
            if (null == pub_date)
            {
                if (!string.IsNullOrEmpty(entryKey))
                {
                    var pub_dateStr = entryKey.Split('\\').FirstOrDefault();
                    pub_date = MiscUtil.pareseDateTimeExactUseCurrentCultureInfo(pub_dateStr);
                }
            }

            S_CHINA_PATENT_TEXTIMAGE sCNPatTxtImg = new S_CHINA_PATENT_TEXTIMAGE() { APPL_TYPE = app_type, APP_NUMBER = CompressUtil.getEntryShortName(entryKey), ARCHIVE_INNER_PATH = entryKey, EXIST_TIF = hasTif, FILE_PATH = filePath, ID = System.Guid.NewGuid().ToString(), IMPORT_SESSION_ID = session_id, IMPORT_TIME = System.DateTime.Now, PATH_TIF = entryKey, PUB_DATE = pub_date };

            try
            {
                entitiesContext.S_CHINA_PATENT_TEXTIMAGE.Add(sCNPatTxtImg);
               //entitiesContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

                //ex.;
            }
            return MiscUtil.jsonSerilizeObject(sCNPatTxtImg);
            //sCNPatTxtImg.APPL_TYPE = app_type;
            //sCNPatTxtImg.APP_NUMBER = CompressUtil.getEntryShortName(entryKey);
            //sCNPatTxtImg.ARCHIVE_INNER_PATH = entryKey;
            //sCNPatTxtImg.FILE_PATH = filePath;
            //sCNPatTxtImg.ID = System.Guid.NewGuid().ToString();
            //sCNPatTxtImg.IMPORT_SESSION_ID = session_id;
            //sCNPatTxtImg.EXIST_TIF = hasTif;
            //sCNPatTxtImg.IMPORT_TIME
        }




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
