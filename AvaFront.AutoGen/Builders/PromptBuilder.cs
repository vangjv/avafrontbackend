using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaFront.AutoGen.Builders
{
    public class PromptBuilder
    {
        private string Prompt { get; set; } = "";
        public PromptBuilder Add(string prompt, bool newLine = true)
        {
            if (newLine)
            {
                Prompt = Prompt + prompt + "\n";
            }
            else
            {
                Prompt = Prompt + prompt;
            }
            return this;
        }

        public string Build()
        {
            return Prompt;
        }
    }
}
