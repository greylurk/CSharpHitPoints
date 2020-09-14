namespace HitPoints.Models {
    public enum HPEventType
    {
        Damage,
        Heal,
        TempHitPoints,
        ShortRest,
        LongRest,
    }
    public class HPEvent
    {
        public int Id { get; set; }
        public HPEventType HPEventType { get; set; }
        public int Amount { get; set; }
        public DamageType? DamageType { get; set; }
    }
}