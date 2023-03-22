using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Text;
using UnityEditor;
using Newtonsoft.Json.Linq;

namespace Module.Automation.Generator
{
    public delegate void DesignGeneratorDelegate(XmlDocument xml, string outputPath);

    static public class XmlManager
    {
        public static void LoadXML(string address, string outputPath, DesignGeneratorDelegate action = null)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Application.dataPath + "/" + address);
            if (xml == null)
                return;

            action?.Invoke(xml, outputPath);
        }

        public static XmlDocument LoadXML(string address)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Application.dataPath + "/" + address);
            return xml;
        }

        public static void LoadAllXML(string address, string outputPath, DesignGeneratorDelegate action = null)
        {
            XmlDocument xml = new XmlDocument();
            DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/" + address);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                if (file.FullName.Contains(".meta"))
                    continue;
                xml.Load(file.FullName);
                action?.Invoke(xml, outputPath);
            }
        }

        public static List<XmlDocument> LoadAllXML(string address)
        {
            List<XmlDocument> data = new List<XmlDocument>();

            DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/" + address);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                if (file.FullName.Contains(".meta"))
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(file.FullName);
                data.Add(xml);
            }
            return data;
        }
    }

    public class EnumGenerator
    {
        string designEnums;
        string enumDatas = "";

        public void Load(string address, string outputPath)
        {
            XmlManager.LoadXML(address, outputPath, Save);
        }

        public void Save(XmlDocument xml, string outputPath)
        {
            if (xml == null)
                return;

            XmlNodeList nodes = xml.SelectNodes("Enum/record");

            foreach (XmlNode node in nodes)
            {
                if (node.SelectSingleNode("ENUM_VALUE").InnerText == "-")
                {

                    if (enumDatas != "")
                    {
                        enumDatas += "    }\n" + string.Format(AutomationFormat.enumSummaryFormat, node.SelectSingleNode("GROUP_ID").InnerText, node.SelectSingleNode("COMMENT").InnerText, node.SelectSingleNode("ENUM_NAME").InnerText);

                    }
                    else
                    {
                        enumDatas += string.Format(AutomationFormat.enumSummaryFormat, node.SelectSingleNode("GROUP_ID").InnerText, node.SelectSingleNode("COMMENT").InnerText, node.SelectSingleNode("ENUM_NAME").InnerText);

                    }
                }
                else
                {
                    string enumCommnet = "";
                    if (node.SelectSingleNode("COMMENT").InnerText != "-")
                    {
                        enumCommnet = "//" + node.SelectSingleNode("COMMENT").InnerText;
                    }
                    enumDatas += string.Format(AutomationFormat.enumFormat, node.SelectSingleNode("ENUM_NAME").InnerText, node.SelectSingleNode("ENUM_VALUE").InnerText, enumCommnet);
                }
            }

            if (enumDatas.Substring(enumDatas.Length - 1, 1) != "}")
            {
                enumDatas += "    }";
            }


            designEnums += string.Format(AutomationFormat.designEnumFormat, enumDatas, System.Text.Encoding.UTF8);
            Debug.Log(designEnums);
            StreamWriter sw;

            sw = new StreamWriter(outputPath + ".cs");

            byte[] bytes = Encoding.Default.GetBytes(designEnums);
            sw.Write(Encoding.UTF8.GetString(bytes));
            sw.Flush();
            sw.Close();
        }
    }


    public class TableGenerator
    {

        Dictionary<int, TableDataInfo> tableInfoData = new Dictionary<int, TableDataInfo>();
        DataMgrStringData dataMgrData;
        DataMessageSerializerStringData dataMessageserializerData;
        public void Load(string address, string outputPath, string dataMgrOutputPath,
                        string readAllDataPath, string comAssetPath, string dataMessageSerializerOutputPath)
        {
            XmlDocument xml = XmlManager.LoadXML(address);
            List<XmlDocument> xmlDatas = XmlManager.LoadAllXML(readAllDataPath);
            foreach (XmlDocument data in xmlDatas)
            {
                GetTableInfo(data);
            }

            XmlNodeList nodes = xml.SelectNodes(xml.DocumentElement.Name + "/verify");
            SaveInfoData(tableInfoData[int.Parse(nodes[0].Attributes["TableId"].Value)], outputPath);
            AddTableAsset(tableInfoData[int.Parse(nodes[0].Attributes["TableId"].Value)], comAssetPath);

            foreach (TableDataInfo data in tableInfoData.Values)
            {
                SaveDataMgrData(data);
                SaveDataMessageSerializer(data);
            }
            ExportDataMgr(dataMgrOutputPath);
            ExportDataMessageserializer(dataMessageSerializerOutputPath);
        }

        public void LoadAll(string address, string outputPath, string dataMgrOutputPath, string comAssetPath,
            string dataMessageSerializerOutputPath)
        {
            List<XmlDocument> xmlDatas = XmlManager.LoadAllXML(address);

            foreach (XmlDocument data in xmlDatas)
            {
                GetTableInfo(data);
            }

            foreach (TableDataInfo data in tableInfoData.Values)
            {
                SaveInfoData(data, outputPath);
                SaveDataMgrData(data);
                SaveDataMessageSerializer(data);
                AddTableAsset(data, comAssetPath);
            }

            ExportDataMgr(dataMgrOutputPath);
            ExportDataMessageserializer(dataMessageSerializerOutputPath);
        }


        public void GetTableInfo(XmlDocument xml)
        {
            if (xml == null)
                return;
            TableDataInfo info = new TableDataInfo();
            XmlNodeList recordNodes = xml.SelectNodes(xml.DocumentElement.Name + "/record");
            XmlNodeList verifyNodes = xml.SelectNodes(xml.DocumentElement.Name + "/verify");

            info.TableName = xml.DocumentElement.Name;
            info.TableID = int.Parse(verifyNodes[0].Attributes["TableId"].Value);
            JObject jsondata = null;
            foreach (XmlNode node in verifyNodes)
            {
                if (node.Attributes["PK"].Value == "Y")
                {
                    info.AddPKData(node.Attributes["ColumnName"].Value, CheckDataType(node.Attributes["Type"].Value));
                }

                if (node.Attributes["StringHashIds"].Value != "-")
                {
                    info.AddVarData(
                        node.Attributes["ColumnName"].Value,
                        CheckDataType(node.Attributes["Type"].Value), node.Attributes["StringHashIds"].Value);
                }
                else
                {
                    info.AddVarData(node.Attributes["ColumnName"].Value
                        , CheckDataType(node.Attributes["Type"].Value));
                }

                if (node.Attributes["IdRule"].Value != "-")
                {
                    jsondata = JObject.Parse(node.Attributes["IdRule"].Value);
                    if (bool.Parse(jsondata["UseListRule"].ToString()))
                    {
                        info.UseListRule = true;
                    }

                }
            }

            if(jsondata != null)
            {
                JArray array = JArray.Parse(jsondata["PKIds"].ToString());
                for (int i = 0, range = array.Count; i < range; ++i)
                {
                    info.AddIsListRule(array[i].ToString());
                    Debug.Log(array[i].ToString());
                }

                array = JArray.Parse(jsondata["FindPKId"].ToString());
                for (int i = 0, range = array.Count; i < range; ++i)
                {
                    info.AddFindPKListRule(array[i].ToString());
                    Debug.Log(array[i].ToString());
                }
            }

            foreach (XmlNode node in recordNodes)
            {
                object[] val = new object[info.VarData.Count];
                for (int i = 0, range = info.VarData.Count; i < range; ++i)
                {
                    string columName = info.VarData[i].ColumName;
                    string type = info.VarData[i].Type;
                    val[i] = CheckDataType(type, node[columName].InnerText);
                }
                info.VarObjectData.Add(val);
            }

            if (!tableInfoData.ContainsKey((int.Parse(verifyNodes[0].Attributes["TableId"].Value))))
            {
                tableInfoData.Add(int.Parse(verifyNodes[0].Attributes["TableId"].Value), info);
            }
        }

        public void SaveInfoData(TableDataInfo info, string outputPath)
        {
            if (info == null)
                return;

            //talbe Info
            string talbeInfoData = "";
            string talbeInfoClassData = "";
            string tableInfoClassVariablesData = "";
            string tableInfoClassConstructParamsData = "";
            string tableInfoClassConstructData = "";
            int protoNum = 1;

            foreach (TableVarData data in info.VarData)
            {
                tableInfoClassVariablesData += string.Format(AutomationFormat.designTalbeInfoClassVariablesFormat,
                    data.Type, data.ColumName, protoNum.ToString());
                tableInfoClassConstructParamsData += data.Type + " " + data.ColumName + (info.VarData[info.VarData.Count - 1] == data ? "" : ",");
                tableInfoClassConstructData += string.Format(AutomationFormat.designTalbeInfoClassContructParamsFormat, data.ColumName);

                if (data.RefTable != "")
                {
                    tableInfoClassVariablesData += string.Format(AutomationFormat.designTalbeInfoClassRefTableVariablesFormat,
                    tableInfoData[int.Parse(data.RefTable)].TableName + "Info", data.ColumName + "_ref");
                }

                protoNum++;
            }

            talbeInfoClassData += string.Format(AutomationFormat.designTalbeInfoClassFormat,
                info.TableName, tableInfoClassVariablesData, tableInfoClassConstructParamsData, tableInfoClassConstructData);

            talbeInfoData += string.Format(AutomationFormat.designTableInfoFormat,
               talbeInfoClassData + SaveInfosData(info));


            StreamWriter sw;

            sw = new StreamWriter(Application.dataPath + outputPath + info.TableName + "Info" + ".cs");

            byte[] bytes = Encoding.Default.GetBytes(talbeInfoData);
            sw.Write(Encoding.UTF8.GetString(bytes));
            sw.Flush();
            sw.Close();
        }

        public string SaveInfosData(TableDataInfo info)
        {
            //talbe Info
            string tableClassName = info.TableName;
            string tableClassVarTypeName = "";
            string tablePKVarName = "";
            string tableClassVarName = "";
            string tablePKVarTypeName = "";
            string tableGetIdRullFuctionCount = "";
            string tableSetUpRefFuction = "";
            string tableInfosIfParams = "";
            string dataInfoTableClassVarName = "";
            //1: Get 함수 전용, 2: GetListById 전용
            string calcArraySegment1 = "", calcArraySegment2="";
            string listData = "";
            string listDataPart = "", listDataPartParam = "";
            string getListByIDFuction = "", getListByIDFuctionParam = "", getListByIDFuctionParmColumnName= "";
            string GetListIdRuleFuction = "", tableGetListIdRuleFuctionCount = "";

            foreach (TableVarData data in info.VarData)
            {
                tableClassVarTypeName += data.Type + " " + data.ColumName + (info.VarData[info.VarData.Count - 1] == data ? "" : ",");
                tableClassVarName += data.ColumName + (info.VarData[info.VarData.Count - 1] == data ? "" : ",");
                dataInfoTableClassVarName += "data." + data.ColumName + (info.VarData[info.VarData.Count - 1] == data ? "" : ",");
                if (data.RefTable != "")
                {
                    tableSetUpRefFuction += string.Format(AutomationFormat.designTableInfosSetupItemIdFuctionFormat, tableInfoData[int.Parse(data.RefTable)].TableName,
                                            info.TableName, data.ColumName, data.Type);
                }
            }

            foreach (TablePKData data in info.PkData)
            {
                tablePKVarTypeName += data.Type + " " + data.ColumName + (info.PkData[info.PkData.Count - 1] == data ? "" : ",");
                tablePKVarName += "data."+data.ColumName + (info.PkData[info.PkData.Count - 1] == data ? "" : ",");
                
                tableInfosIfParams += string.Format(AutomationFormat.designTableInfosInsertIfParamFormat, data.ColumName, (info.PkData[info.PkData.Count - 1] == data ? "" : "&&"));

                tableGetIdRullFuctionCount += string.Format(AutomationFormat.designTableInfosGetIdRullFuctionCountFormat,
                    data.Type);

                calcArraySegment1 += string.Format(AutomationFormat.designTableInfoCalcArraySegmentFormat, data.ColumName, data.Type);
                
                if(data.IsListRuleFindPK)
                {
                    tableGetListIdRuleFuctionCount += string.Format(AutomationFormat.designTableInfosGetIdRullFuctionCountFormat,
                    data.Type);
                    getListByIDFuctionParam += data.Type + " " + data.ColumName + (info.PkData[info.PkData.Count - 1] == data ? "" : ",");
                    getListByIDFuctionParmColumnName += data.ColumName + (info.PkData[info.PkData.Count - 1] == data ? "" : ",");
                    listDataPartParam += "data." + data.ColumName + (info.PkData[info.PkData.Count - 1] == data ? "" : ",");
                    calcArraySegment2 += string.Format(AutomationFormat.designTableInfoCalcArraySegmentFormat, data.ColumName, data.Type);
                }
            }


            getListByIDFuctionParmColumnName=getListByIDFuctionParmColumnName.TrimEnd(',');
            listDataPartParam=listDataPartParam.TrimEnd(',');
            getListByIDFuctionParam = getListByIDFuctionParam.TrimEnd(',');

            if (info.UseListRule)
            {
                listData += string.Format(AutomationFormat.designTableInfosListDataFormat, tableClassName);
                listDataPart += string.Format(AutomationFormat.designTableInfosInitializeListDataPartFormat, listDataPartParam, tableClassName);
                getListByIDFuction += string.Format(AutomationFormat.designTableInfosGetListByIDFuctionFormat, tableClassName, getListByIDFuctionParam, getListByIDFuctionParmColumnName);
                GetListIdRuleFuction += string.Format(AutomationFormat.designTableInfosGetListIdRuleFuctionFormat, getListByIDFuctionParam, tableGetListIdRuleFuctionCount, calcArraySegment2);
            }

            Debug.Log(tableInfosIfParams);
            string result = string.Format(AutomationFormat.designTalbeInfosClassFormat,
                tableClassName, tableClassVarTypeName, tablePKVarName, tableClassVarName,
                tablePKVarTypeName, tableGetIdRullFuctionCount, tableSetUpRefFuction, tableInfosIfParams, dataInfoTableClassVarName
                , calcArraySegment1, listData, listDataPart, getListByIDFuction, GetListIdRuleFuction
                , tablePKVarName.Replace("data.",""));

            return result;
        }

        public void SaveDataMgrData(TableDataInfo info)
        {
            string tableId = info.TableID.ToString();
            string tableClassName = info.TableName;
            List<string> refTable = new List<string>();
            foreach (TableVarData data in info.VarData)
            {
                if (data.RefTable != "")
                {
                    refTable.Add(tableInfoData[int.Parse(data.RefTable)].TableName);
                }
            }

            if (dataMgrData == null)
            {
                dataMgrData = new DataMgrStringData();
            }

            dataMgrData.SetData(tableId, tableClassName, refTable);
        }

        public void SaveDataMessageSerializer(TableDataInfo info)
        {
            string tableId = info.TableID.ToString();
            string tableClassName = info.TableName;

            if (dataMessageserializerData == null)
            {
                dataMessageserializerData = new DataMessageSerializerStringData();
            }

            dataMessageserializerData.SetData(tableId, tableClassName);
        }

        public void ExportDataMgr(string outputPath)
        {
            dataMgrData.ExportDataMgr(outputPath);
        }

        public void ExportDataMessageserializer(string outputPath)
        {
            dataMessageserializerData.ExportDataMessageSerializer(outputPath);
        }

        public void ExportDataByteFile(string outputPath)
        {
            DataMessageSerializer serializer = new DataMessageSerializer();
            foreach (var data in tableInfoData.Values)
            {
                string typeName = "DesignTable." + data.TableName + "Infos";
                System.Type sType = System.Type.GetType(typeName);
                System.Reflection.Assembly dll = System.Reflection.Assembly.LoadFile(Application.dataPath + "/Automation/Output/Dll/DataMgr.dll");
                sType = dll.GetType(typeName);
                System.Reflection.MethodInfo addMethod = sType.GetMethod("Insert");
                object inst = System.Activator.CreateInstance(sType);

                foreach (var val in data.VarObjectData)
                {
                    addMethod.Invoke(inst, val);
                }

                File.WriteAllBytes(Application.dataPath+outputPath + data.TableName + ".bytes", serializer.Serialize(inst));
            }
        }

        public void ExportDataByteOneFile(string tableName,string outputPath)
        {
            DataMessageSerializer serializer = new DataMessageSerializer();
            TableDataInfo data = new TableDataInfo();
            foreach (var val in tableInfoData.Values)
            {
                if (val.TableName == tableName)
                {
                    data = val;
                    break;
                }
            }

            if (data == null)
                return;

            string typeName = "DesignTable." + data.TableName + "Infos";
            System.Type sType = System.Type.GetType(typeName);
            System.Reflection.Assembly dll = System.Reflection.Assembly.LoadFile(Application.dataPath + "/Automation/Output/Dll/DataMgr.dll");
            sType = dll.GetType(typeName);
            System.Reflection.MethodInfo addMethod = sType.GetMethod("Insert");
            object inst = System.Activator.CreateInstance(sType);

            foreach (var val in data.VarObjectData)
            {
                addMethod.Invoke(inst, val);
            }

            File.WriteAllBytes(Application.dataPath + outputPath + data.TableName + ".bytes", serializer.Serialize(inst));

        }

        public void AddTableAsset(TableDataInfo info, string path)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object)) as GameObject;
            if (go == null)
                return;

            ComTableAsset comTableAsset = go.GetComponent<ComTableAsset>();
            if (comTableAsset == null)
                return;

            TableAssetInfo data = new TableAssetInfo(info.TableID, info.TableName+".bytes");
            comTableAsset.Add(data);
            PrefabUtility.SavePrefabAsset(comTableAsset.gameObject);
        }

        public string CheckDataType(string type)
        {
            switch (type)
            {
                case "Int8":
                    return "sbyte";
                case "Int16":
                    return "short";
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "UInt8":
                    return "byte";
                case "UInt16":
                    return "ushort";
                case "UInt32":
                    return "uint";
                case "UInt64":
                    return "ulong";
                case "string":
                case "bool":
                case "float":
                case "double":
                    return type;
            }

            return "";
        }

        public object CheckDataType(string type, string data)
        {
            switch (type)
            {
                case "sbyte":
                    return sbyte.Parse(data);
                case "short":
                    return short.Parse(data);
                case "int":
                    return int.Parse(data);
                case "long":
                    return long.Parse(data);
                case "byte":
                    return byte.Parse(data);
                case "ushort":
                    return ushort.Parse(data);
                case "uint":
                    return uint.Parse(data);
                case "ulong":
                    return ulong.Parse(data);
                case "string":
                    return data;
                case "bool":
                    return data == "Y" ? true : false;
                case "float":
                    return float.Parse(data);
                case "double":
                    return double.Parse(data);
            }

            return null;
        }


    }

}
