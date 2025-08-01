namespace ShipApp.Models
{
    public class Ship
    {
        public long ShipId { get; set; }
        public string ShipName { get; set; }
        public bool Complete { get; set; }
        public bool IsNeeded { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string CheckedStickers { get; set; }
        public bool IsPrinted { get; set; }

        public Ship(string shipName)
        {
            ShipName = shipName;
            Complete = false;
            IsNeeded = false;
            DateCompleted = null;
            CheckedStickers = string.Empty;
            IsPrinted = false;
        }

        public override string? ToString()
        {
            return $"ShipId: {ShipId}, " +
                   $"ShipName: {ShipName}, " +
                   $"Complete: {Complete}, " +
                   $"IsNeeded: {IsNeeded}, " +
                   $"DateCompleted: {(DateCompleted.HasValue ? DateCompleted.Value.ToString("yyyy-MM-dd") : "null")}, " +
                   $"CheckedStickers: {CheckedStickers}, " +
                   $"IsPrinted: {IsPrinted}";
        }
    }

}
