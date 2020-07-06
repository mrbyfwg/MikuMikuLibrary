using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    class Program
    {
        private static void Main(string[] args)
        {
            string moduleIdPath = @"C:\MZZZ\rom\mdata_gm_module_tbl.farc";
            string cstmItemIdPath = @"C:\MZZZ\rom\mdata_gm_customize_item_tbl.farc";
            string chritmPropPath = @"C:\MZZZ\rom\mdata_chritm_prop.farc";
            string objDbPath = @"C:\MZZZ\rom\objset\mdata_obj_db.bin";
            string sprDbPath = @"C:\MZZZ\rom\2d\mdata_spr_db.bin";
            string outputPath = @"C:\PDAFT\MDATA";
            string newFarcRomPath = @"C:\PDAFT\MDATA\M400\rom";
            string oriRomPath = @"C:\PDAFT";
            string[] officialMdataPath = new string[] {oriRomPath,
                                                       oriRomPath+@"\MDATA\m215",
                                                       oriRomPath+@"\MDATA\m220",
                                                       oriRomPath+@"\MDATA\m230",
                                                       oriRomPath+@"\MDATA\m240",
                                                       oriRomPath+@"\MDATA\m250",
                                                       oriRomPath+@"\MDATA\m260",
                                                       oriRomPath+@"\MDATA\m270"};
            SlotEditor se = new SlotEditor(moduleIdPath, cstmItemIdPath, chritmPropPath, objDbPath, sprDbPath);
            se.copyModuleWithNewBody("mikitm025", "");
            se.copyModuleWithNewBody("mikitm026", "");
            se.copyModuleWithNewBody("mikitm031", "");
            se.copyModuleWithNewBody("mikitm057", "");
            se.copyModuleWithNewBody("mikitm064", "");
            se.copyModuleWithNewBody("mikitm075", "");
            se.copyModuleWithNewBody("mikitm114", "");
            se.copyModuleWithNewBody("mikitm115", "");
            se.copyModuleWithNewBody("mikitm124", "");
            se.copyModuleWithNewBody("mikitm129", "");
            se.copyModuleWithNewBody("mikitm139", "");
            se.copyModuleWithNewBody("mikitm143", "");
            se.copyModuleWithNewBody("mikitm144", "");
            se.copyModuleWithNewBody("mikitm149", "");
            se.copyModuleWithNewBody("mikitm174", "");
            se.copyModuleWithNewBody("mikitm175", "");
            se.copyModuleWithNewBody("mikitm176", "");
            se.copyModuleWithNewBody("mikitm177", "");
            se.copyModuleWithNewBody("mikitm178", "");
            se.copyModuleWithNewBody("rinitm002", "");
            se.copyModuleWithNewBody("rinitm026", "");
            se.copyModuleWithNewBody("rinitm046", "");

            se.output(outputPath);
            se.copy2d(newFarcRomPath, officialMdataPath, outputPath);
            se.copyFarc(newFarcRomPath, outputPath);
            se.copySkinParam(newFarcRomPath, officialMdataPath, outputPath);


        }
    }
}
