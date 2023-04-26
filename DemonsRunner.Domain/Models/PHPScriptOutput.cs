using System.Collections.ObjectModel;

namespace DemonsRunner.Domain.Models
{
    public class PHPScriptOutput
    {
        public PHPScript Owner { get; }

        public ICollection<string> Messages { get; } = new ObservableCollection<string>();

        public PHPScriptOutput(PHPScript outputOwner) 
        { 
            Owner = outputOwner; 
        }
    }
}
