﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCustomCosmetics
{
    public class PlayerData
    {
        private DataStorage<Dictionary<ulong, PlayerCosmetic>> DataStorage { get; set; }
        public Dictionary<ulong, PlayerCosmetic> data { get; private set; }
        public PlayerData()
        {
            DataStorage = new DataStorage<Dictionary<ulong, PlayerCosmetic>>(MCustomCosmetics.Instance.Directory, "Cosmetics.json");
        }
        public void Reload()
        {
            data = DataStorage.Read();
            if (data == null)
            {
                data = new Dictionary<ulong, PlayerCosmetic>();
                DataStorage.Save(data);
            }
        }

        public void Save(Dictionary<ulong, PlayerCosmetic> dict)
        {
            data = dict;
        }

        public void CommitToFile()
        {
            DataStorage.Save(data);
        }
    }

    public class PlayerCosmetic
    {
        public string SelectedFit { get; set; }
        public bool AllowSaving { get; set; }
        public Dictionary<string, Outfit> Outfits { get; set; }
    }

    public class Outfit
    {
        public int Hat;
        public int Mask;
        public int Glasses;
        public int Backpack;
        public int Shirt;
        public int Vest;
        public int Pants;
        public HairColor Hair;
        public Dictionary<int, string> skins;
    }

    public class HairColor
    {
        public HairColor(float rr, float gg, float bb)
        {
            R = rr;
            G = gg;
            B = bb;
        }
        public float R;
        public float G;
        public float B;
    }

    public class DataStorage<T> where T : class
    {
        public string DataPath { get; private set; }
        public DataStorage(string dir, string fileName)
        {
            DataPath = Path.Combine(dir, fileName);
        }

        public void Save(T obj)
        {
            string objData = JsonConvert.SerializeObject(obj, Formatting.None);

            using (StreamWriter stream = new StreamWriter(DataPath, false))
            {
                stream.Write(objData);
            }
        }

        public T Read()
        {
            if (File.Exists(DataPath))
            {
                string dataText;
                using (StreamReader stream = File.OpenText(DataPath))
                {
                    dataText = stream.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<T>(dataText);
            }
            else
            {
                return null;
            }
        }
    }
}
