using System;
using System.Collections.Generic;
using System.Text;
using Test.Pojo;

namespace Test
{
    class Logs
    {
        public List<ModuleLogBean> modules = new List<ModuleLogBean>();
        public List<ItemLogBean> items = new List<ItemLogBean>();
        public List<CSTMItemLogBean> cstmItems = new List<CSTMItemLogBean>();
        public String farcPath;

        public List<String> toString()
        {
            List<String> result = new List<string>();
            result.Add("From: "+farcPath);
            //"ModuleName=        " + moduleName, 
            //"SpiritFarcName=    " + sprName, 
            //"OldObjsetFarcName= " + oldObjsetName,
            //"ObjsetFarcName=    " + objsetName, 
            //"MeshName=          " + meshName,
            //"MeshID=            " + meshId,
            result.Add("NewModules:");
            foreach (ModuleLogBean mlb in modules)
            {
                result.Add("ModuleName=       " + mlb.newModule.name);
                result.Add("OldModuleId=      " + mlb.oldModule.id);
                result.Add("SpiritFarcName=   " + "spr_sel_md" + mlb.newModule.id + "cmn.farc");
                result.Add("Cos=");
                result.AddRange(mlb.newCos.toString());
                result.Add("-------------------------------------------------------------------");
            }
            result.Add("");
            result.Add("NewCustomizeItems:");
            foreach(CSTMItemLogBean cilb in cstmItems)
            {
                result.Add("CSTMItemName=     " + cilb.newCSTMItem.name);
                result.Add("OldCSTMItemId=    " + cilb.oldCSTMItem.id);
                //result.Add("SpiritFarcName= " + "spr_sel_md" + cilb.newCSTMItem.id + "cmn.farc");
                result.Add("ObjsetId=         " + cilb.newCSTMItem.obj_id);
                result.Add("-------------------------------------------------------------------");
            }
            result.Add("");
            result.Add("NewItems:");
            foreach (ItemLogBean ilb in items)
            {
                result.Add("ItemName=         " + ilb.newItem.name);
                result.Add("ItemNo=           " + ilb.newItem.no);
                result.Add("OldFarcName=      " + ilb.oldItem.objset[0].objset);
                result.Add("NewFarcName=      " + ilb.newItem.objset[0].objset);
                result.Add("NewMeshName=      " + ilb.newItem.dataObjUid[0].uid);
                result.Add("-------------------------------------------------------------------");
            }
            return result;
        }
    }
}
