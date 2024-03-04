using VContainer;


public class UnitImpl : IUnit
{
    //ステータス
    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    
    private ICalcDamage _calcDamage;
    
    [Inject]
    public UnitImpl(ICalcDamage calcDamage)
    {
        _calcDamage = calcDamage;
        Hp = 100;
        Atk = 30;
        Def = 20;
    } 
    
    //引数で渡されたクラスにダメージを与える
    public IUnit AddDamageToUnit(IUnit enemyUnit)
    {
        int damage = _calcDamage.CalcDamage(Atk, Def);
        enemyUnit.Hp -= damage;
        return enemyUnit;
    }
}
