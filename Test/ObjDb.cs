using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Test
{
    class ObjDb
    {
        public XElement mdata_obj;
        public XElement obj;
        public IEnumerable<XElement> mdata_objList;
        public IEnumerable<XElement> objList;
        public int lastMikNo = 1000;
        public int lastLukNo = 1000;
        public int lastMeiNo = 1000;
        public int lastRinNo = 1000;
        public int lastSakNo = 1000;
        public int lastHakNo = 1000;
        public int lastTetNo = 1000;
        public int lastNerNo = 1000;
        public int lastKaiNo = 1000;
        public int lastLenNo = 1000;
        public int lastId = -1;
        public ObjDb(String mdataObjPath,String objPath)
        {
            mdata_obj = XElement.Load(mdataObjPath);
            mdata_obj.Element("Unknown").Value = "99999";
            obj = XElement.Load(objPath);
            this.mdata_objList = from el in mdata_obj.Element("Objects").Elements("ObjectEntry")
                           select el;
            this.objList = from el in obj.Element("Objects").Elements("ObjectEntry")
                                 select el;
            foreach (XElement x in mdata_objList)
            {
                if (Int32.Parse(x.Element("Id").Value) > lastId) lastId = Int32.Parse(x.Element("Id").Value);
                updateLastChaNo(x.Element("Name").Value);
            }
            mdata_obj.Element("Unknown").Value = "9999";
        }
        private void updateLastChaNo(String name)
        {
            if ((name.Length >= 7)&&(name.Substring(3,3).Equals("itm",StringComparison.InvariantCultureIgnoreCase)))
            {
                int value = Int32.Parse(name.Substring(6));
                String key = name.Substring(0, 6).ToUpper();
                switch (key)
                {
                    case "MIKITM":
                        if (value > lastMikNo) lastMikNo = value;
                        break;
                    case "LUKITM":
                        if (value > lastLukNo) lastLukNo = value;
                        break;
                    case "MEIITM":
                        if (value > lastMeiNo) lastMeiNo = value;
                        break;
                    case "RINITM":
                        if (value > lastRinNo) lastRinNo = value;
                        break;
                    case "SAKITM":
                        if (value > lastSakNo) lastSakNo = value;
                        break;
                    case "HAKITM":
                        if (value > lastHakNo) lastHakNo = value;
                        break;
                    case "TETITM":
                        if (value > lastTetNo) lastTetNo = value;
                        break;
                    case "NERITM":
                        if (value > lastNerNo) lastNerNo = value;
                        break;
                    case "KAIITM":
                        if (value > lastKaiNo) lastKaiNo = value;
                        break;
                    case "LENITM":
                        if (value > lastLenNo) lastLenNo = value;
                        break;
                }
            }
        }
        public void add(String name,int id,String fileName,String TexFileName,String ArcFileName,String MeshName,int MeshId)
        {
            mdata_objList.Last().AddAfterSelf(new XElement("ObjectEntry",
                                           new XElement("Name",name.ToUpper()),
                                           new XElement("Id",id),
                                           new XElement("FileName",fileName.ToLower()),
                                           new XElement("TextureFileName",TexFileName.ToLower()),
                                           new XElement("ArchiveFileName",ArcFileName.ToLower()),
                                           new XElement("Meshes",
                                               new XElement("MeshEntry",
                                                   new XElement("Name",MeshName.ToUpper()),
                                                   new XElement("Id",MeshId)
                                                           )
                                                       )
                                                   )
                                      );
            if (id > lastId) lastId = id;
            updateLastChaNo(name);
        }

        public int getMeshId(String name)
        {

            foreach (XElement x in mdata_objList)
                if (x.Element("Name").Value.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    return Int32.Parse(x.Element("Meshes").Element("MeshEntry").Element("Id").Value);
            foreach (XElement x in objList)
                if (x.Element("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return Int32.Parse(x.Element("Meshes").Element("MeshEntry").Element("Id").Value);
            throw new Exception(name+" ObjsetNameNotFound");
        }
        public String getMeshName(String name)
        {

            foreach (XElement x in mdata_objList)
                if (x.Element("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return x.Element("Meshes").Element("MeshEntry").Element("Name").Value;
            foreach (XElement x in objList)
                if (x.Element("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return x.Element("Meshes").Element("MeshEntry").Element("Name").Value;
            throw new Exception(name + " ObjsetNameNotFound");
        }
        public List<String> toString()
        {
            return new List<String>() { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", mdata_obj.ToString() };
        }
        public String getCharacterNameUpper(String character)
        {
            return getCharacterNameUpper(character,0);
        }
        public String getCharacterNameUpper(String character,int i)
        {
            
            switch (character.ToLower())
            {
                case "miku":
                    return "MIKITM" + (lastMikNo + i).ToString();
                case "luka":
                    return "LUKITM" + (lastLukNo + i).ToString();
                case "meiko":
                    return "MEIITM" + (lastMeiNo + i).ToString();
                case "rin":
                    return "RINITM" + (lastRinNo + i).ToString();
                case "sakine":
                    return "SAKITM" + (lastSakNo + i).ToString();
                case "haku":
                    return "HAKITM" + (lastHakNo + i).ToString();
                case "teto":
                    return "TETITM" + (lastTetNo + i).ToString();
                case "neru":
                    return "NERITM" + (lastNerNo + i).ToString();
                case "kaito":
                    return "KAIITM" + (lastKaiNo + i).ToString();
                case "len":
                    return "LENITM" + (lastLenNo + i).ToString();
                default:
                    throw new Exception(character+ " NonStandardCharactorName");
            }
        }
    }
}
