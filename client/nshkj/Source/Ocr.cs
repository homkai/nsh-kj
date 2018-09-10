using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json.Linq;

namespace nshkj
{
	/// <summary>
	/// ocr
	/// </summary>
	public class Ocr {

        private Baidu.Aip.Ocr.Ocr client;


        public Ocr() {
            // 设置APPID/AK/SK
            var APP_ID = "9888177";
            var API_KEY = "uTegiE2hexxp9gmo18HAGGiu";
            var SECRET_KEY = "iDO57pGadINI7RNv3eXTOVklT5GO4AqG";

            client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
        }

        public List<String> GeneralBasicParse(Bitmap screenshot)
        {
            var image = Bitmap2Byte(screenshot);
            var resultjObject = client.GeneralBasic(image);
            List<string> rows = new List<string>();

            JArray jlist = JArray.Parse(resultjObject["words_result"].ToString());
            for (int i = 0; i < jlist.Count; ++i)
            {
                var rowJObject = JObject.Parse(jlist[i].ToString());
                rows.Add(rowJObject["words"].ToString());
            }
            return rows;
        }

        public static byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }
    }
}
