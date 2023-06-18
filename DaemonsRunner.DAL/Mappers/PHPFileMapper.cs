using DaemonsRunner.DAL.Repositories;
using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.DAL.Mappers
{
    public static class PHPFileMapper
    {
        public static PHPFileDTO MapToDTO(this PHPFile pHPFile)
        {
            return new PHPFileDTO
            {
                Name = pHPFile.Name,
                FullPath = pHPFile.FullPath,
            };
        }

        public static PHPFile MapToModel(this PHPFileDTO pHPFileDTO)
        {
            return PHPFile.Create(pHPFileDTO.Name, pHPFileDTO.FullPath);
        }
    }
}
