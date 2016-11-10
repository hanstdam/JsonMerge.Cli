namespace DevTools.JsonMerge.Cli
{
    using System.Collections.Generic;

    public class JsonMergeOptions
    {
        public JsonMergeOptions()
        {
            Inputs = new List<string>();
            InputFiles = new List<string>();
        }

        public List<string> Inputs { get; set; }

        public string OutputFile { get; set; }

        public List<string> InputFiles { get; set; }
    }
}
