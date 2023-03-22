using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Automation.Generator
{
    class AutomationFormat
    {
        //{0} enum ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿?
        public static string designEnumFormat =
@"namespace DesignEnum
{{
{0}
}}
";
        //{0} enum ID
        //{1} enum ï¿½Ú¸ï¿½Æ®
        //{2} enum ï¿½ï¿½
        public static string enumSummaryFormat =
@"    /// <summary> 
    /// ID : {0}
    /// {1}
    /// </summary> 
    public enum {2}
    {{
";

        //{0} enum name
        //{1} enum value
        //{2} enum comment
        public static string enumFormat =
@"        {0} = {1}, {2}
";

        //{0} talbe class
        public static string designTableInfoFormat =
@"using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
namespace DesignTable
{{
    [ProtoContract]
    {0}
}}
";
        //{0} : table class name
        //{1} : table class variables
        //{2} : class parameter ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ô¼ï¿½
        //{3} : class parameter ï¿½Ò´ï¿½
        public static string designTalbeInfoClassFormat =
@"public class {0}Info
    {{
{1}
        public {0}Info()
        {{
        }}

        public {0}Info({2})
        {{
{3}
        }}
    }}
";
        //{0} : table class variable type
        //{1} : table class variable name
        //{2} : protoNumber
        public static string designTalbeInfoClassVariablesFormat =
@"        [ProtoMember({2})] 
        public {0} {1};
";
        //{0} : table class variable type
        //{1} : table class variable name
        //{2} : protoNumber
        public static string designTalbeInfoClassRefTableVariablesFormat =
@"          public {0} {1};        
";

        //{0} : table class variable name
        public static string designTalbeInfoClassContructParamsFormat =
@"            this.{0} = {0};
";

        //{0} : table class name
        //{1} : table class variable type & name
        //{2} : table pk variable name
        //{3} : table class variable name
        //{4} : table pk variable type & name
        //{5} : designTableInfosGetIdRullFuctionCountFormat
        //{6} : designTableInfosSetupItemIdFuctionFormat
        //{7} : designTableInfosInsertIfParamFormat
        //{8} : dataInfo table class variable name
        //{9} : designTableInfoCalcArraySegmentFormat
        //{10} : designTableInfosListDataFormat
        //{11} : designTableInfosInitializeListDataPartFormat
        //{12} : designTableInfosGetListByIDFuctionFormat
        //{13} : designTableInfosGetListIdRuleFuctionFormat
        //{14} : table pk variable type & name({4}¿Í ´Ù¸£°Ô data. ¾øÀ½)

        public static string designTalbeInfosClassFormat =
@"      [ProtoContract]
    public class {0}Infos
    {{
        [ProtoMember(1)]
        public List<{0}Info> dataInfo = new List<{0}Info>();
        public Dictionary<ArraySegment<byte>, {0}Info> datas = new Dictionary<ArraySegment<byte>, {0}Info>(new DataComparer());
        {10}

        public bool Insert({1})
        {{ 
            foreach({0}Info info in dataInfo)
            {{
                if({7})
                {{
                    return false;
                }}
            }}

            dataInfo.Add(new {0}Info({3}));
            return true;
        }}

        public void Initialize() 
        {{
            foreach(var data in dataInfo)
            {{
                ArraySegment<byte> bytes = GetIdRule({2});
                if (datas.ContainsKey(bytes))
                    continue;
                datas.Add(bytes,new {0}Info({8}));

                {11}
            }}
        }}

        public {0}Info Get({4})
        {{
            {0}Info value = null;
            
            if(datas.TryGetValue(GetIdRule({14}),out value))
                return value;
            
            return null;
        }}

        {12}

        public ArraySegment<byte> GetIdRule({4})
        {{
            ushort total = 0;
            ushort count = 0;
            {5}

            if (total == 0)
               return null;

            byte[] bytes = new byte[total];
            {9}
            
            return bytes;
        }}
        
        {13}

        {6}

    }}
";

        //{0} : table variable type
        public static string designTableInfosGetIdRullFuctionCountFormat =
@"          total += sizeof({0});
";
        //{0} : pk Name
        //{1] : pk type
        public static string designTableInfoCalcArraySegmentFormat =
@"          Array.Copy(BitConverter.GetBytes({0}), 0, bytes, count, sizeof({1}));
            count += sizeof({1});            
";

        //{0} : TableVarData class RefTable
        //{1} : table name
        //{2} : TableVarData class ColumName
        //{3} : TableVarData class Type
        public static string designTableInfosSetupItemIdFuctionFormat =
@"public void SetupRef_item_Id({0}Infos infos)
        {{
            foreach({1}Info data in dataInfo)
            {{
                if(data.{2} != -1)
                {{
                    data.{2}_ref = infos.Get(({3})data.{2});
                }}
            }}
        }}
";
        //{0} : tableName
        public static string designTableInfosListDataFormat =
@"public Dictionary<ArraySegment<byte>,List<{0}Info>> listData = new Dictionary<ArraySegment<byte>, List<{0}Info>>(new DataComparer());";
        //{0} : table pk variable name
        //{1} : AND
        public static string designTableInfosInsertIfParamFormat =
@"info.{0} == {0} {1}";

        //{0} : table Fine List PK variable name
        //{1} : talbeName
        public static string designTableInfosInitializeListDataPartFormat =
@"
bytes = GetListIdRule({0});
if(listData.ContainsKey(bytes))
{{
    listData[bytes].Add(data);
}}
else 
{{
                    
    listData.Add(bytes,new List<{1}Info>());
    listData[bytes].Add(data);
}}
";
        //{0} : talbe name;
        //{1} : parameter & type
        //{2} : table column Name
        public static string designTableInfosGetListByIDFuctionFormat =
@"
public List<{0}Info> GetListById({1})
{{
    List<{0}Info> value = null;
    ArraySegment<byte> bytes = GetListIdRule({2});
    if(listData.TryGetValue(bytes,out value))
        return value;
    return null;
}}
";
        //{0} : table find list pk
        //{1} : designTableInfosGetIdRullFuctionCountFormat
        //{2} : designTableInfoCalcArraySegmentFormat
        public static string designTableInfosGetListIdRuleFuctionFormat =
@"
public ArraySegment<byte> GetListIdRule({0})
{{
    ushort total = 0;
    ushort count = 0;
    {1}
    if (total == 0)
        return null;
            
    byte[] bytes = new byte[total];
    {2}
    return bytes;
}}
";

        //{0} : designMgrEnumVariableFormat
        //{1} : designMgrVariableFormat & designMgrPublicVariableFormat
        //{2} : designMgrRegisterLoadHandlerFuctionFormat
        //{3} : designMgrRegisterClearHandlerFuctionFormat
        //{4} : designMgrLoadClassFuctionFormat
        //{5} : designMgrClearClassFructionFormat
        //{6} : designMgrSetUpRefFuctionFormat
        public static string designMgrFormat =
@"using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System;
namespace DesignTable
{{
    public class DataComparer :  IEqualityComparer<ArraySegment<byte>>
    {{
        public bool Equals([AllowNull] ArraySegment<byte> x, [AllowNull] ArraySegment<byte> y)
        {{
            return x.SequenceEqual(y);
        }}

        public int GetHashCode([DisallowNull] ArraySegment<byte> obj)
        {{
            if(obj == null) throw new ArgumentNullException(""obj"");
            return obj.Sum(y => y);
        }}
    }}

    public enum TableId
    {{
        {0}
    }}
    
    public class DataMgr
    {{
        private delegate void LoadHandler(byte[] data);
        private delegate void ClearHandler();

        private Dictionary<int, DataMgr.LoadHandler> loadHandlerList = new Dictionary<int, LoadHandler>();
        private Dictionary<int, DataMgr.ClearHandler> clearHandlerList = new Dictionary<int, ClearHandler>();
        private bool isCallInit = false;
        DataMessageSerializer serializer = new DataMessageSerializer();

        {1}
        
        public virtual void Init()
        {{
            if (isCallInit)
                return;

            RegisterLoadHandler();
            RegisterClearHandler();
            isCallInit = true;
        }}

        public void LoadData(TableId dataType,byte[] data)
        {{
            loadHandlerList[(int)dataType](data);
        }}

        public void ClearData(TableId[] dataTypes)
        {{
            foreach (int dataType in dataTypes)
            {{
                clearHandlerList[dataType]();
            }}
        }}

        public void ClearData(TableId dataTypes)
        {{
             clearHandlerList[(int)dataTypes]();
        }}
        
        public void ClearDataAll()
        {{
            foreach (DataMgr.ClearHandler clearHandler in clearHandlerList.Values)
            {{
                clearHandler();
            }}
        }}

        private void RegisterLoadHandler()
        {{
            {2}
        }}  

        private void RegisterClearHandler()
        {{
            {3}
        }}
        
        {4}
        
        {5}

        public void SetUpRef()
        {{
            {6}
        }}
        
    }}
}}
";

        //{0} : table name
        //{1} : table id
        public static string designMgrEnumVariableFormat =
@"{0} = {1},
";
        //{0} : talbe name
        public static string designMgrPrivateVariableFormat =
@"private {0}Infos {0}Infos;
";

        //{0} : table name
        //{1} : table name(?žìžë¦¬ëŠ” ?€ë¬¸ìž)
        public static string designMgrPublicVariableFormat =
@"public {0}Infos {1}Infos => {0}Infos;
";

        //{0} : table id
        //{1} : table name
        public static string designMgrRegisterLoadHandlerFuctionFormat =
@"loadHandlerList.Add({0}, new DataMgr.LoadHandler(Load{1}Infos));
";

        //{0} : table id
        //{1} : table name
        public static string designMgrRegisterClearHandlerFuctionFormat =
@"clearHandlerList.Add({0}, ClearData{1}Infos);
";
        //{0} : table name
        //{1} : talbe id
        public static string designMgrLoadClassFuctionFormat =
@"private void Load{0}Infos(byte[] data)
{{
    using (MemoryStream memoryStream = new MemoryStream(data))
    {{
        {0}Infos = serializer.Deserialize({1},data) as {0}Infos;
        {0}Infos.Initialize();
    }}
}}
";
        //{0} : table name
        public static string designMgrClearClassFructionFormat =
@"private void ClearData{0}Infos()
{{
    if({0}Infos != null)
        {0}Infos=null;
}}
";

        //{0} : table name
        //{1} : TableVarData class RefTable
        public static string designMgrSetUpRefFuctionFormat =
@"{0}Infos.SetupRef_item_Id({1}Infos);
";

        //{0} : dataMessageSerializerDeserializeFuctionFormat
        public static string dataMessageSerializerClassFormat =
@"using DesignTable;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public sealed class DataMessageSerializer
{{
    public byte[] Serialize(object tableInfos)
    {{
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        Serializer.Serialize(stream, tableInfos);
        byte[] buffer = stream.ToArray();

        return buffer;
    }}

    public object Deserialize(int tableId, byte[] buffer)
    {{
        System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
        switch (tableId)
        {{
            {0}
        }}

        return null;
    }}
}}
";
        //{0} : table id
        //{1} : table name
        public static string dataMessageSerializerDeserializeFuctionFormat =
@"case {0}:
    return ProtoBuf.Serializer.Deserialize<{1}Infos>(stream);";
    }

}