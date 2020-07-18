
using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    class Program
    {

        //database files path
        static string moduleIdPath; 
        static string cstmItemIdPath;
        static string chritmPropPath;
        static string objDbPath;
        static string sprDbPath;
        //output folder path
        static string outputPath; 
        //mods you want merge
        static string[] newFarcRomPath = new string[0];
        static string oriRomPath; 
        //original DLC path
        static string[] officialMdataPath = new string[0];
        static Boolean allowTextureReplace;
        private static void Main(string[] args)
        {
            try{
                readConfig();
                SlotEditor se = new SlotEditor(moduleIdPath, cstmItemIdPath, chritmPropPath, objDbPath, sprDbPath);
                se.createPath(outputPath);
                for (int i = 0; i <= newFarcRomPath.Length - 1; i++)
                {
                    se.createNewLogs(newFarcRomPath[i]);
                    se.loopObjsetFolder(newFarcRomPath[i] + @"\rom\objset");
                    se.copy2d(newFarcRomPath[i], officialMdataPath, outputPath);
                    se.copyFarc(newFarcRomPath[i],officialMdataPath, outputPath);
                    se.copySkinParam(newFarcRomPath[i], officialMdataPath, outputPath);
                    se.getLogs(outputPath);
                }
                se.output(outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Please Read SlotEditor.ini");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("done！");
            Console.ReadLine();
            return;
        }
        private static void readConfig()
        {
            //从文件读取配置信息
            string programPath = Environment.CurrentDirectory;
            if (!File.Exists(programPath + "\\SlotEditor.ini")) createConfig(programPath + "\\SlotEditor.ini");  
            String[] configFile = File.ReadAllLines(programPath + "\\SlotEditor.ini");
            
            List<string> modsList = new List<string>();
            List<string> dlcList = new List<string>();
            foreach (String line in configFile)
            {
                if ((line.Trim().Length == 0)||(line.Trim()[0].Equals('#'))) { continue; }
                String key = StringCut.splitBeforeEqual(line).Trim();
                String value = StringCut.splitAfterEqual(line).Trim();
                if (value.Equals("")) { throw new Exception("'"+line+ "' ERROR"); }
                if (value[value.Length - 1].Equals('\\')) value = value.Substring(0,value.Length-1);
                

                Console.WriteLine(line);
                switch (key.ToLower())
                {
                    case "gm_module_tbl":
                        moduleIdPath = value;
                        break;
                    case "gm_customize_item_tbl":
                        cstmItemIdPath = value;
                        break;
                    case "chritm_prop":
                        chritmPropPath = value;
                        break;
                    case "obj_db":
                        objDbPath = value;
                        break;
                    case "spr_db":
                        sprDbPath = value;
                        break;
                    case "outputfolder":
                        outputPath = value;
                        break;
                    case "modspathlist":
                        modsList.Add(value);
                        break;
                    case "basegamepath":
                        oriRomPath = value;
                        break;
                    case "officialmdatapathlist":
                        string str = "";
                        if (!value[0].Equals('\\')) str = "\\";
                        dlcList.Add(str + value);
                        break;
                    //case "allowtexturereplace":
                    //    switch (value.ToLower())
                    //    {
                    //        case "true":
                    //            allowTextureReplace = true;
                    //            break;
                    //        case "false":
                    //            allowTextureReplace = false;
                    //            break;
                    //        default:
                    //            throw new Exception(line+" ERROR");
                    //    }
                    //    break;
                    default:
                        Console.WriteLine("undefine:" + key);
                        break;
                }
            }
            for (int i = 0; i <= dlcList.Count-1;i++) dlcList[i] = oriRomPath + dlcList[i];
            dlcList.Insert(0, oriRomPath);
            newFarcRomPath = modsList.ToArray();
            officialMdataPath = dlcList.ToArray();

        }
        private static void createConfig(String path)
        {
            File.WriteAllText(path, Properties.Resources.configText);
        }
    }
}
