using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        List<LogBean> addLogs = new List<LogBean>();
        public SlotEditor(String moduleIdPath,String cstmItemIdPath,String chritmPropPath,String objDbPath,String sprDbPath)
        {
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

        public void copySkinParam(string newFarcRomPath, string[] officialMdataPath, string outputPath)
        {
            foreach(LogBean lb in addLogs)
            {
                String oldSP = "ext_skp_" + lb.oldMeshName + ".txt";
                String newSP = "ext_skp_" + lb.meshName + ".txt";
                int f = 1;
                for (int i = 0; i <= officialMdataPath.Length - 1; i++)
                {
                    try
                    {
                        f = f * copy(oldSP, newSP, newFarcRomPath + @"\skin_param", officialMdataPath[i] + @"\rom\skin_param", outputPath + @"\MZZZ\rom\skin_param");
                    }catch(IOException e) { }
                }
                if (f != 0) Console.WriteLine(oldSP+"-->"+newSP);
            }
        }

        public void copyFarc(string newFarcRomPath,string outputPath)
        {
            foreach(LogBean lb in addLogs)
            {
                String oldfarc = lb.oldObjsetName;
                String newfarc = lb.objsetName.ToLower();
                int f = copy(oldfarc, newfarc, newFarcRomPath + @"\objset", newFarcRomPath + @"\objset", outputPath + @"\MZZZ\rom\objset");
                if (f == -1) Console.WriteLine(oldfarc + "-->" + newfarc);
                else
                {
                    String file = outputPath + @"\MZZZ\rom\objset\" + newfarc;
                    String folder = file.Substring(0, file.Length - 5);
                    FarcPack.Program.Main1(new string[] { file });
                    FileInfo fi = new FileInfo(folder + @"\" + oldfarc.Substring(0, oldfarc.Length - 5) + "_obj.bin"); //旧名
                    fi.MoveTo(folder + @"\" + newfarc.Substring(0, newfarc.Length - 5) + "_obj.bin");//新名

                    int i = setMeshNameAndId(folder + @"\" + newfarc.Substring(0, newfarc.Length - 5) + "_obj.bin", lb.oldMeshName, lb.meshName);
                    if (i == -1) Console.WriteLine(lb.oldMeshName + "->" + lb.meshName);

                    fi = new FileInfo(folder + @"\" + oldfarc.Substring(0, oldfarc.Length - 5) + "_tex.bin"); //旧名
                    fi.MoveTo(folder + @"\" + newfarc.Substring(0, newfarc.Length - 5) + "_tex.bin");//新名
                    FarcPack.Program.Main1(new string[] { folder });
                    Directory.Delete(folder, true);
                }
            }
        }

        public void copy2d(string newFarcRomPath,string[] officialMdataPath, string outputPath)
        {
            foreach (LogBean lb in addLogs) 
            {
                String str = "";
                if (lb.oldModuleId <= 99)
                {
                    str = str + "0";
                    if (lb.oldModuleId <= 9) str = str + "0";
                }

                String oldspr = "spr_sel_md" + str + lb.oldModuleId+"cmn.farc";
                String newspr = lb.sprName;
                String outputPath1 = outputPath + @"\MZZZ\rom\2d";
                int f = 1;
                for (int i = 0; i <= officialMdataPath.Length - 1; i++)
                {
                    try
                    {
                        f = f * copy(oldspr, newspr, newFarcRomPath + @"\2d", officialMdataPath[i] + @"\rom\2d", outputPath1);
                    }
                    catch (IOException e) { }
                }
                if (f != 0) Console.WriteLine(oldspr + "-->" + newspr);
                else
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
        }
        private int copy(String fileName,String newFileName,String localFilePath,String localFilePath1,String saveFilePath)//文件名，新文件名，文件路径，备用文件路径，目标路径
        {
            String path1 = localFilePath +@"\"+ fileName;
            String path2 = localFilePath1 +@"\"+ fileName;
            String savePath = saveFilePath + @"\" + newFileName;
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
            return -1;
        }
        public void output(String str)
        {
            var utf8NoBom = new UTF8Encoding(false);
            DirectoryInfo di = new DirectoryInfo(@str+@"\MZZZ\rom\objset");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\mdata_gm_customize_item_id");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\mdata_chritm_prop");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\mdata_gm_module_tbl");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\2d");
            di.Create();
            di = new DirectoryInfo(@str + @"\MZZZ\rom\skin_param");
            di.Create();
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_gm_module_tbl\gm_module_id.bin", modules.toString(), utf8NoBom);
            FarcPack.Program.Main1(new string[] { @str + @"\MZZZ\rom\mdata_gm_module_tbl" });
            Directory.Delete(@str + @"\MZZZ\rom\mdata_gm_module_tbl", true);
            File.WriteAllLines(@str + @"\MZZZ\rom\mdata_gm_customize_item_id\gm_customize_item_id.bin", cstmItems.toString(), utf8NoBom);
            FarcPack.Program.Main1(new string[] { @str + @"\MZZZ\rom\mdata_gm_customize_item_id" });
            Directory.Delete(@str + @"\MZZZ\rom\mdata_gm_customize_item_id", true);
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

            var loglist = new List<String>();
            foreach (LogBean lb in addLogs) loglist.AddRange(lb.toString());
            File.WriteAllLines(@str + @"\MZZZ\AddLog.txt", loglist, utf8NoBom);
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
            
            //新增身体
            CharacterItemBean newItem = copyItemByNo(mb.chara,OldBodyno);
            //更新缩略图
            String spiritName = sprdb.addMD(mb.id);

            //更新日志
            String oldObjName = chaTbl.findItemByNo(OldBodyno).objset[0].objset;
            addLogs.Add(new LogBean(mb.name,
                moduleId,
                spiritName+".farc",
                oldObjName.ToLower()+".farc", 
                newItem.objset[0].objset+".farc", 
                chaTbl.getMeshName(oldObjName).ToLower(),
                newItem.dataObjUid[0].uid.ToLower()));
        }
        public void copyModuleWithNewBody(String charactor,int bodyId,String name)
        {
            CharacterTbl chaTbl = findCharactor(charactor);
            List<CosBean> coslist = chaTbl.findCosByItemId(bodyId);
            if (coslist.Count == 0) throw new Exception("BodyNotFound");
            ModuleBean mb = modules.findModuleByCosId(charactor,coslist[0].id + 1);
            copyModuleWithNewBody(mb.id,name);
        }
        public void copyModuleWithNewBody(String farcName,String name)
        {
            copyModuleWithNewBody(getStandardName(farcName.Substring(0, 3)), Int32.Parse(farcName.Substring(6))-1, name);
        }
        public CharacterItemBean copyItemByNo(String charactor,int no)
            //复制角色零件
        {
            //复制item
            CharacterTbl chaTbl = findCharactor(charactor);
            CharacterItemBean newitem = new CharacterItemBean(chaTbl.findItemByNo(no));
            if (newitem.attr != 1) 
                if(newitem.type == 1) 
                {
                    //泳装/ST
                    newitem.attr = 1;
                    newitem.haveTexChg = false;
                    newitem.haveTexOrg = false;
                    newitem.haveTexLength = false;
                    newitem.objsetLength = 1;
                }else throw new Exception("CanNotCopyTextureReplacedItem");
            newitem.name = newitem.name + " NEW";
            newitem.objset[0].objset = objdb.getCharacterNameUpper(charactor, 1);
            newitem.dataObjUid[0].uid = objdb.getCharacterNameUpper(charactor, 1) 
                + newitem.dataObjUid[0].uid.Substring(newitem.dataObjUid[0].uid.IndexOf('_'));
            newitem.no = chaTbl.lastItemNo + 1;
            chaTbl.addItem(newitem);
            //复制objdb
            objdb.add(newitem);
            return newitem;
        }
        public void copyCustomizeItemInAllParts(int cstmItemNo,String name)
            //复制配件
        {
            //复制cstmitem
            //添加objdb
            //添加sprdb
            //循环添加chatbl
        }
        public void copyCustomizeItemById(int cstmItemNo,String name,int type)
        {
            
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
                bin[index + idOffset] = 0;
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
