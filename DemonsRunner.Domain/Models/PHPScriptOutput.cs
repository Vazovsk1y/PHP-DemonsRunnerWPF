using System.Text;

namespace DemonsRunner.Domain.Models
{
    public class PHPScriptOutput
    {
        private readonly StringBuilder _outputBuilder = new StringBuilder();

        public PHPScript Owner { get; }

        public string Text => _outputBuilder.ToString();

        public PHPScriptOutput(PHPScript owner) 
        { 
            Owner = owner; 
        }

        public PHPScriptOutput(PHPScript script, string message)
        {
            Owner = script;
            UpdateOutputText(message);
        }

        public void UpdateOutputText(string text) =>_outputBuilder.Append(text + Environment.NewLine);
    }
}
