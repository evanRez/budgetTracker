namespace BudgetTracker.MinimalAPI.Helpers
{
    public class Utility
    {
        public bool IsWithinDateRange(DateOnly startDate, DateOnly endDate, DateOnly dateValue)
        {
            return dateValue >= startDate && dateValue <= endDate;
        }

        public bool IsWithinBudget(decimal goalAmount, params decimal[] budget)
        {
            var totalSpent = budget.Sum();

            return goalAmount >= totalSpent;
        }
    }
}
