namespace DevTools.JsonMerge.Cli
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using System.Linq;
    using System.Text;

    public class JsonMerge
    {
        private readonly JsonMergeOptions options;

        public JsonMerge(JsonMergeOptions options)
        {
            this.options = options;
        }

        public bool Convert()
        {
            try
            {
                List<JObject> jsonFiles = options.InputFiles.Select(i => JObject.Parse(File.ReadAllText(i))).ToList();

                JObject mergedJsonFiles = jsonFiles.Aggregate((a, b) =>
                {
                    a.Merge(b, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    return a;
                });

                File.WriteAllText(options.OutputFile, mergedJsonFiles.ToString(), Encoding.UTF8);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
