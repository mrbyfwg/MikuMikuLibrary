using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using Test.Pojo;

namespace Test
{
    class CustomizeItems
    {
        public CustomizeItemBean[] itemList;
        public int length;
        public int patch;
        public int version;
        public int lastId = -1;
        public int lastSort_index = -1;
        public int lastIndex = -1;
        public CustomizeItems(String[] ori)
        {
            //寻找长度行
            int flag = 0;
            foreach (string line in ori.Reverse())
            {
                switch (StringCut.splitBeforeEqual(line))
                {
                    case "version":
                        version =Int32.Parse(StringCut.splitAfterEqual(line));
                        flag++;
                        break;
                    case "patch":
                        patch = Int32.Parse(StringCut.splitAfterEqual(line));
                        flag++;
                        break;
                    case "length":
                        length = Int32.Parse(StringCut.splitAfterEqual(line));
                        itemList = new CustomizeItemBean[length + 500];
                        flag++;
                        break;
                }
                if (flag == 3) break;
            }
            foreach (string line in ori)
            {
                //读取文件
                //注释
                if (line[0].Equals('#')) { continue; }
                //最后一行
                if (line.Contains("data_list.length")) { break; }
                //切分
                int index = Int32.Parse(StringCut.splitPoint(line,2));
                String key = StringCut.splitBeforeEqual(line);
                String value = StringCut.splitAfterEqual(line);
                if (itemList[index] == null) {itemList[index] = new CustomizeItemBean(); }
                itemList[index].index = index;
                if (lastIndex < index) lastIndex = index;
                switch (key)
                {
                    case "chara":
                        itemList[index].chara = value;
                        break;
                    case "id":
                        if (Int32.Parse(value) > lastId) lastId = Int32.Parse(value);
                        itemList[index].id = Int32.Parse(value);
                        break;
                    case "name":
                        itemList[index].name = value;
                        break;
                    case "ng":
                        itemList[index].ng = Int32.Parse(value);
                        break;
                    case "obj_id":
                        itemList[index].obj_id = Int32.Parse(value);
                        break;
                    case "parts":
                        itemList[index].parts = value;
                        break;
                    case "sell_type":
                        itemList[index].sell_type = Int32.Parse(value);
                        break;
                    case "shop_ed_day":
                        itemList[index].shop_ed_day = Int32.Parse(value);
                        break;
                    case "shop_ed_month":
                        itemList[index].shop_ed_month = Int32.Parse(value);
                        break;
                    case "shop_ed_year":
                        itemList[index].shop_ed_year = Int32.Parse(value);
                        break;
                    case "shop_price":
                        itemList[index].shop_price = Int32.Parse(value);
                        break;
                    case "shop_st_day":
                        itemList[index].shop_st_day = Int32.Parse(value);
                        break;
                    case "shop_st_month":
                        itemList[index].shop_st_month = Int32.Parse(value);
                        break;
                    case "shop_st_year":
                        itemList[index].shop_st_year = Int32.Parse(value);
                        break;
                    case "sort_index":
                        if (Int32.Parse(value) > lastSort_index) lastSort_index = Int32.Parse(value);
                        itemList[index].sort_index = Int32.Parse(value);
                        break;
                    default:
                        Console.WriteLine(value);
                        break;
                }
            }
        }
        public List<String> toString()
        {
            List<String> result = new List<string>();
            for (int i = 1; i <= 214; i++)
            {
                result.Add("#---------------------------------------------");
            }
            CustomizeItemBean[] notNullItemList = (from str in itemList where str != null select str).ToArray();
            CustomizeItemBean[] dicSortItem = (from objDic in notNullItemList
                                          orderby objDic.index.ToString()
                                          select objDic).ToArray();
            foreach (CustomizeItemBean i in dicSortItem)
                result.AddRange(i.toString());
            result.Add("cstm_item.data_list.length=" + length.ToString());
            result.Add("patch=" + patch.ToString());
            result.Add("version=" + version.ToString());
            return result;
        }
        public CustomizeItemBean findCstmItemById(int id)
        {
            foreach(CustomizeItemBean cib in itemList)
            {
                if (cib.id == id) return cib;
            }
            throw new Exception(id+" CSTMItemNotFound");
        }
        public CustomizeItemBean findCstmItemByObjId(int obj_Id)
        {
            foreach (CustomizeItemBean cib in itemList)
            {
                if (cib.obj_id == obj_Id) return cib;
            }
            throw new Exception(obj_Id+" CSTMItemNotFound");
        }
        public void add(CustomizeItemBean cib)
        {
            cib.index = lastIndex + 1;
            if (itemList[cib.index] == null) itemList[cib.index] = new CustomizeItemBean();
            else throw new Exception(cib.index+" UsedCustomizeItemIndex");
            itemList[cib.index] = cib;
            if (cib.index > lastIndex) lastIndex = cib.index;
            if (cib.sort_index > lastSort_index) lastSort_index = cib.sort_index;
            if (cib.id > lastId) lastId = cib.id;
            length++;
        }
    }
}
