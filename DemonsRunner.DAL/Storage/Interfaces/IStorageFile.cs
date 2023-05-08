using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonsRunner.DAL.Storage.Interfaces
{
    public interface IStorageFile
    {
        string Name { get; }

        string FullPath { get; }
    }
}
