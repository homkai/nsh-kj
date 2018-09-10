using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using nshkj.UI.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace nshkj
{
	/// <summary>
	/// Manipulates files and image data.
	/// </summary>
	public class Action {

        /// <summary>
        /// Process the screenshot
        /// </summary>
        /// <param name="screenshot">Screenshot bitmap</param>
        /// <returns>Always returns an ActionResult instance</returns>
        public void Process(Bitmap screenshot)
        {
            System.Windows.Forms.Clipboard.SetImage(screenshot);
            var ocr = new Ocr();
            var result = ocr.GeneralBasicParse(screenshot);

            var kejuQA = new KejuQA();

            string topic = result[0];
            string topicPattern = @"^(.{0,2}(\d*)分)?(.*)$";
            foreach (Match match in Regex.Matches(result[0], topicPattern))
            {
                GroupCollection groups = match.Groups;
                kejuQA.Topic = groups[3].Value;
                kejuQA.Score = groups[2].Length > 0 ? Convert.ToInt32(groups[2].Value): 0;
            }

            var options = result.Count > 1 ? result.GetRange(1, result.Count - 1) : new List<string>();
            kejuQA.Options = options;
            App.UpdateKejuDialog(kejuQA);

            string question = JsonConvert.SerializeObject(kejuQA);
            string res = "";
            try
            {
                // Console.WriteLine("question: "+ question);
                res = App.GetHttpResponse("/api/nshkj/qa/" + System.Web.HttpUtility.UrlEncode(question, System.Text.Encoding.UTF8)); // TODO 你自己的服务
                // Console.WriteLine("res: " + res);
            }
            catch (Exception e)
            {
                return;
            }
            JObject jObject = JObject.Parse(res);
            JArray jlist = JArray.Parse(jObject["optionalAnswers"].ToString());
            string answer = "";
            for (int i = 0; i < jlist.Count; ++i)
            {
                var row = JObject.Parse(jlist[i].ToString());
                string content = row["content"].ToString();
                int score = Convert.ToInt32(row["score"].ToString());
                string desc = row["description"].ToString();

                answer += "【" + content + "】 " ;
                if (score > 0)
                {
                    answer += "正确率：" + score + "% ";
                }
                if (!String.IsNullOrEmpty(desc))
                {
                    answer += "相关：" + desc;
                }
                answer += "\r\n";
            }

            kejuQA.Answer = answer;
            App.UpdateKejuDialog(kejuQA);
        }

    }
}
