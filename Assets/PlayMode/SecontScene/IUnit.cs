
public interface IUnit
{
    public IUnit AddDamageToUnit(IUnit enemyUnit);
    
    //プロパティ
    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    
}
