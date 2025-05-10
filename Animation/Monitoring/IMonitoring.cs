using Domain.Class;

namespace Animation.Monitoring
{
    public interface IMonitoring
    {
        bool IsPaused { get; } 
        void Initialize();
        void UpdateData(OperationResult result);
    }
}