namespace DemonsRunner.Domain.Models
{
    public class PHPScriptMessage
    {
        public PHPScript Sender { get; }

        public string Text { get; }

        public PHPScriptMessage(PHPScript script, string messageText)
        {
            Sender = script;
            Text = messageText;
        }
    }
}
