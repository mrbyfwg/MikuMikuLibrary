
namespace Test
{
    class Program
    {
        private static void Main(string[] args)
        {
            //database files path
            string moduleIdPath = @"C:\MZZZ\rom\mdata_gm_module_tbl.farc";
            string cstmItemIdPath = @"C:\MZZZ\rom\mdata_gm_customize_item_tbl.farc";
            string chritmPropPath = @"C:\MZZZ\rom\mdata_chritm_prop.farc";
            string objDbPath = @"C:\MZZZ\rom\objset\mdata_obj_db.bin";
            string sprDbPath = @"C:\MZZZ\rom\2d\mdata_spr_db.bin";
            //output folder path
            string outputPath = @"C:\PDAFT\MDATA";
            //mods you want merge
            string[] newFarcRomPath = new string[] { @"C:\PDAFT\MDATA\M400",
                                                     @"C:\PDAFT\MDATA\M401",
                                                     @"C:\PDAFT\MDATA\Mkai",
                                                     @"C:\PDAFT\MDATA\Mkim",
                                                     @"C:\PDAFT\MDATA\MnFT",
                                                     @"C:\PDAFT\MDATA\MnFT1",
                                                                            };
            //original game path
            string oriRomPath = @"C:\PDAFT";
            //original DLC path
            string[] officialMdataPath = new string[] {oriRomPath,
                                                       oriRomPath+@"\MDATA\m215",
                                                       oriRomPath+@"\MDATA\m220",
                                                       oriRomPath+@"\MDATA\m230",
                                                       oriRomPath+@"\MDATA\m240",
                                                       oriRomPath+@"\MDATA\m250",
                                                       oriRomPath+@"\MDATA\m260",
                                                       oriRomPath+@"\MDATA\m270"};

            SlotEditor se = new SlotEditor(moduleIdPath, cstmItemIdPath, chritmPropPath, objDbPath, sprDbPath);
            se.createPath(outputPath);
            for (int i = 0; i <= newFarcRomPath.Length - 1; i++)
            {
                se.loopObjsetFolder(newFarcRomPath[i] + @"\rom\objset");
                se.copy2d(newFarcRomPath[i], officialMdataPath, outputPath);
                se.copyFarc(newFarcRomPath[i], outputPath);
                se.copySkinParam(newFarcRomPath[i], officialMdataPath, outputPath);
                se.getLogs(outputPath);
            }
            se.output(outputPath);
        }
    }
}
