public class CalcDamagePvP
{
    public int CalcDamage(int attack, int defence)
    {
        int ret = attack*2 - defence;
        return ret;
    }
}
