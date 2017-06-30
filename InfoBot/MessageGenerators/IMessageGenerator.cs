using System.Threading.Tasks;

namespace InfoBot.MessageGenerators
{
    public interface IMessageGenerator
    {
        Task<string> GetMessageAsync(string command);
    }
}
