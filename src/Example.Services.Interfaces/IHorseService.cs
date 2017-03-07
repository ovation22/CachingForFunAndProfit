using System.Collections.Generic;

namespace Example.Services.Interfaces
{
    public interface IHorseService
    {
        IEnumerable<Dto.Horse> GetAll();
        Dto.Horse Get(int id);
        IEnumerable<Dto.Horse> GetAllCached();
        Dto.Horse GetCached(int id);
        IEnumerable<Dto.Horse> GetAllRepositoryCached();
        Dto.Horse GetRepositoryCached(int id);
    }
}