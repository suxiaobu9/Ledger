namespace Ledger.Shared.Model;

public class MonthlyAccountingVm
{
    public int Month { get; set; }

    public EventDetail[]? Income { get; set; }

    public EventDetail[]? Outlay { get; set; }

    public int? TotalIncome
    {
        get
        {
            if (Income == null)
                return 0;
            return Income.Sum(x => x.Amount);
        }
    }

    public int TotalOutlay
    {
        get
        {
            if (Outlay == null)
                return 0;

            return Outlay.Sum(x => x.Amount);
        }
    }

    public ReportDetail[]? ReportDetails
    {
        get
        {
            if (Income == null || Outlay == null)
                return null;

            var group = Outlay.Select(x => x.Event).Distinct().ToArray();

            return group.Select(x =>
            {
                var total = Outlay.Where(y => y.Event == x).Sum(y => y.Amount);
                return new ReportDetail
                {
                    Event = x,
                    Total = total,
                    Proportion = Math.Round((decimal)total / Outlay.Sum(x => x.Amount) * 100, 2)
                };
            }).OrderByDescending(x => x.Proportion).ToArray();


        }
    }

    public class ReportDetail
    {
        public string? Event { get; set; }

        public int Total { get; set; }

        public decimal Proportion { get; set; }
    }

    public class EventDetail
    {
        public DateTime Date { get; set; }

        public string? Event { get; set; }

        public int Amount { get; set; }
    }
}
