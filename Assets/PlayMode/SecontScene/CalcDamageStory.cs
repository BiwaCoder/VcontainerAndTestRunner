
public class CalcDamageStory : ICalcDamage
{
    public int CalcDamage(int attack, int defence)
    {
        int ret = attack - defence;
        return ret;
    }
    
}
