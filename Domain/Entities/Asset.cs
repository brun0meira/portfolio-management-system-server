using Domain.Entities;
using System.Diagnostics;

public class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public string Name { get; set; }

    public ICollection<Trade> Trades { get; set; }
    public ICollection<Quote> Quotes { get; set; }
    public ICollection<Position> Positions { get; set; }
    public ICollection<Dividend> Dividends { get; set; }
}
