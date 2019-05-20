using System.Threading.Tasks;

namespace SoundIdentification.Command
{
    public interface IQueryCommand
    {
        Task<QueryResult> Query();
    }
}