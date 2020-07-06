using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Pojo
{
    class LogBean
    {
        public String moduleName;
        public int oldModuleId;
        public String sprName;
        public String oldObjsetName;
        public String objsetName;
        public String oldMeshName;
        public String meshName;
        public int meshId = 0;
        public List<ItemBean> itemList;
        public List<String> toString()
        {
            return new List<String>() { "ModuleName=        " + moduleName, 
                                        "SpiritFarcName=    " + sprName, 
                                        "OldObjsetFarcName= " + oldObjsetName,
                                        "ObjsetFarcName=    " + objsetName, 
                                        "MeshName=          " + meshName,
                                        "MeshID=            " + meshId,
                                        "-------------------------------------------------",""};
        }
        public LogBean(String moduleName,int oldModuleId,String sprName,String oldObjsetName,String objsetName,String oldMeshName,String meshName)
        {
            this.moduleName = moduleName;
            this.oldModuleId = oldModuleId; 
            this.sprName = sprName;
            this.oldObjsetName = oldObjsetName;
            this.objsetName = objsetName;
            this.oldMeshName = oldMeshName;
            this.meshName = meshName;
        }
    }
}
