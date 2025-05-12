using Domain.Class;

namespace Animation.Monitoring
{
    public interface IMonitoringAnimation
    {
        bool IsPaused { get; } 
        void Initialize();
        void UpdateData(OperationResultDomain result);
    }
}