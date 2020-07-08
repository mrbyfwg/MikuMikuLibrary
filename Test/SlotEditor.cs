using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Test.Pojo;

namespace Test
{
    class SlotEditor
    {
        private List<String> info = new List<string> { "#mdata_info", "depend.length=0", "version=20161030" };
        private String[] characterList = new string[] {"miku","luka","rin","meiko", "sakine", "haku","teto", "neru", "kaito","len" };
        Modules modules;
        CustomizeItems cstmItems;
        CharacterTbl hakTbl;
        CharacterTbl kaiTbl;
        CharacterTbl lenTbl;
        CharacterTbl lukTbl;
        CharacterTbl meiTbl;
        CharacterTbl mikTbl;
        CharacterTbl nerTbl;
        CharacterTbl rinTbl;
        CharacterTbl sakTbl;
        CharacterTbl tetTbl;
        ObjDb objdb;
        SprDb sprdb;
        Logs logs;
        Boolean allowTextureReplace = false;//暂时无法实现
        public SlotEditor(String moduleIdPath,String cstmItemIdPath,String chritmPropPath,String objDbPath,String sprDbPath)
        {
            this.allowTextureReplace = allowTextureReplace;
            FarcPack.Program.Main1(new string[] {@moduleIdPath});
            string[] moduleid = File.ReadAllLines(@moduleIdPath.Substring(0, @moduleIdPath.Length - 5)+ @"\gm_module_id.bin");
            Directory.Delete(@moduleIdPath.Substring(0, @moduleIdPath.Length - 5), true);
            
            FarcPack.Program.Main1(new string[] { @cstmItemIdPath });
            string[] itemid = File.ReadAllLines(@cstmItemIdPath.Substring(0, @cstmItemIdPath.Length - 5)+ @"\gm_customize_item_id.bin");
            Directory.Delete(@cstmItemIdPath.Substring(0, @cstmItemIdPath.Length - 5), true);

            FarcPack.Program.Main1(new string[] { @chritmPropPath });
            string[] hak = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\hakitm_tbl.txt");
            string[] kai = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\kaiitm_tbl.txt");
            string[] len = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\lenitm_tbl.txt");
            string[] luk = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\lukitm_tbl.txt");
            string[] mei = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\meiitm_tbl.txt");
            string[] mik = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\mikitm_tbl.txt");
            string[] ner = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\neritm_tbl.txt");
            string[] rin = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\rinitm_tbl.txt");
            string[] sak = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\sakitm_tbl.txt");
            string[] tet = File.ReadAllLines(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5) + @"\tetitm_tbl.txt");
            Directory.Delete(@chritmPropPath.Substring(0, @chritmPropPath.Length - 5), true);

            DatabaseConverter.Program.Main1(new string[] { @objDbPath });
            objdb = new ObjDb(@objDbPath.Substring(0, @objDbPath.Length - 4) + ".xml");
            File.Delete(@objDbPath.Substring(0, @objDbPath.Length - 4) + ".xml");

            DatabaseConverter.Program.Main1(new string[] { @sprDbPath });
            sprdb = new SprDb(@sprDbPath.Substring(0, @sprDbPath.Length - 4) + ".xml");
            File.Delete(@sprDbPath.Substring(0, @sprDbPath.Length - 4) + ".xml");

            modules = new Modules(moduleid);
            cstmItems = new CustomizeItems(itemid);
            hakTbl = new CharacterTbl(hak);
            kaiTbl = new CharacterTbl(kai);
            lenTbl = new CharacterTbl(len);
            lukTbl = new CharacterTbl(luk);
            meiTbl = new CharacterTbl(mei);
            mikTbl = new CharacterTbl(mik);
            nerTbl = new CharacterTbl(ner);
            rinTbl = new CharacterTbl(rin);
            sakTbl = new CharacterTbl(sak);
            tetTbl = new CharacterTbl(tet);


        }

        public void loopObjsetFolder(string objsetpath)
        {
            var files = Directory.GetFiles(objsetpath, "*.farc");
            List<String> items = new List<string>();
            foreach (var file in files)
            {
                String farcName = file.ToString().Replace(objsetpath+"\\", "", StringComparison.InvariantCultureIgnoreCase);
                string cha = farcName.Substring(0, 3);
                string farcName1 = farcName.Replace(".farc", "");
                if (farcName1.Substring(6).Equals("000")) continue;
                switch (farcType(farcName))
                {
                    case 0:
                        int no = mikTbl.findItemByNo(Int32.Parse(farcName1.Replace("cmnitm", ""))).no;
                        copyCustomizeItemByObjId(no, "");
                        break;
                    case 1:
                        copyModuleWithNewBody(farcName1, "");
                        break;
                    case 2:
                        items.Add(farcName1);
                        break;
                }
            }
            foreach (String s in items)
            {
                //额外配件的信息
                String cha = s.Substring(0, 3);
                int chano =Int32.Parse( s.Replace(cha + "itm", ""));
                CharacterItemBean cib = copyItemByNo(getStandardName(cha), chano);
                //遍历添加的模组
                foreach (ModuleLogBean mlb in logs.modules)
                    //此模组与额外配件角色对应
                    if(mlb.newModule.chara.Equals(getStandardName(cha),StringComparison.InvariantCultureIgnoreCase))
                        //替换配件
                        foreach (ItemBean ib in findCharactor(mlb.newModule.chara).findCosById(mlb.newCos.id).item)
                            if (ib.item.Equals(chano.ToString()))
                                ib.item = cib.no.ToString();


            }
        }
        private int farcType(string farcName)
        {
            string chaname = farcName.Substring(0, 3);
            if (chaname.Equals("cmn")) return 0;
            CharacterTbl chaTbl = findCharactor(getStandardName(chaname));
            CharacterItemBean cib = chaTbl.findItemByNo(Int32.Parse(farcName.Replace(chaname+"itm","").Replace(".farc","")));
            if (cib.sub_id == 10) return 1;
            else return 2;
        }
        public void createNewLogs(string newFarcRomPath)
        {
            logs = new Logs();
            logs.farcPath = newFarcRomPath;
        }

        public void copySkinParam(string newFarcRomPath, string[] officialMdataPath, string outputPath)
        {
            foreach(ItemLogBean ilb in logs.items)
            {
                String oldSP = "ext_skp_" + ilb.oldItem.dataObjUid[0].uid.ToLower() + ".txt";
                String newSP = "ext_skp_" + ilb.newItem.dataObjUid[0].uid.ToLower() + ".txt";
                int f = 0;
                for (int i = officialMdataPath.Length - 1; i >= 0; i--)
                {

                    f = copy(oldSP, newSP, newFarcRomPath + @"\rom\skin_param", officialMdataPath[i] + @"\rom\skin_param", outputPath + @"\MZZZ\rom\skin_param");
                    if (f == 0) break;
                }
                //if (f == -1) Console.WriteLine(oldSP+"-->"+newSP);
            }
        }

        public void copyFarc(string newFarcRomPath,string[] officialMdataPath,string outputPath)
        {
            foreach(ItemLogBean ilb in logs.items)
            {
                String oldfarc = ilb.oldItem.objset[0].objset.ToLower() + ".farc";
                //脸部复制原始脸
                if (ilb.oldItem.dataObjUid[0].uid.Equals("NULL") && (!allowTextureReplace))
                    oldfarc = ilb.newItem.objset[0].objset.Substring(0, 6).ToLower() + "000.farc";
                String newfarc = ilb.newItem.objset[0].objset.ToLower() + ".farc";
                
                if (ilb.isTextureReplace) Console.WriteLine("Need to manually replace Texture: " + newfarc);
                
                int f = 0;
                foreach (string str in officialMdataPath)
                {
                    f = copy(oldfarc, newfarc, newFarcRomPath + @"\rom\objset", str + @"\rom\objset", outputPath + @"\MZZZ\rom\objset");
                    if (f == 0) break;
                }

                if (f == -1)
                {
                     //Console.WriteLine(oldfarc + "-->" + newfarc);
                }
                else if (f == 0)
                {
                    String file = outputPath + @"\MZZZ\rom\objset\" + newfarc;
                    String folder = file.Substring(0, file.Length - 5);
                    FarcPack.Program.Main1(new string[] { file });
                    FileInfo fi = new FileInfo(folder + @"\" + oldfarc.Substring(0, oldfarc.Length - 5) + "_obj.bin"); //旧名
                    fi.MoveTo(folder + @"\" + newfarc.Substring(0, newfarc.Length - 5) + "_obj.bin");//新名

                    if (!ilb.oldItem.dataObjUid[0].uid.Equals("NULL") && (!allowTextureReplace))
                    {
                        String oldMeshName = ilb.oldItem.dataObjUid[0].uid.ToLower();
                        String newMeshName = ilb.newItem.dataObjUid[0].uid.ToLower();
                        int i = setMeshNameAndId(folder + @"\" + newfarc.Substring(0, newfarc.Length - 5) + "_obj.bin", oldMeshName, newMeshName);
                        if (i == -1) Console.WriteLine(oldMeshName + "->" + newMeshName);
                    }

                    fi = new FileInfo(folder + @"\" + oldfarc.Substring(0, oldfarc.Length - 5) + "_tex.bin"); //旧名
                    fi.MoveTo(folder + @"\" + newfarc.Substring(0, newfarc.Length - 5) + "_tex.bin");//新名
                    FarcPack.Program.Main1(new string[] { folder });
                    Directory.Delete(folder, true);
                }
            }
        }

        public void copy2d(string newFarcRomPath,string[] officialMdataPath, string outputPath)
        {
            foreach (ModuleLogBean mlb in logs.modules) 
            {
                String str = "";
                if (mlb.oldModule.id <= 99)
                {
                    str = str + "0";
                    if (mlb.oldModule.id <= 9) str = str + "0";
                }
                String str1 = "";
                if (mlb.newModule.id <= 99)
                {
                    str = str1 + "0";
                    if (mlb.newModule.id <= 9) str = str + "0";
                }

                String oldspr = "spr_sel_md" + str  + mlb.oldModule.id+"cmn.farc";
                String newspr = "spr_sel_md" + str1 + mlb.newModule.id + "cmn.farc";
                String outputPath1 = outputPath + @"\MZZZ\rom\2d";
                int f = 0;
                for (int i = officialMdataPath.Length-1; i >= 0; i--)
                {
                    f = copy(oldspr, newspr, newFarcRomPath + @"\rom\2d", officialMdataPath[i] + @"\rom\2d", outputPath1);
                    if (f == 0) break;
                }
                if (f == -1) Console.WriteLine(oldspr + "-->" + newspr);
                else if(f == 0)
                {
                    String file = outputPath1 + @"\" + newspr;
                    String folder = file.Substring(0, file.Length - 5);
                    FarcPack.Program.Main1(new string[] { file });
                    FileInfo fi = new FileInfo(folder+@"\"+oldspr.Substring(0,oldspr.Length-5)+".bin"); //旧名
                    fi.MoveTo(folder + @"\" + newspr.Substring(0, newspr.Length - 5) + ".bin");//新名
                    FarcPack.Program.Main1(new string[] { folder });
                    Directory.Delete(folder, true);
                }
            }
            foreach (CSTMItemLogBean clb in logs.cstmItems)
            {
                String str = "";
                if (clb.oldCSTMItem.id <= 99)
                {
                    str = str + "0";
                    if (clb.oldCSTMItem.id <= 9) str = str + "0";
                }
                String str1 = "";
                if (clb.newCSTMItem.id <= 99)
                {
                    str = str1 + "0";
                    if (clb.newCSTMItem.id <= 9) str = str + "0";
                }

                String oldspr = "spr_cmnitm_thmb" + str + clb.oldCSTMItem.id + ".farc";
                String newspr = "spr_cmnitm_thmb" + str1 + clb.newCSTMItem.id + ".farc";
                String outputPath1 = outputPath + @"\MZZZ\rom\2d";
                int f = 0;
                for (int i = officialMdataPath.Length - 1; i >= 0; i--)
                {
                    f = copy(oldspr, newspr, newFarcRomPath + @"\rom\2d", officialMdataPath[i] + @"\rom\2d", outputPath1);
                    if (f == 0) break;
                }
                if (f == -1) Console.WriteLine(oldspr + "-->" + newspr);
                else if (f == 0)
                {
                    String file = outputPath1 + @"\" + newspr;
                    String folder = file.Substring(0, file.Length - 5);
                    FarcPack.Program.Main1(new string[] { file });
                    FileInfo fi = new FileInfo(folder + @"\" + oldspr.Substring(0, oldspr.Length - 5) + ".bin"); //旧名
                    fi.MoveTo(folder + @"\" + newspr.Substring(0, newspr.Length - 5) + ".bin");//新名
                    FarcPack.Program.Main1(new string[] { folder });
                    Directory.Delete(folder, true);
                }
            }
        }
        private int copy(String fileName,String newFileName,String localFilePath,String localFilePath1,String saveFilePath)//文件名，新文件名，文件路径，备用文件路径，目标路径
        {
            String path1 = localFilePath +@"\"+ fileName;
            String path2 = localFilePath1 +@"\"+ fileName;
            String savePath = saveFilePath + @"\" + newFileName;
            Exception e1 = new Exception();
            if (!File.Exists(savePath)) {
                if (File.Exists(path1))
                {
                    FileInfo file1 = new FileInfo(path1);
                    file1.CopyTo(savePath);
                    return 0;
                }
                if (File.Exists(path2))
                {
                    FileInfo file1 = new FileInfo(path2);
                    file1.CopyTo(savePath);
                    return 0;
                } 
            }
            if (e1.Message.Contains("already exists")) return 1;
            return -1;
        }
        public void createPath(String str)
        {
            if (Directory.Exists(str + @"\MZZZ")) throw new Exception("DirectoryExists");
            DirectoryInfo di = new DirectoryInfo(@str + @"\MZZZ\rom\objset");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\gm_customize_item_tbl");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\mdata_chritm_prop");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\mdata_gm_module_tbl");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\2d");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\skin_param");
            di.Create();
        }
        public void output(String str)
        {
            var utf8NoBom = new UTF8Encoding(false);

            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_gm_module_tbl\gm_module_id.bin", modules.toString(), utf8NoBom);
            FarcPack.Program.Main1(new string[] { @str + @"\MZZZ\rom\mdata_gm_module_tbl" });
            Directory.Delete(@str + @"\MZZZ\rom\mdata_gm_module_tbl", true);

            File.WriteAllLines(@str + @"\MZZZ\rom\gm_customize_item_tbl\gm_customize_item_id.bin", cstmItems.toString(), utf8NoBom);
            FarcPack.Program.Main1(new string[] { @str + @"\MZZZ\rom\gm_customize_item_tbl" });
            Directory.Delete(@str + @"\MZZZ\rom\gm_customize_item_tbl", true);

            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\mikitm_tbl.txt", mikTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\hakitm_tbl.txt", hakTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\kaiitm_tbl.txt", kaiTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\lenitm_tbl.txt", lenTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\mikitm_tbl.txt", mikTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\lukitm_tbl.txt", lukTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\meiitm_tbl.txt", meiTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\neritm_tbl.txt", nerTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\rinitm_tbl.txt", rinTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\sakitm_tbl.txt", sakTbl.toString(), utf8NoBom);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_chritm_prop\tetitm_tbl.txt", tetTbl.toString(), utf8NoBom);
            FarcPack.Program.Main1(new string[] { @str + @"\MZZZ\rom\mdata_chritm_prop"});
            Directory.Delete(@str + @"\MZZZ\rom\mdata_chritm_prop", true);

            File.WriteAllLines(@str + @"\MZZZ\rom\objset\mdata_obj_db.xml", objdb.toString(), utf8NoBom);
            DatabaseConverter.Program.Main1(new string[] { @str + @"\MZZZ\rom\objset\mdata_obj_db.xml" });
            File.WriteAllLines(@str + @"\MZZZ\rom\2d\mdata_spr_db.xml", sprdb.toString(), utf8NoBom);
            DatabaseConverter.Program.Main1(new string[] { @str + @"\MZZZ\rom\2d\mdata_spr_db.xml" });
            File.Delete(@str + @"\MZZZ\rom\2d\mdata_spr_db.xml");
            File.Delete(@str + @"\MZZZ\rom\objset\mdata_obj_db.xml");

            File.WriteAllLines(@str + @"\MZZZ\info.txt", info, utf8NoBom);
        }
        public void getLogs(String str)
        {
            File.WriteAllLines(@str + @"\MZZZ\" + logs.farcPath.Substring(logs.farcPath.Length - 4) + "Logs.txt", logs.toString(),Encoding.UTF8);
        }
        public void addBaseModuleWithNewBody(String charactor,String name)
        {
            copyModuleWithNewBody(charactor, 1 , name);
        }
        public void copyModuleWithNewBody(int moduleId,String name)
        {
            ModuleBean mb = new ModuleBean(modules.findModuleById(moduleId));
            CharacterTbl chaTbl = findCharactor(mb.chara);

            //更新module
            mb.id = modules.lastModuleId + 1;
            mb.sort_index = modules.lastSortIndex + 1;
            if (name.Equals("")) { } else mb.name = name;
            int oriCosId =StringCut.cosString2Id(mb.cos);
            mb.cos = StringCut.cosId2String(chaTbl.lastCosId + 1);
            modules.add(mb);

            //更新cos
            CosBean newCos = new CosBean(chaTbl.findCosById(oriCosId));
            int OldBodyno = chaTbl.findBodyNo(newCos);
            newCos.id = chaTbl.lastCosId + 1;
            foreach (ItemBean i in newCos.item)
                if (Int32.Parse(i.item) == OldBodyno) i.item = (chaTbl.lastItemNo + 1).ToString();
            chaTbl.addCos(newCos);
            logs.modules.Add(new ModuleLogBean(modules.findModuleById(moduleId), mb, chaTbl.findCosById(oriCosId), newCos));

            //新增身体
            CharacterItemBean newItem = copyItemByNo(mb.chara,OldBodyno);

            //更新缩略图
            String spiritName = sprdb.addMD(mb.id);

            //更新日志

        }
        public void copyModuleWithNewBody(String charactor,int bodyId,String name)
        {
            CharacterTbl chaTbl = findCharactor(charactor);
            List<CosBean> coslist = chaTbl.findCosByItemId(bodyId);
            if (coslist.Count == 0) throw new Exception("BodyNotFound");
            ModuleBean mb = modules.findModuleByCos(charactor,StringCut.cosId2String(coslist[0].id));
            copyModuleWithNewBody(mb.id,name);
        }
        public void copyModuleWithNewBody(String farcName,String name)
        {

            copyModuleWithNewBody(getStandardName(farcName.Substring(0, 3)), Int32.Parse(farcName.Substring(6)), name);
        }
        public CharacterItemBean copyItemByNo(String charactor,int no,int itemid = -1,Boolean isTextureReplace = false)
            //复制角色零件
        {
            //复制item
            CharacterTbl chaTbl = findCharactor(charactor);
            CharacterItemBean newitem = new CharacterItemBean(chaTbl.findItemByNo(no));
            String oriUid = newitem.dataObjUid[0].uid;
            newitem.name = newitem.name + " NEW";

            String objsetName = "";
            if (itemid == -1)
            {
                //添加普通角色配件
                newitem.no = chaTbl.lastItemNo + 1;
                objsetName = objdb.getCharacterNameUpper(charactor, 1);
            }
            else
            {
                //添加通用配件，因为要指定编号
                newitem.no = itemid;
                objsetName = "cmnitm" + itemid.ToString();
            }
            newitem.objset[0].objset = objsetName;

            if (newitem.attr != 1)
                //换色
                if (!allowTextureReplace)
                    //不允许换色
                    if (newitem.dataObjUid[0].uid.Equals("NULL"))
                    {
                        //脸,不能直接复制org_itm因为没有数据
                        //此外在copyFarc也要特殊处理
                        newitem.attr = 1;
                        newitem.haveTexChg = false;
                        newitem.haveTexOrg = false;
                        newitem.haveTexLength = false;
                        isTextureReplace = true;
                    }
                    else
                        //复制原色零件
                        return copyItemByNo(charactor, newitem.org_itm, itemid,true);
                else
                {
                    //允许换色
                    //copyFarc也要特殊处理
                }

            if (! newitem.dataObjUid[0].uid.Equals("NULL"))
            {
                newitem.dataObjUid[0].uid = objsetName
                    + newitem.dataObjUid[0].uid.Substring(newitem.dataObjUid[0].uid.IndexOf('_'));
                //避免meshname过长
                if ((newitem.dataObjUid[0].uid.Length == 32) && (oriUid.Length < 32))
                    newitem.dataObjUid[0].uid = newitem.dataObjUid[0].uid.Substring(0, newitem.dataObjUid[0].uid.Length - 1);
            }

            chaTbl.addItem(newitem);
            //复制objdb
            objdb.add(newitem);
            logs.items.Add(new ItemLogBean(chaTbl.findItemByNo(no), newitem,isTextureReplace));
            return newitem;
        }
        public void copyCustomizeItemByObjId(int cstmItemNo,String name)
        {
            CustomizeItemBean cib = new CustomizeItemBean(cstmItems.findCstmItemByObjId(cstmItemNo));
            cib.id = cstmItems.lastId + 1;
            cib.sort_index = cstmItems.lastSort_index + 1;
            cib.obj_id = findLastItemId()+1;
            cstmItems.add(cib);
            logs.cstmItems.Add(new CSTMItemLogBean(cstmItems.findCstmItemByObjId(cstmItemNo), cib));
            for(int i = 0; i <= 9; i++)
                copyItemByNo(characterList[i], cstmItemNo, cib.obj_id);
            sprdb.addCST(cib.id);
        }
        private CharacterTbl findCharactor(String charactor)
        {
            switch (charactor.ToLower())
            {
                case "miku":
                    return mikTbl;
                case "luka":
                    return lukTbl;
                case "meiko":
                    return meiTbl;
                case "rin":
                    return rinTbl;
                case "sakine":
                    return sakTbl;
                case "haku":
                    return hakTbl;
                case "teto":
                    return tetTbl;
                case "neru":
                    return nerTbl;
                case "kaito":
                    return kaiTbl;
                case "len":
                    return lenTbl;
                default:
                    throw new Exception("NonStandardCharactorName");
            }
        }
        private String getStandardName(String charactor)
        {
            switch (charactor.ToLower())
            {
                case "mik":
                    return "miku";
                case "luk":
                    return "luka";
                case "mei":
                    return "meiko";
                case "rin":
                    return "rin";
                case "sak":
                    return "sakine";
                case "hak":
                    return "haku";
                case "tet":
                    return "teto";
                case "ner":
                    return "neru";
                case "kai":
                    return "kaito";
                case "len":
                    return "len";
                default:
                    throw new Exception("NonStandardCharactorName");
            }
        }
        private int findLastItemId()
        {
            int result = -1;
            for (int i = 0; i <= 9; i++)
                if (findCharactor(characterList[i]).lastItemNo > result) result = findCharactor(characterList[i]).lastItemNo;
            return result;
        }
        private static int IndexOf(byte[] srcBytes, byte[] searchBytes, int offset = 0)
        {
            if (offset == -1) { return -1; }
            if (srcBytes == null) { return -1; }
            if (searchBytes == null) { return -1; }
            if (srcBytes.Length == 0) { return -1; }
            if (searchBytes.Length == 0) { return -1; }
            if (srcBytes.Length < searchBytes.Length) { return -1; }
            for (var i = offset; i < srcBytes.Length - searchBytes.Length; i++)
            {
                if (srcBytes[i] != searchBytes[0]) continue;
                if (searchBytes.Length == 1) { return i; }
                var flag = true;
                for (var j = 1; j < searchBytes.Length; j++)
                {
                    if (srcBytes[i + j] != searchBytes[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag) { return i; }
            }
            return -1;
        }
        public static int setMeshNameAndId(String path,String oldMeshName,String newMeshName)
        {
            byte[] bin = File.ReadAllBytes(path);
            byte[] oldMeshNameByte = Encoding.ASCII.GetBytes(oldMeshName);
            int idOffset = oldMeshNameByte.Length / 16 * 16 + 16;
            int index = IndexOf(bin, oldMeshNameByte);
            byte[] firstPart = bin.Skip(0).Take(index).ToArray();
            byte[] secondPart = bin.Skip(index + idOffset + 16).ToArray();
            int id = bin[index + idOffset] + bin[index + idOffset + 1] * 256;
            byte[] newMeshNameByte = Encoding.ASCII.GetBytes(newMeshName);
            int newIdOffset = newMeshNameByte.Length / 16 * 16 + 16;

            //MeshName长度不能变化过大
            if (idOffset != newIdOffset)
            {
                bin[index + idOffset] = 0;
                bin[index + idOffset+1] = 0;
                File.Delete(path);
                File.WriteAllBytes(path, bin);
                return -1;
            }

            List<byte> final = new List<byte>();
            final.AddRange(firstPart);
            //set MeshName
            final.AddRange(newMeshNameByte);
            for (int i = newMeshNameByte.Length; i <= newIdOffset - 1; i++) final.Add(0);
            //set ID
            final.Add(0);
            final.Add(0);
            for (int i = 3; i <= 16; i++) final.Add(0);
            final.AddRange(secondPart);
            File.Delete(path);
            File.WriteAllBytes(path, final.ToArray());
            return 0;
        }
    }
}
