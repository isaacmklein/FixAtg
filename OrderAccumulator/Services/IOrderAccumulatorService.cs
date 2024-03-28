using OrderAccumulator.Domain.Entities;

namespace OrderAccumulator.Services
{
    public interface IOrderAccumulatorService
    {
        bool ProcessOrder(Order order);
    }
}